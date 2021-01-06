using LottoLion.BaseLib.Models.Entity;
using System.Collections.Generic;
using System.Linq;

namespace LottoLion.BaseLib.Controllers
{
    public partial class WinnerScoring
    {
        public int SelectScoring(LottoLionContext ltctx, TbLionWinner winner, TbLionFactor factor, List<short> winner_digits)
        {
            var _no_jackpot = 0;

            // clear
            {
                factor.NoJackpot1 = 0;
                factor.WinningAmount1 = 0;

                factor.NoJackpot2 = 0;
                factor.WinningAmount2 = 0;

                factor.NoJackpot3 = 0;
                factor.WinningAmount3 = 0;

                factor.NoJackpot4 = 0;
                factor.WinningAmount4 = 0;

                factor.NoJackpot5 = 0;
                factor.WinningAmount5 = 0;
            }

            var _selects = ltctx.TbLionSelect
                                .Where(x => x.SequenceNo == winner.SequenceNo && x.Ranking == 0)
                                .ToList();

            foreach (var _s in _selects)
            {
                var _select_digits = new List<short>();
                {
                    _select_digits.Add(_s.Digit1);
                    _select_digits.Add(_s.Digit2);
                    _select_digits.Add(_s.Digit3);
                    _select_digits.Add(_s.Digit4);
                    _select_digits.Add(_s.Digit5);
                    _select_digits.Add(_s.Digit6);
                }

                var _match_count = 0;
                foreach (var _x in _select_digits)
                    if (winner_digits.Exists(x => x == _x) == true)
                        _match_count++;

                _no_jackpot++;

                if (_match_count >= 6)
                {
                    _s.Ranking = 1;
                    _s.Amount = winner.Amount1;

                    factor.NoJackpot1++;
                    factor.WinningAmount1 += winner.Amount1;
                }
                else if (_match_count >= 5)
                {
                    if (_select_digits.Exists(x => x == winner.Digit7) == true)
                    {
                        _s.Ranking = 2;
                        _s.Amount = winner.Amount2;

                        factor.NoJackpot2++;
                        factor.WinningAmount2 += winner.Amount2;
                    }
                    else
                    {
                        _s.Ranking = 3;
                        _s.Amount = winner.Amount3;

                        factor.NoJackpot3++;
                        factor.WinningAmount3 += winner.Amount3;
                    }
                }
                else if (_match_count >= 4)
                {
                    _s.Ranking = 4;
                    _s.Amount = winner.Amount4;

                    factor.NoJackpot4++;
                    factor.WinningAmount4 += winner.Amount4;
                }
                else if (_match_count >= 3)
                {
                    _s.Ranking = 5;
                    _s.Amount = winner.Amount5;

                    factor.NoJackpot5++;
                    factor.WinningAmount5 += winner.Amount5;
                }
                else
                {
                    _no_jackpot--;

                    // 낙첨(6위) select(추출) 번호는 삭제 함
                    ltctx.TbLionSelect.Remove(_s);
                }
            }

            return _no_jackpot;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="winner"></param>
        /// <param name="winner_digits"></param>
        /// <param name="login_id">로그인 사용자 아이디</param>
        public int ChoiceScoring(LottoLionContext ltctx, TbLionWinner winner, TbLionJackpot jackpot, List<short> winner_digits, string login_id)
        {
            var _no_jackpot = 0;

            // clear
            {
                jackpot.NoJackpot = 0;
                jackpot.WinningAmount = 0;
            }

            var _choices = ltctx.TbLionChoice
                                .Where(x => x.SequenceNo == winner.SequenceNo && x.LoginId == login_id && x.Ranking == 0)
                                .ToList();

            foreach (var _c in _choices)
            {
                var _choice_digits = new List<short>();
                {
                    _choice_digits.Add(_c.Digit1);
                    _choice_digits.Add(_c.Digit2);
                    _choice_digits.Add(_c.Digit3);
                    _choice_digits.Add(_c.Digit4);
                    _choice_digits.Add(_c.Digit5);
                    _choice_digits.Add(_c.Digit6);
                }

                var _match_count = 0;
                foreach (var _x in _choice_digits)
                    if (winner_digits.Exists(x => x == _x) == true)
                        _match_count++;

                _no_jackpot++;

                if (_match_count >= 6)
                {
                    _c.Ranking = 1;
                    _c.Amount = winner.Amount1;

                    jackpot.NoJackpot++;
                    jackpot.WinningAmount += _c.Amount;
                }
                else if (_match_count >= 5)
                {
                    if (_choice_digits.Exists(x => x == winner.Digit7) == true)
                    {
                        _c.Ranking = 2;
                        _c.Amount = winner.Amount2;
                    }
                    else
                    {
                        _c.Ranking = 3;
                        _c.Amount = winner.Amount3;
                    }

                    jackpot.NoJackpot++;
                    jackpot.WinningAmount += _c.Amount;
                }
                else if (_match_count >= 4)
                {
                    _c.Ranking = 4;
                    _c.Amount = winner.Amount4;

                    jackpot.NoJackpot++;
                    jackpot.WinningAmount += _c.Amount;
                }
                else if (_match_count >= 3)
                {
                    _c.Ranking = 5;
                    _c.Amount = winner.Amount5;

                    jackpot.NoJackpot++;
                    jackpot.WinningAmount += _c.Amount;
                }
                else
                {
                    _no_jackpot--;

                    _c.Ranking = 6;
                    _c.Amount = 0;

                    // 낙첨(6위) select(추출) 번호는 삭제 함
                    //ltctx.TbLionChoice.Remove(_c);
                }
            }

            return _no_jackpot;
        }
    }
}