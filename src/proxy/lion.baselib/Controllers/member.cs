using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LottoLion.BaseLib.Controllers
{
    public class WinnerMember
    {
        private static int __sequence_no = 0;
        private static ConcurrentDictionary<int, TDigits> __choice_pool = new ConcurrentDictionary<int, TDigits>();

        private ConcurrentDictionary<int, TDigits> GetChoicePool(LottoLionContext ltctx, int sequence_no)
        {
            lock (__choice_pool)
            {
                if (__sequence_no != sequence_no)
                {
                    var _select = ltctx.TbLionSelect
                                    .Where(x => x.SequenceNo == sequence_no && x.IsUsed == false)
                                    .Select(x => new TDigits
                                    {
                                        select_no = x.SelectNo,
                                        digits = new short[]
                                        {
                                            x.Digit1, x.Digit2, x.Digit3, x.Digit4, x.Digit5, x.Digit6
                                        },
                                        is_used = x.IsUsed.Value,
                                        is_left = x.IsLeft.Value
                                    })
                                    .OrderBy(x => x.select_no)
                                    .ToList();

                    __choice_pool.Clear();

                    var _sequence_key = 0;
                    foreach (var _s in _select)
                    {
                        _sequence_key++;
                        __choice_pool.TryAdd(_sequence_key, _s);
                    }

                    __sequence_no = sequence_no;
                }

                return __choice_pool;
            }
        }

        private List<TDigits> GetNumbersByRandom(short[] digits, int no_choice)
        {
            var _result = new List<TDigits>();

            var _random = new Random();
            var _numbers = new (int digit, bool used)[45];

            for (var c = 1; c <= no_choice; c++)
            {
                for (int i = 1; i <= 45; i++)
                {
                    _numbers[i - 1].digit = i;
                    _numbers[i - 1].used = false;
                }

                var _choice = new TDigits()
                {
                    select_no = 0,
                    digits = new short[6],
                    is_used = false,
                    is_left = false
                };

                for (int j = 1; j <= 6; j++)
                {
                    var _digit = (short)_random.Next(1, 45);

                    if (j <= 3)
                    {
                        var _reserved = digits[j - 1];
                        if (_reserved >= 1 && _reserved <= 45)
                            _digit = _reserved;
                    }

                    if (_numbers[_digit - 1].used == true)
                    {
                        j--;
                        continue;
                    }

                    _numbers[_digit - 1].used = true;
                    _choice.digits[j - 1] = _digit;
                }

                if (_result.Any(x => x.digits.Intersect(_choice.digits).Count() == 6) == true)
                {
                    c--;
                    continue;
                }

                _result.Add(_choice);
            }

            return _result;
        }

        private List<TDigits> GetNumbers(int[] choice_pool, short[] digits, int no_choice)
        {
            var _result = new List<TDigits>();

            lock (__choice_pool)
            {
                var _random = new Random();
                var _pool_size = choice_pool.Count();

                var _limit = 0;
                for (var _c = 1; _c <= no_choice; _c++)
                {
                    if (++_limit > 1024)
                        break;

                    var _offset = (_pool_size > no_choice ? _random.Next(1, _pool_size) : _c) - 1;
                    if (_pool_size <= _offset)
                        continue;

                    var _key = choice_pool[_offset];

                    var _s = (TDigits)null;
                    if (__choice_pool.TryGetValue(_key, out _s) == false)
                    {
                        _c--;
                        continue;
                    }

                    if (_s.is_used == true)
                    {
                        _c--;
                        continue;
                    }

                    _s.is_used = true;

                    var _choice = new TDigits()
                    {
                        select_no = _s.select_no,
                        digits = _s.digits,
                        is_used = _s.is_used,
                        is_left = _s.is_left
                    };

                    _result.Add(_choice);
                }
            }

            if (_result.Count() < no_choice)
            {
                var _remain = no_choice - _result.Count();

                var _random_choice = GetNumbersByRandom(digits, _remain);
                _result.AddRange(_random_choice);
            }

            return _result;
        }

        private List<TDigits> GetNumbersByGivenDigits(short[] digits, int no_choice)
        {
            var _choice_pool = __choice_pool
                                    .Where(c => c.Value.is_used == false
                                        && (digits[0] == 0 || c.Value.digits.Contains(digits[0]) == true)
                                        && (digits[1] == 0 || c.Value.digits.Contains(digits[1]) == true)
                                        && (digits[2] == 0 || c.Value.digits.Contains(digits[2]) == true)
                                    )
                                    .Select(x => x.Key)
                                    .ToArray();

            return GetNumbers(_choice_pool, digits, no_choice);
        }

        private List<TDigits> GetNumbersByPool(short[] digits, int no_choice)
        {
            var _choice_pool = __choice_pool
                                    .Where(c => c.Value.is_used == false)
                                    .Select(x => x.Key)
                                    .ToArray();

            return GetNumbers(_choice_pool, digits, no_choice);
        }

        /// <summary>
        /// select(추출) 테이블에서 member(회원)의 choice(선택) 테이블로 random(무작위)로 복사 함
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="choice"></param>
        /// <param name="no_choiced">추가 전에 이미 할당 된 레코드의 갯수</param>
        /// <returns></returns>
        public int WinnerChoice(LottoLionContext ltctx, TbLionMember member, TChoice choice, int no_choiced)
        {
            var _result = new List<TDigits>();

            var _no_choice = member.MaxSelectNumber - no_choiced;
            {
                GetChoicePool(ltctx, choice.sequence_no);

                var _digits = new List<short>();
                {
                    _digits.Add(member.Digit1);
                    _digits.Add(member.Digit2);
                    _digits.Add(member.Digit3);
                }

                if (_digits.Sum(x => x) > 0)
                    _result = GetNumbersByGivenDigits(_digits.ToArray(), _no_choice);
                else
                    _result = GetNumbersByPool(_digits.ToArray(), _no_choice);
            }

            var _choice_no = _result.Max(x => x.select_no) + 100;

            foreach (var _s in _result)
            {
                var _select_no = _s.select_no;
                if (_select_no > 0)
                {
                    var _select = ltctx.TbLionSelect
                                    .Where(s => s.SequenceNo == choice.sequence_no && s.SelectNo == _select_no)
                                    .SingleOrDefault();

                    if (_select != null)
                        _select.IsUsed = true;
                }
                else
                    _select_no = _choice_no++;

                var _c = new TbLionChoice()
                {
                    SequenceNo = choice.sequence_no,

                    LoginId = choice.login_id,
                    ChoiceNo = _select_no,

                    Digit1 = _s.digits[0],
                    Digit2 = _s.digits[1],
                    Digit3 = _s.digits[2],
                    Digit4 = _s.digits[3],
                    Digit5 = _s.digits[4],
                    Digit6 = _s.digits[5],

                    IsMailSent = false,

                    Amount = 0m,
                    Ranking = 0,

                    Remark = ""
                };

                ltctx.TbLionChoice.Add(_c);
            }

            return _result.Count();
        }
    }
}