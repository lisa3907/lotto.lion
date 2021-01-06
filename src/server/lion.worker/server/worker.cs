using LottoLion.BaseLib;
using LottoLion.BaseLib.Controllers;
using LottoLion.BaseLib.Models;
using LottoLion.BaseLib.Types;
using Npgsql;
using NpgsqlTypes;
using OdinSdk.BaseLib.Logger;
using OdinSdk.BaseLib.Net.Smtp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LottoLion.Worker.Server
{
    /// <summary>
    ///
    /// </summary>
    public class Worker : IDisposable
    {
        private static CLogger __clogger = new CLogger();
        private static LConfig __cconfig = new LConfig();

        private static MailSenderLottoLion __mail_sender = new MailSenderLottoLion();
        private static SmtpDirectSender __smtp_direct = new SmtpDirectSender();

        /// <summary>
        ///
        /// </summary>
        public static async Task<(int success, int failure)> DoMailErrorRecoveryAsync()
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = LTCX.GetNewContext())
            {
                var _members = _ltctx.TbLionMember
                                        .Where(m => m.IsAlive == true && m.MailError == true)
                                        .ToList();

                foreach (var _member in _members)
                {
                    try
                    {
                        _ltctx.Database.BeginTransaction();

                        var _verify = await __smtp_direct.VerifyMailServerConnection2Async(__mail_sender.MailSenderAddress, _member.EmailAddress);
                        if (_verify.error_code == SmtpError.GENERIC_SUCCESS)
                        {
                            _member.MailError = false;
                            _result.success++;
                        }
                        else
                            _result.failure++;

                        //if (_verify == SmtpError.RCPT_ERROR)
                        //    _member.IsAlive = false;

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
        public static (int success, int failure) DropChoiceClearing(int week_term = 4)
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = LTCX.GetNewContext())
            {
                var _sequence_no = WinnerReader.GetThisWeekSequenceNo() - week_term;

                try
                {
                    _ltctx.Database.BeginTransaction();

                    var _choice_count = _ltctx.TbLionChoice
                                                .Where(c => c.SequenceNo <= _sequence_no && c.Ranking > 5)
                                                .Count();

                    if (_choice_count > 0)
                    {
                        var _delete_count = _ltctx.ExecuteCommand(
                                                "DELETE FROM tb_lion_choice WHERE sequence_no <= @sequence_no AND ranking > 5",
                                                new NpgsqlParameter("@sequence_no", NpgsqlDbType.Integer) { Value = _sequence_no }
                                            );

                        _result.success = _delete_count;

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
        public static (int success, int failure) DropSelectClearing(int week_term = 1)
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = LTCX.GetNewContext())
            {
                var _sequence_no = WinnerReader.GetThisWeekSequenceNo() - week_term;

                try
                {
                    _ltctx.Database.BeginTransaction();

                    var _select_count = _ltctx.TbLionSelect
                                                .Where(s => s.SequenceNo <= _sequence_no && s.Ranking > 4)
                                                .Count();

                    if (_select_count > 0)
                    {
                        var _delete_count = _ltctx.ExecuteCommand(
                                                "DELETE FROM tb_lion_select WHERE sequence_no <= @sequence_no AND ranking > 4",
                                                new NpgsqlParameter("@sequence_no", NpgsqlDbType.Integer) { Value = _sequence_no }
                                            );

                        _result.success = _delete_count;
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
        public static (int success, int failure) DropNotifyClearing(int week_term = 1)
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = LTCX.GetNewContext())
            {
                var _notify_time = DateTime.Now.AddDays(week_term * -7);

                try
                {
                    _ltctx.Database.BeginTransaction();

                    var _notify_count = _ltctx.TbLionNotify
                                                .Where(n => n.IsRead == true && n.NotifyTime.ToUniversalTime() < _notify_time.ToUniversalTime())
                                                .Count();

                    if (_notify_count > 0)
                    {
                        var _delete_count = _ltctx.ExecuteCommand(
                                                "DELETE FROM tb_lion_notify WHERE notify_time < @notify_time",
                                                new NpgsqlParameter("@notify_time", NpgsqlDbType.TimestampTz) { Value = _notify_time }
                                            );

                        _result.success = _delete_count;
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
        public static (int success, int failure) LeaveChoiceClearing()
        {
            var _result = (success: 0, failure: 0);

            using (var _ltctx = LTCX.GetNewContext())
            {
                try
                {
                    _ltctx.Database.BeginTransaction();

                    var _choice_count = _ltctx.SelectQuery<TNoRecord>("SELECT COUNT(*) as no_record "
                            + "FROM tb_lion_choice "
                            + "WHERE login_id IN "
                            + "( "
                            + "     SELECT login_id "
                            + "     FROM "
                            + "     ( "
                            + "         SELECT login_id "
                            + "         FROM tb_lion_choice "
                            + "         GROUP BY login_id "
                            + "     ) c "
                            + "     WHERE NOT c.login_id IN "
                            + "     ( "
                            + "         SELECT login_id FROM tb_lion_member "
                            + "         WHERE is_alive=true"
                            + "     ) "
                            + ")"
                        )
                        .SingleOrDefault();

                    if (_choice_count.no_record > 0)
                    {
                        var _delete_count = _ltctx.ExecuteCommand("DELETE "
                            + "FROM tb_lion_choice "
                            + "WHERE login_id IN "
                            + "( "
                            + "     SELECT login_id "
                            + "     FROM "
                            + "     ( "
                            + "         SELECT login_id "
                            + "         FROM tb_lion_choice "
                            + "         GROUP BY login_id "
                            + "     ) c "
                            + "     WHERE NOT c.login_id IN "
                            + "     ( "
                            + "         SELECT login_id FROM tb_lion_member "
                            + "         WHERE is_alive=true"
                            + "     ) "
                            + ")"
                        );

                        _result.success = _delete_count;
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
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">60분에 한번 실행</param>
        /// <param name="console_out"></param>
        public static async void Start(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 60 * 1)
        {
            __clogger.WriteDebug("[worker] starting assister...");

            while (true)
            {
                await Task.Delay(0);

                try
                {
                    var _mail_recovery = await DoMailErrorRecoveryAsync();
                    {
                        var _no_recovery = _mail_recovery.success + _mail_recovery.failure;
                        if (_no_recovery > 0)
                            __clogger.WriteDebug($"[worker] mail recovery {_mail_recovery.success}/{_no_recovery}");
                    }

                    var _choice_clearing = DropChoiceClearing();
                    {
                        var _no_clearing = _choice_clearing.success + _choice_clearing.failure;
                        if (_no_clearing > 0)
                            __clogger.WriteDebug($"[worker] choice clearing {_choice_clearing.success}/{_no_clearing}");
                    }

                    var _select_clearing = DropSelectClearing();
                    {
                        var _no_clearing = _select_clearing.success + _select_clearing.failure;
                        if (_no_clearing > 0)
                            __clogger.WriteDebug($"[worker] select clearing {_select_clearing.success}/{_no_clearing}");
                    }

                    var _notify_clearing = DropNotifyClearing();
                    {
                        var _no_clearing = _notify_clearing.success + _notify_clearing.failure;
                        if (_no_clearing > 0)
                            __clogger.WriteDebug($"[worker] notify clearing {_notify_clearing.success}/{_no_clearing}");
                    }

                    var _leave_clearing = LeaveChoiceClearing();
                    {
                        var _no_clearing = _leave_clearing.success + _leave_clearing.failure;
                        if (_no_clearing > 0)
                            __clogger.WriteDebug($"[worker] leave clearing {_leave_clearing.success}/{_no_clearing}");
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

        /// <summary>
        ///
        /// </summary>
        public static void Stop()
        {
            __clogger.WriteDebug("[worker] stopping assister...");
        }

        public void Dispose()
        {
        }
    }
}