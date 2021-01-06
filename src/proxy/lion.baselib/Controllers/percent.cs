using LottoLion.BaseLib.Models.Entity;
using System.Collections.Generic;

namespace LottoLion.BaseLib.Controllers
{
    public class WinnerPercent
    {
        private (short[] lj, short[] ls) GetWinnerPercentage(TbLionWinner[] winners, int sequence_no)
        {
            var _result = (lj: new short[10], ls: new short[10]);

            var _winner_digits = new List<short>();
            {
                var _winner = winners[sequence_no - 1];

                _winner_digits.Add(_winner.Digit1);
                _winner_digits.Add(_winner.Digit2);
                _winner_digits.Add(_winner.Digit3);
                _winner_digits.Add(_winner.Digit4);
                _winner_digits.Add(_winner.Digit5);
                _winner_digits.Add(_winner.Digit6);
            }

            var _inumber = new short[45];

            for (int j = 1; j <= 10; j++)
            {
                var _offset = sequence_no - j - 1;
                if (_offset >= 0)
                {
                    var _prev = winners[_offset];

                    _inumber[_prev.Digit1 - 1]++;
                    _inumber[_prev.Digit2 - 1]++;
                    _inumber[_prev.Digit3 - 1]++;
                    _inumber[_prev.Digit4 - 1]++;
                    _inumber[_prev.Digit5 - 1]++;
                    _inumber[_prev.Digit6 - 1]++;
                }

                for (short i = 1; i <= 45; i++)
                {
                    if (_inumber[i - 1] > 0)
                    {
                        _result.ls[j - 1]++;

                        if (_winner_digits.Exists(x => x == i) == true)
                            _result.lj[j - 1]++;
                    }
                }
            }

            return _result;
        }

        public TbLionPercent AnalysisPercentage(LottoLionContext ltctx, TbLionWinner[] winners, int sequence_no)
        {
            var _p = GetWinnerPercentage(winners, sequence_no);

            var _percent = new TbLionPercent()
            {
                SequenceNo = sequence_no,

                LJackpot01 = _p.lj[0],
                LSelect01 = _p.ls[0],

                LJackpot02 = _p.lj[1],
                LSelect02 = _p.ls[1],

                LJackpot03 = _p.lj[2],
                LSelect03 = _p.ls[2],

                LJackpot04 = _p.lj[3],
                LSelect04 = _p.ls[3],

                LJackpot05 = _p.lj[4],
                LSelect05 = _p.ls[4],

                LJackpot06 = _p.lj[5],
                LSelect06 = _p.ls[5],

                LJackpot07 = _p.lj[6],
                LSelect07 = _p.ls[6],

                LJackpot08 = _p.lj[7],
                LSelect08 = _p.ls[7],

                LJackpot09 = _p.lj[8],
                LSelect09 = _p.ls[8],

                LJackpot10 = _p.lj[9],
                LSelect10 = _p.ls[9],

                RJackpot01 = (short)(6 - _p.lj[0]),
                RSelect01 = (short)(45 - _p.ls[0]),

                RJackpot02 = (short)(6 - _p.lj[1]),
                RSelect02 = (short)(45 - _p.ls[1]),

                RJackpot03 = (short)(6 - _p.lj[2]),
                RSelect03 = (short)(45 - _p.ls[2]),

                RJackpot04 = (short)(6 - _p.lj[3]),
                RSelect04 = (short)(45 - _p.ls[3]),

                RJackpot05 = (short)(6 - _p.lj[4]),
                RSelect05 = (short)(45 - _p.ls[4]),

                RJackpot06 = (short)(6 - _p.lj[5]),
                RSelect06 = (short)(45 - _p.ls[5]),

                RJackpot07 = (short)(6 - _p.lj[6]),
                RSelect07 = (short)(45 - _p.ls[6]),

                RJackpot08 = (short)(6 - _p.lj[7]),
                RSelect08 = (short)(45 - _p.ls[7]),

                RJackpot09 = (short)(6 - _p.lj[8]),
                RSelect09 = (short)(45 - _p.ls[8]),

                RJackpot10 = (short)(6 - _p.lj[9]),
                RSelect10 = (short)(45 - _p.ls[9])
            };

            return _percent;
        }
    }
}