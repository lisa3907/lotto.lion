//
//  Networking.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 29..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "Networking.h"


#define SERVERURL @"http://lottoapi.odinsoftware.co.kr"

@implementation Networking

#pragma for NSData Request common Method

- (NSData *)sendSynchronousRequest:(NSURLRequest *)request returningResponse:(NSURLResponse **)response error:(NSError **)error
{
    
    NSError __block *err = NULL;
    NSData __block *data;
    BOOL __block reqProcessed = false;
    NSURLResponse __block *resp;
    
    [[[NSURLSession sharedSession] dataTaskWithRequest:request completionHandler:^(NSData * _Nullable _data, NSURLResponse * _Nullable _response, NSError * _Nullable _error) {
        resp = _response;
        err = _error;
        data = _data;
        reqProcessed = true;
    }] resume];
    
    while (!reqProcessed) {
        [NSThread sleepForTimeInterval:0];
    }
    
    *response = resp;
    *error = err;
    return data;
}


- (NSString*) getGuestToken{
    
    NSLog(@"<<< getGuestToken >>>");
    
    NSString * forReturnString;

    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/GetGuestToken",SERVERURL]]];
    // Create the Method "GET" or "POST"
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
//    [request setValue:@"application/xml; charset=utf-8" forHTTPHeaderField:@"Content-Type"];
    
    // Convert your data and set your request's HTTPBody property
//    NSString *stringData = @"some data";
//    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
//    request.HTTPBody = requestBodyData;
    

    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    if(error){
        NSLog(@">>>>>>>>> getGuestToken error: %@", error);
    }
    
    NSLog(@"getGuestToken responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
                           
//                           stringByRemovingPercentEncoding:NSUTF8StringEncoding];
    
    NSLog(@"getGuestToken message : %@", escapeStr);
    
    forReturnString = [responseDict objectForKey:@"result"];
    
    if(forReturnString != nil){
        return forReturnString;
    }else{
        return @"nil value";
    }
}


- (NSDictionary*) GetThisWeekPrize: (NSString*)_guest_token{
    
    NSLog(@"<<< GetThisWeekPrize >>>");
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/lotto/GetThisWeekPrize",SERVERURL]]];
    
    // Create the Method "GET" or "POST"
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _guest_token];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    //    NSString *stringData = @"some data";
    //    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    //    request.HTTPBody = requestBodyData;
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    if(error){
        NSLog(@">>>>>>>>> GetThisWeekPrize error: %@", error);
    }
    
    NSLog(@"GetThisWeekPrize responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetThisWeekPrize message : %@", escapeStr);
    
    return responseDict;
    
}


- (NSDictionary*) GetPrizeBySeqNo: (NSString*)_guest_token SeqNum: (int)_sequence_no{

    NSLog(@"<<< GetPrizeBySeqNo = %d >>>", _sequence_no);
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/lotto/GetPrizeBySeqNo",SERVERURL]]];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _guest_token];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    NSString *stringData = [NSString stringWithFormat:@"sequence_no=%d", _sequence_no];
    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    // Create the Method "GET" or "POST"
    [request setHTTPMethod:@"POST"];
    [request setHTTPBody: requestBodyData];
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    if(error){
        NSLog(@">>>>>>>>> GetPrizeBySeqNo error: %@", error);
    }
    
    NSLog(@"GetPrizeBySeqNo responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetPrizeBySeqNo message : %@", escapeStr);
    
    return responseDict;

}

- (NSDictionary*) SendMailToCheckMailAddress: (NSString*)_guest_token Email: (NSString*)_email{
    
    NSLog(@"<<< SendMailToCheckMailAddress = %@ >>>", _email);
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/SendMailToCheckMailAddress",SERVERURL]]];
    
    // Create the Method "GET" or "POST"
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _guest_token];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    NSString *stringData = [NSString stringWithFormat:@"mail_address=%@", _email];
    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    request.HTTPBody = requestBodyData;
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    if(error){
        NSLog(@">>>>>>>>> SendMailToCheckMailAddress error: %@", error);
    }
    
    NSLog(@"SendMailToCheckMailAddress responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"SendMailToCheckMailAddress message : %@", escapeStr);
    
    return responseDict;

}


