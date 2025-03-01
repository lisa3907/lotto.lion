//
//  CountSetViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 6..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "CountSetViewController.h"

@interface CountSetViewController (){

    
    NSMutableArray * arrayToDelete;
    
    NSMutableArray * numbersData;
    
    BOOL editCheck;
    
}

@end

@implementation CountSetViewController
@synthesize countArray, checkMode, maxCount, SelectedRowCountArr;

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    maxCount = [appDelegate.userInfoDic objectForKey:@"maxSelectNumber"];
    
    editCheck = NO;
    
    NSLog(@"checkMode : %@", checkMode);
    NSLog(@"maxCount : %@", maxCount);
    
    if([checkMode isEqualToString:@"number"]){
        
        numbersData = [[NSMutableArray alloc]init];
        
        for (int i = 1 ; i <46 ; i++){
            
            NSString * temp = [NSString stringWithFormat:@"%d", i];
        
            [numbersData addObject:temp];
        }
        
        NSLog(@"SelectedRowCountArr : %@", SelectedRowCountArr);
        
        arrayToDelete = [[NSMutableArray alloc]init];
        
        if(SelectedRowCountArr == nil){
            SelectedRowCountArr = [[NSMutableArray alloc]init];
        }
        
        //set nav
        UIBarButtonItem *newAddButton = [[UIBarButtonItem alloc] initWithTitle:@"추가" style:UIBarButtonItemStylePlain target:self action:@selector(add:)];
        self.navigationItem.rightBarButtonItem = newAddButton;
        
        self.navigationItem.title = @"고정번호선택";
    
    }else if ([checkMode isEqualToString:@"count"]){
    
        self.navigationItem.title = @"번호생성갯수";
    }
    
}

-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];
    
}


- (void)viewWillDisappear:(BOOL)animated{
    [super viewWillDisappear:animated];
    
    NSLog(@"viewWillDisappear <>");
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    if([checkMode isEqualToString:@"number"] && appDelegate.edited == YES){

        if(SelectedRowCountArr.count == 0){
            [self UpdateInfoWithMaxCount:maxCount Num1:@"0" Num2:@"0" Num3:@"0"];
        }else if (SelectedRowCountArr.count == 1){
            [self UpdateInfoWithMaxCount:maxCount Num1:[SelectedRowCountArr objectAtIndex:0] Num2:@"" Num3:@""];
        }else if(SelectedRowCountArr.count == 2){
            [self UpdateInfoWithMaxCount:maxCount Num1:[SelectedRowCountArr objectAtIndex:0] Num2:[SelectedRowCountArr objectAtIndex:1] Num3:@""];
        }else{
            [self UpdateInfoWithMaxCount:maxCount Num1:[SelectedRowCountArr objectAtIndex:0] Num2:[SelectedRowCountArr objectAtIndex:1] Num3:[SelectedRowCountArr objectAtIndex:2]];
        }

    }
    
    
}



- (void)add:(id)sender{
    NSLog(@"add");
    
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    
    if(SelectedRowCountArr.count >= 3){
        // Alert
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                 message:@"번호 선택은 최대 3개입니다."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
        
    }else{
    
        appDelegate.edited = YES;
        
        NSLog(editCheck ? @" edited : Yes" : @" edited : No");
    
        int i = 0;
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"번호선택"
                                                                                 message:@""
                                                                          preferredStyle:UIAlertControllerStyleActionSheet];
        
        for(NSString *item in numbersData){
            UIAlertAction *defaultAction = [UIAlertAction actionWithTitle:item
                                                               style:UIAlertActionStyleDefault
                                                           handler:^(UIAlertAction * action){
                                                               [self didSelectRowInAlertController:i];
                                                           }];
            [alertController addAction:defaultAction];
            i++;
        }
        
        UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleDefault handler:nil];
        
        [alertController addAction:cancelAction];
        
        self.filterView = alertController;
        [self presentViewController:self.filterView animated:YES completion:nil];
    
    }
}


-(void)didSelectRowInAlertController:(NSInteger)row{
    NSLog(@"select row : %ld", (long)row);
    
    NSString * item = [NSString stringWithFormat:@"%d", (int)row+1];
    
    [SelectedRowCountArr addObject:item];
    
    NSLog(@"SelectedRowCountArr : %@", SelectedRowCountArr);
    
    [self.tableView reloadData];
}



- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    
    if([checkMode isEqualToString:@"count"]){
        return [countArray count];
    }else if([checkMode isEqualToString:@"number"]){
        
        return [SelectedRowCountArr count];
        
    }else{
        return 0;
    }
    
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"Cell" forIndexPath:indexPath];
    cell.selectionStyle = UITableViewCellSelectionStyleNone;
    
    // Configure the cell...
    // reset cell accessory style
    cell.accessoryType = UITableViewCellAccessoryNone;
    
    if([checkMode isEqualToString:@"count"]){
        
        cell.selectionStyle = UITableViewCellSelectionStyleDefault;

        cell.textLabel.text = [NSString stringWithFormat:@"%@개", [[countArray objectAtIndex:indexPath.row]stringValue]];
        
        if([maxCount intValue] == [[countArray objectAtIndex:indexPath.row]intValue]){
            [cell setAccessoryType:UITableViewCellAccessoryCheckmark];
        }
        
    }else if([checkMode isEqualToString:@"number"]){
        
        cell.textLabel.text = [NSString stringWithFormat:@"%@", [SelectedRowCountArr objectAtIndex:indexPath.row]];
        
//        [cell setAccessoryType:UITableViewCellAccessoryNone];
//        for (int i = 0; i < SelectedRowCountArr.count; i++) {
//            NSUInteger num = [[SelectedRowCountArr objectAtIndex:i] intValue]-1;
//            
//            if (num == indexPath.row) {
//                [cell setAccessoryType:UITableViewCellAccessoryCheckmark];
//                // Once we find a match there is no point continuing the loop
//                break;
//            }
//        }
    }
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath{
    
    NSLog(@"select row : %ld", (long)indexPath.row);
    
    
    
    UITableViewCell *cell = [tableView cellForRowAtIndexPath:indexPath];
   
    
    if([checkMode isEqualToString:@"count"]){
        
        cell.accessoryType = UITableViewCellAccessoryCheckmark;
        
        [self UpdateInfoWithMaxCount:[[countArray objectAtIndex:indexPath.row]stringValue] Num1:[SelectedRowCountArr objectAtIndex:0] Num2:[SelectedRowCountArr objectAtIndex:1] Num3:[SelectedRowCountArr objectAtIndex:2]];
        
        [self.navigationController popViewControllerAnimated:YES];
        
        
    }else if([checkMode isEqualToString:@"number"]){
        
        
        [arrayToDelete addObject:SelectedRowCountArr[indexPath.row]];
        
        
        NSLog(@"The selected To : %@", arrayToDelete);
        
    }
    

    
}

-(void) tableView:(UITableView *)tableView didDeselectRowAtIndexPath:(NSIndexPath *)indexPath {
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    UITableViewCell *cell = [tableView cellForRowAtIndexPath:indexPath];
    
    if([checkMode isEqualToString:@"count"]){
    
        cell.accessoryType = UITableViewCellAccessoryNone;
        
    }else if([checkMode isEqualToString:@"number"]){
    
        cell.accessoryType = UITableViewCellAccessoryNone;
        [arrayToDelete removeObject:SelectedRowCountArr[indexPath.row]];
        NSLog(@"The selected To delete: %@", arrayToDelete);
    }
}


- (UITableViewCellEditingStyle)tableView:(UITableView *)tableView editingStyleForRowAtIndexPath:(NSIndexPath *)indexPath{

    return UITableViewCellEditingStyleDelete;
}

-(BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath{
    
    if([checkMode isEqualToString:@"number"]){
        return YES;
    }else{
        return NO;
    }
}

-(void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath{
    if([checkMode isEqualToString:@"number"]){
        AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
        
        appDelegate.edited = YES;
        
        NSLog(editCheck ? @" edited : Yes" : @" edited : No");
        
        [SelectedRowCountArr removeObjectAtIndex:indexPath.row];
        [self.tableView deleteRowsAtIndexPaths:[NSMutableArray arrayWithObject:indexPath] withRowAnimation:UITableViewRowAnimationFade];
        
        NSLog(@"SelectedRowCountArr : %@", SelectedRowCountArr);
        
    }

}

#pragma Mark Network

- (void)UpdateInfoWithMaxCount:(NSString*)_maxCount Num1:(NSString*)_num1 Num2:(NSString*)_num2 Num3:(NSString*)_num3{
    AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
    BOOL responseBoolean;
    
    
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
            
            NSString * userName = [appDelegate.userInfoDic objectForKey:@"loginName"];
            
            responseBoolean = [appDelegate.networkInstance UpdateUserInfo:appDelegate.userToken LoginName:userName MaxSelectNumber:_maxCount Num1:_num1 Num2:_num2 Num3:_num3];
        }
        
    }
    @catch (NSException *exception){
        
        
        
    }
    @finally{
        
        if(responseBoolean){
            NSLog(@"UpdateInfo >>> Success");
            appDelegate.edited = YES;
            [self getUserInfo:appDelegate.userToken];
            
        }else{
            NSLog(@"UpdateInfo >>> FAIL !!!!");
            UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"알림"
                                                                                     message:@"유저정보 변경에 실패했습니다.\n네트워크 환경을 확인해주세요."
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
