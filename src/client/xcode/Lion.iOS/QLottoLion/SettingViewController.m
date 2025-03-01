//
//  SettingViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 6..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "SettingViewController.h"
#import "CountSetViewController.h"
#import "LoginViewController.h"
#import "NewEmailViewController.h"
#import "NewPasswordViewController.h"
#import "logoutViewController.h"
#import "MessageViewController.h"

@interface SettingViewController (){

    int messageCount;
}

@end

@implementation SettingViewController
@synthesize Cell1, Cell2, Cell3, filterView, inboxCell;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    Cell1.detailTextLabel.text = @"로그인 필요";
    Cell2.detailTextLabel.text = @"로그인 필요";
    Cell3.detailTextLabel.text = @"로그인 필요";
    inboxCell.detailTextLabel.text = @"로그인 필요";
    
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
    
}

-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];
    
}


-(void)viewDidAppear:(BOOL)animated{
    [super viewDidAppear:animated];
    
    NSLog(@"viewDidAppear");
    [self checkUserToken];
}


- (void)checkUserToken{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    NSLog(@"user token : %@", appDelegate.userToken);
    
    if(appDelegate.userToken == nil || [appDelegate.userToken isEqualToString:@""] || appDelegate.logined == NO){
        
        Cell1.detailTextLabel.text = @"로그인 필요";
        Cell2.detailTextLabel.text = @"로그인 필요";
        Cell3.detailTextLabel.text = @"로그인 필요";
        inboxCell.detailTextLabel.text = @"로그인 필요";
        
        LoginViewController *loginViewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"loginView"];
        
        [self.navigationController pushViewController:loginViewCont animated:YES];
        
    }else{
    
        [self getMessageCount];
    }
    
    if(appDelegate.edited){
    
        [self setSettingView];
        
        appDelegate.edited = NO;
    }
    
    
}


- (void)setSettingView{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    Cell1.detailTextLabel.text = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"maxSelectNumber"]];
    
    NSString * num1 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit1"]];
    NSString * num2 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit2"]];
    NSString * num3 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit3"]];
    
    if(![num1 isEqualToString:@"0"] && ![num2 isEqualToString:@"0"] && ![num3 isEqualToString:@"0"]){
        Cell2.detailTextLabel.text = [NSString stringWithFormat:@"%@, %@, %@", num1, num2, num3];
    }else if(![num1 isEqualToString:@"0"] && [num2 isEqualToString:@"0"] && [num3 isEqualToString:@"0"]){
        Cell2.detailTextLabel.text = num1;
    }else if(![num1 isEqualToString:@"0"] && ![num2 isEqualToString:@"0"] && [num3 isEqualToString:@"0"]){
        Cell2.detailTextLabel.text = [NSString stringWithFormat:@"%@, %@", num1, num2];
    }else{
        Cell2.detailTextLabel.text = @"없음";
    }
    
    Cell3.detailTextLabel.text = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"emailAddress"]];
    Cell3.textLabel.text = @"이메일";
}

#pragma mark - password

- (BOOL) validateEmail: (NSString *) email {
    NSString *emailRegex = @"[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}";
    NSPredicate *emailTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", emailRegex];
    BOOL isValid = [emailTest evaluateWithObject:email];
    return isValid;
}

- (BOOL) validatePassword: (NSString *)_password {
    NSString *pwRegex = @"^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$";
    NSPredicate *pwTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", pwRegex];
    BOOL isValid = [pwTest evaluateWithObject:_password];
    return isValid;
}

#pragma mark -- network

- (void)getUserInfo:(NSString*)userToken{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary * responseDict;
    
    
    @try {
        if([userToken isEqualToString:@""] || userToken == nil){
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"서버가 응답하지 않습니다.\n 잠시 후 다시 시도해주시기 바랍니다."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            [self excuteLogout];
            return;
        }else{
            responseDict = [appDelegate.networkInstance GetUserInfor:userToken];
        }
        
    }
    @catch (NSException *exception){
        
        
        
    }
    @finally{
        
        appDelegate.userInfoDic = [[responseDict objectForKey:@"result"]mutableCopy];
        
        NSLog(@"userInfoDic : %@", appDelegate.userInfoDic);
        
    }
}

