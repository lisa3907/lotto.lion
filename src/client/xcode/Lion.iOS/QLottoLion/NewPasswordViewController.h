//
//  NewPasswordViewController.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 4. 17..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface NewPasswordViewController : UITableViewController <UITextFieldDelegate>
@property (weak, nonatomic) IBOutlet UITableViewCell *changeOKCell;

@property (weak, nonatomic) IBOutlet UITextField *tfOriginal;
@property (weak, nonatomic) IBOutlet UITextField *tfNew;


@end
