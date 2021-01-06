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
    /// 현재 날짜를 확인 하여 당첨 번호를 '나눔로또' 사이트에서 크롤링 하도록
    /// 큐에 회차를 push 합니다.
    /// 이렇게 하면, reciever 큐에서 pop 하여 winner 테이블에 저장 합니다.
    /// </summary>
    public partial class Collector
    {
        private static WinnerQ __winnerQ = new WinnerQ();

        private static async Task<int> WinnerReadingAsync(LottoLionContext ltctx, int winner_no)
        {
            var _result = 0;

            try
            {
                var _winner_no = ltctx.TbLionWinner.Max(w => w.SequenceNo);

                for (_winner_no++; _winner_no <= winner_no; _winner_no++)
                {
                    var _selection = new TSelector()
                    {
                        sequence_no = _winner_no,
                        sent_by_queue = true
                    };

                    await __winnerQ.SendQAsync(_selection);

                    _result++;
                }
            }
            catch (Exception ex)
            {
                __clogger.WriteLog(ex);
            }

            return _result;
        }

        /// <summary>
        /// 현재 날짜를 확인 하여 당첨 번호를 '나눔로또' 사이트에서 읽어 옵니다.
        /// winner 큐에 그 값을 push 하는 동작 까지 수행 합니다.
        /// 이렇게 하면, 시스템에 동작이 멈추는 경우에도 큐에 남아 있게 되어서, 처리에 문제가 없습니다.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">60분에 한번 크롤링 실행</param>
        /// <param name="console_out"></param>
        public static async void StartWinner(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 60 * 1)
        {
            __clogger.WriteDebug("[send-winner] starting collctor winnerQ...");

            while (true)
            {
                try
                {
                    using (var _ltctx = LTCX.GetNewContext())
                    {
                        var _winner_no = WinnerReader.GetThisWeekSequenceNo();

                        // 1 시간에 1회
                        // 현재 시간을 계산하여, 회차가 증가 되었으면
                        // 나눔로또 홈페이지를 크롤링 하여 저장 하도록 큐에 명령을 발송 합니다.
                        var _no_winner = await WinnerReadingAsync(_ltctx, _winner_no);
                        if (_no_winner > 0)
                            __clogger.WriteDebug($"[send-winner] ({_winner_no}) push winner => {_no_winner} games");
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

                var _cancelled = tokenSource.Token.WaitHandle.WaitOne(sleep_seconds * 1000);
                if (_cancelled == true)
                    break;
            }
        }
    }
}