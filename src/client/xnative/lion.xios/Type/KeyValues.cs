using System.Collections.Generic;

namespace Lion.XIOS.Type
{
    /// <summary>
    /// 
    /// </summary>
    public class TKeyValue
    {
        /// <summary>
        /// 
        /// </summary>
        public int key
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int value
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TKeyValues
    {
        public string loginId;

        public List<TKeyValue> numbers;
    }
}