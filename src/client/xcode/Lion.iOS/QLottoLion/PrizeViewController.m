//
//  PrizeViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 28..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "PrizeViewController.h"
#import "prizeCell.h"
#import "AdsViewCell.h"


static NSString *const AdUnitId = @"ca-app-pub-8599301686845489/3287348059";

@interface PrizeViewController () <GADVideoControllerDelegate, GADBannerViewDelegate> {

    NSDictionary * ThisWeekPrizeDic;
    NSDictionary * PrizeBySeqDic;
    int lastSeqNum;
    int tempNum;
    
    NSArray *amountArray;
    NSArray *countArray;
    NSMutableArray *seqArray;
    
    NSNumberFormatter *numberFormatter;
    
    BOOL adReceived;
}

@end

@implementation PrizeViewController

@synthesize SeqView, prizeTableView, imgNum1, imgNum2, imgNum3, imgNum4, imgNum5, imgNum6, imgNum7, internetView, myLoadIndicator;
@synthesize lblSeq, lblSeq2, lblDate, lblDate2, lblPrize, lblPrize2, lblNum1, lblNum2, lblNum3, lblNum4, lblNum5, lblNum6, lblNum7;

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    adReceived = YES;
    
    [SeqView setHidden:YES];
    [internetView setHidden:YES];
    
    numberFormatter = [[NSNumberFormatter alloc] init];
    [numberFormatter setLocale:[NSLocale currentLocale]];
    [numberFormatter setNumberStyle:NSNumberFormatterDecimalStyle];
    
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    if(appDelegate.guestToken == nil || [appDelegate.guestToken isEqualToString:@""]){
        
        [self getGuestTokenForPrizeView];
        [self getThisWeekPrizeDic];
    }
    
    
    if(appDelegate.recentSeqNum){
        [self labelsSetting];
        
        lastSeqNum = appDelegate.recentSeqNum -1;
        tempNum = lastSeqNum;
        
        [self getPrizeBySeqDic:lastSeqNum];
        
        [prizeTableView reloadData];
        
        [self configureView];
        [self makeSeqCount];
    }
    
}

-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];

}

- (void)viewDidAppear:(BOOL)animated{
    [super viewDidAppear:animated];
    
    // reload
    [self getThisWeekPrizeDic];
    
    [self labelsSetting];
    
    if(tempNum != lastSeqNum){
        
        [myLoadIndicator startAnimating];
        
        tempNum = lastSeqNum;
        
//        [self performSelectorOnMainThread:@selector(getPrizePerform) withObject:myLoadIndicator waitUntilDone:NO];
        
        [self getPrizeBySeqDic:lastSeqNum];
        
        [prizeTableView reloadData];
        
        [self configureView];
        
        [myLoadIndicator stopAnimating];
    }
}


-(void)getPrizePerform{
    
    [self getPrizeBySeqDic:lastSeqNum];
    
    [prizeTableView reloadData];
    
    [myLoadIndicator stopAnimating];
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

- (void)labelsSetting{
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];

    lblSeq.text = [NSString stringWithFormat:@"%d회차", appDelegate.recentSeqNum];
    lblDate.text = [NSString stringWithFormat:@"%@",
                    [TimeFormatter getStringFromDatetime: [[ThisWeekPrizeDic objectForKey:@"result"] objectForKey:@"issueDate"]]];
    lblPrize.text = [NSString stringWithFormat:@"%@원", [numberFormatter stringFromNumber:[[ThisWeekPrizeDic objectForKey:@"result"] objectForKey:@"predictAmount"]]];
    lblPrize2.text = [NSString stringWithFormat:@"이번주 누적 판매금액 : %@원", [numberFormatter stringFromNumber:[[ThisWeekPrizeDic objectForKey:@"result"] objectForKey:@"salesAmount"]]];

}


- (void)getGuestTokenForPrizeView{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    @try{
        appDelegate.guestToken = [appDelegate.networkInstance getGuestToken];
    
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
        //retry
        [self getGuestTokenForPrizeView];
    }
    @finally{
        NSLog(@"guestToken : %@", appDelegate.guestToken);
        
    }

}


