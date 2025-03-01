//
//  SettingViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 6..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SettingViewController : UITableViewController

@property (strong, nonatomic) IBOutlet UITableViewCell *Cell1;

@property (strong, nonatomic) IBOutlet UITableViewCell *Cell2;

@property (strong, nonatomic) IBOutlet UITableViewCell *Cell3;

@property (weak, nonatomic) IBOutlet UITableViewCell *inboxCell;


@property (strong, nonatomic) NSString * userTokenForSetting;

@property (nonatomic, strong) UIAlertController * filterView;

@property (nonatomic, strong) NSDictionary * userInfoDic;


- (void)getUserInfo:(NSString*)userToken;
- (void)setSettingView;
- (void)checkUserToken;

@end
