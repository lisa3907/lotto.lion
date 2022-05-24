using Lion.Share.Data;
using Lion.Share.Pipe;
using Microsoft.EntityFrameworkCore;

namespace Lion.Worker.Engine.Collector
{
    /// <summary>
    /// analysis(분석)의 회차번호와 select(추출)의 회차번를 비교하여
    /// 분석은 되었지만 추출 하지 못한 회차의 예상 번호가 있으면
    /// 큐를 통해 명령을 발송 합니다.
    /// </summary>
    public class Selector : BackgroundService
    {
        private readonly ILogger<Selector> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly PipeClient _pipe_client;

        public Selector(ILogger<Selector> logger, IDbContextFactory<AppDbContext> contextFactory, PipeClient pipeClient)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _pipe_client = pipeClient;
        }

        public async Task<int> Analysis2SelectAsync(AppDbContext ltctx, int analysis_no)
        {
            var _no_select = 0;
            try
            {
                _no_select = await _pipe_client.AnalysisSelectAsync(ltctx, analysis_no);
                if (_no_select > 0)
                    _logger.LogInformation($"[collector.selector] ({analysis_no}) push select => {_no_select} games");

            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                _logger.LogError(ex, "collector.selector");
            }

            return _no_select;
        }

        /// <summary>
        /// 주기적으로 select(추출) 테이블을 생성 합니다.
        /// winner(당첨) 테이블의 마지막 번호를 확인 하여 +1 회차에 해당하는 번호를 추출 합니다.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[collector.selector] starting collctor selectorQ...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var _ltctx = _contextFactory.CreateDbContext())
                    {
                        var _analysis_no = _ltctx.tb_lion_analysis.Max(x => x.SequenceNo);

                        // analysis(분석)의 회차번호와 select(추출)의 회차번를 비교하여
                        // 분석은 되었지만 추출 하지 못한 회차의 예상 번호를 처리 합니다.
                        await Analysis2SelectAsync(_ltctx, _analysis_no);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "collector.selector");
                }

                await Task.Delay(60 * 60 * 1 * 1000, stoppingToken);
            }
        }
    }
}