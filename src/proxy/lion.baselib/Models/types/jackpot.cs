using LottoLion.BaseLib.Models.Entity;
using System.Collections.Generic;

namespace LottoLion.BaseLib.Types
{
    /// <summary>
    ///
    /// </summary>
    public class TJackpots
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

        public List<TJackpot> rows
        {
            get;
            set;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class TJackpot : TbLionChoice
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