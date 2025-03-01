//
//  SignUpViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 12..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "SignUpViewController.h"

@interface SignUpViewController (){
    
    BOOL emailCheck;
}


@end

@implementation SignUpViewController
@synthesize tfEmail, tfConfirm, tfID, tfName, tfPassword, tfPassword2nd;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    self.navigationItem.title = @"회원가입";
    
    [self.confirmButtonCell.textLabel setBackgroundColor: [UIColor clearColor]];
    [self.confirmButtonCell2.textLabel setBackgroundColor: [UIColor clearColor]];
    [self.signUpButtonCell.textLabel setBackgroundColor: [UIColor clearColor]];

    
    emailCheck = false;
    
    tfEmail.keyboardType = UIKeyboardTypeEmailAddress;
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    if(appDelegate.guestToken == nil || [appDelegate.guestToken isEqualToString:@""]){
        [self getGuestToken];
    }
    
    
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];
    
}

#pragma mark -- validate

-(BOOL) NSStringIsValidEmail:(NSString *)checkString
{
    BOOL stricterFilter = NO;
    NSString *stricterFilterString = @"^[A-Z0-9a-z\\._%+-]+@([A-Za-z0-9-]+\\.)+[A-Za-z]{2,4}$";
    NSString *laxString = @"^.+@([A-Za-z0-9-]+\\.)+[A-Za-z]{2}[A-Za-z]*$";
    NSString *emailRegex = stricterFilter ? stricterFilterString : laxString;
    NSPredicate *emailTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", emailRegex];
    return [emailTest evaluateWithObject:checkString];
}

- (BOOL) validatePassword: (NSString *)_password {
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

    return 3;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {

    
    if(section == 0){
        return 2;
    }else if (section == 1){
        return 2;
    }else if (section == 2){
        return 5;
    }else{
        return 0;
    }
    
}


-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{
    
    if(indexPath.section == 0 && indexPath.row == 1){
        NSLog(@"Email sending");
        
        if ([tfEmail.text isEqualToString:@""] || ![self NSStringIsValidEmail:tfEmail.text]) {
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"올바른 이메일 주소를 입력해주세요."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            
        }else{
            //Email Check
            
//            NSString * message = [NSString stringWithFormat:@"%@\n인증 코드를 발송하였습니다.",tfEmail.text];
//            
//            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
//                                                                                     message:message
//                                                                              preferredStyle:UIAlertControllerStyleAlert];
//            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
//                                                               style:UIAlertActionStyleDefault
//                                                             handler:nil];
//            [alertController addAction:actionOk];
//            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
//            [vc presentViewController:alertController animated:YES completion:nil];
            
            [self sendCheckEmail:tfEmail.text];
            
            tfConfirm.backgroundColor = [UIColor whiteColor];
            [tfConfirm setEnabled: YES];
        }

    }
    
    
    if(indexPath.section == 1 && indexPath.row == 1){
        NSLog(@"Email Check");
        if ([tfConfirm.text isEqualToString:@""] || [tfConfirm.text length] <= 5) {
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"인증코드를 정확히 입력해주세요."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            
        }else{
            //Number Check
            [self sendCheckEmailConfirm:tfEmail.text Number:tfConfirm.text];
            
        }
    }
    
    
    if(indexPath.section == 2 && indexPath.row == 4){
        NSLog(@"SignUp");
        if(emailCheck){
            
            
            if([tfID.text isEqualToString:@""]){
                UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                         message:@"아이디를 입력해주세요."
                                                                                  preferredStyle:UIAlertControllerStyleAlert];
                UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                                   style:UIAlertActionStyleDefault
                                                                 handler:nil];
                [alertController addAction:actionOk];
                UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
                [vc presentViewController:alertController animated:YES completion:nil];
                
            }else if ([tfEmail.text isEqualToString:@""] || [self NSStringIsValidEmail:tfEmail.text]) {
                
                UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                         message:@"유효한 이메일을 입력해주세요."
                                                                                  preferredStyle:UIAlertControllerStyleAlert];
                UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                                   style:UIAlertActionStyleDefault
                                                                 handler:nil];
                [alertController addAction:actionOk];
                UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
                [vc presentViewController:alertController animated:YES completion:nil];
                
            }else if ([tfConfirm.text isEqualToString:@""] || [tfConfirm.text length] <= 5){
                UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                         message:@"인증번호를 입력해주세요."
                                                                                  preferredStyle:UIAlertControllerStyleAlert];
                UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                                   style:UIAlertActionStyleDefault
                                                                 handler:nil];
                [alertController addAction:actionOk];
                UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
                [vc presentViewController:alertController animated:YES completion:nil];
                
            }else if([tfName.text isEqualToString:@""]){
                UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                         message:@"이름을 입력해주세요."
                                                                                  preferredStyle:UIAlertControllerStyleAlert];
                UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                                   style:UIAlertActionStyleDefault
                                                                 handler:nil];
                [alertController addAction:actionOk];
                UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
                [vc presentViewController:alertController animated:YES completion:nil];
                
            }else if([tfPassword.text isEqualToString:@""] || ![self validatePassword:tfPassword.text]){
                UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                         message:@"6글자 이상의 비밀번호를 입력해주세요."
                                                                                  preferredStyle:UIAlertControllerStyleAlert];
                UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                                   style:UIAlertActionStyleDefault
                                                                 handler:nil];
                [alertController addAction:actionOk];
                UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
                [vc presentViewController:alertController animated:YES completion:nil];
                
            }else if (![tfPassword.text isEqualToString:tfPassword2nd.text]){
                UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                         message:@"2개의 패스워드가 일치하지 않흡니다."
                                                                                  preferredStyle:UIAlertControllerStyleAlert];
                UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                                   style:UIAlertActionStyleDefault
                                                                 handler:nil];
                [alertController addAction:actionOk];
                UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
                [vc presentViewController:alertController animated:YES completion:nil];
            
            }else{
                // 회원 가입 요청
                
                AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
                
                [self requestSignin:tfID.text LoginName:tfName.text Password:tfPassword.text Email:tfEmail.text DeviceID:appDelegate.APNSKey CheckNumber:tfConfirm.text];
                
            }
        }else{
            // 이메일 재확인 경고
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"이메일 인증을 선택해주세요."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
        }
    }
    
    [tableView deselectRowAtIndexPath:indexPath animated:YES];

}