- (BOOL) CheckMailAddress: (NSString*)_guest_token Email: (NSString*)_email Checknumber: (NSString*)_check_number{

    NSLog(@"<<< CheckMailAddress = %@ >>>", _email);
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/CheckMailAddress",SERVERURL]]];
    
    // Create the Method "GET" or "POST"
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _guest_token];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
//    NSDictionary *params = @{
//                             @"mail_address":_email,
//                             @"check_number":_check_number,
//                             };
//    
//    NSLog(@"params : %@", params);
//    
//    NSError *error;
//    //json format to send the data
//    NSData * jsondata = [NSJSONSerialization dataWithJSONObject:params
//                                                        options:NSJSONWritingPrettyPrinted
//                                                          error:&error];
//    [request setValue:@"application/json" forHTTPHeaderField:@"Content-type"];
//    [request setValue:@"application/json" forHTTPHeaderField:@"Accept"];
//    [request setHTTPMethod:@"POST"];
//    [request setValue:[NSString stringWithFormat:@"%lu", (unsigned long)[jsondata length]] forHTTPHeaderField:@"Content-Length"];
//    [request setHTTPBody:jsondata];
    
    
    NSString *parameter = [NSString stringWithFormat:@"mail_address=%@&check_number=%@", _email, _check_number];
    NSData *parameterData = [parameter dataUsingEncoding:NSASCIIStringEncoding allowLossyConversion:YES];
    [request setHTTPBody:parameterData];
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    if(error){
        NSLog(@">>>>>>>>> CheckMailAddress error: %@", error);
    }
    
    NSLog(@"CheckMailAddress responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"CheckMailAddress message : %@", escapeStr);
    
    BOOL  returnBoolean = [[responseDict objectForKey:@"success"]boolValue];
    
    
    return returnBoolean;
}


- (NSDictionary*) ChangeMailAddress: (NSString*)_userToken Email: (NSString*)_email{
    
    NSLog(@"<<< ChangeMailAddress = %@ >>>", _email);
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/SendMailToChangeMailAddress",SERVERURL]]];
    
    // Create the Method "GET" or "POST"
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];

    NSString *parameter = [NSString stringWithFormat:@"mail_address=%@", _email];
    NSData *parameterData = [parameter dataUsingEncoding:NSASCIIStringEncoding allowLossyConversion:YES];
    [request setHTTPBody:parameterData];
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    if(error){
        NSLog(@">>>>>>>>> ChangeMailAddress error: %@", error);
    }
    
    NSLog(@"ChangeMailAddress responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"ChangeMailAddress message : %@", escapeStr);
    
    return responseDict;
}


- (NSDictionary*) GetTokenByLoginId: (NSString*)_guest_token LoginID: (NSString*)_login_id Password: (NSString*)_password DeviceType:(NSString*)_device_type DeviceID:(NSString*)_device_id{

    NSLog(@"<<< GetTokenByLoginId = %@ >>>", _login_id );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/GetTokenByLoginId",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _guest_token];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    
//#if !(TARGET_IPHONE_SIMULATOR)
//    if (_device_id == nil){
//        _device_id = @"";
//    }
//#else
//    if (_device_id == nil){
//        _device_id = @"e0ca5695e264e7d9ef06f794f66ca37807ad22bda92aa5f48f66ae164568799b";
//    }
//    
//#endif
    

    NSString *parameter = [NSString stringWithFormat:@"login_id=%@&password=%@&device_type=%@&device_id=%@", _login_id, _password, _device_type, _device_id];
    
    NSLog(@"parameter = %@", parameter);
    NSData *parameterData = [parameter dataUsingEncoding:NSASCIIStringEncoding allowLossyConversion:YES];
    [request setHTTPBody:parameterData];
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> GetTokenByLoginId error: %@", error);
    }
    
    
    NSLog(@"GetTokenByLoginId responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetTokenByLoginId message : %@", escapeStr);
    
    return responseDict;
}


