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
        /// ���� ȸ��
        /// </summary>
        public int SequenceNo
        {
            get;
            set;
        }

        /// <summary>
        /// ��÷ ��
        /// </summary>
        public DateTime IssueDate
        {
            get;
            set;
        }

        /// <summary>
        /// ���� ��÷��
        /// </summary>
        public decimal PredictAmount
        {
            get;
            set;
        }

        /// <summary>
        /// ���� �Ǹű�
        /// </summary>
        public decimal SalesAmount
        {
            get;
            set;
        }
    }
}