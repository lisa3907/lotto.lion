using Lion.Share.Data;
using Lion.Share.Pipe;
using MailKit.Net.Pop3;
using Microsoft.EntityFrameworkCore;

namespace Lion.Worker.Engine.Collector
{
    /// <summary>
    /// analysis(분석) 회차번호와 winner(당첨) 회차 번호를 비교 하여
    /// 분석 되지 않은 회차를 분석하도록 큐에 명령을 발송 합니다.
    /// </summary>
    public class Analyst : BackgroundService
    {
        private readonly ILogger<Analyst> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly PipeClient _pipe_client;

        public Analyst(ILogger<Analyst> logger, IDbContextFactory<AppDbContext> contextFactory, PipeClient pipeClient)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _pipe_client = pipeClient;
        }

        public async Task<int> Winner2AnalysisAsync(AppDbContext ltctx, int winner_no)
        {
            var _no_analysis = 0;

            try
            {
                _no_analysis = await _pipe_client.WinnerCollectorAsync(ltctx, winner_no);
                if (_no_analysis > 0)
                    _logger.LogInformation($"[collector.analyst] ({winner_no}) push analysis => {_no_analysis} games");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "collector.analyst");
            }

            return _no_analysis;
        }

        /// <summary>
        /// winner(당첨) 테이블을 확인 하여 analysys(분석) 테이블에 없는 회차에 한해 분석을 실시 합니다.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[collector.analyst] starting collctor analysisQ...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var _ltctx = _contextFactory.CreateDbContext())
                    {
                        var _winner_no = _ltctx.tb_lion_winner.Max(x => x.SequenceNo);

                        // 1 시간에 1회
                        // analysis(분석) 회차번호와 winner(당첨) 회차 번호를 비교 하여
                        // 분석 되지 않은 회차를 분석하도록 큐에 명령을 발송 합니다.
                        await Winner2AnalysisAsync(_ltctx, _winner_no);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "collector.analyst");
                }

                await Task.Delay(60 * 60 * 1 * 1000, stoppingToken);
            }
        }
    }
}