#pragma mark -- button actions




#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 4;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    
    
    if(section == 0){
        return 1;
    }else if(section == 1){
        return 2;
    }else if(section == 2){
        return 2;
    }else if(section == 3){
        return 1;
    }
    
    else{
        return 0;
    }
    
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{
    
//    if(indexPath.section == 1 && indexPath.row == 1){
//        NSLog(@"Logout try");
//    
//        UIAlertController * alertController = [UIAlertController alertControllerWithTitle: @"Logout"
//                                                                                  message: @"로그아웃 하시겠습니까?"
//                                                                           preferredStyle:UIAlertControllerStyleAlert];
//        
//        UIAlertAction * logout = [UIAlertAction actionWithTitle:@"로그아웃" style:UIAlertActionStyleDefault
//                                                        handler:^(UIAlertAction * action) {
//                                                            // logout
//                                                            NSLog(@"logout");
//                                                            
//                                                            [self excuteLogout];
//                                                      }];
//        
//        
//        UIAlertAction * cancel = [UIAlertAction actionWithTitle:@"취소" style:UIAlertActionStyleDefault
//                                                        handler:^(UIAlertAction * action) {
//                                                            
//                                                            NSLog(@"cancel btn");
//                                                            [alertController dismissViewControllerAnimated:YES completion:nil];
//                                                            
//                                                        }];
//        [alertController addAction:logout];
//        [alertController addAction:cancel];
//        [self presentViewController:alertController animated:YES completion:nil];
//        
//    }
    if(indexPath.section == 0 && indexPath.row == 0){
        
        NSLog(@"move to logout");
    
        logoutViewController *logoutViewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"logoutView"];
        
        [self.navigationController pushViewController:logoutViewCont animated:YES];
    
    
    }else if (indexPath.section == 1&& indexPath.row ==0){
    
        
        
        CountSetViewController *countViewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"countSet"];
        
        countViewCont.countArray = [NSArray arrayWithObjects:@5,@10,@15,@20,@25,@30,@35,@40,@45,@50,@55,@60,@65,@70,@75,@80,@85,@90,@95,@100,nil];
        countViewCont.checkMode = @"count";
        
        
        [self.navigationController pushViewController:countViewCont animated:YES];
        
        
        
    }else if (indexPath.section == 1&& indexPath.row ==1){
        
        
        CountSetViewController *countViewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"countSet"];
        
        countViewCont.checkMode = @"number";
        
        NSMutableArray *tempArray = [[NSMutableArray alloc]init];
        
        AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
        
        NSString * num1 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit1"]];
        NSString * num2 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit2"]];
        NSString * num3 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit3"]];

        if(![num1 isEqualToString:@"0"]){
            [tempArray addObject:num1];
        }
        if(![num2 isEqualToString:@"0"]){
            [tempArray addObject:num2];
        }
        if (![num3 isEqualToString:@"0"]){
            [tempArray addObject:num3];
        }

        NSLog(@"tempArray : %@", tempArray);
        
        countViewCont.SelectedRowCountArr = tempArray;
        
        [self.navigationController pushViewController:countViewCont animated:YES];
        
    }else if(indexPath.section == 2 && indexPath.row == 0){
       
        NewEmailViewController *newEmailViewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"newEmailView"];
        
        [self.navigationController pushViewController:newEmailViewCont animated:YES];
        
        
    }else if(indexPath.section == 2 && indexPath.row == 1){
       
    
        NewPasswordViewController *newPasswordViewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"newPasswordView"];
        
        [self.navigationController pushViewController:newPasswordViewCont animated:YES];
    }else if(indexPath.section == 3 && indexPath.row == 0){
        
        if(messageCount == 0){
            //alert
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"수신된 메세지가 없습니다."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
    
            UIViewController *rootViewController = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [rootViewController presentViewController:alertController animated:YES completion:nil];
    
        }else{
            MessageViewController *viewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"messageView"];
            
            [self.navigationController pushViewController:viewCont animated:YES];
        
        }
    
    }
    
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
}





