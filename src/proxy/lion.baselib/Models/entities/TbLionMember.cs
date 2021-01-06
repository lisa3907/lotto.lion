using System;

namespace LottoLion.BaseLib.Models.Entity
{
    public partial class TbLionMember
    {
        public string LoginId
        {
            get; set;
        }

        public string LoginName
        {
            get; set;
        }

        public string LoginPassword
        {
            get; set;
        }

        public string DeviceType
        {
            get; set;
        }

        public string DeviceId
        {
            get; set;
        }

        public string AccessToken
        {
            get; set;
        }

        public bool? IsAlive
        {
            get; set;
        }

        public string PhoneNumber
        {
            get; set;
        }

        public string EmailAddress
        {
            get; set;
        }

        public bool? MailError
        {
            get; set;
        }

        public bool? IsMailSend
        {
            get; set;
        }

        public bool? IsDirectSend
        {
            get; set;
        }

        public bool? IsNumberChoice
        {
            get; set;
        }

        public short MaxSelectNumber
        {
            get; set;
        }

        public short Digit1
        {
            get; set;
        }

        public short Digit2
        {
            get; set;
        }

        public short Digit3
        {
            get; set;
        }

        public DateTime CreateTime
        {
            get; set;
        }

        public string Remark
        {
            get; set;
        }

        public DateTime UpdateTime
        {
            get; set;
        }
    }
}