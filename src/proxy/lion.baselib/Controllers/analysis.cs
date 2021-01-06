using LottoLion.BaseLib.Models.Entity;

namespace LottoLion.BaseLib.Controllers
{
    public class WinnerAnalysis
    {
        /// <summary>
        /// 1.홀짝 치우침
        /// </summary>
        private void EvenOddBalance(TbLionAnalysis[] analysies, TbLionAnalysis analysis, int sequence_no)
        {
            var _cs = analysis.Digit1 % 2
                    + analysis.Digit2 % 2
                    + analysis.Digit3 % 2
                    + analysis.Digit4 % 2
                    + analysis.Digit5 % 2
                    + analysis.Digit6 % 2;

            analysis.WinOdd = (short)_cs;
            analysis.WinEven = (short)(6 - _cs);

            if (sequence_no > 5)
            {
                var _prev1 = analysies[sequence_no - 1 - 1];
                var _prev5 = analysies[sequence_no - 5 - 1];

                var _rs = _prev1.SumOdd + _cs - _prev5.WinOdd;

                analysis.SumOdd = (short)_rs;
                analysis.SumEven = (short)((5 * 6) - _rs);
            }
            else if (sequence_no > 1)
            {
                var _prev1 = analysies[sequence_no - 1 - 1];

                var _rs = _prev1.SumOdd + _cs;

                analysis.SumOdd = (short)_rs;
                analysis.SumEven = (short)((sequence_no * 6) - _rs);
            }
            else
            {
                analysis.SumOdd = (short)_cs;
                analysis.SumEven = (short)(6 - _cs);
            }

            if (analysis.SumOdd < analysis.SumEven)
                analysis.OddEven = $"짝+{analysis.SumEven - analysis.SumOdd:00}";
            else if (analysis.SumOdd > analysis.SumEven)
                analysis.OddEven = $"홀+{analysis.SumOdd - analysis.SumEven:00}";
            else
                analysis.OddEven = $"None";
        }

        public const int MiddleNumber = 45 / 2 + 1;

        /// <summary>
        /// 2.고저 치우침
        /// </summary>
        private void HighLowBalance(TbLionAnalysis[] analysies, TbLionAnalysis analysis, int sequence_no)
        {
            var _cs = (analysis.Digit1 > MiddleNumber ? 1 : 0)
                    + (analysis.Digit2 > MiddleNumber ? 1 : 0)
                    + (analysis.Digit3 > MiddleNumber ? 1 : 0)
                    + (analysis.Digit4 > MiddleNumber ? 1 : 0)
                    + (analysis.Digit5 > MiddleNumber ? 1 : 0)
                    + (analysis.Digit6 > MiddleNumber ? 1 : 0);

            analysis.WinHigh = (short)_cs;
            analysis.WinLow = (short)(6 - _cs);

            if (sequence_no > 5)
            {
                var _prev1 = analysies[sequence_no - 1 - 1];
                var _prev5 = analysies[sequence_no - 5 - 1];

                var _rs = _prev1.SumHigh + _cs - _prev5.WinHigh;

                analysis.SumHigh = (short)_rs;
                analysis.SumLow = (short)((5 * 6) - _rs);
            }
            else if (sequence_no > 1)
            {
                var _prev1 = analysies[sequence_no - 1 - 1];

                var _rs = _prev1.SumHigh + _cs;

                analysis.SumHigh = (short)_rs;
                analysis.SumLow = (short)((sequence_no * 6) - _rs);
            }
            else
            {
                analysis.SumHigh = (short)_cs;
                analysis.SumLow = (short)(6 - _cs);
            }

            if (analysis.SumHigh < analysis.SumLow)
                analysis.HighLow = $"저+{analysis.SumLow - analysis.SumHigh:00}";
            else if (analysis.SumHigh > analysis.SumLow)
                analysis.HighLow = $"고+{analysis.SumHigh - analysis.SumLow:00}";
            else
                analysis.HighLow = $"None";
        }

        /// <summary>
        /// 3.합계 치우침
        /// </summary>
        private void SumBalance(TbLionAnalysis analysis)
        {
            var _cs = analysis.Digit1
                    + analysis.Digit2
                    + analysis.Digit3
                    + analysis.Digit4
                    + analysis.Digit5
                    + analysis.Digit6;

            analysis.SumFrequence = (short)_cs;
            analysis.SumBalance = (short)(_cs - 138);
        }

        /// <summary>
        /// 4.숫자 집단 치우침
        /// </summary>
        private void GroupBalance(TbLionAnalysis analysis, short[] digits)
        {
            analysis.Group45 = 0;
            analysis.Group36 = 0;
            analysis.Group27 = 0;
            analysis.Group18 = 0;
            analysis.Group9 = 0;

            for (int i = 0; i < digits.Length; i++)
            {
                if (digits[i] > 36)
                    analysis.Group45++;
                else if (digits[i] > 27)
                    analysis.Group36++;
                else if (digits[i] > 18)
                    analysis.Group27++;
                else if (digits[i] > 9)
                    analysis.Group18++;
                else
                    analysis.Group9++;
            }
        }

