using Lion.Share.Data.Models;

namespace Lion.Share.Data.DTO
{
    /// <summary>
    ///
    /// </summary>
    public class dFilter
    {
        /// <summary>
        /// 회차
        /// </summary>
        public int sequence_no
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public int start_select_no
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool is_left_selection
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public mAnalysis[] analysies
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public mFactor factor
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public List<dDigits> elects
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short[] sampling_digits
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public List<dKeyValue> sum_balances
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short sum_odd
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short sum_even
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short sum_high
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short sum_low
        {
            get;
            set;
        }
    }
}