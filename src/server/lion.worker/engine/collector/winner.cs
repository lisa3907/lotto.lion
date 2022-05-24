using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Data.DTO;
using Lion.Share.Pipe;
using Microsoft.EntityFrameworkCore;

namespace Lion.Worker.Engine.Collector
{
    /// <summary>
    /// 현재 날짜를 확인 하여 당첨 번호를 '나눔로또' 사이트에서 크롤링 하도록
    /// 큐에 회차를 push 합니다.
    /// 이렇게 하면, reciever 큐에서 pop 하여 winner 테이블에 저장 합니다.
    /// </summary>
    public class Winner : BackgroundService
    {
        private readonly ILogger<Winner> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly WinnerReader _winnerReader;
        private readonly PipeClient _pipe_client;

        public Winner(ILogger<Winner> logger, IDbContextFactory<AppDbContext> contextFactory, WinnerReader winnerReader, PipeClient pipeClient)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _winnerReader = winnerReader;
            _pipe_client = pipeClient;
        }

        private async Task<int> WinnerReadingAsync(AppDbContext ltctx, int winner_no)
        {
            var _result = 0;

            try
            {
                var _winner_no = ltctx.tb_lion_winner.Max(w => w.SequenceNo);

                for (_winner_no++; _winner_no <= winner_no; _winner_no++)
                {
                    var _selection = new dSelector()
                    {
                        sequence_no = _winner_no,
                        sent_by_queue = true
                    };

                    if (await _pipe_client.RequestToWinnerQ(_selection))
                        _result++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "collector.winner");
            }

            return _result;
        }

        /// <summary>
        /// 현재 날짜를 확인 하여 당첨 번호를 '나눔로또' 사이트에서 읽어 옵니다.
        /// winner 큐에 그 값을 push 하는 동작 까지 수행 합니다.
        /// 이렇게 하면, 시스템에 동작이 멈추는 경우에도 큐에 남아 있게 되어서, 처리에 문제가 없습니다.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[collector.winner] starting collctor winnerQ...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var _ltctx = _contextFactory.CreateDbContext())
                    {
                        var _winner_no = _winnerReader.GetThisWeekSequenceNo();

                        // 1 시간에 1회
                        // 현재 시간을 계산하여, 회차가 증가 되었으면
                        // 나눔로또 홈페이지를 크롤링 하여 저장 하도록 큐에 명령을 발송 합니다.
                        var _no_winner = await WinnerReadingAsync(_ltctx, _winner_no);
                        if (_no_winner > 0)
                            _logger.LogInformation($"[collector.winner] ({_winner_no}) push winner => {_no_winner} games");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "collector.winner");
                }

                await Task.Delay(60 * 60 * 1 * 1000, stoppingToken);
            }
        }
    }
}