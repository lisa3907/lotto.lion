//
//  AppDelegate.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 27..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "Networking.h"


@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) NSString *userID;
@property (strong, nonatomic) NSString *userPW;
@property (strong, nonatomic) NSString *APNSKey;
@property (strong, nonatomic) NSString *userToken;
@property (strong, nonatomic) NSString *guestToken;
@property  int recentSeqNum;
@property (strong, nonatomic) NSString *recentSeqDate;
@property (strong, nonatomic) NSDictionary * userInfoDic;

@property BOOL edited;
@property BOOL logined;

@property (strong, nonatomic) Networking *networkInstance;

@end

