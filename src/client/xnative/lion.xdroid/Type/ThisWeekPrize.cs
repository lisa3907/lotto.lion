using System;

namespace Lion.XDroid.Type
{
    public class NextWeekPrize
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime LastReadTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime NextReadTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ReadInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 다음 회차
        /// </summary>
        public int SequenceNo
        {
            get;
            set;
        }

        /// <summary>
        /// 추첨 일
        /// </summary>
        public DateTime IssueDate
        {
            get;
            set;
        }

        /// <summary>
        /// 예상 당첨금
        /// </summary>
        public decimal PredictAmount
        {
            get;
            set;
        }

        /// <summary>
        /// 누적 판매금
        /// </summary>
        public decimal SalesAmount
        {
            get;
            set;
        }
    }
}