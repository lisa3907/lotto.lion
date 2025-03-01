//
//  SignUpViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 31..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "SignUpViewController2.h"

@interface SignUpViewController2 (){

    NSString * guestToken;
    
    BOOL emailCheck;
}

@end

@implementation SignUpViewController
@synthesize tfEmail, tfConfirm, tfID, tfName, tfPassword;

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    emailCheck = false;
    [self getGuestToken];
    
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

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
    NSString *pwRegex = @"^(?=.*\\d)(?=.*[~`!@#$%\\^&*()-])(?=.*[a-zA-Z]).{4,8}$";
    NSPredicate *pwTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", pwRegex];
    BOOL isValid = [pwTest evaluateWithObject:_password];
    return isValid;
}

- (void)getGuestToken{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    guestToken = [appDelegate.networkInstance getGuestToken];
    
    NSLog(@"guestToken : %@", guestToken);
}

- (void)sendCheckEmail:(NSString *)email {
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary * responseDict;
    
    @try {
        if([guestToken isEqualToString:@"nil value"]){
            [self errorForNetwork];
            return;
        }else{
            responseDict = [appDelegate.networkInstance SendMailToCheckMailAddress:guestToken Email:email];
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
        
        emailCheck = [[responseDict objectForKey:@"success"]boolValue];
        
        if(emailCheck){     // success
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
            NSString * errorMessage = [responseDict objectForKey:@"message"];
            
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
        if([guestToken isEqualToString:@"nil value"]){
            [self errorForNetwork];
            return;
        }else{
            responeCheck = [appDelegate.networkInstance CheckMailAddress:guestToken Email:email Checknumber:number];
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
        if([guestToken isEqualToString:@"nil value"] || guestToken == nil){
            return;
        }else{
            responseDict = [appDelegate.networkInstance AddMemberByLoginId:guestToken LoginID:_loginID LoginName:_login_name PassWord:_password Email:_email DeviceID:_device_id DeviceType:@"I" CheckNumber:_check_number];
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
            NSString * responseMessage = [responseDict objectForKey:@"message"];
            
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



#pragma mark Button Actions


- (IBAction)btnSendEmailAction:(UIButton *)sender {
    
    if ([tfEmail.text isEqualToString:@""]) {
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"이메일을 입력해주세요."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
        
    }else{
        //Email Check
        if([self NSStringIsValidEmail:tfEmail.text]){
             [self sendCheckEmail:tfEmail.text];
        }else{
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"유효한 이메일주소를 입력해주세요."
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

- (IBAction)btnEmailCheckAction:(UIButton *)sender {
    if ([tfConfirm.text isEqualToString:@""]) {
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"이메일을 입력해주세요."
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


- (IBAction)btnOkAction:(UIButton *)sender {
    NSLog(@"OK Button Action");
    
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
        
        }else if ([tfEmail.text isEqualToString:@""] && [self NSStringIsValidEmail:tfEmail.text]) {
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"유효한 이메일을 입력해주세요."
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
            
        }else if ([tfConfirm.text isEqualToString:@""]){
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
        
        }else if([tfPassword.text isEqualToString:@""] && [self validatePassword:tfPassword.text]){
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"유효한 패스워드를 입력해주세요."
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



#pragma mark TextFieldDelegate

- (BOOL)textFieldShouldReturn:(UITextField *)textField {
    
    [textField resignFirstResponder];
    
    return YES;
}


@end