#pragma mark - Network

- (void)getMessageCount{
    
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    int responseInt;
    
    
    @try {
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
            
            if(appDelegate.userToken){
                responseInt = [appDelegate.networkInstance GetAlertCount:appDelegate.userToken];
            }else{
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
            
        }
        
    }
    @catch (NSException *exception){
        
    }
    @finally{
        
        messageCount = responseInt;
        
        NSLog(@"message Count: %d", messageCount);
        
        if(messageCount > 0){
//            [badgeImage setHidden:NO];
//            [badgeCount setHidden:NO];
            inboxCell.detailTextLabel.text = [NSString stringWithFormat:@"%d", messageCount];
            
        }else{
            
//            [badgeImage setHidden:YES];
//            [badgeCount setHidden:YES];
            inboxCell.detailTextLabel.text = @"";
        }
        
    }
    
}



#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
//    [segue destinationViewController];
    // Pass the selected object to the new view controller.
    CountSetViewController * countViewCont = [segue destinationViewController];
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
//    countViewCont.maxCount = [appDelegate.userInfoDic objectForKey:@"maxSelectNumber"];
//    countViewCont.userToken = userTokenForSetting;
//    countViewCont.userInfoDic = appDelegate.userInfoDic;
    
    if([segue.identifier isEqual: @"countSet"]){
        NSLog(@"<><> segue 1 <><>");
        countViewCont.countArray = [NSArray arrayWithObjects:@5,@10,@15,@20,@25,@30,@35,@40,@45,@50,@55,@60,@65,@70,@75,@80,@85,@90,@95,@100,nil];
        countViewCont.checkMode = @"count";
        
    }else if([segue.identifier isEqual: @"numberSet"]){
        NSLog(@"<><> segue 2 <><>");
        
        countViewCont.checkMode = @"number";
        
        NSMutableArray *tempArray = [[NSMutableArray alloc]init];
        
        NSString * num1 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit1"]];
        NSString * num2 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit2"]];
        NSString * num3 = [NSString stringWithFormat:@"%@", [appDelegate.userInfoDic objectForKey:@"digit3"]];
        
        if(![num1 isEqualToString:@"0"]){
            [tempArray addObject:num1];
        }
        if(![num2 isEqualToString:@"0"]){
            [tempArray addObject:num2];
        }
        if (![num3 isEqualToString:@"0"]){
            [tempArray addObject:num3];
        }
        
        NSLog(@"tempArray : %@", tempArray);
        
        countViewCont.SelectedRowCountArr = tempArray;
        
    }
    
}


#pragma LOGOUT

- (void)excuteLogout{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    appDelegate.userToken = @"";
//    userTokenForSetting = @"";
    
    Cell1.detailTextLabel.text = @"로그인 필요";
    Cell2.detailTextLabel.text = @"로그인 필요";
    Cell3.detailTextLabel.text = @"로그인 필요";
    
    appDelegate.userInfoDic = nil;
    
    LoginViewController *viewCont2 = [self.storyboard instantiateViewControllerWithIdentifier:@"loginView"];
    
    [self.navigationController pushViewController:viewCont2 animated:YES];
    
    //alert
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                             message:@"로그아웃 되었습니다."
                                                                      preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                       style:UIAlertActionStyleDefault
                                                     handler:nil];
    [alertController addAction:actionOk];
    
    UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
    [vc presentViewController:alertController animated:YES completion:nil];

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
