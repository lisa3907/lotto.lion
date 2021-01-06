using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Types;
using OdinSdk.BaseLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LottoLion.BaseLib.Controllers
{
    public partial class WinnerSelector
    {
        private static CLogger __clogger = new CLogger();
        private static LConfig __cconfig = new LConfig();

        private int MinCountLottoSelect
        {
            get
            {
                return __cconfig.GetAppInteger("minimum.count.lotto.select");
            }
        }

        private short[] GetLNumbereByPercent(TbLionAnalysis[] analysies, int sequence_no, int no_sampling)
        {
            var _result = new List<short>();

            for (var _previous_no = 1; _previous_no < sequence_no; _previous_no++)
            {
                var _offset = sequence_no - _previous_no - 1;
                if (_offset < 0)
                    break;

                var _winner = analysies[_offset];

                var _idigits = new short[6];
                {
                    _idigits[0] = _winner.Digit1;
                    _idigits[1] = _winner.Digit2;
                    _idigits[2] = _winner.Digit3;
                    _idigits[3] = _winner.Digit4;
                    _idigits[4] = _winner.Digit5;
                    _idigits[5] = _winner.Digit6;
                }

                for (var i = 1; i <= 6; i++)
                {
                    var _d = _idigits[i - 1];

                    if (_result.Exists(x => x == _d) == true)
                        continue;

                    if (_result.Count() < no_sampling)
                        _result.Add(_d);
                }
            }

            return _result.OrderBy(x => x).ToArray();
        }

        private short[] GetRNumbereByPercent(TbLionAnalysis[] analysies, int sequence_no, int no_sampling)
        {
            var _result = new List<short>();

            var _left_sample = GetLNumbereByPercent(analysies, sequence_no, (45 - no_sampling)).ToList();
            for (var i = 1; i <= 45; i++)
            {
                if (_left_sample.Exists(x => x == i) == false)
                    _result.Add((short)i);
            }

            return _result.OrderBy(x => x).ToArray();
        }

        /// <summary>
        /// n개의 원소를 가지는 집합에서 k개의 부분집합을 고르는 조합
        /// </summary>
        /// <param name="sampling"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private IEnumerable<short[]> GetCombinations(short[] sampling, int k)
        {
            var _n = sampling.Length;

            // Please do add error handling for when r>n
            if (_n < 1 || k > _n || k < 1)
                throw new Exception($"out of range: n={_n}, k={k}");

            var _idx = new int[k];

            for (var i = 1; i <= k; i++)
                _idx[i - 1] = i;

            do
            {
                var _r = new short[6];

                // Write current combination
                for (var j = 1; j <= k; j++)
                {
                    // or whatever you want to do with the numbers

                    _r[j - 1] = sampling[_idx[j - 1] - 1];
                }

                yield return _r;

                // Locate last non-max index
                var _next = k;
                while (_idx[_next - 1] == _n - k + _next)
                {
                    _next = _next - 1;

                    if (_next == 0)
                    {
                        // All indexes have reached their max, so we're done
                        break;
                    }
                }

                if (_next == 0)
                    break;

                // Increase it and populate the following indexes accordingly
                _idx[_next - 1] = _idx[_next - 1] + 1;
                for (var j = _next + 1; j <= k; j++)
                    _idx[j - 1] = _idx[_next - 1] + j - _next;
            }
            while (true);
        }

        /// <summary>
        /// 1.홀짝 치우침
        /// </summary>
        private (short sum_odd, short sum_even) EvenOddBalance(TbLionAnalysis[] analysies, int sequence_no)
        {
            var _result = (sum_odd: (short)0, sum_even: (short)0);

            for (var i = 1; i <= 4; i++)
            {
                if (sequence_no - i - 1 < 0)
                    break;

                var _prev = analysies[sequence_no - i - 1];
                _result.sum_odd += _prev.WinOdd;
            }

            _result.sum_even = (short)(24 - _result.sum_odd);

            return _result;
        }

        /// <summary>
        /// 2.고저 치우침
        /// </summary>
        private (short sum_high, short sum_low) HighLowBalance(TbLionAnalysis[] analysies, int sequence_no)
        {
            var _result = (sum_high: (short)0, sum_low: (short)0);

            for (var i = 1; i <= 4; i++)
            {
                if (sequence_no - i - 1 < 0)
                    break;

                var _prev = analysies[sequence_no - i - 1];
                _result.sum_high += _prev.WinHigh;
            }

            _result.sum_low = (short)(24 - _result.sum_high);

            return _result;
        }

        private bool Filtering(TFilter filter, short[] digits)
        {
            // 홀짝 확인
            var _odd_even = 0;
            {
                for (int i = 0; i < 6; i++)
                    _odd_even += digits[i] % 2;

                // 홀수 갯수가 0,1 또는 5,6 이면 제외
                if (_odd_even <= filter.factor.MinOddEven || _odd_even >= filter.factor.MaxOddEven)
                    return false;

                //if (filter.factor.OddSelection == true)
                //{
                //    // 홀수: 2, 3, 4만 통과 되므로 최소 12를 초과 해야 함
                //    if (filter.sum_odd > 12)
                //    {
                //        _odd_even += filter.sum_odd;
                //        if (_odd_even < 15)
                //            return false;
                //    }
                //    else
                //    {
                //        if (_odd_even < 3)
                //            return false;
                //    }
                //}
                //else
                //{
                //    // 짝수
                //    if (filter.sum_even > 12)
                //    {
                //        _odd_even += (6 - _odd_even) + filter.sum_even;
                //        if (_odd_even < 15)
                //            return false;
                //    }
                //    else
                //    {
                //        if (_odd_even > 3)
                //            return false;
                //    }
                //}
            }

            // 고저선택
            var _high_low = 0;
            {
                for (int i = 0; i < 6; i++)
                    _high_low += (digits[i] > WinnerAnalysis.MiddleNumber) ? 1 : 0;

                // (45 / 2 + 1) 초과 갯수가 0,1 또는 5,6 이면 제외
                if (_high_low <= filter.factor.MinHighLow || _high_low >= filter.factor.MaxHighLow)
                    return false;

                //if (filter.factor.HighSelection == true)
                //{
                //    // 고
                //    if (filter.sum_high > 12)
                //    {
                //        _high_low += filter.sum_high;
                //        if (_high_low < 15)
                //            return false;
                //    }
                //    else
                //    {
                //        if (_high_low < 3)
                //            return false;
                //    }
                //}
                //else
                //{
                //    // 저
                //    if (filter.sum_low > 12)
                //    {
                //        _high_low += (6 - _high_low) + filter.sum_low;
                //        if (_high_low < 15)
                //            return false;
                //    }
                //    else
                //    {
                //        if (_high_low > 3)
                //            return false;
                //    }
                //}
            }

            // 합계확인
            var _sum_balance = 0;
            {
                for (int i = 0; i < 6; i++)
                    _sum_balance += digits[i];

                var _sum_exist = filter.sum_balances.Where(x => x.key == _sum_balance).Sum(y => y.value);
                if (_sum_exist <= filter.factor.MaxSumFrequence)
                    return false;
            }

            // 끝수확인
            var _last_number = 0;
            {
                for (int i = 0; i < 5; i++)
                {
                    var _n = 1;

                    for (int j = i + 1; j < 6; j++)
                        if (digits[i] % 10 == digits[j] % 10)
                            _n++;

                    if (_n > _last_number)
                        _last_number = _n;
                }

                if (_last_number >= filter.factor.MaxLastNumber)
                    return false;
            }

            // 연변확인
            var _series_number = 0;
            {
                var _series_count = 1;

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

                if (_series_number >= filter.factor.MaxSeriesNumber)
                    return false;
            }

            // 숫자집단
            var _group_balance = 0;
            {
                var _group = new short[5];
                for (int i = 0; i < 5; i++)
                    _group[i] = 0;

                for (int i = 0; i < 6; i++)
                {
                    if (digits[i] > 36)
                        _group[4]++;
                    else if (digits[i] > 27)
                        _group[3]++;
                    else if (digits[i] > 18)
                        _group[2]++;
                    else if (digits[i] > 9)
                        _group[1]++;
                    else
                        _group[0]++;
                }

                for (int i = 0; i < 5; i++)
                {
                    if (_group[i] <= 0)
                        _group_balance++;
                }

                if (_group_balance >= filter.factor.MaxGroupBalance)
                    return false;
            }

            // 기록확인
            var _same_jackpot = 0;
            {
                for (int j = 0; j < filter.sequence_no - 1; j++)
                {
                    var _prev = filter.analysies[j];

                    var _count = 0;
                    for (var k = 0; k < 6; k++)
                    {
                        var _value = digits[k];

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

                    if (_count >= filter.factor.MaxSameDigits)
                        _same_jackpot++;
                }

                if (_same_jackpot >= filter.factor.MaxSameJackpot)
                    return false;
            }

            return true;
        }

        private (int no_select, int no_combine) WinnerElect(LottoLionContext ltctx, TFilter filter)
        {
            var _result = (no_select: 0, no_combine: 0);

            // 완전 조합으로 번호를 생성 후 filtering 하여 확률적으로 당첨이 낮은 set을 제외 시킵니다.
            var _combinations = GetCombinations(filter.sampling_digits, 6);
            foreach (var _numbers in _combinations)
            {
                _result.no_combine++;

                if (Filtering(filter, _numbers) == false)
                    continue;

                _result.no_select++;

                var _tdigits = new TDigits()
                {
                    select_no = _result.no_select + filter.start_select_no,
                    digits = _numbers.OrderBy(n => n).ToArray(),
                    is_used = false,
                    is_left = filter.is_left_selection
                };

                filter.elects.Add(_tdigits);
            }

            var _digits_compare = new TDigitsComparer();
            {
                filter.elects = filter.elects
                                    .Distinct(_digits_compare)
                                    .ToList();
            }

            return _result;
        }

        private void WinnerShuffle(TFilter filter)
        {
            var _random = new Random();

            var _n = filter.elects.Count;
            for (var i = 0; i < _n; i++)
            {
                var _r = i + (int)(_random.NextDouble() * (_n - i));

                var _t = filter.elects[_r];
                filter.elects[_r] = filter.elects[i];
                filter.elects[i] = _t;
            }
        }

        private int WinnerWrite(LottoLionContext ltctx, TFilter filter)
        {
            var _result = 0;

            foreach (var _d in filter.elects)
            {
                var _select = new TbLionSelect()
                {
                    SequenceNo = filter.sequence_no,
                    SelectNo = _d.select_no,

                    Digit1 = _d.digits[0],
                    Digit2 = _d.digits[1],
                    Digit3 = _d.digits[2],
                    Digit4 = _d.digits[3],
                    Digit5 = _d.digits[4],
                    Digit6 = _d.digits[5],

                    IsUsed = false,
                    IsLeft = _d.is_left,

                    Amount = 0m,
                    Ranking = 0,

                    Remark = ""
                };

                ltctx.TbLionSelect.Add(_select);

                _result++;
            }

            return _result;
        }

        /// <summary>
        /// 번호를 추출 합니다. 숫자 선택 후 제외 시키는 방법으로 처리 하게 됩니다.
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public int WinnerSelect(LottoLionContext ltctx, TbLionFactor factor)
        {
            var _analysies = ltctx.TbLionAnalysis
                                    .Where(x => x.SequenceNo < factor.SequenceNo)
                                    .OrderBy(x => x.SequenceNo)
                                    .ToArray();

            var _sum_odd_even = EvenOddBalance(_analysies, factor.SequenceNo);
            var _sum_high_low = HighLowBalance(_analysies, factor.SequenceNo);

            var _sum_balances = _analysies
                                    .GroupBy(x => x.SumFrequence)
                                    .Select(
                                        y => new TKeyValue
                                        {
                                            key = y.Key,
                                            value = y.Count()
                                        }
                                    )
                                    .ToList();

            var _filter = new TFilter
            {
                sequence_no = factor.SequenceNo,

                is_left_selection = false,
                start_select_no = 0,

                elects = new List<TDigits>(),
                sampling_digits = null,

                analysies = _analysies,
                factor = factor,

                sum_balances = _sum_balances,

                sum_odd = _sum_odd_even.sum_odd,
                sum_even = _sum_odd_even.sum_even,
                sum_high = _sum_high_low.sum_high,
                sum_low = _sum_high_low.sum_low
            };

            // 좌측
            var _left_samplings = GetLNumbereByPercent(_analysies, factor.SequenceNo, factor.LNoSampling);
            {
                _filter.is_left_selection = true;
                _filter.sampling_digits = _left_samplings;

                if (_left_samplings.Length > 0)
                {
                    var _selection = WinnerElect(ltctx, _filter);
                    _filter.start_select_no += _selection.no_select;

                    factor.LNoCombination = _selection.no_combine;
                    factor.LNoExtraction = _selection.no_select;
                }
            }

            // 우측
            var _right_samplings = GetRNumbereByPercent(_analysies, factor.SequenceNo, factor.RNoSampling);
            {
                _filter.is_left_selection = false;
                _filter.sampling_digits = _right_samplings;

                if (_right_samplings.Length > 0)
                {
                    var _selection = WinnerElect(ltctx, _filter);
                    _filter.start_select_no += _selection.no_select;

                    factor.RNoCombination = _selection.no_combine;
                    factor.RNoExtraction = _selection.no_select;
                }
            }

            // shuffle
            WinnerShuffle(_filter);

            // write to db
            return WinnerWrite(ltctx, _filter);
        }
    }
}