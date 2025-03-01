//
//  SignUpViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 31..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface SignUpViewController2 : UIViewController <UITextFieldDelegate>

@property (strong, nonatomic) IBOutlet UITextField *tfEmail;
@property (strong, nonatomic) IBOutlet UITextField *tfConfirm;
@property (strong, nonatomic) IBOutlet UITextField *tfID;
@property (strong, nonatomic) IBOutlet UITextField *tfName;
@property (strong, nonatomic) IBOutlet UITextField *tfPassword;

@end
