//
//  NumbersViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 28..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "NumbersViewController.h"
#import "SignUpViewController.h"
#import "LoginViewController.h"
#import "NumbersCell.h"
#import "AdsViewCell.h"

static NSString *const AdUnitId = @"ca-app-pub-8599301686845489/3287348059";

@interface NumbersViewController (){

    NSString * guestToken;
    NSDictionary * userInfoDic;

    
    NSMutableArray *seqArray;
    int lastSeqNum;
    int presentSeqNum;
    
    NSNumberFormatter *numberFormatter;
    
    BOOL adReceived;
    
    UIBarButtonItem *rightButton;
    
    int firstSeq;
    
}

@end

@implementation NumbersViewController
@synthesize numbersArray, recentSeqNum, userToken, recentSeqDate, noticeLabel, myIndicator;
@synthesize loginView, numbersTable, lblTurn, seqTable, seqView, lblDate;

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    adReceived = YES;
    [loginView setHidden:NO];
    
    numberFormatter = [[NSNumberFormatter alloc] init];
    [numberFormatter setLocale:[NSLocale currentLocale]];
    [numberFormatter setNumberStyle:NSNumberFormatterDecimalStyle];
    
    [seqView setHidden:YES];
    

//    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
//    
//    if(appDelegate.userToken == nil || [appDelegate.userToken isEqualToString:@""]){
//        
//        LoginViewController *viewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"loginView"];
//        
//        [self.navigationController pushViewController:viewCont animated:YES];
//        
//    }
}



- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}



#pragma mark -- first Login check

-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];
    
}

- (void)viewDidAppear:(BOOL)animated{
    [super viewDidAppear:animated];
    [loginView setHidden:NO];
    
    [self checkLoginAndMove];
    
    NSLog(@"viewDidAppear");
}




- (void)checkLoginAndMove{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];

    if(appDelegate.userToken == nil || [appDelegate.userToken isEqualToString:@""] || appDelegate.logined == NO){
      
        LoginViewController *viewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"loginView"];
        
//        CATransition *transition = [CATransition animation];
//        transition.duration = 0.3f;
//        transition.timingFunction = [CAMediaTimingFunction functionWithName:kCAMediaTimingFunctionEaseInEaseOut];
//        transition.type = kCATransitionPush;
//        transition.subtype = kCATransitionFromTop;
//        [self.navigationController.view.layer addAnimation:transition forKey:nil];
        
        [self.navigationController pushViewController:viewCont animated:NO];
        
//        [self.navigationController pushViewController:viewCont animated:YES];
        
    }else{
    
        [myIndicator startAnimating];

        recentSeqNum = appDelegate.recentSeqNum;
        recentSeqDate = appDelegate.recentSeqDate;
        
        [self performSelectorOnMainThread:@selector(settingViews) withObject:myIndicator waitUntilDone:NO];
        
//        if(recentSeqNum != nil){
//            lastSeqNum = [recentSeqNum intValue] - 1;
//            
//            [self getNumbers:lastSeqNum];
//            [self getUserLastSeqNo];
//            
//        }
//        
//        [loginView setHidden:YES];
//        
//        if(lastSeqNum == [recentSeqNum intValue]){
//            [self.btnForward setHidden:YES];
//            lblTurn.text = [NSString stringWithFormat:@"%d회차 (예상)", lastSeqNum];
//            
//        }else if (firstSeq == recentSeqNum){
//            [self.btnForward setHidden:YES];
//            [self.btnBackward setHidden:YES];
//            lblTurn.text = [NSString stringWithFormat:@"%d회차 (예상)", lastSeqNum];
//            
//        }else{
//            [self.btnForward setHidden:NO];
//            lblTurn.text = [NSString stringWithFormat:@"%d회차 (완료)", lastSeqNum];
//        }
        
        
//        [self getMessageCount];
        
        NSLog(@"appDelegate.userToken : %@", appDelegate.userToken);
        NSLog(@"recentSeqNum : %d", recentSeqNum);
        
    }
}

