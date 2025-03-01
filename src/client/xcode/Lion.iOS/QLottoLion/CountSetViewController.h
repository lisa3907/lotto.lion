//
//  CountSetViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 6..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface CountSetViewController : UITableViewController

@property (nonatomic, strong) NSArray * countArray;

@property (nonatomic, strong) NSString * checkMode;

@property (nonatomic, strong) NSString * maxCount;

@property (nonatomic, strong) NSString * emailAddress;

@property (nonatomic, strong) NSMutableArray * SelectedRowCountArr;

@property (nonatomic, strong) UIAlertController * filterView;



@end
