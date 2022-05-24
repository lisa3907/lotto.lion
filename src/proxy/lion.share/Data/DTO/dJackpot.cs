using Lion.Share.Data.Models;

namespace Lion.Share.Data.DTO
{
    /// <summary>
    ///
    /// </summary>
    public class dJackpots
    {
        public int count
        {
            get;
            set;
        }

        public int limit
        {
            get;
            set;
        }

        public List<dJackpot> rows
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class dJackpot : mChoice
    {
        /// <summary>
        ///
        /// </summary>
        public bool jackpot1
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool jackpot2
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool jackpot3
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool jackpot4
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool jackpot5
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public bool jackpot6
        {
            get;
            set;
        }
    }
}