        /// <summary>
        /// 5.HotCold 치우침
        /// </summary>
        private void HotColdCalc(TbLionAnalysis[] analysies, TbLionAnalysis analysis, short[] digits, int sequence_no)
        {
            var _hotcold = new short[6];

            analysis.HotColdL10 = 0;
            analysis.HotColdSum = 0;

            for (int i = 0; i < digits.Length; i++)
            {
                var _value = digits[i];

                var _found = (short)0;

                for (int j = sequence_no - 1; j > 0; j--)
                {
                    var _prev = analysies[j - 1];

                    if (_value == _prev.Digit1 || _value == _prev.Digit2 || _value == _prev.Digit3
                        || _value == _prev.Digit4 || _value == _prev.Digit5 || _value == _prev.Digit6
                        )
                        break;

                    _found++;
                }

                _hotcold[i] = _found;

                if (_found < 10)
                    analysis.HotColdL10++;

                analysis.HotColdSum += _found;
            }

            analysis.HotColdAvg = analysis.HotColdSum / 6.0m;

            analysis.HotCold1 = _hotcold[0];
            analysis.HotCold2 = _hotcold[1];
            analysis.HotCold3 = _hotcold[2];
            analysis.HotCold4 = _hotcold[3];
            analysis.HotCold5 = _hotcold[4];
            analysis.HotCold6 = _hotcold[5];
        }

        private (short r, short m) WinnerNumberCount(TbLionAnalysis[] analysies, short[] digits, int sequence_no, int depth)
        {
            var _inumber = new short[45];

            for (int j = 1; j <= depth; j++)
            {
                var _offset = sequence_no - j - 1;
                if (_offset < 0)
                    break;

                var _prev = analysies[_offset];

                _inumber[_prev.Digit1 - 1]++;
                _inumber[_prev.Digit2 - 1]++;
                _inumber[_prev.Digit3 - 1]++;
                _inumber[_prev.Digit4 - 1]++;
                _inumber[_prev.Digit5 - 1]++;
                _inumber[_prev.Digit6 - 1]++;
            }

            var _r = (short)0;
            var _m = (short)0;

            for (int i = 1; i <= 45; i++)
            {
                if (_inumber[i - 1] > 0)
                {
                    _r++;

                    for (var n = 1; n <= 6; n++)
                    {
                        if (digits[n - 1] == i)
                            _m++;
                    }
                }
            }

            return (_r, _m);
        }

        /// <summary>
        /// 6.퍼센티지 계산
        /// </summary>
        private void PercentageCalc(TbLionAnalysis[] analysies, TbLionAnalysis analysis, short[] digits, int sequence_no)
        {
            var _inumber3 = WinnerNumberCount(analysies, digits, sequence_no, 3);
            {
                analysis.NoDigits3 = _inumber3.r;
                analysis.NoWinner3 = _inumber3.m;
            }

            var _inumber5 = WinnerNumberCount(analysies, digits, sequence_no, 5);
            {
                analysis.NoDigits5 = _inumber5.r;
                analysis.NoWinner5 = _inumber5.m;
            }

            var _inumber10 = WinnerNumberCount(analysies, digits, sequence_no, 10);
            {
                analysis.NoDigits10 = _inumber10.r;
                analysis.NoWinner10 = _inumber10.m;
            }
        }

        /// <summary>
        /// 7.당첨 차트 계산
        /// </summary>
        private void WinerChart(TbLionAnalysis[] analysies, TbLionAnalysis analysis, short[] digits, int sequence_no)
        {
            for (var n = 1; n <= 6; n++)
            {
                var _count = 0;
                var _value = digits[n - 1];

                for (int j = 1; j <= 5; j++)
                {
                    var _offset = sequence_no - j - 1;
                    if (_offset < 0)
                        break;

                    var _prev = analysies[_offset];

                    if (_value == _prev.Digit1 || _value == _prev.Digit2 || _value == _prev.Digit3
                        || _value == _prev.Digit4 || _value == _prev.Digit5 || _value == _prev.Digit6
                        )
                        _count++;
                }

                if (_count >= 4)
                    analysis.WinChart4++;
                else if (_count >= 3)
                    analysis.WinChart3++;
                else if (_count >= 2)
                    analysis.WinChart2++;
                else if (_count >= 1)
                    analysis.WinChart1++;
                else
                    analysis.WinChart0++;
            }
        }

