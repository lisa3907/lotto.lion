using LottoLion.BaseLib.Models.Entity;
using System.Collections.Generic;

namespace LottoLion.BaseLib.Types
{
    /// <summary>
    ///
    /// </summary>
    public class TFilter
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
        public TbLionAnalysis[] analysies
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public TbLionFactor factor
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public List<TDigits> elects
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
        public List<TKeyValue> sum_balances
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