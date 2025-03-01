//
//  AppDelegate.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 27..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "AppDelegate.h"
#import <UserNotifications/UserNotifications.h>
#import "NumbersViewController.h"


#define SYSTEM_VERSION_GRATERTHAN_OR_EQUALTO(v)  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedAscending)


@interface AppDelegate () <UIApplicationDelegate, UNUserNotificationCenterDelegate>

@end

@implementation AppDelegate

@synthesize userID, userPW, APNSKey;


- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    // Override point for customization after application launch.
    
    [self userSetting];
    
    self.networkInstance = [[Networking alloc] init];
    
    [self registerForRemoteNotifications];
    
    [GADMobileAds configureWithApplicationID:@"ca-app-pub-8599301686845489~1509563653"];
    
    return YES;
}

- (void)registerForRemoteNotifications {
    if(SYSTEM_VERSION_GRATERTHAN_OR_EQUALTO(@"10.0")){
        UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
        center.delegate = self;
        [center requestAuthorizationWithOptions:(UNAuthorizationOptionSound | UNAuthorizationOptionAlert | UNAuthorizationOptionBadge) completionHandler:^(BOOL granted, NSError * _Nullable error){
            if(!error){
                [[UIApplication sharedApplication] registerForRemoteNotifications];
            }
        }];
    }
    else {
        // Code for old versions
        
        // for iOS 8
        [[UIApplication sharedApplication] registerUserNotificationSettings:[UIUserNotificationSettings settingsForTypes:(UIUserNotificationTypeSound | UIUserNotificationTypeAlert | UIUserNotificationTypeBadge) categories:nil]];
        [[UIApplication sharedApplication] registerForRemoteNotifications];
        
    }
    // Badge Reset
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
}

//Called when a notification is delivered to a foreground app.
-(void)userNotificationCenter:(UNUserNotificationCenter *)center willPresentNotification:(UNNotification *)notification withCompletionHandler:(void (^)(UNNotificationPresentationOptions options))completionHandler{
    NSLog(@"User Info : %@",notification.request.content.userInfo);
    completionHandler(UNAuthorizationOptionSound | UNAuthorizationOptionAlert | UNAuthorizationOptionBadge);
}

//Called to let your app know which action was selected by the user for a given notification.
-(void)userNotificationCenter:(UNUserNotificationCenter *)center didReceiveNotificationResponse:(UNNotificationResponse *)response withCompletionHandler:(void(^)())completionHandler{
    NSLog(@"User Info : %@",response.notification.request.content.userInfo);
    completionHandler();
}


- (void)applicationWillResignActive:(UIApplication *)application {
    // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
    // Use this method to pause ongoing tasks, disable timers, and invalidate graphics rendering callbacks. Games should use this method to pause the game.
    
    _logined = NO;
}


- (void)applicationDidEnterBackground:(UIApplication *)application {
    // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later.
    // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.

    _logined = NO;
}


- (void)applicationWillEnterForeground:(UIApplication *)application {
    // Called as part of the transition from the background to the active state; here you can undo many of the changes made on entering the background.
    // Badge Reset
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
    
   
    [self getGuestToken];
}


- (void)applicationDidBecomeActive:(UIApplication *)application {
    // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
}


- (void)applicationWillTerminate:(UIApplication *)application {
    // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
}

#pragma mark -- userDefaults

- (void)userSetting{
    NSDictionary *dicOptionDefaults =
    [NSDictionary dictionaryWithObjectsAndKeys:
     @"",@"ID",
     @"",@"PW",
     [NSNumber numberWithBool:NO],@"isSaveID",
     [NSNumber numberWithBool:NO],@"isSavePW",
     nil];

    [[NSUserDefaults standardUserDefaults]registerDefaults:dicOptionDefaults];
    
    NSLog(@"User Option Defaults : %@",dicOptionDefaults);
    self.edited = NO;
    self.logined = NO;
    self.userInfoDic = [[NSDictionary alloc]init];
    
    [[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isSaveID"];
    [[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isSavePW"];
}



#pragma mark -- Push Noti


- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
//    NSString * deviceTokenString = [[[[deviceToken description] stringByReplacingOccurrencesOfString: @"<" withString: @""]
//                                     stringByReplacingOccurrencesOfString: @">" withString: @""]
//                                     stringByReplacingOccurrencesOfString: @" " withString: @""];
//    NSLog(@"deviceTokenString is : %@",deviceTokenString);
    
    /**
     * 이 메서드가 호출되면 APNS에 디바이스 정보를 등록하고 64바이트의 문자열을 받아오게 된다.
     */
    NSMutableString *deviceId = [NSMutableString string];
    const unsigned char* ptr = (const unsigned char*) [deviceToken bytes];
    
    for(int i = 0 ; i < 32 ; i++)
    {
        [deviceId appendFormat:@"%02x", ptr[i]];
    }
    
    // 여기서 추출된 deviceId를 서드파티 서비스의 서버로 전송하여 관리한다.
    NSLog(@"APNS Device Token: %@", deviceId);
    APNSKey = deviceId;
}

- (void)application:(UIApplication*)application didFailToRegisterForRemoteNotificationsWithError:(NSError*)error
{
    NSLog(@"Failed to get token, error: %@", error);
}


#pragma mark -- Network

- (void)getGuestToken{
    
    @try{
        _guestToken = [_networkInstance getGuestToken];
        
    }
    
    @catch(NSException *exception){
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"서버가 응답하지 않습니다.\n 잠시 후 다시 시도해주시기 바랍니다."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
        
    }
    @finally{
        NSLog(@"guestToken : %@", _guestToken);
        
    }
    
}



@end