- (void)configureView
{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    lblSeq.text = [NSString stringWithFormat:@"%d회차", appDelegate.recentSeqNum];
    lblSeq2.text = [NSString stringWithFormat:@"%d회차", tempNum];
    lblDate.text = [NSString stringWithFormat:@"%@",
                    [TimeFormatter getStringFromDatetime: [[ThisWeekPrizeDic objectForKey:@"result"] objectForKey:@"issueDate"]]];
    lblDate2.text = [NSString stringWithFormat:@"%@",
                    [TimeFormatter getStringFromDatetime: [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"issueDate"]]];
    
    lblPrize.text = [NSString stringWithFormat:@"%@원", [numberFormatter stringFromNumber:[[ThisWeekPrizeDic objectForKey:@"result"] objectForKey:@"predictAmount"]]];
    lblPrize2.text = [NSString stringWithFormat:@"이번주 누적 판매금액 : %@원", [numberFormatter stringFromNumber:[[ThisWeekPrizeDic objectForKey:@"result"] objectForKey:@"salesAmount"]]];
    // Numbers
    lblNum1.text = [[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit1"]stringValue];
    lblNum2.text = [[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit2"]stringValue];
    lblNum3.text = [[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit3"]stringValue];
    lblNum4.text = [[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit4"]stringValue];
    lblNum5.text = [[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit5"]stringValue];
    lblNum6.text = [[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit6"]stringValue];
    lblNum7.text = [[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit7"]stringValue];
    // Image
    imgNum1.image =[UIImage imageNamed:[self makeImgName:[[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit1"]intValue]]];
    imgNum2.image =[UIImage imageNamed:[self makeImgName:[[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit2"]intValue]]];
    imgNum3.image =[UIImage imageNamed:[self makeImgName:[[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit3"]intValue]]];
    imgNum4.image =[UIImage imageNamed:[self makeImgName:[[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit4"]intValue]]];
    imgNum5.image =[UIImage imageNamed:[self makeImgName:[[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit5"]intValue]]];
    imgNum6.image =[UIImage imageNamed:[self makeImgName:[[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit6"]intValue]]];
    imgNum7.image =[UIImage imageNamed:[self makeImgName:[[[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"digit7"]intValue]]];
    
    amountArray = [NSArray arrayWithObjects:
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"amount1"],
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"amount2"],
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"amount3"],
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"amount4"],
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"amount5"], nil];
    
    countArray = [NSArray arrayWithObjects:
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"count1"],
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"count2"],
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"count3"],
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"count4"],
                   [[PrizeBySeqDic objectForKey:@"result"] objectForKey:@"count5"], nil];
    
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



- (void)makeSeqCount{
    
    int tempSeq = lastSeqNum;
    seqArray = [[NSMutableArray alloc] init];
    for(int i = 0; i < 100; i++){
        [seqArray addObject:[NSString stringWithFormat:@"%d",tempSeq - i]];
    }
    
//    NSLog(@"seqArray : %@", seqArray);
}




#pragma mark -- network

- (void)getThisWeekPrizeDic{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary *responseDict;
    
    @try {
        
        if(appDelegate.guestToken == nil || [appDelegate.guestToken isEqualToString:@""]){
            if([appDelegate.guestToken isEqualToString:@"nil value"]){
                return;
            }else{
                responseDict = [appDelegate.networkInstance GetThisWeekPrize:appDelegate.guestToken];
            }
        
        }else{
            responseDict = [appDelegate.networkInstance GetThisWeekPrize:appDelegate.guestToken];
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
        ThisWeekPrizeDic = responseDict;
        
        appDelegate.recentSeqNum = [[[ThisWeekPrizeDic objectForKey:@"result"] objectForKey:@"sequenceNo"]intValue] ;
        
        appDelegate.recentSeqDate = [NSString stringWithFormat:@"%@", [TimeFormatter getStringFromDatetime: [[ThisWeekPrizeDic objectForKey:@"result"] objectForKey:@"issueDate"]]];
    }
    
    NSLog(@"ThisWeekPrizeDic = %@", ThisWeekPrizeDic);
    NSLog(@"recentSeqNum = %d", appDelegate.recentSeqNum);
    
    [myLoadIndicator stopAnimating];
}



- (void)getPrizeBySeqDic: (int)_seq{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    NSDictionary *responseDict;
    
    @try {
        
        if(appDelegate.guestToken == nil || [appDelegate.guestToken isEqualToString:@""]){
            if([appDelegate.guestToken isEqualToString:@"nil value"]){
                return;
            }else{
                responseDict = [appDelegate.networkInstance GetPrizeBySeqNo:appDelegate.guestToken SeqNum:_seq];
            }
        }else{
        
            responseDict = [appDelegate.networkInstance GetPrizeBySeqNo:appDelegate.guestToken SeqNum:_seq];
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
        PrizeBySeqDic = responseDict;
        
//        [prizeTableView reloadData];
        
        [myLoadIndicator startAnimating];
        
        [prizeTableView performSelectorOnMainThread:@selector(reloadData) withObject:myLoadIndicator waitUntilDone:NO];
        
        [myLoadIndicator stopAnimating];
    }
    
    NSLog(@"PrizeBySeqDic = %@", PrizeBySeqDic);
    
//    [myLoadIndicator stopAnimating];
}


#pragma mark Button Actions

- (IBAction)btnTurnAction:(UIButton *)sender {
    NSLog(@"btnTurnButton");
    [SeqView setHidden:NO];
}
- (IBAction)btnCloseAction:(UIButton *)sender {
    [SeqView setHidden:YES];
}

- (IBAction)btnOkAction:(UIButton *)sender {
    
//    [self getPrizeBySeqDic:lastSeqNum];
    
    [SeqView setHidden:YES];
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
    if(tableView.tag == 11){
    
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
    if(tableView.tag == 11){
        
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
    if(tableView.tag == 11){
        // Prize Table
        return 5;
    }else{
        return 55;
    }
}

// This will tell your UITableView what data to put in which cells in your table.
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    
    if(tableView.tag == 11){
        // Prize Table
        static NSString *simpleTableIdentifier = @"prizeCell";
        
        prizeCell *cell = [tableView dequeueReusableCellWithIdentifier:simpleTableIdentifier];
        
        if (cell == nil) {
            NSArray *nib = [[NSBundle mainBundle] loadNibNamed:@"prizeCell" owner:self options:nil];
            cell = [nib objectAtIndex:0];
        }
        
        cell.label1.text = [NSString stringWithFormat:@"%ld등", (long)indexPath.row+1];
        cell.label2.text = [NSString stringWithFormat:@"%@명", [numberFormatter stringFromNumber:[countArray objectAtIndex:indexPath.row]]];
        cell.label3.text = [NSString stringWithFormat:@"%@원", [numberFormatter stringFromNumber:[amountArray objectAtIndex:indexPath.row]]];
        
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
//        cell.selectionStyle = UITableViewCellSelectionStyleNone;
        
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
            
        }
        
        cell.accessoryType = UITableViewCellAccessoryNone;
        cell.textLabel.text = [NSString stringWithFormat:@"%@회", [seqArray objectAtIndex:indexPath.row]];
        cell.textLabel.textAlignment = NSTextAlignmentCenter;
        
        return cell;
    }
    
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{

    if(tableView.tag == 99){
        
        
        tempNum = [[seqArray objectAtIndex:indexPath.row]intValue];
        
        NSLog(@"Did Select : %d", tempNum);
        
        [myLoadIndicator startAnimating];
        
        [self getPrizeBySeqDic:tempNum];
        
//        [prizeTableView reloadData];
        
//        [self performSelectorOnMainThread:@selector(getPrizeDicPerformTempNum) withObject:myLoadIndicator waitUntilDone:NO];
        
        [self configureView];
        
        [SeqView setHidden:YES];
        
        
        [tableView deselectRowAtIndexPath:indexPath animated:YES];
    }
}


//- (void)getPrizeDicPerformTempNum{
//    
//    [self getPrizeBySeqDic:tempNum];
//    
//    [myLoadIndicator stopAnimating];
//}

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
        
        [internetView setHidden:NO];
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
        
        [internetView setHidden:YES];
    
        
    }

}


#pragma mark - GADNativeExpressAdViewDelegate

- (void)nativeExpressAdViewDidReceiveAd:(GADNativeExpressAdView *)nativeExpressAdView {
    if (nativeExpressAdView.videoController.hasVideoContent) {
        NSLog(@"Received ad an with a video asset.");
    } else {
        NSLog(@"Received ad an without a video asset.");
    }
}

#pragma mark - GADVideoControllerDelegate

- (void)videoControllerDidEndVideoPlayback:(GADVideoController *)videoController {
    NSLog(@"Playback has ended for this ad's video asset.");
}

#pragma mark - banner delegate

- (void)adView:(GADBannerView *)adView didFailToReceiveAdWithError:(GADRequestError *)error {
    NSLog(@"adView:didFailToReceiveAdWithError: %@", error.localizedDescription);
}

@end
