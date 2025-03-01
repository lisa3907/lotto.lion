//
//  logoutViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 17..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "logoutViewController.h"
#import "SettingViewController.h"
#import "LoginViewController.h"

@interface logoutViewController ()

@property (weak, nonatomic) IBOutlet UITableViewCell *signOutCell;

@end

@implementation logoutViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    [self.signOutCell.textLabel setBackgroundColor: [UIColor clearColor]];
    
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];
    
}

- (void)viewWillDisappear:(BOOL)animated{
    [super viewWillDisappear:animated];
    
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {

    return 2;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {

    return 1;
}

-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{
    
    if(indexPath.section == 0 && indexPath.row == 0){
        
        [self excuteLogout];
        
        
    }else if(indexPath.section == 1 && indexPath.row == 0){
        
       
        NSLog(@"signOut try");

        UIAlertController * alertController = [UIAlertController alertControllerWithTitle: @"알림"
                                                                                  message: @"'로또사자'를 탈퇴 하시겠습니까?"
                                                                           preferredStyle:UIAlertControllerStyleAlert];

        UIAlertAction * signOut = [UIAlertAction actionWithTitle:@"탈퇴" style:UIAlertActionStyleDefault
                                                        handler:^(UIAlertAction * action) {
                                                            // logout
                                                            NSLog(@"signOut click");

                                                            [self excuteSignOut];
                                                            
                                                      }];


        UIAlertAction * cancel = [UIAlertAction actionWithTitle:@"취소" style:UIAlertActionStyleDefault
                                                        handler:^(UIAlertAction * action) {

                                                            NSLog(@"cancel btn");
                                                            [alertController dismissViewControllerAnimated:YES completion:nil];

                                                        }];
        [alertController addAction:signOut];
        [alertController addAction:cancel];
        [self presentViewController:alertController animated:YES completion:nil];
        
     
    
    }
}


/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

#pragma mark - Network

- (void)excuteSignOut{

    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    
    @try{
    
        if([appDelegate.userToken isEqualToString:@""] || appDelegate.userToken == nil){
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"서버가 응답하지 않습니다.\n 잠시 후 다시 시도해주시기 바랍니다."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            
            return;
        }else{
            [appDelegate.networkInstance LeaveMember:appDelegate.userToken];
        }
    }
    @catch (NSException *exception){
        
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
        
        appDelegate.userToken = @"";
        appDelegate.logined = NO;
        
        [[NSUserDefaults standardUserDefaults] setObject:@"" forKey:@"ID"];
        [[NSUserDefaults standardUserDefaults] setObject:@"" forKey:@"PW"];
        [[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isSaveID"];
        [[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isSavePW"];
    
//        [self.navigationController popViewControllerAnimated:YES];
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"탈퇴하였습니다."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
//        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
//                                                           style:UIAlertActionStyleDefault
//                                                         handler:nil];
        
        UIAlertAction * actionOk = [UIAlertAction actionWithTitle:@"확인" style:UIAlertActionStyleDefault
                                                         handler:^(UIAlertAction * action) {
                                                             
                                                             
                                                             [self.navigationController popToRootViewControllerAnimated:YES];
                                                             
                                                         }];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    
    }

}



#pragma LOGOUT

- (void)excuteLogout{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    appDelegate.logined = NO;
    
    SettingViewController * settingViewCont = [[SettingViewController alloc]init];
    
    appDelegate.userToken = @"";
    
    settingViewCont.userInfoDic = nil;
    
    settingViewCont.userTokenForSetting = @"";
    
    settingViewCont.Cell1.detailTextLabel.text = @"로그인 필요";
    settingViewCont.Cell2.detailTextLabel.text = @"로그인 필요";
    settingViewCont.Cell3.detailTextLabel.text = @"로그인 필요";
    
//    [self.navigationController popViewControllerAnimated:YES];
    
    
    //alert
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                             message:@"로그아웃 되었습니다."
                                                                      preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction * actionOk = [UIAlertAction actionWithTitle:@"확인" style:UIAlertActionStyleDefault
                                                      handler:^(UIAlertAction * action) {
                                                          
                                                          [self.navigationController popToRootViewControllerAnimated:YES];
                                                          
                                                      }];
    [alertController addAction:actionOk];
    
//    UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
//    [vc presentViewController:alertController animated:YES completion:nil];
    [self presentViewController:alertController animated:YES completion:nil];
    
//    [self.navigationController popViewControllerAnimated:YES];
    
}

#pragma mark - check internet
- (void) internetCheck{
    
    NSLog(@"online mode check");
    if ([[Reachability reachabilityForInternetConnection]currentReachabilityStatus]==NotReachable)
    {
        //connection unavailable
        NSLog(@"online mode OFF");
        AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
        appDelegate.logined = NO;
        appDelegate.userToken = @"";
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"연결 오류"
                                                                                 message:@"네트워크 연결 상태 확인 후\n다시 시도해주세요."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
        
    }else{
        NSLog(@"online mode ON");
    }
    
}

@end
