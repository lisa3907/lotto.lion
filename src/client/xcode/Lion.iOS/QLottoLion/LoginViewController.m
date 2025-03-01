//
//  LoginViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 11..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "LoginViewController.h"
#import "NumbersViewController.h"
#import "SignUpViewController.h"
#import "RenewViewController.h"

@interface LoginViewController ()

@end

@implementation LoginViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    
    [self.loginButtonCell.textLabel setBackgroundColor: [UIColor clearColor]];
    [self.signUpButtonCell.textLabel setBackgroundColor: [UIColor clearColor]];
    [self.renewButtonCell.textLabel setBackgroundColor:[UIColor clearColor]];
    
    self.navigationItem.title = @"로그인";
    
    
}



- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}



- (void)viewWillAppear:(BOOL)animated{
    [super viewWillAppear:animated];
    
    [self internetCheck];
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    if(appDelegate.logined){
    
        [self.navigationController popViewControllerAnimated:YES];
        
    }else{
        
        if([appDelegate.guestToken isEqualToString:@""] || appDelegate.guestToken == nil){
            
            [self getGuestToken];
        }else{
            
            NSLog(@"Guest Token Stored");
        }
        
        [self setSwitchs];
    
    }

}


- (void)setSwitchs{
    //save check 스위치 상태
    if([[NSUserDefaults standardUserDefaults] boolForKey:@"isSaveID"]) {
        [self.switchID setOn:YES];
    } else {
        [self.switchID setOn:NO];
    }
    
    if([[NSUserDefaults standardUserDefaults] boolForKey:@"isSavePW"]) {
        [self.switchPW setOn:YES];
    } else {
        [self.switchPW setOn:NO];
    }
    
    if([[NSUserDefaults standardUserDefaults] stringForKey:@"ID"]){
        self.tfID.text = [[NSUserDefaults standardUserDefaults] stringForKey:@"ID"];
    }
    
    
    if([[NSUserDefaults standardUserDefaults] stringForKey:@"PW"]){
        self.tfPW.text = [[NSUserDefaults standardUserDefaults] stringForKey:@"PW"];
    }

}


#pragma mark -- TextField Delegate

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
    [self.tableView endEditing:YES];
}


- (BOOL)textFieldShouldReturn:(UITextField *)textField {
    
    if(textField.tag == 1){
        [_tfPW becomeFirstResponder];
    }else{
        [textField resignFirstResponder];
    }
    
    
    return YES;
}

- (void)textFieldDidEndEditing:(UITextField *)textField{
    
    [textField resignFirstResponder];
}

- (BOOL) validatePassword: (NSString *)_password {
    NSString *pwRegex = @"^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$";
    NSPredicate *pwTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", pwRegex];
    BOOL isValid = [pwTest evaluateWithObject:_password];
    return isValid;
}



#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 3;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    
    if(section == 0){
        return 3;
    }else if (section == 1){
        return 1;
    }else if (section == 2){
        return 1;
    }else{
        return 0;
    }
    
}

-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{
//    UITableViewCell *cell = [tableView cellForRowAtIndexPath:indexPath];
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    if(indexPath.section == 0){
        NSLog(@"section 0 : %ld", (long)indexPath.row);
        
        if(indexPath.row == 2){
            [self loginWithID:_tfID.text Password:_tfPW.text];
        }
        
    }else if(indexPath.section == 1 && indexPath.row == 0){
        NSLog(@"section 1 : %ld", (long)indexPath.row);
        if(appDelegate.logined){
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"로그인 상태입니다."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            return;
            
        }else{
            SignUpViewController *signUpViewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"signUpview"];
            
            [self.navigationController pushViewController:signUpViewCont animated:YES];
        
        }
        
    }else if(indexPath.section == 2 && indexPath.row == 0){
        RenewViewController * renewViewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"renewView"];
        [self.navigationController pushViewController:renewViewCont animated:YES];
        
    }
    
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}



#pragma mark - Network

- (void)getGuestToken{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    appDelegate.guestToken = [appDelegate.networkInstance getGuestToken];
    
    NSLog(@"appDelegate.guestToken : %@", appDelegate.guestToken);
}


- (void)loginWithID:(NSString*)loginID Password:(NSString*)password{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary * responseDict;
    
    @try {
        if([appDelegate.guestToken isEqualToString:@"nil value"] || appDelegate.guestToken == nil){
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
            responseDict = [appDelegate.networkInstance GetTokenByLoginId:appDelegate.guestToken LoginID:loginID Password:password DeviceType:@"I" DeviceID:appDelegate.APNSKey];
            
        }
        
    }
    
    @catch (NSException *exception) {
        
        NSString *errorString = [[responseDict objectForKey:@"message"]stringByRemovingPercentEncoding];
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"에러"
                                                                                 message:errorString
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    }
    
    @finally {
        
        BOOL tempBool = [[responseDict objectForKey:@"success"]boolValue];
        
        if(!tempBool){
            //Login Fail
            NSString *errorString = [[responseDict objectForKey:@"message"]stringByRemovingPercentEncoding];
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"로그인 에러"
                                                                                     message:errorString
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            
        }else{
            //Login OK
            if([responseDict objectForKey:@"result"] != nil && ![[responseDict objectForKey:@"result"] isEqualToString:@""] && tempBool){
                NSLog(@"User Logined");
                
                appDelegate.userToken = [responseDict objectForKey:@"result"];  //get user token
                
                
                if(self.switchID.isOn){
                    [[NSUserDefaults standardUserDefaults] setObject:self.tfID.text forKey:@"ID"];
                }else{
                    [[NSUserDefaults standardUserDefaults] setObject:@"" forKey:@"ID"];
                }
                
                if(self.switchPW.isOn){
                    [[NSUserDefaults standardUserDefaults] setObject:self.tfPW.text forKey:@"PW"];
                }else{
                    [[NSUserDefaults standardUserDefaults] setObject:@"" forKey:@"PW"];
                }
                
                [self getUserInfo:appDelegate.userToken];
                
                appDelegate.logined = YES;
                appDelegate.edited = YES;
                
                NSLog(@"<><> login <><>");
                
                // Back
                [self.navigationController popViewControllerAnimated:YES];
                
            }
            
        }
        
    }
    
}



- (void)getUserInfo:(NSString*)userToken{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary * responseDict;
    
    
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
            responseDict = [appDelegate.networkInstance GetUserInfor:appDelegate.userToken];
        }
        
    }
    @catch (NSException *exception){
        
        
        
    }
    @finally{
        
        appDelegate.userInfoDic = [[responseDict objectForKey:@"result"]mutableCopy];
        
        NSLog(@"userInfoDic : %@", appDelegate.userInfoDic);
    }
}




- (IBAction)IDSwitchAction:(UISwitch *)sender {
    if(sender.isOn){
        [[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isSaveID"];
    }else{
        [[NSUserDefaults standardUserDefaults] setBool:NO forKey:@"isSaveID"];
    }
}

- (IBAction)PWSwitchAction:(UISwitch *)sender {
    if(sender.isOn){
        [[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isSavePW"];
    }else{
        [[NSUserDefaults standardUserDefaults] setBool:NO forKey:@"isSavePW"];
    }
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
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
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
