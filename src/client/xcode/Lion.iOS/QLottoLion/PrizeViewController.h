//
//  PrizeViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 28..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface PrizeViewController : UIViewController <UITableViewDelegate, UITableViewDataSource>


@property (strong, nonatomic) IBOutlet UITableView *prizeTableView;

@property (strong, nonatomic) IBOutlet UILabel *lblSeq;
@property (strong, nonatomic) IBOutlet UILabel *lblSeq2;

@property (strong, nonatomic) IBOutlet UILabel *lblDate;
@property (strong, nonatomic) IBOutlet UILabel *lblDate2;

@property (strong, nonatomic) IBOutlet UILabel *lblPrize;
@property (strong, nonatomic) IBOutlet UILabel *lblPrize2;

@property (strong, nonatomic) IBOutlet UILabel *lblNum1;
@property (strong, nonatomic) IBOutlet UILabel *lblNum2;
@property (strong, nonatomic) IBOutlet UILabel *lblNum3;
@property (strong, nonatomic) IBOutlet UILabel *lblNum4;
@property (strong, nonatomic) IBOutlet UILabel *lblNum5;
@property (strong, nonatomic) IBOutlet UILabel *lblNum6;
@property (strong, nonatomic) IBOutlet UILabel *lblNum7;

@property (strong, nonatomic) IBOutlet UIView *SeqView;

//@property (strong, nonatomic) NSMutableArray *prizeArray;

@property (weak, nonatomic) IBOutlet UIView *internetView;

@property (weak, nonatomic) IBOutlet UIActivityIndicatorView *myLoadIndicator;


@property (weak, nonatomic) IBOutlet UIImageView *imgNum1;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum2;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum3;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum4;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum5;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum6;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum7;

@end
