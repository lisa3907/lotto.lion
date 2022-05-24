namespace Lion.Share.Data.DTO
{
    /// <summary>
    ///
    /// </summary>
    public class dSelector
    {
        /// <summary>
        ///
        /// </summary>
        public int sequence_no
        {
            get;
            set;
        }

        /// <summary>
        /// true 이면 기존 값이 있더라도 다시 update 합니다.
        /// </summary>
        public bool sent_by_queue
        {
            get;
            set;
        }
    }
}