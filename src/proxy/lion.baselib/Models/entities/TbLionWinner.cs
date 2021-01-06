using System;

namespace LottoLion.BaseLib.Models.Entity
{
    public partial class TbLionWinner
    {
        public int SequenceNo
        {
            get; set;
        }

        public DateTime IssueDate
        {
            get; set;
        }

        public DateTime PaymentDate
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

        public short Digit4
        {
            get; set;
        }

        public short Digit5
        {
            get; set;
        }

        public short Digit6
        {
            get; set;
        }

        public short Digit7
        {
            get; set;
        }

        public short AutoSelect
        {
            get; set;
        }

        public int Count1
        {
            get; set;
        }

        public decimal Amount1
        {
            get; set;
        }

        public int Count2
        {
            get; set;
        }

        public decimal Amount2
        {
            get; set;
        }

        public int Count3
        {
            get; set;
        }

        public decimal Amount3
        {
            get; set;
        }

        public int Count4
        {
            get; set;
        }

        public decimal Amount4
        {
            get; set;
        }

        public int Count5
        {
            get; set;
        }

        public decimal Amount5
        {
            get; set;
        }

        public string Remark
        {
            get; set;
        }
    }
}