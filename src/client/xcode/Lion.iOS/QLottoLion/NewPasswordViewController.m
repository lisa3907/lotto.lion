//
//  NewPasswordViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 17..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "NewPasswordViewController.h"
#import "SettingViewController.h"

@interface NewPasswordViewController ()

@end

@implementation NewPasswordViewController
@synthesize changeOKCell, tfOriginal, tfNew;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    [changeOKCell.textLabel setBackgroundColor: [UIColor clearColor]];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];
}

#pragma mark - password

- (BOOL) validateEmail: (NSString *) email {
    NSString *emailRegex = @"[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}";
    NSPredicate *emailTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", emailRegex];
    BOOL isValid = [emailTest evaluateWithObject:email];
    return isValid;
}

- (BOOL) validatePassword: (NSString *)_password {
//    NSString *pwRegex = @"^(?=.*[A-Za-z])(?=.*\\d)(?=.*[$@$!%*#?&])[A-Za-z\\d$@$!%*#?&]{6,}$";
//    NSPredicate *pwTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", pwRegex];
//    BOOL isValid = [pwTest evaluateWithObject:_password];
//    return isValid;
    
    BOOL isValid;
    if([_password length] >= 6){
        isValid = YES;
    }else{
        isValid = NO;
    }
    
    return isValid;
}


#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {

    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {

    return 3;
}


-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{
    
    if(indexPath.section == 0 && indexPath.row == 2){
        
        [self passwordChangeAlert:tfOriginal.text NewPassword:tfNew.text];
    }
}


#pragma mark - Network

- (void)passwordChangeAlert:(NSString*)password1 NewPassword:(NSString*)password2{
    
    BOOL check = [self validatePassword:password2];
    
    if([password1 isEqualToString:@""]){
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"비밀번호를 입력해주세요."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    }else if([password2 isEqualToString:@""]){
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"새로운 비밀번호를 입력해주세요."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    }else if (!check){
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"6글자 이상의 비밀번호를 입력해주세요."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    }else if(![password1 isEqualToString:password2]){
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"2개의 비밀번호가 일치하지 않습니다."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    
    }else if ([password1 isEqualToString:password2] && check){
        //success
        [self changePassword:password2];
        
    }
    
}





- (void)changePassword:(NSString*)newPassword{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary * responseDict;
    
    @try {
        if([appDelegate.userToken isEqualToString:@"nil value"]){
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"서버가 응답하지 않습니다.\n 잠시 후 다시 시도해주시기 바랍니다."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            [self presentViewController:alertController animated:YES completion:nil];
            return;
        }else{
            responseDict = [appDelegate.networkInstance ChangePassword:appDelegate.userToken NewPassword:newPassword];
        }
        
    }
    
    @catch (NSException *exception) {
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"서버가 응답하지 않습니다.\n 잠시 후 다시 시도해주시기 바랍니다."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        [self presentViewController:alertController animated:YES completion:nil];
        return;
    }
    
    @finally {
        BOOL returnBoolean = [[responseDict objectForKey:@"success"]boolValue];
        NSString * errorMessage = [[responseDict objectForKey:@"message"]stringByRemovingPercentEncoding];
        
        if(returnBoolean){
            // success changed password
            [[NSUserDefaults standardUserDefaults] setObject:@"" forKey:@"ID"];
            [[NSUserDefaults standardUserDefaults] setObject:@"" forKey:@"PW"];
            [[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isSaveID"];
            [[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isSavePW"];
            
            [self excuteLogout];
            
            [self.navigationController popViewControllerAnimated:YES];
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"변경완료"
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            
            
        }else{
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:errorMessage
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



/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

#pragma mark TextFieldDelegate

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
    [self.tableView endEditing:YES];
}


- (BOOL)textFieldShouldReturn:(UITextField *)textField {
    
    [textField resignFirstResponder];
    
    return YES;
}

- (void)textFieldDidEndEditing:(UITextField *)textField{
    
    [textField resignFirstResponder];
}



#pragma LOGOUT

- (void)excuteLogout{
    NSLog(@"Logout");
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    SettingViewController * settingViewCont = [[SettingViewController alloc]init];
    
    appDelegate.userToken = @"";
    settingViewCont.userTokenForSetting = @"";
    appDelegate.logined = NO;
    
    settingViewCont.Cell1.detailTextLabel.text = @"로그인 필요";
    settingViewCont.Cell2.detailTextLabel.text = @"로그인 필요";
    settingViewCont.Cell3.detailTextLabel.text = @"로그인 필요";
    
    settingViewCont.userInfoDic = nil;
    
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