#pragma mark -- Network

- (void)errorForNetwork{
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



- (void)getGuestToken{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    appDelegate.guestToken = [appDelegate.networkInstance getGuestToken];
    
    NSLog(@"guestToken : %@", appDelegate.guestToken);
}

- (void)sendCheckEmail:(NSString *)email {
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary * responseDict;
    
    @try {
        if([appDelegate.guestToken isEqualToString:@"nil value"]){
            [self errorForNetwork];
            return;
        }else{
            responseDict = [appDelegate.networkInstance SendMailToCheckMailAddress:appDelegate.guestToken Email:email];
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
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    }
    
    @finally {
        
        BOOL responseBoolean = [[responseDict objectForKey:@"success"]boolValue];
        
        if(responseBoolean){     // success
            emailCheck = YES;
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"입력하신 이메일 주소로 인증번호를 발송하였습니다.\n받으신 인증번호를 입력해주세요."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
        }else{              // fail
            
            emailCheck = NO;
            NSString * errorMessage = [[responseDict objectForKey:@"message"]stringByRemovingPercentEncoding];
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"실패"
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
    
    NSLog(emailCheck ? @"EmailCheck : Yes" : @"EmailCheck : No");
}

- (void)sendCheckEmailConfirm:(NSString *)email Number:(NSString *)number{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    BOOL responeCheck;
    
    @try {
        if([appDelegate.guestToken isEqualToString:@"nil value"]){
            [self errorForNetwork];
            return;
        }else{
            responeCheck = [appDelegate.networkInstance CheckMailAddress:appDelegate.guestToken Email:email Checknumber:number];
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
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    }
    
    @finally {
        
        if(responeCheck){
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"인증완료"
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            
            // TODO:
            
            tfConfirm.backgroundColor = [UIColor lightGrayColor];
            [tfConfirm setEnabled: NO];
            
        }else{
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"인증실패\n확인 후 다시 입력해주세요."
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



- (void)requestSignin:(NSString *)_loginID
            LoginName:(NSString *)_login_name
             Password:(NSString *)_password Email:(NSString *)_email
             DeviceID:(NSString *)_device_id
          CheckNumber:(NSString *)_check_number{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary * responseDict;
    
    @try {
        if([appDelegate.guestToken isEqualToString:@"nil value"] || appDelegate.guestToken == nil){
            return;
        }else{
            responseDict = [appDelegate.networkInstance AddMemberByLoginId:appDelegate.guestToken LoginID:_loginID LoginName:_login_name PassWord:_password Email:_email DeviceID:_device_id DeviceType:@"I" CheckNumber:_check_number];
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
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
    }
    
    @finally {
        NSLog(@"Sign in request Sended");
        
        BOOL returnBoolean = [[responseDict objectForKey:@"success"]boolValue];
        
        if(returnBoolean){
            // Back
            NSLog(@"SignUp Success!!!");
            [[self navigationController] popViewControllerAnimated:YES];
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"가입성공"
                                                                                     message:@"회원가입이 완료되었습니다."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            
        }else{
            // error message
            NSString * responseMessage = [[responseDict objectForKey:@"message"]stringByRemovingPercentEncoding];
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"가입실패"
                                                                                     message:responseMessage
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


/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

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
