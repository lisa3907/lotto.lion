using System.Text.RegularExpressions;

namespace OdinSdk.BaseLib.Extension
{
    /// <summary>
    ///
    /// </summary>
    public static partial class CExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string RemoveWhiteSpace(this string self)
        {
            return new string(self.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="split_size"></param>
        /// <returns></returns>
        public static string[] SplitByLength(this string source, int split_size)
        {
            var _result = new List<string>();

            for (int i = 0; i < source.Length; i += split_size)
            {
                if (split_size + i > source.Length)
                    split_size = source.Length - i;

                _result.Add(source.Substring(i, split_size));
            }

            return _result.ToArray();
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
        ///
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int length)
        {
            var _result = "";

            var _random = new Random();
            {
                var _s_string = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

                for (var i = 0; i < length; i++)
                    _result += _s_string[_random.Next(0, _s_string.Length)];
            }

            return _result;
        }

        public static bool IsNumber(this string s)
        {
            return s.All(char.IsDigit);
        }
    }
}