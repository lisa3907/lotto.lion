using Lion.Share.Data;
using Lion.Share.Data.Models;

namespace Lion.Share.Controllers
{
    public partial class WinnerScoring
    {
        public bool PutJackpot(AppDbContext ltctx, mMember member, int sequence_no, short no_choice)
        {
            var _result = false;

            var _jackpot = ltctx.tb_lion_jackpot
                                .Where(j => j.SequenceNo == sequence_no && j.LoginId == member.LoginId)
                                .FirstOrDefault();

            if (_jackpot == null)
            {
                _jackpot = new mJackpot()
                {
                    SequenceNo = sequence_no,
                    LoginId = member.LoginId,

                    Digit1 = member.Digit1,
                    Digit2 = member.Digit2,
                    Digit3 = member.Digit3,

                    NoChoice = no_choice,

                    NoJackpot = 0,
                    WinningAmount = 0
                };

                ltctx.tb_lion_jackpot.Add(_jackpot);
            }
            else
            {
                _jackpot.Digit1 = member.Digit1;
                _jackpot.Digit2 = member.Digit2;
                _jackpot.Digit3 = member.Digit3;

                _jackpot.NoChoice = no_choice;
            }

            return _result;
        }

        public mJackpot GetJackpot(AppDbContext ltctx, mMember member, int sequence_no, short no_choice)
        {
            var _jackpot = ltctx.tb_lion_jackpot
                                .Where(j => j.SequenceNo == sequence_no && j.LoginId == member.LoginId)
                                .FirstOrDefault();

            if (_jackpot == null)
            {
                _jackpot = new mJackpot()
                {
                    SequenceNo = sequence_no,
                    LoginId = member.LoginId,

                    Digit1 = member.Digit1,
                    Digit2 = member.Digit2,
                    Digit3 = member.Digit3,

                    NoChoice = no_choice,

                    NoJackpot = 0,
                    WinningAmount = 0
                };

                ltctx.tb_lion_jackpot.Add(_jackpot);
            }

            return _jackpot;
        }
    }
}