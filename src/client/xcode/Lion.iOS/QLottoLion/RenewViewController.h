//
//  RenewViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 19..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface RenewViewController : UITableViewController <UITextFieldDelegate>

@property (weak, nonatomic) IBOutlet UITextField *tfEmail;

@property (weak, nonatomic) IBOutlet UITableViewCell *sendEmailButtonCell;


@end
