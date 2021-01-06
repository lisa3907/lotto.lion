using LottoLion.BaseLib.Controllers;
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
    /// Analysis(분석) & Scoring(채점)
    /// </summary>
    public partial class Receiver
    {
        private static AnalysisQ __analysisQ = new AnalysisQ();

        private static WinnerAnalysis __winner_analysis = new WinnerAnalysis();
        private static WinnerPercent __winner_percent = new WinnerPercent();
        private static WinnerScoring __winner_scoring = new WinnerScoring();

        private static async Task<bool> WinnerAnalysisAsync(LottoLionContext ltctx, int from_analysis_no, int till_analysis_no)
        {
            var _result = false;

            try
            {
                ltctx.Database.BeginTransaction();

                if (till_analysis_no > from_analysis_no)
                {
                    var _winners = ltctx.TbLionWinner
                                        .OrderBy(w => w.SequenceNo)
                                        .ToList();

                    var _analysies = ltctx.TbLionAnalysis
                                        .OrderBy(a => a.SequenceNo)
                                        .ToList();

                    for (var _analysis_no = from_analysis_no + 1; _analysis_no <= till_analysis_no; _analysis_no++)
                    {
                        var _analysis = __winner_analysis.AnalysisWinner(ltctx, _winners.ToArray(), _analysies.ToArray(), _analysis_no);
                        ltctx.TbLionAnalysis.Add(_analysis);

                        _analysies.Add(_analysis);

                        var _percent = __winner_percent.AnalysisPercentage(ltctx, _winners.ToArray(), _analysis_no);
                        ltctx.TbLionPercent.Add(_percent);
                    }

                    await ltctx.SaveChangesAsync();
                }

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

        private static async Task<bool> WinnerScoringAsync(LottoLionContext ltctx, int scoring_no)
        {
            var _result = false;

            try
            {
                ltctx.Database.BeginTransaction();

                // 채점 하고자는 차수의 당첨번호
                var _winner = ltctx.TbLionWinner
                                    .Where(w => w.SequenceNo == scoring_no)
                                    .SingleOrDefault();

                if (_winner != null)
                {
                    var _wdigits = new List<short>();
                    {
                        _wdigits.Add(_winner.Digit1);
                        _wdigits.Add(_winner.Digit2);
                        _wdigits.Add(_winner.Digit3);
                        _wdigits.Add(_winner.Digit4);
                        _wdigits.Add(_winner.Digit5);
                        _wdigits.Add(_winner.Digit6);
                    }

                    var _no_select = ltctx.TbLionSelect
                                        .Where(x => x.SequenceNo == scoring_no && x.Ranking == 0)
                                        .Count();

                    // 채점 할 내용이 없으면 skip
                    if (_no_select > 0)
                    {
                        var _factor = __winner_selector.GetFactor(ltctx, scoring_no);

                        // 회차 별 전체 select(추출) 내용 채점 실시
                        // 낙첨(6위) select(추출) 번호는 삭제 함
                        __winner_scoring.SelectScoring(ltctx, _winner, _factor, _wdigits);
                    }

                    var _choice_ids = ltctx.TbLionChoice
                                            .Where(c => c.SequenceNo == scoring_no && c.Ranking == 0)
                                            .GroupBy(c => c.LoginId)
                                            .Select(c => c.Key)
                                            .ToList();

                    // member(회원) 별 choice(선택) 내용 채점 실시
                    foreach (var _login_id in _choice_ids)
                    {
                        var _member = ltctx.TbLionMember
                                                .Where(m => m.LoginId == _login_id)
                                                .SingleOrDefault();
                        if (_member == null)
                            continue;

                        var _no_choice = ltctx.TbLionChoice
                                            .Where(x => x.SequenceNo == scoring_no && x.LoginId == _login_id && x.Ranking == 0)
                                            .Count();

                        if (_no_choice <= 0)
                            continue;

                        var _jackpot = __winner_scoring.GetJackpot(ltctx, _member, scoring_no, (short)_no_choice);
                        __winner_scoring.ChoiceScoring(ltctx, _winner, _jackpot, _wdigits, _login_id);
                    }

                    await ltctx.SaveChangesAsync();
                }

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

        private static void WriteAnalysis(int sequence_no, string message)
        {
            __clogger.WriteDebug($"[recv-analysis] ({sequence_no}) => '{message}'");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public static async Task<bool> StartAnalysis(LottoLionContext ltctx, TSelector selection)
        {
            var _result = false;

            var _max_winner_no = ltctx.TbLionWinner.Max(a => a.SequenceNo);
            if (selection.sequence_no >= 1 && selection.sequence_no <= _max_winner_no)
            {
                var _max_analysis_no = ltctx.TbLionAnalysis.Max(a => a.SequenceNo);

                // winner(당첨) 번호를 (analysis)분석 합니다.
                if (await WinnerAnalysisAsync(ltctx, _max_analysis_no, selection.sequence_no) == false)
                    WriteAnalysis(selection.sequence_no, "analysis failure");
                else
                    WriteAnalysis(selection.sequence_no, "analysis success");

                // select(추출) 번호와 choice(선택) 번호를 scoring(채점) 합니다.
                if (await WinnerScoringAsync(ltctx, selection.sequence_no) == false)
                    WriteAnalysis(selection.sequence_no, "scoring failure");
                else
                    WriteAnalysis(selection.sequence_no, "scoring success");

                _result = true;
            }
            else
                WriteAnalysis(selection.sequence_no, "out-of-winner failure");

            return _result;
        }

        /// <summary>
        /// winner(당첨) 테이블을 확인 하여 analysys(분석) 테이블에 없는 회차에 한해 분석을 실시 합니다.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">1분에 한번 큐 확인</param>
        /// <param name="console_out"></param>
        public static async void StartAnalysisQ(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 1 * 1)
        {
            __clogger.WriteDebug("[recv-analysis] starting receiver analysisQ...");

            while (true)
            {
                await __analysisQ.RecvQAsync<TSelector>(async (_selection) =>
                {
                    var _result = false;

                    try
                    {
                        using (var _ltctx = LTCX.GetNewContext())
                        {
                            _result = await StartAnalysis(_ltctx, _selection);
                            if (_result == true)
                            {
                                // next step
                                var _today_seqno = WinnerReader.GetThisWeekSequenceNo();
                                if (_selection.sequence_no == _today_seqno)
                                    await Collector.Analysis2SelectAsync(_ltctx, _selection.sequence_no);
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