- (void)settingViews{
    
    
    if(recentSeqNum){
        
        presentSeqNum = recentSeqNum;
        
        NSLog(@"init presentSeqNum = %d", presentSeqNum);
        
        lastSeqNum = recentSeqNum - 1;
        
        [self getNumbers:recentSeqNum];
        [self getUserLastSeqNo];
        
    }

    if(presentSeqNum == recentSeqNum){
        [self.btnForward setHidden:YES];
        lblTurn.text = [NSString stringWithFormat:@"%d회차 (예상)", recentSeqNum];
        
    }else if ( firstSeq == recentSeqNum){
        [self.btnForward setHidden:YES];
        [self.btnBackward setHidden:YES];
        lblTurn.text = [NSString stringWithFormat:@"%d회차 (예상)", recentSeqNum];
        
    }else{
        [self.btnForward setHidden:NO];
        lblTurn.text = [NSString stringWithFormat:@"%d회차 (완료)", recentSeqNum];
    }
    
    
//    [self getMessageCount];
    
    [myIndicator stopAnimating];

}


- (NSString*)makeImgName:(int)number{
    NSString * imgName;
    
    if(number < 11){
        imgName = @"Circled-Y.png";
    }else if (number < 21){
        imgName = @"Circled-B.png";
    }else if (number < 31){
        imgName = @"Circled-R.png";
    }else if (number < 41){
        imgName = @"Circled-K.png";
    }else if (number < 46){
        imgName = @"Circled-G.png";
    }
    
    return imgName;
}


#pragma mark -- sub

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
    [self.view endEditing:YES];
}


- (BOOL) validatePassword: (NSString *)_password {
    NSString *pwRegex = @"^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$";
    NSPredicate *pwTest = [NSPredicate predicateWithFormat:@"SELF MATCHES %@", pwRegex];
    BOOL isValid = [pwTest evaluateWithObject:_password];
    return isValid;
}

//- (void)makeSeqCount{
//    
//    int tempSeq = [recentSeqNum intValue];
//    seqArray = [[NSMutableArray alloc] init];
//    for(int i = 0; i < 100; i++){
//        [seqArray addObject:[NSString stringWithFormat:@"%d",tempSeq - i]];
//    }
//    
//    [seqTable reloadData];
//}

#pragma mark -- TextField Delegate

- (BOOL)textFieldShouldReturn:(UITextField *)textField {
    
    [textField resignFirstResponder];
    
    return YES;
}

- (void)textFieldDidEndEditing:(UITextField *)textField{
    
    [textField resignFirstResponder];
}

#pragma mark -- Network

- (void)getUserInfo{

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
            [self excuteLogout];
            return;
        }else{
            responseDict = [appDelegate.networkInstance GetUserInfor:appDelegate.userToken];
        }
        
    }
    @catch (NSException *exception){
    
        [self excuteLogout];
    
    }
    @finally{
    
        userInfoDic = [responseDict mutableCopy];
        
        NSLog(@"userInfoDic : %@", userInfoDic);
    
    }

}


- (void)getNumbers: (int)_seqNum{
    NSLog(@"getNumbers");
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
            [self excuteLogout];
            return;
        }else{
            
            if(recentSeqNum){
                responseDict = [appDelegate.networkInstance GetUserChoices:appDelegate.userToken SeqNo:_seqNum];
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
        
        [self excuteLogout];
    }
    @finally{
        
        numbersArray = [[responseDict objectForKey:@"result"] mutableCopy];
        
        NSLog(@"numbersArray numbers: %@", numbersArray);
        
//        [numbersTable reloadData];
        
        [numbersTable performSelectorOnMainThread:@selector(reloadData) withObject:myIndicator waitUntilDone:NO];
        
        
        if(numbersArray.count == 0 && recentSeqNum == _seqNum){
            [loginView setHidden:NO];
            noticeLabel.text = @"나의 예상 번호를 생성하고 있습니다.\n예상 소요시간은 30분 입니다.";
        }else if (numbersArray.count == 0){
            [loginView setHidden:NO];
            noticeLabel.text = @"당첨된 번호가 없습니다.";
        }else{
            [loginView setHidden:YES];
            noticeLabel.text = @"Loading ...";
        }
        
    }

}





