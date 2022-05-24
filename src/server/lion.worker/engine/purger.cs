using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Data.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OdinSdk.BaseLib.Net.Smtp;
using System.Data;

namespace Lion.Worker.Engine
{
    /// <summary>
    ///
    /// </summary>
    public class Purger : BackgroundService
    {
        private readonly ILogger<Purger> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly MailSenderLottoLion _mailSender;
        private readonly SmtpDirectSender _smtpDirect;
        private readonly WinnerReader _winnerReader;

        public Purger(ILogger<Purger> logger, IDbContextFactory<AppDbContext> contextFactory, MailSenderLottoLion mailSender, SmtpDirectSender smtpDirect, WinnerReader winnerReader)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _mailSender = mailSender;
            _smtpDirect = smtpDirect;
            _winnerReader = winnerReader;
        }

        /// <summary>
        ///
        /// </summary>
        public async Task<(int success, int failure)> DoMailErrorRecoveryAsync()
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = _contextFactory.CreateDbContext())
            {
                var _members = _ltctx.tb_lion_member
                                        .Where(m => m.IsAlive == true && m.MailError == true)
                                        .ToList();

                foreach (var _member in _members)
                {
                    try
                    {
                        _ltctx.Database.BeginTransaction();

                        var _verify = await _smtpDirect.VerifyMailServerConnection2Async(_mailSender.MailSenderAddress, _member.EmailAddress);
                        if (_verify.error_code == SmtpError.GENERIC_SUCCESS)
                        {
                            _logger.LogInformation($"[worker.purger] mail error recovered '{_member.EmailAddress}', {_verify.error_code}, {_verify.error_message}");

                            _member.MailError = false;
                            _result.success++;
                        }
                        else
                        {
                            _logger.LogInformation($"[worker.purger] mail error detected '{_member.EmailAddress}', {_verify.error_code}, {_verify.error_message}");

                            var _error_msgs = _verify.error_message.Split(' ');
                            if (_error_msgs.Length > 0 && _error_msgs[0] == "550")
                                _member.IsAlive = false;

                            _result.failure++;
                        }

                        _ltctx.SaveChanges();

                        _ltctx.Database.CommitTransaction();
                    }
                    catch
                    {
                        _ltctx.Database.RollbackTransaction();
                        _result.failure++;
                    }
                }
            }

            return _result;
        }

        /// <summary>
        /// 매주 4주전 6등 삭제
        /// </summary>
        public (int success, int failure) DropChoiceClearing(int week_term = 4)
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = _contextFactory.CreateDbContext())
            {
                var _sequence_no = _winnerReader.GetThisWeekSequenceNo() - week_term;

                try
                {
                    _ltctx.Database.BeginTransaction();

                    var _choice_count = _ltctx.tb_lion_choice
                                                .Where(c => c.SequenceNo <= _sequence_no && c.Ranking > 5)
                                                .Count();

                    if (_choice_count > 0)
                    {
                        var _parameters = new List<SqlParameter>
                        {
                              new SqlParameter("@sequence_no", SqlDbType.SmallInt) { Value = _sequence_no }
                        };
                       
                        _result.success = _ltctx.Database.ExecuteSqlRaw(
                                "DELETE FROM tb_lion_choice WHERE SequenceNo <= @sequence_no AND Ranking > 5",
                                _parameters
                            );

                        _ltctx.SaveChanges();
                    }

                    _ltctx.Database.CommitTransaction();
                }
                catch
                {
                    _ltctx.Database.RollbackTransaction();
                    _result.failure++;
                }
            }

            return _result;
        }

        /// <summary>
        /// 매주 1주전 5, 6등 삭제
        /// </summary>
        /// <param name="week_term"></param>
        /// <returns></returns>
        public (int success, int failure) DropSelectClearing(int week_term = 1)
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = _contextFactory.CreateDbContext())
            {
                var _sequence_no = _winnerReader.GetThisWeekSequenceNo() - week_term;

                try
                {
                    _ltctx.Database.BeginTransaction();

                    var _select_count = _ltctx.tb_lion_select
                                                .Where(s => s.SequenceNo <= _sequence_no && s.Ranking > 4)
                                                .Count();

                    if (_select_count > 0)
                    {
                        var _parameters = new List<SqlParameter>
                        {
                              new SqlParameter("@sequence_no", SqlDbType.SmallInt) { Value = _sequence_no }
                        };
                        
                        _result.success = _ltctx.Database.ExecuteSqlRaw(
                                "DELETE FROM tb_lion_select WHERE SequenceNo <= @sequence_no AND Ranking > 4",
                                _parameters
                            );

                        _ltctx.SaveChanges();
                    }

                    _ltctx.Database.CommitTransaction();
                }
                catch
                {
                    _ltctx.Database.RollbackTransaction();
                }
            }

            return _result;
        }

        /// <summary>
        /// 1주 전 푸쉬 메시지 삭제
        /// </summary>
        public (int success, int failure) DropNotifyClearing(int week_term = 1)
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = _contextFactory.CreateDbContext())
            {
                var _notify_time = DateTime.Now.AddDays(week_term * -7);

                try
                {
                    _ltctx.Database.BeginTransaction();

                    var _notify_count = _ltctx.tb_lion_notify
                                                .Where(n => n.IsRead == true && n.NotifyTime < _notify_time)
                                                .Count();

                    if (_notify_count > 0)
                    {
                        var _parameters = new List<SqlParameter>
                        {
                              new SqlParameter("@notify_time", SqlDbType.DateTime2) { Value = _notify_time }
                        };
                        
                        _result.success = _ltctx.Database.ExecuteSqlRaw(
                                "DELETE FROM tb_lion_notify WHERE IsRead = 1 AND NotifyTime < @notify_time",
                                _parameters
                            );

                        _ltctx.SaveChanges();
                    }

                    _ltctx.Database.CommitTransaction();
                }
                catch
                {
                    _ltctx.Database.RollbackTransaction();
                }
            }

            return _result;
        }

        /// <summary>
        /// 탈퇴 회원 제공 번호 삭제
        /// </summary>
        public (int success, int failure) LeaveChoiceClearing()
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = _contextFactory.CreateDbContext())
            {
                try
                {
                    var _choice = _ltctx.SelectQuery<dNoRecord>("SELECT COUNT(*) as no_record "
                            + "FROM tb_lion_choice "
                            + "WHERE LoginId IN "
                            + "( "
                            + "     SELECT LoginId "
                            + "     FROM "
                            + "     ( "
                            + "         SELECT LoginId "
                            + "         FROM tb_lion_choice "
                            + "         GROUP BY LoginId "
                            + "     ) c "
                            + "     WHERE NOT c.LoginId IN "
                            + "     ( "
                            + "         SELECT LoginId FROM tb_lion_member "
                            + "         WHERE IsAlive=1"
                            + "     ) "
                            + ")"
                        )
                        .SingleOrDefault();

                    _ltctx.Database.BeginTransaction();

                    if (_choice != null && _choice.no_record > 0)
                    {
                        _result.success = _ltctx.Database.ExecuteSqlRaw("DELETE "
                            + "FROM tb_lion_choice "
                            + "WHERE LoginId IN "
                            + "( "
                            + "     SELECT LoginId "
                            + "     FROM "
                            + "     ( "
                            + "         SELECT LoginId "
                            + "         FROM tb_lion_choice "
                            + "         GROUP BY LoginId "
                            + "     ) c "
                            + "     WHERE NOT c.LoginId IN "
                            + "     ( "
                            + "         SELECT LoginId FROM tb_lion_member "
                            + "         WHERE IsAlive=1"
                            + "     ) "
                            + ")"
                        );

                        _ltctx.SaveChanges();
                    }

                    _ltctx.Database.CommitTransaction();
                }
                catch
                {
                    _ltctx.Database.RollbackTransaction();
                }
            }

            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[worker.purger] starting assister...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var _mail_recovery = await DoMailErrorRecoveryAsync();
                    {
                        var _no_recovery = _mail_recovery.success + _mail_recovery.failure;
                        if (_no_recovery > 0)
                            _logger.LogInformation($"[worker.purger] mail recovery {_mail_recovery.success}/{_no_recovery}");
                    }

                    var _choice_clearing = DropChoiceClearing();
                    {
                        var _no_clearing = _choice_clearing.success + _choice_clearing.failure;
                        if (_no_clearing > 0)
                            _logger.LogInformation($"[worker.purger] choice clearing {_choice_clearing.success}/{_no_clearing}");
                    }

                    var _select_clearing = DropSelectClearing();
                    {
                        var _no_clearing = _select_clearing.success + _select_clearing.failure;
                        if (_no_clearing > 0)
                            _logger.LogInformation($"[worker.purger] select clearing {_select_clearing.success}/{_no_clearing}");
                    }

                    var _notify_clearing = DropNotifyClearing();
                    {
                        var _no_clearing = _notify_clearing.success + _notify_clearing.failure;
                        if (_no_clearing > 0)
                            _logger.LogInformation($"[worker.purger] notify clearing {_notify_clearing.success}/{_no_clearing}");
                    }

                    var _leave_clearing = LeaveChoiceClearing();
                    {
                        var _no_clearing = _leave_clearing.success + _leave_clearing.failure;
                        if (_no_clearing > 0)
                            _logger.LogInformation($"[worker.purger] leave clearing {_leave_clearing.success}/{_no_clearing}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "purger");
                }

                await Task.Delay(60 * 60 * 1 * 1000, stoppingToken);
            }
        }
    }
}