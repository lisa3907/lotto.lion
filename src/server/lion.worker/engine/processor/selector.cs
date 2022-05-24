using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Data.DTO;
using Lion.Share.Data.Models;
using Lion.Share.Pipe;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Runtime.Versioning;

namespace Lion.Worker.Engine.Processor
{
    /// <summary>
    /// Selection (번호추출)
    /// </summary>
    public class Selector : BackgroundService
    {
        private readonly ILogger<Selector> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly WinnerSelector _winnerSelector;
        private readonly WinnerReader _winnerReader;

        private readonly PipeClient _pipe_client;

        public Selector(ILogger<Selector> logger, IDbContextFactory<AppDbContext> contextFactory, WinnerSelector winnerSelector, WinnerReader winnerReader, PipeClient pipeClient)
        {
            _logger = logger;
            _contextFactory = contextFactory;

            _winnerSelector = winnerSelector;
            _winnerReader = winnerReader;
            _pipe_client = pipeClient;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="sequence_no">예측회차</param>
        /// <returns></returns>
        private async Task<mFactor> NextGameSelectAsync(AppDbContext ltctx, int sequence_no)
        {
            var _factor = (mFactor)null;

            try
            {
                ltctx.Database.BeginTransaction();

                var _no_select = ltctx.tb_lion_select
                                        .Where(x => x.SequenceNo == sequence_no)
                                        .Count();

                // 해당 회차에 기 select(추출) 된 내역이 없고, _factor(인자)의 조합 갯수가 없어야 함
                if (_no_select <= 0)
                {
                    // 해당 회차에 해당하는 추출 factor(인자)를 읽어 옵니다.
                    _factor = _winnerSelector.GetFactor(ltctx, sequence_no);

                    if ((_factor.LNoCombination + _factor.RNoCombination) <= 0)
                    {
                        // 번호를 추출 합니다. 표본을 27개 정도로 계산하게 되면 약 8만~10만개 정도 추출 하게 됩니다.
                        _winnerSelector.WinnerSelect(ltctx, _factor);
                    }

                    await ltctx.SaveChangesAsync();
                }

                ltctx.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                _logger.LogError(ex, "receiver.selector");
            }

            return _factor;
        }

        private void WriteSelect(int sequence_no, string message)
        {
            _logger.LogInformation($"[receiver.selector] ({sequence_no}) => '{message}'");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<bool> StartSelect(AppDbContext ltctx, dSelector selector)
        {
            var _result = false;

            var _prev_analysis = ltctx.tb_lion_analysis
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
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[receiver.selector] starting receiver selectorQ...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var _read_line = (string)null;
                    if (PQueue.QSelector.TryDequeue(out _read_line) == false)
                    {
                        await Task.Delay(10);
                        continue;
                    }

                    var _request = JsonConvert.DeserializeObject<VmsRequest<dSelector>>(_read_line);
                    {
                        var _selector = _request.data;

                        using (var _ltctx = _contextFactory.CreateDbContext())
                        {
                            if (await StartSelect(_ltctx, _selector) == true)
                            {
                                // next step
                                var _today_seqno = _winnerReader.GetThisWeekSequenceNo();
                                if (_selector.sequence_no == _today_seqno)
                                    await _pipe_client.SelectMemberAsync(_ltctx, _winnerReader, _selector.sequence_no, true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "receiver.selector");
                }
            }
        }
    }
}