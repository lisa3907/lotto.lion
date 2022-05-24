using System.Data.Common;
using System.Text.RegularExpressions;

namespace Lion.Share
{
    /// <summary>
    ///
    /// </summary>
    public static partial class CExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string InnerMessage(this Exception ex)
        {
            return ex.InnerException().Message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception InnerException(this Exception ex)
        {
            var _ix = ex;

            while (_ix.InnerException != null)
                _ix = _ix.InnerException;

            return _ix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this object value)
        {
            return value is not null && !String.IsNullOrEmpty(value.ToString().Trim());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string SafeGetString(this DbDataReader dbReader, string columnName)
        {
            for (int colIndex = 0; colIndex < dbReader.FieldCount; colIndex++)
            {
                if (dbReader.GetName(colIndex).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!dbReader.IsDBNull(colIndex))
                        return dbReader.GetString(colIndex);

                    break;
                }
            }

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static long SafeGetInt64(this DbDataReader dbReader, string columnName)
        {
            for (int colIndex = 0; colIndex < dbReader.FieldCount; colIndex++)
            {
                if (dbReader.GetName(colIndex).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!dbReader.IsDBNull(colIndex))
                        return dbReader.GetInt64(colIndex);

                    break;
                }
            }

            return 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="toSearch"></param>
        /// <param name="toFind"></param>
        /// <returns></returns>
        public static bool Like(this string toSearch, string toFind)
        {
            return new Regex(
                                @"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z",
                                RegexOptions.Singleline
                            )
                .IsMatch(toSearch);
        }

        /// <summary>
        /// 연-월-일(yyyy-MM-dd) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime d)
        {
            return d.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 연월일(yyyyMMdd) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateString2(this DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 시분초(HHmmss) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToTimeString2(this DateTime d)
        {
            return d.ToString("HHmmss");
        }

        /// <summary>
        /// 연-월-일 시:분:초(yyyy-MM-dd HH:mm:ss) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime d)
        {
            return d.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 연-월-일T시:분:초.Zone(yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateTimeZoneString(this DateTime d)
        {
            return d.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }

        /// <summary>
        /// 연-월-일 시:분(yyyy-MM-dd HH:mm) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateHHmmString(this DateTime d)
        {
            return d.ToString("yyyy-MM-dd HH:mm");
        }
    }
}