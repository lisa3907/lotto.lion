//
//  SignUpViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 12..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SignUpViewController : UITableViewController <UITextFieldDelegate>

@property (strong, nonatomic) IBOutlet UITextField *tfEmail;
@property (strong, nonatomic) IBOutlet UITextField *tfConfirm;
@property (strong, nonatomic) IBOutlet UITextField *tfID;
@property (strong, nonatomic) IBOutlet UITextField *tfName;
@property (strong, nonatomic) IBOutlet UITextField *tfPassword;
@property (strong, nonatomic) IBOutlet UITextField *tfPassword2nd;


@property (weak, nonatomic) IBOutlet UITableViewCell *confirmButtonCell;
@property (weak, nonatomic) IBOutlet UITableViewCell *confirmButtonCell2;
@property (weak, nonatomic) IBOutlet UITableViewCell *signUpButtonCell;

@end
