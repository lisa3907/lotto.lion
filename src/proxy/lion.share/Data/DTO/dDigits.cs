namespace Lion.Share.Data.DTO
{
    /// <summary>
    ///
    /// </summary>
    public class dDigits
    {
        /// <summary>
        ///
        /// </summary>
        public int select_no
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short[] digits
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool is_used
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool is_left
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class TDigitsComparer : IEqualityComparer<dDigits>
    {
        /// <summary>
        ///
        /// </summary>
        public bool Equals(dDigits x, dDigits y)
        {
            return x.digits.OrderBy(a => a).SequenceEqual(y.digits.OrderBy(b => b));
        }

        /// <summary>
        ///
        /// </summary>
        public int GetHashCode(dDigits o)
        {
            var _hash_code = 0;

            if (o != null)
            {
                _hash_code = 17;
                for (int i = 0; i < 6; i++)
                    _hash_code = _hash_code * 19 + o.digits[i];
            }

            return _hash_code;
        }
    }
}