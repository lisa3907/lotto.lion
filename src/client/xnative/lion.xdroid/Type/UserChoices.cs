using System.Collections.Generic;

namespace Lion.XDroid.Type
{
    public class UserChoice
    {
        public short digit1;
        public short digit2;
        public short digit3;
        public short digit4;
        public short digit5;
        public short digit6;
        public short ranking;
        public decimal amount;
    }

    public class UserChoices
    {
        public int sequenceNo;
        public string loginId;

        public List<UserChoice> choice;
    }
}