- (NSDictionary*) AddMemberByLoginId: (NSString*)_guest_token
                    LoginID: (NSString*)_loginID
                  LoginName: (NSString*)_login_name
                   PassWord: (NSString*)_password Email:(NSString *)_email_address
                   DeviceID: (NSString*)_device_id
                 DeviceType: (NSString*)_device_type
                CheckNumber: (NSString*)_check_number{
    
    NSLog(@"<<< AddMemberByLoginId = %@ >>>", _loginID);
    
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/AddMemberByLoginId",SERVERURL]]];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _guest_token];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Create the Method "GET" or "POST"
    [request setHTTPMethod:@"POST"];
    
    // Convert your data and set your request's HTTPBody property
    
//#if !(TARGET_IPHONE_SIMULATOR)
//    if (_device_id == nil){
//        _device_id = @"";
//    }
//#else
//    if (_device_id == nil){
//        _device_id = @"e0ca5695e264e7d9ef06f794f66ca37807ad22bda92aa5f48f66ae164568799b";
//    }
//#endif
    
    // ENCORDING //
//    NSUInteger encoding = CFStringConvertEncodingToNSStringEncoding(kCFStringEncodingEUC_KR);
//    const char * eucKRString = [_loginID cStringUsingEncoding:encoding];
//    NSString *_newLoginID = [NSString stringWithUTF8String:eucKRString];
    
    NSString *_newLoginID = [_loginID stringByAddingPercentEncodingWithAllowedCharacters:NSCharacterSet.URLQueryAllowedCharacterSet];
    
    //NSSTRING
    NSString *parameter = [NSString stringWithFormat:@"login_id=%@&login_name=%@&password=%@&mail_address=%@&device_type=%@&device_id=%@&check_number=%@", _newLoginID, _login_name, _password, _email_address, _device_type, _device_id, _check_number];
    NSLog(@"parameter : %@", parameter);
    NSData *parameterData = [parameter dataUsingEncoding:NSASCIIStringEncoding allowLossyConversion:YES];
//    NSString *postLength = [NSString stringWithFormat:@"%lu", (unsigned long)[parameterData length]];
//    [request setValue:postLength forHTTPHeaderField:@"Content-Length"];
    [request setHTTPBody:parameterData];
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    if(error){
        NSLog(@">>>>>>>>> AddMemberByLoginId error2: %@", error);
    }
    
    
    NSLog(@"AddMemberByLoginId responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"AddMemberByLoginId message : %@", escapeStr);
    
    return responseDict;
}

- (NSDictionary*) GetUserInfor: (NSString*)_userToken{
    NSLog(@"<<< GetUserInfor = %@ >>>", _userToken );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/GetUserInfor",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> GetUserInfor error: %@", error);
    }
    
    
    NSLog(@"GetUserInfor responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetUserInfor message : %@", escapeStr);
    
    return responseDict;

}

- (NSDictionary*) GetUserChoices: (NSString*)_userToken SeqNo:(int)_sequence_no{

    NSLog(@"<<< GetUserChoices = %@ >>>", _userToken );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/lotto/GetUserChoices",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    //NSSTRING
    NSString *parameter = [NSString stringWithFormat:@"sequence_no=%d", _sequence_no];
    NSLog(@"parameter : %@", parameter);
    NSData *parameterData = [parameter dataUsingEncoding:NSASCIIStringEncoding allowLossyConversion:YES];
    [request setHTTPBody:parameterData];
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> GetUserChoices error: %@", error);
    }
    
    
    NSLog(@"GetUserChoices responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetUserChoices message : %@", escapeStr);
    
    return responseDict;

}

//Alert
- (NSDictionary*) GetAlertMessage: (NSString*)_userToken Time:(NSString*)_time{
    
    NSLog(@"<<< GetAlertMessage = %@ >>>", _userToken );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/notify/GetMessages",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    NSString *stringData = [NSString stringWithFormat:@"notify_time=%@", _time];
    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    request.HTTPBody = requestBodyData;
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> GetAlertMessage error: %@", error);
    }
    
    NSLog(@"GetAlertMessage responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetAlertMessage message : %@", escapeStr);
    
    return responseDict;


}

- (int) GetAlertCount: (NSString*)_userToken{
    
    NSLog(@"<<< GetAlertCount = %@ >>>", _userToken );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/notify/GetCount",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> GetAlertCount error: %@", error);
    }
    
    
    NSLog(@"GetAlertCount responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetAlertCount message : %@", escapeStr);
    
    int returnInt = [[responseDict objectForKey:@"result"]intValue] ;
    
    return returnInt;
}

- (BOOL) ClearAlert: (NSString*)_userToken{
    
    NSLog(@"<<< ClearAlert = %@ >>>", _userToken );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/notify/Clear",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> ClearAlert error: %@", error);
    }
    
    
    NSLog(@"ClearAlert responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"ClearAlert message : %@", escapeStr);
    
    BOOL returnBoolean = [[responseDict objectForKey:@"success"]boolValue];
    
    return returnBoolean;
}

- (BOOL) UpdateUserInfo: (NSString*)_userToken LoginName: (NSString*)_login_name MaxSelectNumber:(NSString *)_max_select_number
                   Num1:(NSString*)_num1 Num2:(NSString*)_num2 Num3:(NSString*)_num3{

    NSLog(@"<<< UpdateUserInfo = %@ >>>", _userToken );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/UpdateUserInfor",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    NSString *stringData = [NSString stringWithFormat:@"login_name=%@&max_select_number=%@&digit1=%@&digit2=%@&digit3=%@", _login_name, _max_select_number, _num1, _num2, _num3];
    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    request.HTTPBody = requestBodyData;
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> GetAlertMessage error: %@", error);
    }
    
    NSLog(@"GetAlertMessage responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetAlertMessage message : %@", escapeStr);
    
    BOOL returnBoolean = [[responseDict objectForKey:@"success"]boolValue];
    
    return returnBoolean;

}