- (void)getUserLastSeqNo{
    
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
            [self excuteLogout];
            return;
        }else{
            responseDict = [appDelegate.networkInstance GetUserSequenceNos:appDelegate.userToken];
        }
        
    }
    @catch (NSException *exception){
        
        [self excuteLogout];
        
    }
    @finally{
        
        userInfoDic = [responseDict mutableCopy];
        
        NSLog(@"getUserLastSeqNo : %@", userInfoDic);
        
//        NSArray * tempArray = [NSArray arrayWithObject:[userInfoDic objectForKey:@"result"]];
//        
//        NSLog(@"tempArray : %@", tempArray);
        
        firstSeq = [[[[userInfoDic objectForKey:@"result"] lastObject] objectForKey:@"key"]intValue] ;
        
        NSLog(@"firstSeq : %d", firstSeq);
    }
    
}



#pragma mark -- button actions 


- (IBAction)btnSignUpAction:(UIButton *)sender {


    //    UIStoryboard *storyboard = [UIStoryboard storyboardWithName:@"Main" bundle:[NSBundle mainBundle]];
    SignUpViewController *viewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"SignUp"];
    
    [self.navigationController pushViewController:viewCont animated:YES];
    
    

}

- (IBAction)btnTurnAction:(UIButton *)sender {
    [seqView setHidden:NO];
}

- (IBAction)btnBackAction:(UIButton *)sender {
    [seqView setHidden:YES];
    
}

- (IBAction)btnTurnOKAction:(UIButton *)sender {
    [seqView setHidden:YES];
}

- (IBAction)btnMessageAction:(UIButton *)sender {
    
    
    NSLog(@"btnMessageAction");
    
    NSString *messageString = [NSString stringWithFormat:@"%d회차 예상 번호를 메일로 다시 받으시겠습니까?\n재발송에 1분 정도 소요됩니다.", recentSeqNum];

    UIAlertController * alertController = [UIAlertController alertControllerWithTitle: @"메일 재발송"
                                                                              message:messageString
                                                                       preferredStyle:UIAlertControllerStyleAlert];

    UIAlertAction * logout = [UIAlertAction actionWithTitle:@"재발송" style:UIAlertActionStyleDefault
                                                    handler:^(UIAlertAction * action) {
                                                        // TO DO what you want
                                                        NSLog(@"재발송");

                                                        [self resendUserNumbers:recentSeqNum];
                                                  }];


    UIAlertAction * cancel = [UIAlertAction actionWithTitle:@"취소" style:UIAlertActionStyleDefault
                                                    handler:^(UIAlertAction * action) {

                                                        NSLog(@"cancel btn");
                                                        [alertController dismissViewControllerAnimated:YES completion:nil];

                                                    }];
    [alertController addAction:logout];
    [alertController addAction:cancel];
    [self presentViewController:alertController animated:YES completion:nil];
    
//    if(messageCount == 0){
//        //alert
//        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
//                                                                                 message:@"수신된 메세지가 없습니다."
//                                                                          preferredStyle:UIAlertControllerStyleAlert];
//        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
//                                                           style:UIAlertActionStyleDefault
//                                                         handler:nil];
//        [alertController addAction:actionOk];
//        
////        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
//        [self presentViewController:alertController animated:YES completion:nil];
//    
//    }else{
//        MessageViewController *viewCont = [self.storyboard instantiateViewControllerWithIdentifier:@"messageView"];
//        
//        //    viewCont.userToken = userToken;
//        
//        //    [self clearMsgCount];
//        
//        [self.navigationController pushViewController:viewCont animated:YES];
//    
//    }

}




- (IBAction)SeqforwardAction:(UIButton *)sender {
    NSLog(@"forward");
    
    NSLog(@"presentSeqNum++ : %d", presentSeqNum);
    
    [myIndicator startAnimating];
    
    presentSeqNum++;
    
    [self.btnBackward setHidden:NO];
    
    if(presentSeqNum == recentSeqNum){
        
       [self.btnForward setHidden:YES];
        
        lblTurn.text = [NSString stringWithFormat:@"%d회차 (예상)", presentSeqNum];
        
    }else{
        
        [self.btnForward setHidden:NO];
        
        lblTurn.text = [NSString stringWithFormat:@"%d회차 (완료)", presentSeqNum];
    }
    
    [self performSelectorOnMainThread:@selector(loadingUserNumbers) withObject:myIndicator waitUntilDone:NO];
}




