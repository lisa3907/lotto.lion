using LottoLion.BaseLib.Controllers;
using LottoLion.BaseLib.Models;
using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Queues;
using LottoLion.BaseLib.Types;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LottoLion.Worker.Engine
{
    /// <summary>
    /// Selection (번호추출)
    /// </summary>
    public partial class Receiver
    {
        private static SelectorQ __selectorQ = new SelectorQ();
        private static WinnerSelector __winner_selector = new WinnerSelector();

        /// <summary>
        ///
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="sequence_no">예측회차</param>
        /// <returns></returns>
        private static async Task<TbLionFactor> NextGameSelectAsync(LottoLionContext ltctx, int sequence_no)
        {
            var _factor = (TbLionFactor)null;

            try
            {
                ltctx.Database.BeginTransaction();

                var _no_select = ltctx.TbLionSelect
                                        .Where(x => x.SequenceNo == sequence_no)
                                        .Count();

                // 해당 회차에 기 select(추출) 된 내역이 없고, _factor(인자)의 조합 갯수가 없어야 함
                if (_no_select <= 0)
                {
                    // 해당 회차에 해당하는 추출 factor(인자)를 읽어 옵니다.
                    _factor = __winner_selector.GetFactor(ltctx, sequence_no);

                    if ((_factor.LNoCombination + _factor.RNoCombination) <= 0)
                    {
                        // 번호를 추출 합니다. 표본을 27개 정도로 계산하게 되면 약 8만~10만개 정도 추출 하게 됩니다.
                        __winner_selector.WinnerSelect(ltctx, _factor);
                    }

                    await ltctx.SaveChangesAsync();
                }

                ltctx.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();
                __clogger.WriteLog(ex);
            }

            return _factor;
        }

        private static void WriteSelect(int sequence_no, string message)
        {
            __clogger.WriteDebug($"[recv-select] ({sequence_no}) => '{message}'");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static async Task<bool> StartSelect(LottoLionContext ltctx, TSelector selector)
        {
            var _result = false;

            var _prev_analysis = ltctx.TbLionAnalysis
                                            .Where(x => x.SequenceNo == (selector.sequence_no - 1))
                                            .Count();

            // 바로 앞의 analysis(분석) 레코드가 있어야 select(추출) 진행 함
            if (_prev_analysis > 0)
            {
                // 다음 회차의 예측 번호를 select(추출) 합니다.
                var _factor = await NextGameSelectAsync(ltctx, selector.sequence_no);

                if ((_factor.LNoExtraction + _factor.RNoExtraction) > 0)
                {
                    WriteSelect(selector.sequence_no, $"selected:{(_factor.LNoExtraction + _factor.RNoExtraction)}");
                    _result = true;
                }
            }
            else
                WriteSelect(selector.sequence_no, "not-found-analysis failure");

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">1분에 한번 큐 확인</param>
        /// <param name="console_out"></param>
        public static async void StartSelectorQ(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 1 * 1)
        {
            __clogger.WriteDebug("[recv-select] starting receiver selectorQ...");

            while (true)
            {
                await __selectorQ.RecvQAsync<TSelector>(async (_selector) =>
                {
                    try
                    {
                        using (var _ltctx = LTCX.GetNewContext())
                        {
                            if (await StartSelect(_ltctx, _selector) == true)
                            {
                                // next step
                                var _today_seqno = WinnerReader.GetThisWeekSequenceNo();
                                if (_selector.sequence_no == _today_seqno)
                                    await Collector.Select2MemberAsync(_ltctx, _selector.sequence_no);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        __clogger.WriteLog(ex);
                    }
                    finally
                    {
                        GC.Collect(2, GCCollectionMode.Optimized);
                    }

                    return true;
                },
                tokenSource: tokenSource);

                var _cancelled = tokenSource.Token.WaitHandle.WaitOne(sleep_seconds * 1000);
                if (_cancelled == true)
                    break;
            }
        }
    }
}