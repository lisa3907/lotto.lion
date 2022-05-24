using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Data.DTO;
using Lion.Share.Pipe;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Runtime.Versioning;

namespace Lion.Worker.Engine.Processor
{
    /// <summary>
    /// Analysis(분석) & Scoring(채점)
    /// </summary>
    public class Analyst : BackgroundService
    {
        private readonly ILogger<Analyst> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly WinnerAnalysis _winnerAnalysis;
        private readonly WinnerPercent _winnerPercent;
        private readonly WinnerScoring _winnerScoring;
        private readonly WinnerSelector _winnerSelector;
        private readonly WinnerReader _winnerReader;

        private readonly PipeClient _pipe_client;

        public Analyst(ILogger<Analyst> logger,
                IDbContextFactory<AppDbContext> contextFactory,
                WinnerAnalysis winnerAnalysis, WinnerPercent winnerPercent, 
                WinnerScoring winnerScoring, WinnerSelector winnerSelector, WinnerReader winnerReader, PipeClient pipeClient)
        {
            _logger = logger;
            _contextFactory = contextFactory;

            _winnerAnalysis = winnerAnalysis;
            _winnerPercent = winnerPercent;
            _winnerScoring = winnerScoring;
            _winnerSelector = winnerSelector;
            _winnerReader = winnerReader;

            _pipe_client = pipeClient;
        }

        private async Task<bool> WinnerAnalysisAsync(AppDbContext ltctx, int from_analysis_no, int till_analysis_no)
        {
            var _result = false;

            try
            {
                ltctx.Database.BeginTransaction();

                if (till_analysis_no > from_analysis_no)
                {
                    var _winners = ltctx.tb_lion_winner
                                        .OrderBy(w => w.SequenceNo)
                                        .ToList();

                    var _analysies = ltctx.tb_lion_analysis
                                        .OrderBy(a => a.SequenceNo)
                                        .ToList();

                    for (var _analysis_no = from_analysis_no + 1; _analysis_no <= till_analysis_no; _analysis_no++)
                    {
                        var _analysis = _winnerAnalysis.AnalysisWinner(ltctx, _winners.ToArray(), _analysies.ToArray(), _analysis_no);
                        ltctx.tb_lion_analysis.Add(_analysis);

                        _analysies.Add(_analysis);

                        var _percent = _winnerPercent.AnalysisPercentage(ltctx, _winners.ToArray(), _analysis_no);
                        ltctx.tb_lion_percent.Add(_percent);
                    }

                    await ltctx.SaveChangesAsync();
                }

                ltctx.Database.CommitTransaction();
                _result = true;
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                _logger.LogError(ex, "receiver.analyst");
            }

            return _result;
        }

        private async Task<bool> WinnerScoringAsync(AppDbContext ltctx, int scoring_no)
        {
            var _result = false;

            try
            {
                ltctx.Database.BeginTransaction();

                // 채점 하고자는 차수의 당첨번호
                var _winner = ltctx.tb_lion_winner
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

                    var _no_select = ltctx.tb_lion_select
                                        .Where(x => x.SequenceNo == scoring_no && x.Ranking == 0)
                                        .Count();

                    // 채점 할 내용이 없으면 skip
                    if (_no_select > 0)
                    {
                        var _factor = _winnerSelector.GetFactor(ltctx, scoring_no);

                        // 회차 별 전체 select(추출) 내용 채점 실시
                        // 낙첨(6위) select(추출) 번호는 삭제 함
                        _winnerScoring.SelectScoring(ltctx, _winner, _factor, _wdigits);
                    }

                    var _choice_ids = ltctx.tb_lion_choice
                                            .Where(c => c.SequenceNo == scoring_no && c.Ranking == 0)
                                            .GroupBy(c => c.LoginId)
                                            .Select(c => c.Key)
                                            .ToList();

                    // member(회원) 별 choice(선택) 내용 채점 실시
                    foreach (var _login_id in _choice_ids)
                    {
                        var _member = ltctx.tb_lion_member
                                                .Where(m => m.LoginId == _login_id)
                                                .SingleOrDefault();
                        if (_member == null)
                            continue;

                        var _no_choice = ltctx.tb_lion_choice
                                            .Where(x => x.SequenceNo == scoring_no && x.LoginId == _login_id && x.Ranking == 0)
                                            .Count();

                        if (_no_choice <= 0)
                            continue;

                        var _jackpot = _winnerScoring.GetJackpot(ltctx, _member, scoring_no, (short)_no_choice);
                        _winnerScoring.ChoiceScoring(ltctx, _winner, _jackpot, _wdigits, _login_id);
                    }

                    await ltctx.SaveChangesAsync();
                }

                ltctx.Database.CommitTransaction();
                _result = true;
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                _logger.LogError(ex, "receiver.analyst");
            }

            return _result;
        }

        private void WriteAnalysis(int sequence_no, string message)
        {
            _logger.LogInformation($"[receiver.analyst] ({sequence_no}) => '{message}'");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public async Task<bool> StartAnalysis(AppDbContext ltctx, dSelector selection)
        {
            var _result = false;

            var _max_winner_no = ltctx.tb_lion_winner.Max(a => a.SequenceNo);
            if (selection.sequence_no >= 1 && selection.sequence_no <= _max_winner_no)
            {
                var _max_analysis_no = ltctx.tb_lion_analysis.Max(a => a.SequenceNo);

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
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[receiver.analyst] starting receiver analysisQ...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var _read_line = (string)null;
                    if (PQueue.QAnalyst.TryDequeue(out _read_line) == false)
                    {
                        await Task.Delay(10);
                        continue;
                    }

                    var _request = JsonConvert.DeserializeObject<VmsRequest<dSelector>>(_read_line);
                    {
                        var _selector = _request.data;

                        using (var _ltctx = _contextFactory.CreateDbContext())
                        {
                            var _aresult = await StartAnalysis(_ltctx, _selector);
                            if (_aresult == true)
                            {
                                // next step
                                var _today_seqno = _winnerReader.GetThisWeekSequenceNo();
                                if (_selector.sequence_no == _today_seqno)
                                    await _pipe_client.AnalysisSelectAsync(_ltctx, _selector.sequence_no);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "receiver.analyst");
                }
            }
        }
    }
}