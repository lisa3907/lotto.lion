namespace LottoLion.BaseLib.Models.Entity
{
    public partial class TbLionFactor
    {
        public int SequenceNo
        {
            get; set;
        }

        public short LNoSampling
        {
            get; set;
        }

        public int LNoCombination
        {
            get; set;
        }

        public int LNoExtraction
        {
            get; set;
        }

        public short RNoSampling
        {
            get; set;
        }

        public int RNoCombination
        {
            get; set;
        }

        public int RNoExtraction
        {
            get; set;
        }

        public bool? OddSelection
        {
            get; set;
        }

        public bool? HighSelection
        {
            get; set;
        }

        public short MinOddEven
        {
            get; set;
        }

        public short MaxOddEven
        {
            get; set;
        }

        public short MinHighLow
        {
            get; set;
        }

        public short MaxHighLow
        {
            get; set;
        }

        public short MaxSumFrequence
        {
            get; set;
        }

        public short MaxGroupBalance
        {
            get; set;
        }

        public short MaxLastNumber
        {
            get; set;
        }

        public short MaxSeriesNumber
        {
            get; set;
        }

        public short MaxSameDigits
        {
            get; set;
        }

        public short MaxSameJackpot
        {
            get; set;
        }

        public int NoJackpot1
        {
            get; set;
        }

        public decimal WinningAmount1
        {
            get; set;
        }

        public int NoJackpot2
        {
            get; set;
        }

        public decimal WinningAmount2
        {
            get; set;
        }

        public int NoJackpot3
        {
            get; set;
        }

        public decimal WinningAmount3
        {
            get; set;
        }

        public int NoJackpot4
        {
            get; set;
        }

        public decimal WinningAmount4
        {
            get; set;
        }

        public int NoJackpot5
        {
            get; set;
        }

        public decimal WinningAmount5
        {
            get; set;
        }
    }
}