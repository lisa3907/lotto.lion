//
//  TimeFormatter.m
//  playBall
//
//  Created by silencer on 13. 3. 7..
//  Copyright (c) 2013년 oraion soft. All rights reserved.
//

#import "TimeFormatter.h"

@implementation TimeFormatter

+ (NSString *)dateConverter1:(NSString *)inputDate{
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    [formatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss"];
    NSDate * date = [formatter dateFromString:inputDate];
    [formatter setDateFormat:@"yyyy-MM-dd"];
    return [formatter stringFromDate:date];
}

+ (NSString *)dateConverter2:(NSString *)inputDate{
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    [formatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss.sss"];
    NSDate * date = [formatter dateFromString:inputDate];
    [formatter setDateFormat:@"yyyy-MM-dd"];
    return [formatter stringFromDate:date];
}

+ (NSString *)dateConverter3:(NSString *)inputDate{
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    [formatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss.ss"];
    NSDate * date = [formatter dateFromString:inputDate];
    [formatter setDateFormat:@"yy년 MM월 dd일자"];
    return [formatter stringFromDate:date];
}

+ (NSString *)dateConverter4:(NSString *)inputDate{
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    [formatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss"];
    NSDate * date = [formatter dateFromString:inputDate];
    [formatter setDateFormat:@"MM월 dd일"];
    return [formatter stringFromDate:date];
}


+ (NSString *)timeFormatConvertToSeconds:(NSString *)timeSecs
{
    int totalSeconds=[timeSecs intValue];
    int seconds = totalSeconds % 60;
    int minutes = (totalSeconds / 60) % 60;
    int hours = totalSeconds / 3600;
    return [NSString stringWithFormat:@"%02d:%02d:%02d",hours, minutes, seconds];
}

+ (NSString *)getStringFromDatetime:(NSString*)dateTimeStr {
    NSArray * temp = [dateTimeStr componentsSeparatedByString:@"T"];
    return [temp objectAtIndex:0];
}

+ (NSDate *)getDateFromString:(NSString*)string {
    
    NSArray * temp = [string componentsSeparatedByString:@"T"];
    NSString * string1 = [temp objectAtIndex:0];
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    [formatter setDateFormat:@"yyyy-MM-dd"];
    NSDate * date = [formatter dateFromString:string1];
    return date;
}

+ (NSString *)getStringFromDate:(NSDate*)date {
    
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    [formatter setDateFormat:@"yyyy-MM-dd"];
    NSString * string = [formatter stringFromDate:date];
    return string;
}




//
//  UtilityForDate.m
//  J2entyFramework
//
//  Created by j2enty on 11. 11. 3..
//  Copyright (c) 2011년 j2enty. All rights reserved.
//

/*
 MARK: Name -> _getDateWithDateFormat:
 MARK: 입력받은 형식으로 오늘의 날짜를 리턴한다.
 MARK: Param -> Format:(NSString *)format
 MARK: Return -> NSString
 */
- (NSString *)_getDateWithDateFormat:(NSString *)format
{
    NSTimeInterval timeStamp = [[NSDate date] timeIntervalSince1970];
    NSDate *date = [NSDate dateWithTimeIntervalSince1970:timeStamp];
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setDateFormat:format];
    
    return [dateFormatter stringFromDate:date];
}


/*
 MARK: Name -> getToday
 MARK: 오늘의 날짜를 리턴한다.
 MARK: Param
 MARK: Return -> NSString
 */
+ (NSString *)getToday
{
    TimeFormatter *selfObject = [[TimeFormatter alloc] init];
    return [selfObject _getDateWithDateFormat:@"dd"];
}


/*
 MARK: Name -> getTodayMonth
 MARK: 이번 '달'을 리턴한다.
 MARK: Param
 MARK: Return -> NSString
 */
+ (NSString *)getTodayMonth
{
    TimeFormatter *selfObject = [[TimeFormatter alloc] init];
    return [selfObject _getDateWithDateFormat:@"MM"];
}


/*
 MARK: Name -> getTodayYear
 MARK: 올해 년도를 리턴한다.
 MARK: Param
 MARK: Return -> NSString
 */
+ (NSString *)getTodayYear
{
    TimeFormatter *selfObject = [[TimeFormatter alloc] init];
    return [selfObject _getDateWithDateFormat:@"yyyy"];
}


/*
 MARK: Name -> getTodayWithDateFormat:
 MARK: 입력받은 날짜형식으로 오늘의 날짜를 리턴한다.
 MARK: Param -> Format:(NSString *)format
 MARK: Return -> NSString
 */
+ (NSString *)getTodayWithDateFormat:(NSString *)format
{
    TimeFormatter *selfObject = [[TimeFormatter alloc] init];
    return [selfObject _getDateWithDateFormat:format];
}


/*
 MARK: Name -> getTodayTimeStamp
 MARK: 현재시간에 대한 타임스탬프 값을 리턴한다.
 MARK: Param
 MARK: Return -> NSTimeInterval
 */
+ (NSTimeInterval)getTodayTimeStamp
{
    return [[NSDate date] timeIntervalSince1970];
}


/*
 MARK: Name -> getDateFromTimestamp:dateDisplayType:
 MARK: 입력받은 타임스탬프와 날짜표현식을 적용한 날짜를 리턴한다.
 MARK: Param -> Timestamp:(NSTimeInterval)timestamp : 원하는 시점의 타임스탬프 값
 MARK: Param -> dateDisplayType:(NSString *)dateType : 표현하고자 하는 날짜표현식
 MARK: Return -> NSString
 */
+ (NSString *)getDateFromTimestamp:(NSTimeInterval)timestamp dateDisplayType:(NSString *)dateType
{
    NSDate *date = [NSDate dateWithTimeIntervalSince1970:timestamp];
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setDateFormat:dateType];
    
    return [dateFormatter stringFromDate:date];
}


/*
 MARK: Name -> _getDayOfWeek
 MARK: 오늘에 대한 요일을 리턴한다.
 MARK: Param
 MARK: Return -> NSInteger (1:일요일/2:월요일/3:화요일/4:수요일/5:목요일/6:금요일/7:토요일)
 */
- (NSInteger)_getDayOfWeek
{
    NSTimeInterval timeStamp = [[NSDate date] timeIntervalSince1970];
    NSDate *date = [NSDate dateWithTimeIntervalSince1970:timeStamp];
    
    NSCalendar *gregorian = [[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
    NSDateComponents *weekDayComponents = [gregorian components:NSCalendarUnitWeekday fromDate:date];
    
    return [weekDayComponents weekday];
}


+ (NSInteger)_getDayOfWeekWithDate:(NSDate*)date
{
    
//    NSTimeInterval timeStamp = [[NSDate date] timeIntervalSince1970];
//    NSDate *date = [NSDate dateWithTimeIntervalSince1970:timeStamp];
    
    NSCalendar *gregorian = [[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
    NSDateComponents *weekDayComponents = [gregorian components:NSCalendarUnitWeekday fromDate:date];
    
    return [weekDayComponents weekday];
}


/*
 MARK: Name -> getIntegerTodayOfWeek
 MARK: 오늘에 대한 요일을 int형으로 리턴한다.
 MARK: Param
 MARK: Return -> NSInteger (1:일요일/2:월요일/3:화요일/4:수요일/5:목요일/6:금요일/7:토요일)
 */
+ (NSInteger)getIntegerTodayOfWeek
{
    TimeFormatter *selfObject = [[TimeFormatter alloc] init];
    return [selfObject _getDayOfWeek];
}


/*
 MARK: Name -> getCoordinate2DValueFromStringAddress
 MARK: 오늘에 대한 요일을 String형으로 리턴한다.
 MARK: Param
 MARK: Return -> NSString (일/월/화/수/목/금/토)
 */
+ (NSString *)getStringTodayOfWeek
{
    TimeFormatter *selfObject = [[TimeFormatter alloc] init];
    switch ([selfObject _getDayOfWeek])
    {
        case 1: return @"일"; break;
        case 2: return @"월"; break;
        case 3: return @"화"; break;
        case 4: return @"수"; break;
        case 5: return @"목"; break;
        case 6: return @"금"; break;
        case 7: return @"토"; break;
        default:             break;
    }
    
    return @"";
}


@end
   
