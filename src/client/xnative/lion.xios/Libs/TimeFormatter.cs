using Foundation;
using System;

namespace Lion.XiOS.Libs
{
    public static class UnixTimeHelper
    {
        public static double GetUnixEpoch(this DateTime dateTime)
        {
            var unixTime = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return unixTime.TotalSeconds;
        }
    }

    public class TimeFormatter
    {
        public static string DateConverter1(string inputDate)
        {
            var formatter = new NSDateFormatter();
            formatter.DateFormat = "yyyy-MM-dd'T'HH:mm:ss";

            NSDate date = formatter.Parse(inputDate);
            formatter.DateFormat = "yyyy-MM-dd";

            return formatter.ToString(date);
        }

        public static string DateConverter2(string inputDate)
        {
            var formatter = new NSDateFormatter();
            formatter.DateFormat = "yyyy-MM-dd'T'HH:mm:ss.sss";

            NSDate date = formatter.Parse(inputDate);
            formatter.DateFormat = "yyyy-MM-dd";

            return formatter.ToString(date);
        }

        public static string DateConverter3(string inputDate)
        {
            var formatter = new NSDateFormatter();
            formatter.DateFormat = "yyyy-MM-dd'T'HH:mm:ss.ss";

            NSDate date = formatter.Parse(inputDate);
            formatter.DateFormat = "yy년 MM월 dd일";

            return formatter.ToString(date);
        }

        public static string DateConverter4(string inputDate)
        {
            var formatter = new NSDateFormatter();
            formatter.DateFormat = "yyyy-MM-dd'T'HH:mm:ss";

            NSDate date = formatter.Parse(inputDate);
            formatter.DateFormat = "MM월 dd일";

            return formatter.ToString(date);
        }

        public static string TimeFormatConvertToSeconds(string timeSecs)
        {
            int totalSeconds = Convert.ToInt32(timeSecs);
            int seconds = totalSeconds % 60;
            int minutes = (totalSeconds / 60) % 60;
            int hours = totalSeconds / 3600;

            return $"{hours:d2}:{minutes:d2}:{seconds:d2}";
        }

        public static string GetStringFromDatetime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string GetStringFromDatetime(string dateTimeStr)
        {
            var temp = dateTimeStr.Split('T');
            return temp[0];
        }

        public static NSDate GetDateFromString(string theString)
        {
            var temp = theString.Split('T');
            var string1 = temp[0];

            var formatter = new NSDateFormatter();
            formatter.DateFormat = "yyyy-MM-dd";

            NSDate date = formatter.Parse(string1);
            return date;
        }

        public static string GetStringFromDate(NSDate date)
        {
            var formatter = new NSDateFormatter();
            formatter.DateFormat = "yyyy-MM-dd";

            var theString = formatter.ToString(date);
            return theString;
        }

        private string _getDateWithDateFormat(string format)
        {
            var timeStamp = DateTime.Now.GetUnixEpoch();
            var date = NSDate.FromTimeIntervalSinceReferenceDate(timeStamp);

            var dateFormatter = new NSDateFormatter();
            dateFormatter.DateFormat = format;

            return dateFormatter.ToString(date);
        }

        public static string GetToday()
        {
            var selfObject = new TimeFormatter();
            return selfObject._getDateWithDateFormat("dd");
        }

        public static string GetTodayMonth()
        {
            var selfObject = new TimeFormatter();
            return selfObject._getDateWithDateFormat("MM");
        }

        public static string GetTodayYear()
        {
            var selfObject = new TimeFormatter();
            return selfObject._getDateWithDateFormat("yyyy");
        }

        public static string GetTodayWithDateFormat(string format)
        {
            var selfObject = new TimeFormatter();
            return selfObject._getDateWithDateFormat(format);
        }

        public static double GetTodayTimeStamp()
        {
            return DateTime.Now.GetUnixEpoch();
        }

        public static string GetDateFromTimestampDateDisplayType(double timestamp, string dateType)
        {
            var date = NSDate.FromTimeIntervalSinceReferenceDate(timestamp);

            var dateFormatter = new NSDateFormatter();
            dateFormatter.DateFormat = dateType;

            return dateFormatter.ToString(date);
        }

        public int _getDayOfWeek()
        {
            var timeStamp = DateTime.Now.GetUnixEpoch();

            var date = NSDate.FromTimeIntervalSinceReferenceDate(timeStamp);
            var gregorian = new NSCalendar(NSCalendarType.Gregorian);

            var weekDayComponents = gregorian.Components(NSCalendarUnit.Weekday, date);
            return (int)weekDayComponents.Weekday;
        }

        public static int _getDayOfWeekWithDate(NSDate date)
        {
            var gregorian = new NSCalendar(NSCalendarType.Gregorian);
            var weekDayComponents = gregorian.Components(NSCalendarUnit.Weekday, date);
            return (int)weekDayComponents.Weekday;
        }

        /// <summary>
        /// Name -> GetIntegerTodayOfWeek
        /// 오늘에 대한 요일을 int형으로 리턴한다.
        /// </summary>
        /// <returns>NSInteger(1:일요일/2:월요일/3:화요일/4:수요일/5:목요일/6:금요일/7:토요일)</returns>
        public static int GetIntegerTodayOfWeek()
        {
            var selfObject = new TimeFormatter();
            return selfObject._getDayOfWeek();
        }

        /// <summary>
        ///  Name -> GetStringTodayOfWeek
        ///  오늘에 대한 요일을 String형으로 리턴한다.
        /// </summary>
        /// <returns>NSString(일/월/화/수/목/금/토)</returns>
        public static string GetStringTodayOfWeek()
        {
            var _result = "";

            var selfObject = new TimeFormatter();
            switch (selfObject._getDayOfWeek())
            {
                case 1:
                    _result = "일";
                    break;
                case 2:
                    _result = "월";
                    break;
                case 3:
                    _result = "화";
                    break;
                case 4:
                    _result = "수";
                    break;
                case 5:
                    _result = "목";
                    break;
                case 6:
                    _result = "금";
                    break;
                case 7:
                    _result = "토";
                    break;
                default:
                    break;
            }

            return _result;
        }
    }
}