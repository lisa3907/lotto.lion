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
    ///
    /// </summary>
    public partial class Receiver
    {
        private static WinnerQ __winnerQ = new WinnerQ();
        private static WinnerReader __winner_reader = new WinnerReader();

        private static TSelector __last_selector = new TSelector();

        private static async Task<bool> WinnerUpdateAsync(LottoLionContext ltctx, TbLionWinner winner)
        {
            var _result = false;

            try
            {
                ltctx.Database.BeginTransaction();

                var _record = ltctx.TbLionWinner
                                    .Where(w => w.SequenceNo == winner.SequenceNo)
                                    .SingleOrDefault();

                if (_record != null)
                    ltctx.Entry(_record).CurrentValues.SetValues(winner);
                else
                    ltctx.TbLionWinner.Add(winner);

                await ltctx.SaveChangesAsync();

                ltctx.Database.CommitTransaction();
                _result = true;
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();
                __clogger.WriteLog(ex);
            }

            return _result;
        }

        private static void WriteWinner(int sequence_no, string message)
        {
            __clogger.WriteDebug($"[recv-winner] ({sequence_no}) => '{message}'");
        }

        /// <summary>
        /// winner(당첨) 번호를 winner 큐에서 꺼내와서 테이블에 추가 또는 변경 합니다.
        /// 큐를 이용 하여 처리 함으로 시스템이 오류가 발생해도 처리에 문제가 없습니다.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">60분에 한번 큐 확인</param>
        /// <param name="console_out"></param>
        public static async void StartWinnerQ(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 60 * 1)
        {
            __clogger.WriteDebug("[recv-winner] starting receiver winnerQ...");

            while (true)
            {
                await __winnerQ.RecvQAsync<TSelector>(async (_selection) =>
                {
                    var _result = true;

                    try
                    {
                        if (__last_selector.sequence_no != _selection.sequence_no)
                        {
                            __last_selector = _selection;

                            using (var _ltctx = LTCX.GetNewContext())
                            {
                                //'나눔로또'에서 크롤링 합니다.
                                var _winner = await __winner_reader.ReadWinnerBall(_selection.sequence_no);
                                if (_winner != null)
                                {
                                    // 당첨번호를 테이블에 저장 합니다.
                                    if (await WinnerUpdateAsync(_ltctx, _winner) == true)
                                        WriteWinner(_selection.sequence_no, "insert success");
                                    else
                                        WriteWinner(_selection.sequence_no, "insert failure");
                                }
                                else
                                    WriteWinner(_selection.sequence_no, "crawling failure");

                                // next step
                                var _today_seqno = WinnerReader.GetThisWeekSequenceNo();
                                if (_selection.sequence_no == _today_seqno)
                                    await Collector.Winner2AnalysisAsync(_ltctx, _selection.sequence_no);
                            }
                        }
                        else
                            WriteWinner(_selection.sequence_no, "skip winner crawling...");
                    }
                    catch (Exception ex)
                    {
                        __clogger.WriteLog(ex);
                    }
                    finally
                    {
                        GC.Collect(2, GCCollectionMode.Optimized);
                    }

                    return _result;
                },
                tokenSource: tokenSource);

                var _cancelled = tokenSource.Token.WaitHandle.WaitOne(sleep_seconds * 1000);
                if (_cancelled == true)
                    break;
            }
        }
    }
}