- (NSDictionary*) ChangePassword: (NSString *)_userToken NewPassword: (NSString *)_newPassword{
    NSLog(@"<<< ChangePassword = %@ >>>", _newPassword );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/ChangePassword",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    NSString *stringData = [NSString stringWithFormat:@"password=%@", _newPassword];
    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    request.HTTPBody = requestBodyData;
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> ChangePassword error: %@", error);
    }
    
    NSLog(@"ChangePassword responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"ChangePassword message : %@", escapeStr);
    
    return responseDict;
}

- (NSDictionary*) UpdateMailAddress: (NSString *)_userToken MailAddress: (NSString *)_mailAddress ConfirmCode:(NSString *)_confirmCode{

    NSLog(@"<<< UpdateMailAddress = %@ \nConfirmCode = %@>>>", _mailAddress, _confirmCode );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/UpdateMailAddress",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    NSString *stringData = [NSString stringWithFormat:@"mail_address=%@&check_number=%@", _mailAddress, _confirmCode];
    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    request.HTTPBody = requestBodyData;
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> UpdateMailAddress error: %@", error);
    }
    
    NSLog(@"UpdateMailAddress responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"UpdateMailAddress message : %@", escapeStr);
    
    return responseDict;
}

- (BOOL) LeaveMember: (NSString*)_userToken{

    NSLog(@"<<< LeaveMember = %@", _userToken );
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/LeaveMember",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
//    NSString *stringData = [NSString stringWithFormat:@"mail_address=%@&check_number=%@", _mailAddress, _confirmCode];
//    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
//    request.HTTPBody = requestBodyData;
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> LeaveMember error: %@", error);
    }
    
    NSLog(@"LeaveMember responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"LeaveMember message : %@", escapeStr);
    
    BOOL returnBoolean = [[responseDict objectForKey:@"success"]boolValue];
    
    return returnBoolean;

}

- (NSDictionary*) SendMailToRecoveryId: (NSString *)_guestToken MailAddress: (NSString *)_mailAddress{

    NSLog(@"<<< SendMailToRecoveryId_mailAddress = %@>>>", _mailAddress);
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/user/SendMailToRecoveryId",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _guestToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    NSString *stringData = [NSString stringWithFormat:@"mail_address=%@", _mailAddress];
    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    request.HTTPBody = requestBodyData;
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> SendMailToRecoveryId error: %@", error);
    }
    
    NSLog(@"SendMailToRecoveryId responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"SendMailToRecoveryId message : %@", escapeStr);
    
    return responseDict;
}

- (NSDictionary*) GetUserSequenceNos: (NSString *)_userToken{

    NSLog(@"<<< GetUserSequenceNos >>>");
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/lotto/GetUserSequenceNos",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> SendMailToRecoveryId error: %@", error);
    }
    
    NSLog(@"GetUserSequenceNos responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"GetUserSequenceNos message : %@", escapeStr);
    
    return responseDict;
}

- (NSDictionary*) SendChoicedNumbers: (NSString *)_userToken SeqNo:(int)_seqNo{

    NSLog(@"<<< SendChoicedNumbers = %d>>>", _seqNo);
    
    // Create the request.
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@/api/lotto/SendChoicedNumbers",SERVERURL]]];
    [request setHTTPMethod:@"POST"];
    
    // This is how we set header fields
    NSString * forValue = [NSString stringWithFormat:@"Bearer %@", _userToken];
    [request setValue:forValue forHTTPHeaderField:@"Authorization"];
    
    // Convert your data and set your request's HTTPBody property
    NSString *stringData = [NSString stringWithFormat:@"sequence_no=%d", _seqNo];
    NSData *requestBodyData = [stringData dataUsingEncoding:NSUTF8StringEncoding];
    request.HTTPBody = requestBodyData;
    
    NSURLResponse *response;
    NSError *error;
    NSDictionary *responseDict;
    
    NSData * urlData = [self sendSynchronousRequest:request returningResponse:&response error:&error];
    
    responseDict = [NSJSONSerialization JSONObjectWithData:urlData options:0 error:&error];
    
    
    if(error){
        NSLog(@">>>>>>>>> SendChoicedNumbers error: %@", error);
    }
    
    NSLog(@"SendChoicedNumbers responseDict: %@", responseDict);
    
    NSString *escapeStr = [[responseDict objectForKey:@"message"] stringByRemovingPercentEncoding];
    
    NSLog(@"SendChoicedNumbers message : %@", escapeStr);
    
    return responseDict;

}


@end
