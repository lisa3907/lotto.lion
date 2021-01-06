using LottoLion.BaseLib.Models.Entity;
using System.Linq;

namespace LottoLion.BaseLib.Controllers
{
    public partial class WinnerScoring
    {
        public bool PutJackpot(LottoLionContext ltctx, TbLionMember member, int sequence_no, short no_choice)
        {
            var _result = false;

            var _jackpot = ltctx.TbLionJackpot
                                .Where(j => j.SequenceNo == sequence_no && j.LoginId == member.LoginId)
                                .FirstOrDefault();

            if (_jackpot == null)
            {
                _jackpot = new TbLionJackpot()
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

                ltctx.TbLionJackpot.Add(_jackpot);
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

        public TbLionJackpot GetJackpot(LottoLionContext ltctx, TbLionMember member, int sequence_no, short no_choice)
        {
            var _jackpot = ltctx.TbLionJackpot
                                .Where(j => j.SequenceNo == sequence_no && j.LoginId == member.LoginId)
                                .FirstOrDefault();

            if (_jackpot == null)
            {
                _jackpot = new TbLionJackpot()
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

                ltctx.TbLionJackpot.Add(_jackpot);
            }

            return _jackpot;
        }
    }
}