//
//  NumbersViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 28..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>


@interface NumbersViewController : UIViewController <UITextFieldDelegate, UITableViewDelegate, UITableViewDataSource, GADBannerViewDelegate>

@property (strong, nonatomic) IBOutlet UIView *loginView;

@property (strong, nonatomic) IBOutlet UILabel *lblTurn;

@property (strong, nonatomic) IBOutlet UITableView *numbersTable;

@property (strong, nonatomic) IBOutlet UIView *seqView;

@property (strong, nonatomic) IBOutlet UITableView *seqTable;

@property (strong, nonatomic) IBOutlet UILabel *lblDate;

//@property (strong, nonatomic) IBOutlet UIImageView *badgeImage;

//@property (weak, nonatomic) IBOutlet UILabel *badgeCount;

@property (weak, nonatomic) IBOutlet UILabel *noticeLabel;

@property (weak, nonatomic) IBOutlet UIButton *btnForward;

@property (weak, nonatomic) IBOutlet UIButton *btnBackward;


@property (strong, nonatomic) NSArray * numbersArray;

@property int recentSeqNum;

@property (strong, nonatomic) NSString * recentSeqDate;

@property (strong, nonatomic) NSString * userToken;


@property (weak, nonatomic) IBOutlet UIActivityIndicatorView *myIndicator;

//- (void)getMessageCount;

- (void)checkLoginAndMove;

@end
