using Lion.Share.Data;
using Lion.Share.Data.Models;

namespace Lion.Share.Controllers
{
    public partial class WinnerSelector
    {
        public mFactor GetFactor(AppDbContext ltctx, int sequence_no)
        {
            var _analysis = ltctx.tb_lion_analysis
                                .Where(x => x.SequenceNo < sequence_no)
                                .OrderByDescending(x => x.SequenceNo)
                                .Take(1)
                                .SingleOrDefault();

            var _factor = ltctx.tb_lion_factor
                                .Where(x => x.SequenceNo <= sequence_no)
                                .OrderByDescending(x => x.SequenceNo)
                                .Take(1)
                                .SingleOrDefault();

            if (_factor != null)
            {
                if (_factor.SequenceNo != sequence_no)
                {
                    var _new_factor = new mFactor()
                    {
                        SequenceNo = sequence_no,

                        LNoSampling = _factor.LNoSampling,                                  // 좌측 조합을 위한 번호 모집단 (보통 27개 이하)
                        LNoCombination = 0,                                                 // 좌측 완전 조합 갯수
                        LNoExtraction = 0,                                                  // 각 회차 별 필터링을 거쳐 추출 된 게임 갯수

                        RNoSampling = _factor.RNoSampling,                                  // 우측 조합을 위한 번호 모집단 (보통 27개 이하)
                        RNoCombination = 0,                                                 // 우측 완전 조합 갯수
                        RNoExtraction = 0,                                                  // 각 회차 별 필터링을 거쳐 추출 된 게임 갯수

                        MinOddEven = _factor.MinOddEven,                                    // 홀수 최소 발생 갯수
                        MaxOddEven = _factor.MaxOddEven,                                    // 홀수 최대 발생 갯수

                        MinHighLow = _factor.MinHighLow,                                    // 고저 최소 발생 갯수
                        MaxHighLow = _factor.MaxHighLow,                                    // 고저 최대 발생 갯수

                        OddSelection = _analysis.SumOdd > _analysis.SumEven,                // 홀수 짝수 선택(홀짝 계산 값이 홀수 값 이면 제외 함)
                        HighSelection = _analysis.SumHigh > _analysis.SumLow,               // 고저 번호 선택(고저 계산 값이 낮은 값 이면 제외 함)

                        MaxSumFrequence = _factor.MaxSumFrequence,                          // 합계 발생 빈도(과거 당첨번호 합계가 1번 이하 이면 제외 함)
                        MaxGroupBalance = _factor.MaxGroupBalance,                          // 최대 집단 빈도(번호가 없는 집단이 3개 이상 이면 제외 함)

                        MaxLastNumber = _factor.MaxLastNumber,                              // 끝수 동일 갯수(끝자리 숫자가 3개 이상 동일 하면 제외 함)
                        MaxSeriesNumber = _factor.MaxSeriesNumber,                          // 일련 번호 갯수(3자리 이상 번호가 연번이면 제외 함)

                        MaxSameDigits = _factor.MaxSameDigits,                              // 과거 당첨 번호 중 같은 번호가 출현한 갯수
                        MaxSameJackpot = _factor.MaxSameJackpot,                            // 같은 번호가 3회 이상 발생 한 흔적이 거의 없음

                        NoJackpot1 = 0,
                        NoJackpot2 = 0,
                        NoJackpot3 = 0,
                        NoJackpot4 = 0,
                        NoJackpot5 = 0,

                        WinningAmount1 = 0m,
                        WinningAmount2 = 0m,
                        WinningAmount3 = 0m,
                        WinningAmount4 = 0m,
                        WinningAmount5 = 0m
                    };

                    ltctx.tb_lion_factor.Add(_new_factor);
                    _factor = _new_factor;
                }
                else
                {
                    _factor.OddSelection = _analysis.SumOdd > _analysis.SumEven;
                    _factor.HighSelection = _analysis.SumHigh > _analysis.SumLow;
                }
            }
            else
            {
                var _new_factor = new mFactor()
                {
                    SequenceNo = sequence_no,

                    LNoSampling = 30,                                                   // 좌측 조합을 위한 번호 모집단 (보통 27개 이하)
                    LNoCombination = 0,                                                 // 좌측 완전 조합 갯수
                    LNoExtraction = 0,                                                  // 각 회차 별 필터링을 거쳐 추출 된 게임 갯수

                    RNoSampling = 0,                                                    // 우측 조합을 위한 번호 모집단 (보통 27개 이하)
                    RNoCombination = 0,                                                 // 우측 완전 조합 갯수
                    RNoExtraction = 0,                                                  // 각 회차 별 필터링을 거쳐 추출 된 게임 갯수

                    MinOddEven = 1,                                                     // 홀수 최소 발생 갯수
                    MaxOddEven = 5,                                                     // 홀수 최대 발생 갯수

                    MinHighLow = 1,                                                     // 고저 최소 발생 갯수
                    MaxHighLow = 5,                                                     // 고저 최대 발생 갯수

                    OddSelection = _analysis.SumOdd > _analysis.SumEven,                // 홀수 짝수 선택(홀짝 계산 값이 홀수 값 이면 제외 함)
                    HighSelection = _analysis.SumHigh > _analysis.SumLow,               // 고저 번호 선택(고저 계산 값이 낮은 값 이면 제외 함)

                    MaxSumFrequence = 2,                                                // 합계 발생 빈도(과거 당첨번호 합계가 1번 이하 이면 제외 함)
                    MaxGroupBalance = 3,                                                // 최대 집단 빈도(번호가 없는 집단이 3개 이상 이면 제외 함)

                    MaxLastNumber = 3,                                                  // 끝수 동일 갯수(끝자리 숫자가 3개 이상 동일 하면 제외 함)
                    MaxSeriesNumber = 3,                                                // 일련 번호 갯수(3자리 이상 번호가 연번이면 제외 함)

                    MaxSameDigits = 4,                                                  // 과거 당첨 번호 중 같은 번호가 출현한 갯수
                    MaxSameJackpot = 3,                                                 // 같은 번호가 3회 이상 발생 한 흔적이 거의 없음

                    NoJackpot1 = 0,
                    NoJackpot2 = 0,
                    NoJackpot3 = 0,
                    NoJackpot4 = 0,
                    NoJackpot5 = 0,

                    WinningAmount1 = 0m,
                    WinningAmount2 = 0m,
                    WinningAmount3 = 0m,
                    WinningAmount4 = 0m,
                    WinningAmount5 = 0m
                };

                ltctx.tb_lion_factor.Add(_new_factor);
                _factor = _new_factor;
            }

            return _factor;
        }
    }
}