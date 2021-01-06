using System;

namespace LottoLion.BaseLib.Models.Entity
{
    public partial class TbLionNotify
    {
        public string LoginId
        {
            get; set;
        }

        public DateTime NotifyTime
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public bool? IsRead
        {
            get; set;
        }
    }
}