- (IBAction)SeqBackAction:(UIButton *)sender {
    NSLog(@"backward");
    
    [myIndicator startAnimating];
    
    presentSeqNum--;
    
    NSLog(@"presentSeqNum-- : %d", presentSeqNum);
    
    [self.btnForward setHidden:NO];
    
    if(presentSeqNum == 0 || presentSeqNum == firstSeq){
        [self.btnBackward setHidden:YES];
    }
    
    lblTurn.text = [NSString stringWithFormat:@"%d회차 (완료)", presentSeqNum];
    
    [self performSelectorOnMainThread:@selector(loadingUserNumbers) withObject:myIndicator waitUntilDone:NO];
    
}


- (void)loadingUserNumbers{
    
    [self getNumbers:presentSeqNum];
    
    [myIndicator stopAnimating];
}


#pragma mark UITableViewDelegate

-(void)tableView:(UITableView *)tableView willDisplayCell:(UITableViewCell *)cell forRowAtIndexPath:(NSIndexPath *)indexPath{
    
    if ([tableView respondsToSelector:@selector(setSeparatorInset:)]) {
        [tableView setSeparatorInset:UIEdgeInsetsZero];
    }
    
    if ([tableView respondsToSelector:@selector(setLayoutMargins:)]) {
        [tableView setLayoutMargins:UIEdgeInsetsZero];
    }
    
    if ([cell respondsToSelector:@selector(setLayoutMargins:)]) {
        [cell setLayoutMargins:UIEdgeInsetsZero];
    }
}

// Header
-(UIView *)tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)section
{
    if(tableView.tag == 111){
        
        /* Create custom view to display section header... */
        
        AdsViewCell * header = [[[NSBundle mainBundle] loadNibNamed:@"AdsViewCell" owner:self options:nil] objectAtIndex:0];
        
        // banner view
        
        header.GADBannerView.adUnitID = AdUnitId;
        header.GADBannerView.delegate = self;
        header.GADBannerView.rootViewController = self;
        
        GADRequest *request = [GADRequest request];
#if !(TARGET_IPHONE_SIMULATOR)
        
#else
        request.testDevices = @[ kGADSimulatorID ];
#endif
        [header.GADBannerView loadRequest:request];
        
        return header;
    }else{
        return nil;
    }
}

// Header Height
- (CGFloat)tableView:(UITableView *)tableView heightForHeaderInSection:(NSInteger)section
{
    if(tableView.tag == 111){
        
        if(adReceived){
            return 51.0;
        }else{
            return 0;
        }
    }else{
        return 0;
    }
}


// This will tell your UITableView how many rows you wish to have in each section.
- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if(tableView.tag == 111){
        // Prize Table
        return [numbersArray count];
    }else{
        return 55;
    }
}

