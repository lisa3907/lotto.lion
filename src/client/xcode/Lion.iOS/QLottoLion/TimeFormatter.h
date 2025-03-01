//
//  TimeFormatter.h
//  playBall
//
//  Created by silencer on 13. 3. 7..
//  Copyright (c) 2013년 oraion soft. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface TimeFormatter : NSObject

+ (NSString *)dateConverter1:(NSString *)inputDate;
+ (NSString *)dateConverter2:(NSString *)inputDate;
+ (NSString *)dateConverter3:(NSString *)inputDate;
+ (NSString *)dateConverter4:(NSString *)inputDate;
+ (NSString *)timeFormatConvertToSeconds:(NSString *)timeSecs;
+ (NSString *)getStringFromDatetime:(NSString*)dateTimeStr;
+ (NSDate *)getDateFromString:(NSString*)string;
+ (NSString *)getStringFromDate:(NSDate*)date;

//
//  UtilityForDate.h
//  J2entyFramework
//
//  Created by j2enty on 11. 11. 3..
//  Copyright (c) 2011년 j2enty. All rights reserved.
//

// LSW
+ (NSInteger)_getDayOfWeekWithDate:(NSDate*)date;
// LSW

- (NSString *)_getDateWithDateFormat:(NSString *)format;
- (NSInteger)_getDayOfWeek;

+ (NSString *)getToday;
+ (NSString *)getTodayMonth;
+ (NSString *)getTodayYear;
+ (NSString *)getTodayWithDateFormat:(NSString *)format;
+ (NSTimeInterval)getTodayTimeStamp;
+ (NSString *)getDateFromTimestamp:(NSTimeInterval)timestamp dateDisplayType:(NSString *)dateType;
+ (NSInteger)getIntegerTodayOfWeek;
+ (NSString *)getStringTodayOfWeek;




@end
