using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Pipe;
using Microsoft.EntityFrameworkCore;

namespace Lion.Worker.Engine.Collector
{
    /// <summary>
    /// analysis(분석)의 회차번호와 select(추출)의 회차번를 비교하여
    /// 분석은 되었지만 추출 하지 못한 회차의 예상 번호를 추출 합니다.
    /// 이후, member(회원)들에게 메일을 발송 하도록 큐에 명령을 발송 합니다.
    /// </summary>
    public class Choicer : BackgroundService
    {
        private readonly ILogger<Choicer> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly WinnerReader _winnerReader;
        private readonly PipeClient _pipe_client;

        public Choicer(ILogger<Choicer> logger, IDbContextFactory<AppDbContext> contextFactory, WinnerReader winnerReader, PipeClient pipeClient)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _winnerReader = winnerReader;
            _pipe_client = pipeClient;
        }

        public async Task<int> Select2MemberAsync(AppDbContext ltctx, int selectNo, bool resend = true)
        {
            var _no_member = 0;

            try
            {
                _no_member = await _pipe_client.SelectMemberAsync(ltctx, _winnerReader, selectNo, resend);
                if (_no_member > 0)
                    _logger.LogInformation($"[collector.choicer] ({selectNo}) push member => {_no_member} members");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "collector.choicer");
            }

            return _no_member;
        }

        /// <summary>
        /// 주기적으로 select(추출) 테이블을 생성 합니다.
        /// winner(당첨) 테이블의 마지막 번호를 확인 하여 +1 회차에 해당하는 번호를 추출 합니다.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">1시간에 한번 번호 발송</param>
        /// <param name="console_out"></param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[collector.choicer] starting collctor memberQ...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var _ltctx = _contextFactory.CreateDbContext())
                    {
                        var _select_no = _ltctx.tb_lion_select.Max(x => x.SequenceNo);
                        await Select2MemberAsync(_ltctx, _select_no, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "collector.choicer");
                }
 
                await Task.Delay(60 * 60 * 1 * 1000, stoppingToken);
            }
        }
    }
}