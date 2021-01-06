namespace LottoLion.BaseLib.Models.Entity
{
    public partial class TbLionJackpot
    {
        public int SequenceNo
        {
            get; set;
        }

        public string LoginId
        {
            get; set;
        }

        public short NoChoice
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

        public int NoJackpot
        {
            get; set;
        }

        public decimal WinningAmount
        {
            get; set;
        }
    }
}