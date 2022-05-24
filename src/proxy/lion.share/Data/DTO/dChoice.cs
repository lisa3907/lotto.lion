namespace Lion.Share.Data.DTO
{
    /// <summary>
    ///
    /// </summary>
    public class dChoice
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
        /// 회원ID
        /// </summary>
        public string login_id
        {
            get;
            set;
        }

        /// <summary>
        /// 큐에서 자동 발송 요청 한것인지 여부
        /// </summary>
        public bool resend
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public int no_choice
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short digit1
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short digit2
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public short digit3
        {
            get;
            set;
        }
    }
}