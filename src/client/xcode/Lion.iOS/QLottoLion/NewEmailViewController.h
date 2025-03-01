//
//  NewEmailViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 17..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface NewEmailViewController : UITableViewController <UITextFieldDelegate>

@property (weak, nonatomic) IBOutlet UITableViewCell *confirmSendCell;
@property (weak, nonatomic) IBOutlet UITableViewCell *confirmOKCell;

@property (weak, nonatomic) IBOutlet UITextField *tfEmail;
@property (weak, nonatomic) IBOutlet UITextField *tfCode;


@end