// This will tell your UITableView what data to put in which cells in your table.
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    
    if(tableView.tag == 111){
        // Numbers Table
        static NSString *simpleTableIdentifier = @"NumbersCell";
        
        NumbersCell *cell = [tableView dequeueReusableCellWithIdentifier:simpleTableIdentifier];
        
        if (cell == nil) {
            NSArray *nib = [[NSBundle mainBundle] loadNibNamed:@"NumbersCell" owner:self options:nil];
            cell = [nib objectAtIndex:0];
        }
        
        [cell.whiteView setHidden:YES];
        
        cell.num1.text = [NSString stringWithFormat:@"%@", [[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit1"]];
        cell.num2.text = [NSString stringWithFormat:@"%@", [[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit2"]];
        cell.num3.text = [NSString stringWithFormat:@"%@", [[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit3"]];
        cell.num4.text = [NSString stringWithFormat:@"%@", [[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit4"]];
        cell.num5.text = [NSString stringWithFormat:@"%@", [[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit5"]];
        cell.num6.text = [NSString stringWithFormat:@"%@", [[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit6"]];
        
        cell.imgNum1.image = [UIImage imageNamed:[self makeImgName:[[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit1"]intValue]]];
        cell.imgNum2.image = [UIImage imageNamed:[self makeImgName:[[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit2"]intValue]]];
        cell.imgNum3.image = [UIImage imageNamed:[self makeImgName:[[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit3"]intValue]]];
        cell.imgNum4.image = [UIImage imageNamed:[self makeImgName:[[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit4"]intValue]]];
        cell.imgNum5.image = [UIImage imageNamed:[self makeImgName:[[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit5"]intValue]]];
        cell.imgNum6.image = [UIImage imageNamed:[self makeImgName:[[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"digit6"]intValue]]];
        
        if([[[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"ranking"]stringValue] isEqualToString:@"6"]){
            [cell.whiteView setHidden:NO];
            cell.label1.text = @"당첨 결과 : X";
            cell.label2.text = [NSString stringWithFormat:@"당첨금 : %@원", [numberFormatter stringFromNumber:[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"amount"]]];
        }else if([[[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"ranking"]stringValue] isEqualToString:@"0"]){
            cell.label1.text = [NSString stringWithFormat:@"추첨 일자 : %@", recentSeqDate];
            cell.label2.text = @"1등 당첨을 기원합니다";
        }else{
            cell.label1.text = [NSString stringWithFormat:@"당첨 결과 : %@등", [numberFormatter stringFromNumber:[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"ranking"]]];
            cell.label2.text = [NSString stringWithFormat:@"당첨금 : %@원", [numberFormatter stringFromNumber:[[numbersArray objectAtIndex:indexPath.row] objectForKey:@"amount"]]];
        }

        UIColor *bColor = [UIColor whiteColor];
        
        if(indexPath.row % 2 != 0){
            bColor = [UIColor colorWithRed:210/255.f green:210/255.f blue:210/255.f alpha:0.6];
        }
        
        cell.backgroundColor = bColor;
        
        return cell;
        
    }else{
        // Seq Table
        static NSString *CellIdentifier = @"SeqCell";
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
            
        }
        
        cell.accessoryType = UITableViewCellAccessoryNone;
        cell.textLabel.text = [NSString stringWithFormat:@"%@회", [seqArray objectAtIndex:indexPath.row]];
        if(indexPath.row == 0){
            cell.textLabel.text = [NSString stringWithFormat:@"%@회 (예상번호)", [seqArray objectAtIndex:indexPath.row]];
        }
        cell.textLabel.textAlignment = NSTextAlignmentCenter;
        
        return cell;
    }
    
}

//- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{
//    
//    if(tableView.tag == 99){
//        
//        if(indexPath.row == 0){
//            lblDate.text = [NSString stringWithFormat:@"추첨일 : %@", recentSeqDate];
//            [rightButton setEnabled:NO];
//            
//        }else{
//            lblDate.text = @"추첨완료";
//            [rightButton setEnabled:YES];
//        }
//        
//        lastSeqNum = [[seqArray objectAtIndex:indexPath.row]intValue];
//        
//        NSLog(@"Did Select : %d", lastSeqNum);
//        
//        [self getNumbers:lastSeqNum];
//        
//        lblTurn.text = [NSString stringWithFormat:@"%d회차", lastSeqNum];
//        
//        [numbersTable reloadData];
//        
//        
//        [seqView setHidden:YES];
//        
//        
//    }
//}


#pragma mark -- Network


- (void)resendUserNumbers:(int)seqNo{
    
    
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
            [self excuteLogout];
            return;
        }else{
            responseDict = [appDelegate.networkInstance SendChoicedNumbers:appDelegate.userToken SeqNo:seqNo];
        }
        
    }
    @catch (NSException *exception){
        
        [self excuteLogout];
        
    }
    @finally{
        
        NSLog(@"resendUserNumbers Sended");
        
        BOOL returnBoolean = [[responseDict objectForKey:@"success"]boolValue];
        
        if(returnBoolean){
            // Back
            NSLog(@"resendUserNumbers Success!!!");
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"가입하신 이메일주소로 예상번호를 발송했습니다."
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
            
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
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


#pragma LOGOUT

- (void)excuteLogout{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    appDelegate.userToken = @"";
    userToken = @"";
    
    userInfoDic = nil;
    
    LoginViewController *viewCont2 = [self.storyboard instantiateViewControllerWithIdentifier:@"loginView"];
    
    [self.navigationController pushViewController:viewCont2 animated:YES];
    
    //alert
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                             message:@"네트워크 연결 불안정으로 로그아웃 되었습니다.\n연결상태 확인 후 앱을 재실행 해주세요."
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
