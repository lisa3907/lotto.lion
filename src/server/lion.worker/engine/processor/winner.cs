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
    ///
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

        private async Task<bool> WinnerUpdateAsync(AppDbContext ltctx, mWinner winner)
        {
            var _result = false;

            try
            {
                ltctx.Database.BeginTransaction();

                var _record = ltctx.tb_lion_winner
                                    .Where(w => w.SequenceNo == winner.SequenceNo)
                                    .SingleOrDefault();

                if (_record != null)
                    ltctx.Entry(_record).CurrentValues.SetValues(winner);
                else
                    ltctx.tb_lion_winner.Add(winner);

                await ltctx.SaveChangesAsync();

                ltctx.Database.CommitTransaction();
                _result = true;
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                _logger.LogError(ex, "receiver.winner");
            }

            return _result;
        }

        private void WriteWinner(int sequence_no, string message)
        {
            _logger.LogInformation($"[receiver.winner] ({sequence_no}) => '{message}'");
        }

        /// <summary>
        /// winner(당첨) 번호를 winner 큐에서 꺼내와서 테이블에 추가 또는 변경 합니다.
        /// 큐를 이용 하여 처리 함으로 시스템이 오류가 발생해도 처리에 문제가 없습니다.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[receiver.winner] starting receiver winnerQ...");

            var _lastSelector = new dSelector();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var _read_line = (string)null;
                    if (PQueue.QWinner.TryDequeue(out _read_line) == false)
                    {
                        await Task.Delay(10);
                        continue;
                    }

                    var _request = JsonConvert.DeserializeObject<VmsRequest<dSelector>>(_read_line);
                    {
                        var _selector = _request.data;

                        if (_lastSelector.sequence_no != _selector.sequence_no)
                        {
                            _lastSelector.sequence_no = _selector.sequence_no;

                            using (var _ltctx = _contextFactory.CreateDbContext())
                            {
                                //'나눔로또'에서 크롤링 합니다.
                                var _winner = await _winnerReader.ReadWinnerBall(_selector.sequence_no);
                                if (_winner != null)
                                {
                                    // 당첨번호를 테이블에 저장 합니다.
                                    if (await WinnerUpdateAsync(_ltctx, _winner) == true)
                                        WriteWinner(_selector.sequence_no, "insert success");
                                    else
                                        WriteWinner(_selector.sequence_no, "insert failure");
                                }
                                else
                                    WriteWinner(_selector.sequence_no, "crawling failure");

                                // next step
                                var _today_seqno = _winnerReader.GetThisWeekSequenceNo();
                                if (_selector.sequence_no == _today_seqno)
                                    await _pipe_client.WinnerCollectorAsync(_ltctx, _selector.sequence_no);
                            }
                        }
                        else
                            WriteWinner(_selector.sequence_no, "skip winner crawling...");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "receiver.winner");
                }
            }
        }
    }
}