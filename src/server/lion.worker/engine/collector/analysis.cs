using LottoLion.BaseLib.Models;
using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Queues;
using LottoLion.BaseLib.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LottoLion.Worker.Engine
{
    /// <summary>
    /// analysis(분석) 회차번호와 winner(당첨) 회차 번호를 비교 하여
    /// 분석 되지 않은 회차를 분석하도록 큐에 명령을 발송 합니다.
    /// </summary>
    public partial class Collector
    {
        private static AnalysisQ __analysisQ = new AnalysisQ();

        private static async Task<int> WinnerAnalysisAsync(LottoLionContext ltctx, int winner_no)
        {
            var _result = 0;

            try
            {
                var _sequence_nos = new List<int>();

                var _analysis_no = ltctx.TbLionAnalysis.Max(x => x.SequenceNo);
                {
                    for (_analysis_no++; _analysis_no <= winner_no; _analysis_no++)
                        _sequence_nos.Add(_analysis_no);
                }

                var _select_nos = ltctx.TbLionSelect
                                        .Where(s => s.SequenceNo <= winner_no && s.Ranking == 0)
                                        .GroupBy(s => s.SequenceNo)
                                        .Select(s => s.Key)
                                        .ToList();
                {
                    foreach (var _s in _select_nos)
                    {
                        if (_sequence_nos.Exists(x => x == _s) == false)
                            _sequence_nos.Add(_s);
                    }
                }

                var _choice_nos = ltctx.TbLionChoice
                                        .Where(c => c.SequenceNo <= winner_no && c.Ranking == 0)
                                        .GroupBy(c => c.SequenceNo)
                                        .Select(c => c.Key)
                                        .ToList();
                {
                    foreach (var _c in _choice_nos)
                    {
                        if (_sequence_nos.Exists(x => x == _c) == false)
                            _sequence_nos.Add(_c);
                    }
                }

                foreach (var _n in _sequence_nos)
                {
                    var _selection = new TSelector()
                    {
                        sequence_no = _n,
                        sent_by_queue = true
                    };

                    await __analysisQ.SendQAsync(_selection);

                    _result++;
                }
            }
            catch (Exception ex)
            {
                __clogger.WriteLog(ex);
            }

            return _result;
        }

        public static async Task<int> Winner2AnalysisAsync(LottoLionContext ltctx, int winner_no)
        {
            var _no_analysis = await WinnerAnalysisAsync(ltctx, winner_no);
            if (_no_analysis > 0)
                __clogger.WriteDebug($"[send-analysis] ({winner_no}) push analysis => {_no_analysis} games");

            return _no_analysis;
        }

        /// <summary>
        /// winner(당첨) 테이블을 확인 하여 analysys(분석) 테이블에 없는 회차에 한해 분석을 실시 합니다.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">60분에 한번 동작</param>
        /// <param name="console_out"></param>
        public static async void StartAnalysis(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 60 * 1)
        {
            __clogger.WriteDebug("[send-analysis] starting collctor analysisQ...");

            while (true)
            {
                try
                {
                    using (var _ltctx = LTCX.GetNewContext())
                    {
                        var _winner_no = _ltctx.TbLionWinner.Max(x => x.SequenceNo);

                        // 1 시간에 1회
                        // analysis(분석) 회차번호와 winner(당첨) 회차 번호를 비교 하여
                        // 분석 되지 않은 회차를 분석하도록 큐에 명령을 발송 합니다.
                        await Winner2AnalysisAsync(_ltctx, _winner_no);
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