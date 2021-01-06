namespace LottoLion.BaseLib.Models.Entity
{
    public partial class TbLionSelect
    {
        public int SequenceNo
        {
            get; set;
        }

        public int SelectNo
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

        public bool? IsUsed
        {
            get; set;
        }

        public bool? IsLeft
        {
            get; set;
        }

        public short Ranking
        {
            get; set;
        }

        public decimal Amount
        {
            get; set;
        }

        public string Remark
        {
            get; set;
        }
    }
}