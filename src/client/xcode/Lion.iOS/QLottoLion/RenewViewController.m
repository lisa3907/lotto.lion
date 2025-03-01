//
//  RenewViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 19..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "RenewViewController.h"

@interface RenewViewController () 

@end

@implementation RenewViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    [self.sendEmailButtonCell.textLabel setBackgroundColor:[UIColor clearColor]];
    
    self.tfEmail.keyboardType = UIKeyboardTypeEmailAddress;
    
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];
    
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return 2;
}


-(void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{


    if(indexPath.section == 0 && indexPath.row ==1){
        AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
        
        if([self NSStringIsValidEmail:self.tfEmail.text]){
        
            [self SendMailToRecoveryIdWithGusetToken:appDelegate.guestToken Email:self.tfEmail.text];
        
        }else{
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"올바른 이메일 주소를 입력해 주세요."
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

#pragma mark -- Network

- (void)SendMailToRecoveryIdWithGusetToken:(NSString*)_guestToken Email:(NSString*)_email{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary * responseDict;
    
    @try{
        responseDict = [appDelegate.networkInstance SendMailToRecoveryId:_guestToken MailAddress:_email];
        
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
        
        BOOL returnBoolean = [[responseDict objectForKey:@"success"]boolValue];
        
        NSString * message;
        
        if(returnBoolean){
            
            [self.navigationController popViewControllerAnimated:YES];
            
            message = [NSString stringWithFormat:@"%@로 메일을 발송했습니다.", _email];
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:message
                                                                              preferredStyle:UIAlertControllerStyleAlert];
            UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                               style:UIAlertActionStyleDefault
                                                             handler:nil];
            [alertController addAction:actionOk];
            UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
            [vc presentViewController:alertController animated:YES completion:nil];
        
        }else{
        
            message = [[responseDict objectForKey:@"message"]stringByRemovingPercentEncoding];
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:message
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
