//
//  NumbersCell.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 5..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface NumbersCell : UITableViewCell

@property (strong, nonatomic) IBOutlet UILabel *num1;
@property (strong, nonatomic) IBOutlet UILabel *num2;
@property (strong, nonatomic) IBOutlet UILabel *num3;
@property (strong, nonatomic) IBOutlet UILabel *num4;
@property (strong, nonatomic) IBOutlet UILabel *num5;
@property (strong, nonatomic) IBOutlet UILabel *num6;

@property (strong, nonatomic) IBOutlet UILabel *label1;
@property (strong, nonatomic) IBOutlet UILabel *label2;

@property (weak, nonatomic) IBOutlet UIImageView *imgNum1;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum2;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum3;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum4;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum5;
@property (weak, nonatomic) IBOutlet UIImageView *imgNum6;

@property (weak, nonatomic) IBOutlet UIView *whiteView;


@end
