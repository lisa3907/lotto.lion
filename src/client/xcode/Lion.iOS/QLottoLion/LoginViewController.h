//
//  LoginViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 11..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface LoginViewController : UITableViewController <UITextFieldDelegate>

@property (strong, nonatomic) IBOutlet UITableViewCell *loginButtonCell;

@property (strong, nonatomic) IBOutlet UITableViewCell *signUpButtonCell;

@property (weak, nonatomic) IBOutlet UITableViewCell *renewButtonCell;


@property (strong, nonatomic) IBOutlet UITextField *tfID;
@property (strong, nonatomic) IBOutlet UITextField *tfPW;

@property (weak, nonatomic) IBOutlet UISwitch *switchID;
@property (weak, nonatomic) IBOutlet UISwitch *switchPW;

- (IBAction)IDSwitchAction:(UISwitch *)sender;
- (IBAction)PWSwitchAction:(UISwitch *)sender;


@end
