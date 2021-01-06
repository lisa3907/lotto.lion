using LottoLion.BaseLib.Controllers;
using LottoLion.BaseLib.Models;
using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Queues;
using LottoLion.BaseLib.Types;
using OdinSdk.BaseLib.Net.Smtp;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LottoLion.Worker.Engine
{
    /// <summary>
    /// choice(선택) & mail-send(메일발송)
    /// </summary>
    public partial class Receiver
    {
        private static MemberQ __memberQ = new MemberQ();
        private static WinnerMember __winner_member = new WinnerMember();

        private static PrintOutLottoLion __lotto_printer = new PrintOutLottoLion();
        private static MailSenderLottoLion __mail_sender = new MailSenderLottoLion();
        private static NotifyPushLottoLion __push_notify = new NotifyPushLottoLion();

        private static ConcurrentDictionary<string, int> __choice_seqno = new ConcurrentDictionary<string, int>();

        private static async Task<int> NextGameChoiceAsync(LottoLionContext ltctx, TbLionMember member, TChoice choice)
        {
            var _no_choice = 0;

            try
            {
                var _counter = ltctx.TbLionChoice
                                        .Where(x => x.SequenceNo == choice.sequence_no && x.LoginId == choice.login_id)
                                        .Count();

                // 할당량 만큼 choice(선택) 되었으면 더이상 추가 하지 않음
                if (member.MaxSelectNumber > _counter)
                {
                    _no_choice = __winner_member.WinnerChoice(ltctx, member, choice, _counter);
                    __winner_scoring.PutJackpot(ltctx, member, choice.sequence_no, (short)_no_choice);

                    await ltctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                __clogger.WriteLog(ex);
            }

            return _no_choice;
        }

        private static async Task<int> NextGameSenderAsync(LottoLionContext ltctx, TbLionWinner winner, TbLionMember member, TChoice choice)
        {
            var _result = 0;

            try
            {
                ltctx.Database.BeginTransaction();

                if (member.MailError == false)
                {
                    var _choices = ltctx.TbLionChoice
                                        .Where(c => c.SequenceNo == choice.sequence_no && c.LoginId == choice.login_id
                                                    && (c.IsMailSent == false || choice.resend == true))
                                        .OrderBy(c => c.Digit1).ThenBy(c => c.Digit2).ThenBy(c => c.Digit3)
                                        .ThenBy(c => c.Digit4).ThenBy(c => c.Digit5).ThenBy(c => c.Digit6)
                                        .ToArray();

                    choice.no_choice = _choices.Length;

                    // 해당 회차에 처리하고자 하는 회원에게 메일 발송이 안된 entity만을 처리 합니다.
                    if (choice.no_choice > 0)
                    {
                        var _verify = await __mail_sender.TestMailServerConnectionAsync(member);
                        if (_verify.error_code == SmtpError.GENERIC_SUCCESS)
                        {
                            var _curr_factor = __winner_selector.GetFactor(ltctx, choice.sequence_no - 0);
                            var _last_factor = __winner_selector.GetFactor(ltctx, choice.sequence_no - 1);

                            // 선택 번호들을 이미지 처리 함, 최종 zip(압축) 파일로 생성 합니다.
                            var _zip_file = __lotto_printer.SaveLottoSheet(_choices, choice);

                            var _last_jackpot = __winner_scoring.GetJackpot(ltctx, member, choice.sequence_no - 1, member.MaxSelectNumber);

                            // zip 파일을 첨부 하여 메일 발송 합니다.
                            await __mail_sender.SendLottoZipFile(_zip_file, choice, winner, member, _curr_factor, _last_factor, _last_jackpot);

                            // 메일 발송 했음을 표기 함
                            _choices
                                .ToList()
                                .ForEach(c => c.IsMailSent = true);

                            _result = choice.no_choice;

                            await ltctx.SaveChangesAsync();
                        }
                        else
                        {
                            WriteMember(choice.sequence_no, choice.login_id, $"mail error '{member.EmailAddress}', {_verify.error_code}, {_verify.error_message}");
                        }
                    }
                }

                ltctx.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();
                __clogger.WriteLog(ex);
            }

            return _result;
        }

        private static async Task<bool> PushChoiceNotification(LottoLionContext ltctx, TChoice choice, TbLionMember member, int no_sending)
        {
            var _result = false;

            var _message = $"{choice.sequence_no}회 예측번호가 {member.EmailAddress}로 {no_sending}게임 발송 되었습니다.";
            {
                var _push = await __push_notify.PushNotification(ltctx, member, "예측번호발송", _message);
                if (_push.success == true)
                    WriteMember(choice.sequence_no, choice.login_id, $"push choice");

                _result = _push.success;
            }

            return _result;
        }

        private static void WriteMember(int sequence_no, string login_id, string message)
        {
            __clogger.WriteDebug($"[recv-member] ({sequence_no}, {login_id}) => '{message}'");
        }

        public static async Task<bool> StartChoiceAsync(LottoLionContext ltctx, TChoice choice)
        {
            var _result = false;

            var _winner = ltctx.TbLionWinner
                                .Where(w => w.SequenceNo == choice.sequence_no - 1)
                                .SingleOrDefault();

            var _member = ltctx.TbLionMember
                                .Where(m => m.LoginId == choice.login_id && m.IsAlive == true)
                                .SingleOrDefault();

            if (_winner != null && _member != null)
            {
                // 다음 회차 번호를 select(추출) 테이블에서 choice(선택) 합니다.
                var _no_choice = await NextGameChoiceAsync(ltctx, _member, choice);
                if (_no_choice > 0)
                    WriteMember(choice.sequence_no, choice.login_id, $"game choiced {_no_choice} games");

                if (_no_choice > 0 || choice.resend == true)
                {
                    // 위에서 선택한 게임을 메일로 발송 합니다.
                    var _no_sending = await NextGameSenderAsync(ltctx, _winner, _member, choice);
                    if (_no_sending > 0)
                    {
                        WriteMember(choice.sequence_no, choice.login_id, $"mail sent {_no_sending} games");

                        var _is_notify = await PushChoiceNotification(ltctx, choice, _member, _no_sending);
                        if (_is_notify == true)
                            WriteMember(choice.sequence_no, choice.login_id, $"push notified");
                    }
                }

                _result = true;
            }

            return _result;
        }

        /// <summary>
        /// member(회원)에게 할당 된 최대 번호 갯수 만큼을 select(추출) 테이블에서 choice(선택) 테이블로 복사 후
        /// A4 사이즈에 최대 15 게임씩을 이미지로 만들어 메일 발송 합니다.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">1분에 한번 큐 확인</param>
        public static async void StartMemberQ(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 1 * 1)
        {
            __clogger.WriteDebug("[recv-member] starting receiver memberQ...");

            while (true)
            {
                await __memberQ.RecvQAsync<TChoice>(async (_choicer) =>
                {
                    try
                    {
                        if (_choicer.sequence_no >= 800)
                        {
                            var _sequence_no = __choice_seqno.ContainsKey(_choicer.login_id) ? __choice_seqno[_choicer.login_id] : 0;
                            if (_sequence_no != _choicer.sequence_no || _choicer.resend == true)
                            {
                                __choice_seqno[_choicer.login_id] = _choicer.sequence_no;

                                using (var _ltctx = LTCX.GetNewContext())
                                {
                                    await StartChoiceAsync(_ltctx, _choicer);
                                }
                            }
                            else
                                WriteMember(_choicer.sequence_no, _choicer.login_id, "already processed user-id");
                        }
                        else
                            WriteMember(_choicer.sequence_no, _choicer.login_id, "out-of-choice failure");
                    }
                    catch (Exception ex)
                    {
                        __clogger.WriteLog(ex);
                    }
                    finally
                    {
                        GC.Collect(2, GCCollectionMode.Optimized);
                    }

                    return true;
                },
                tokenSource: tokenSource);

                var _cancelled = tokenSource.Token.WaitHandle.WaitOne(sleep_seconds * 1000);
                if (_cancelled == true)
                    break;
            }
        }
    }
}