using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Data.DTO;
using Lion.Share.Data.Models;
using Lion.Share.Pipe;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OdinSdk.BaseLib.Net.Smtp;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace Lion.Worker.Engine.Processor
{
    /// <summary>
    /// choice(선택) & mail-send(메일발송)
    /// </summary>
    public class Choicer : BackgroundService
    {
        private readonly ILogger<Choicer> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly WinnerMember _winnerMember;
        private readonly WinnerScoring _winnerScoring;
        private readonly WinnerSelector _winnerSelector;

        private readonly PrintOutLottoLion _printOut;
        private readonly MailSenderLottoLion _mailSender;
        private readonly NotifyPushLottoLion _notifyPush;

        private readonly ConcurrentDictionary<string, int> _choiceSeqno;

        public Choicer(ILogger<Choicer> logger,
                IDbContextFactory<AppDbContext> contextFactory,
                WinnerMember winnerMember, WinnerScoring winnerScoring, WinnerSelector winnerSelector,
                PrintOutLottoLion printOut, MailSenderLottoLion mailSender, NotifyPushLottoLion notifyPush
            )
        {
            _logger = logger;
            _contextFactory = contextFactory;

            _winnerMember = winnerMember;
            _winnerScoring = winnerScoring;
            _winnerSelector = winnerSelector;

            _printOut = printOut;
            _mailSender = mailSender;
            _notifyPush = notifyPush;

            _choiceSeqno = new ConcurrentDictionary<string, int>();
        }

        private async Task<int> NextGameChoiceAsync(AppDbContext ltctx, mMember member, dChoice choice)
        {
            var _no_choice = 0;

            try
            {
                var _counter = ltctx.tb_lion_choice
                                        .Where(x => x.SequenceNo == choice.sequence_no && x.LoginId == choice.login_id)
                                        .Count();

                // 할당량 만큼 choice(선택) 되었으면 더이상 추가 하지 않음
                if (member.MaxSelectNumber > _counter)
                {
                    _no_choice = _winnerMember.WinnerChoice(ltctx, member, choice, _counter);
                    _winnerScoring.PutJackpot(ltctx, member, choice.sequence_no, (short)_no_choice);

                    await ltctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                _logger.LogError(ex, "receiver.choicer");
            }

            return _no_choice;
        }

        [SupportedOSPlatform("windows")]
        private async Task<int> NextGameSenderAsync(AppDbContext ltctx, mWinner winner, mMember member, dChoice choice)
        {
            var _result = 0;

            try
            {
                ltctx.Database.BeginTransaction();

                if (member.MailError == false)
                {
                    var _choices = ltctx.tb_lion_choice
                                        .Where(c => c.SequenceNo == choice.sequence_no && c.LoginId == choice.login_id
                                                    && (c.IsMailSent == false || choice.resend == true))
                                        .OrderBy(c => c.Digit1).ThenBy(c => c.Digit2).ThenBy(c => c.Digit3)
                                        .ThenBy(c => c.Digit4).ThenBy(c => c.Digit5).ThenBy(c => c.Digit6)
                                        .ToArray();

                    choice.no_choice = _choices.Length;

                    // 해당 회차에 처리하고자 하는 회원에게 메일 발송이 안된 entity만을 처리 합니다.
                    if (choice.no_choice > 0)
                    {
                        var _verify = await _mailSender.TestMailServerConnectionAsync(member);
                        if (_verify.error_code == SmtpError.GENERIC_SUCCESS)
                        {
                            var _curr_factor = _winnerSelector.GetFactor(ltctx, choice.sequence_no - 0);
                            var _last_factor = _winnerSelector.GetFactor(ltctx, choice.sequence_no - 1);

                            // 선택 번호들을 이미지 처리 함, 최종 zip(압축) 파일로 생성 합니다.
                            var _zip_file = _printOut.SaveLottoSheet(_choices, choice);

                            var _last_jackpot = _winnerScoring.GetJackpot(ltctx, member, choice.sequence_no - 1, member.MaxSelectNumber);

                            // zip 파일을 첨부 하여 메일 발송 합니다.
                            await _mailSender.SendLottoZipFile(_zip_file, choice, winner, member, _curr_factor, _last_factor, _last_jackpot);

                            // 메일 발송 했음을 표기 함
                            _choices
                                .ToList()
                                .ForEach(c => c.IsMailSent = true);

                            _result = choice.no_choice;
                        }
                        else
                        {
                            WriteMember(choice.sequence_no, choice.login_id, $"mail error '{member.EmailAddress}', {_verify.error_code}, {_verify.error_message}");
                        }

                        await ltctx.SaveChangesAsync();
                    }
                }

                ltctx.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                _logger.LogError(ex, "receiver.choicer");
            }

            return _result;
        }

        private async Task<bool> PushChoiceNotification(AppDbContext ltctx, dChoice choice, mMember member, int no_sending)
        {
            var _result = false;

            var _message = $"{choice.sequence_no}회 예측번호가 {member.EmailAddress}로 {no_sending}게임 발송 되었습니다.";
            {
                var _push = await _notifyPush.PushNotification(ltctx, member, "예측번호발송", _message);
                if (_push.success == true)
                    WriteMember(choice.sequence_no, choice.login_id, $"push choice");

                _result = _push.success;
            }

            return _result;
        }

        private void WriteMember(int sequence_no, string login_id, string message)
        {
            _logger.LogInformation($"[receiver.choicer] ({sequence_no}, {login_id}) => '{message}'");
        }

        [SupportedOSPlatform("windows")]
        public async Task<bool> StartChoiceAsync(AppDbContext ltctx, dChoice choice)
        {
            var _result = false;

            var _winner = ltctx.tb_lion_winner
                                .Where(w => w.SequenceNo == choice.sequence_no - 1)
                                .SingleOrDefault();

            var _member = ltctx.tb_lion_member
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
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[receiver.choicer] starting receiver memberQ...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var _read_line = (string)null;
                    if (PQueue.QChoicer.TryDequeue(out _read_line) == false)
                    {
                        await Task.Delay(10);
                        continue;
                    }

                    var _request = JsonConvert.DeserializeObject<VmsRequest<dChoice>>(_read_line);
                    {
                        var _choicer = _request.data;

                        var _sequence_no = _choiceSeqno.ContainsKey(_choicer.login_id) ? _choiceSeqno[_choicer.login_id] : 0;
                        if (_sequence_no != _choicer.sequence_no || _choicer.resend == true)
                        {
                            _choiceSeqno[_choicer.login_id] = _choicer.sequence_no;

                            using (var _ltctx = _contextFactory.CreateDbContext())
                                await StartChoiceAsync(_ltctx, _choicer);
                        }
                        else
                            WriteMember(_choicer.sequence_no, _choicer.login_id, "already processed user-id");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "receiver.choicer");
                }
            }
        }
    }
}