        /// <summary>
        /// 8.합계빈도 확인
        /// </summary>
        private void ReviewFrequency(TbLionAnalysis[] analysies, TbLionAnalysis analysis, short[] digits, int sequence_no)
        {
            var _sum_frequence = 0;
            for (var n = 1; n <= 6; n++)
                _sum_frequence += digits[n - 1];

            for (int j = 1; j < sequence_no; j++)
            {
                var _prev = analysies[j - 1];

                // 8.합계빈도: 출현 빈도
                if (_prev.SumFrequence == _sum_frequence)
                    analysis.SumAppearance++;

                var _edge_high_low = (short)0;

                var _count = 0;
                for (var k = 1; k <= 6; k++)
                {
                    var _value = digits[k - 1];

                    if (_value > MiddleNumber)
                        _edge_high_low++;

                    if (_value == _prev.Digit1)
                        _count++;
                    if (_value == _prev.Digit2)
                        _count++;
                    if (_value == _prev.Digit3)
                        _count++;
                    if (_value == _prev.Digit4)
                        _count++;
                    if (_value == _prev.Digit5)
                        _count++;
                    if (_value == _prev.Digit6)
                        _count++;
                }

                // 9.기록확인: 동일한 잭팟이 있었는지 검색(2회 이상이 어느 정도 인지 분석)
                if (_count == 4)
                    analysis.SameJackpot4++;
                else if (_count == 5)
                    analysis.SameJackpot5++;
                else if (_count == 6)
                    analysis.SameJackpot6++;

                // 10.고저확인: 극단 적인 고저 출현 분석(6 숫자 모두가 높거나 낮은 경우 분석)
                if (_edge_high_low > 5)
                    analysis.EdgeHighLow = _edge_high_low;
                else if (_edge_high_low < 1)
                    analysis.EdgeHighLow = (short)(_edge_high_low * -1);
            }

            // 11.숫자집단: 특정 그룹에만 출현 하는 경우의 수 분석(3개 이상 분석)
            analysis.EdgeGroup = (short)((analysis.Group9 == 0 ? 1 : 0)
                               + (analysis.Group18 == 0 ? 1 : 0)
                               + (analysis.Group27 == 0 ? 1 : 0)
                               + (analysis.Group36 == 0 ? 1 : 0)
                               + (analysis.Group45 == 0 ? 1 : 0));

            // 12.끝수확인: 1자리의 값이 동일한 것이 출현할 경우의 수 분석(3개 이상 분석)
            for (var j = 1; j <= 5; j++)
            {
                var _mod_j = digits[j - 1] % 10;

                var _m = (short)1;
                for (var k = j + 1; k <= 6; k++)
                {
                    var _mod_k = digits[k - 1] % 10;
                    if (_mod_j == _mod_k)
                        _m++;
                }

                if (_m > analysis.EdgeNumber)
                    analysis.EdgeNumber = _m;
            }

            // 13.연번확인: 일련 번호가 몇개 까지 나오는지 확인(3개 이상 분석)
            var _series_count = (short)1;
            var _series_number = (short)0;

            var _x = digits[0];
            for (var k = 2; k <= 6; k++)
            {
                var _y = digits[k - 1];

                if (_x + 1 == _y)
                {
                    _series_count++;

                    if (_series_number < _series_count)
                        _series_number = _series_count;
                }
                else
                    _series_count = 1;

                _x = _y;
            }

            analysis.SeriesNumber = _series_number;
        }

        public TbLionAnalysis AnalysisWinner(LottoLionContext ltctx, TbLionWinner[] winners, TbLionAnalysis[] analysies, int analysis_no)
        {
            var _winner = winners[analysis_no - 1];

            var _analysis = new TbLionAnalysis()
            {
                SequenceNo = analysis_no,

                Digit1 = _winner.Digit1,
                Digit2 = _winner.Digit2,
                Digit3 = _winner.Digit3,
                Digit4 = _winner.Digit4,
                Digit5 = _winner.Digit5,
                Digit6 = _winner.Digit6,
                Digit7 = _winner.Digit7
            };

            var _digits = new short[6];
            {
                _digits[0] = _analysis.Digit1;
                _digits[1] = _analysis.Digit2;
                _digits[2] = _analysis.Digit3;
                _digits[3] = _analysis.Digit4;
                _digits[4] = _analysis.Digit5;
                _digits[5] = _analysis.Digit6;
            }

            // 1.홀짝 치우침
            EvenOddBalance(analysies, _analysis, analysis_no);

            // 2.고저 치우침
            HighLowBalance(analysies, _analysis, analysis_no);

            // 3.합계 치우침
            SumBalance(_analysis);

            // 4.숫자 집단 치우침
            GroupBalance(_analysis, _digits);

            // 5.HotCold 치우침
            HotColdCalc(analysies, _analysis, _digits, analysis_no);

            // 6.퍼센티지 계산
            PercentageCalc(analysies, _analysis, _digits, analysis_no);

            // 7.당첨 차트 계산
            WinerChart(analysies, _analysis, _digits, analysis_no);

            // 8.합계빈도 확인
            ReviewFrequency(analysies, _analysis, _digits, analysis_no);

            return _analysis;
        }
    }
}