//
//  Networking.h
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 29..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface Networking : NSObject

- (NSString*) getGuestToken;

- (NSDictionary*) GetThisWeekPrize: (NSString*)_guest_token;

- (NSDictionary*) GetPrizeBySeqNo: (NSString*)_guest_token SeqNum: (int)_sequence_no;

- (NSDictionary*) SendMailToCheckMailAddress: (NSString*)_guest_token Email: (NSString*)_email;

- (BOOL) CheckMailAddress: (NSString*)_guest_token Email: (NSString*)_email Checknumber: (NSString*)_check_number;

- (NSDictionary*) ChangeMailAddress: (NSString*)_guest_token Email: (NSString*)_email;

- (NSDictionary*) GetTokenByLoginId: (NSString*)_guest_token
                            LoginID: (NSString*)_login_id
                           Password: (NSString*)_password DeviceType:(NSString*)_device_type DeviceID:(NSString*)_device_id;


- (NSDictionary*) AddMemberByLoginId: (NSString*)_guest_token
                    LoginID: (NSString*)_loginID
                  LoginName: (NSString*)_login_name PassWord: (NSString*)_password Email:(NSString *)_email_address
                   DeviceID: (NSString *)_device_id
                 DeviceType: (NSString*)_device_type
                CheckNumber: (NSString*)_check_number;

- (NSDictionary*) GetUserInfor: (NSString*)_userToken;

- (NSDictionary*) GetUserChoices: (NSString*)_userToken SeqNo:(int)_sequence_no;

- (NSDictionary*) GetAlertMessage: (NSString*)_userToken Time:(NSString*)_time;

- (int) GetAlertCount: (NSString*)_userToken;

- (BOOL) ClearAlert: (NSString*)_userToken;

- (BOOL) UpdateUserInfo: (NSString*)_userToken LoginName: (NSString*)_login_name MaxSelectNumber:(NSString *)_max_select_number
                   Num1:(NSString*)_num1 Num2:(NSString*)_num2 Num3:(NSString*)_num3;


- (NSDictionary*) ChangePassword: (NSString *)_userToken NewPassword: (NSString *)_newPassword;

- (NSDictionary*) UpdateMailAddress: (NSString *)_userToken MailAddress: (NSString *)_mailAddress ConfirmCode:(NSString *)_confirmCode;

- (BOOL) LeaveMember: (NSString*)_userToken;

- (NSDictionary*) SendMailToRecoveryId: (NSString *)_userToken MailAddress: (NSString *)_mailAddress;


- (NSDictionary*) GetUserSequenceNos: (NSString *)_userToken;

- (NSDictionary*) SendChoicedNumbers: (NSString *)_userToken SeqNo:(int)_seqNo;


@end
