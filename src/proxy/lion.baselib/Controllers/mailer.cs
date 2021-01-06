using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Types;
using MailKit.Net.Smtp;
using MimeKit;
using OdinSdk.BaseLib.Net.Smtp;
using System.Threading.Tasks;

namespace LottoLion.BaseLib.Controllers
{
    public class MailSenderLottoLion
    {
        private static LConfig __cconfig = new LConfig();
        private static SmtpDirectSender __smtp_direct = new SmtpDirectSender();

        public string ServiceProvider
        {
            get
            {
                return __cconfig.GetAppString("lotto.sender.service.provider.name");
            }
        }

        public string ServiceProviderPhone
        {
            get
            {
                return __cconfig.GetAppString("lotto.sender.service.provider.phone");
            }
        }

        public string ServiceProviderHomePage
        {
            get
            {
                return __cconfig.GetAppString("lotto.sender.service.provider.homepage");
            }
        }

        public string ServiceProviderMailTo
        {
            get
            {
                return __cconfig.GetAppString("lotto.sender.service.provider.mailto");
            }
        }

        public string MailSenderName
        {
            get
            {
                return __cconfig.GetAppString("lotto.sheet.mail.sender.name");
            }
        }

        public string MailSenderAddress
        {
            get
            {
                return __cconfig.GetAppString("lotto.sheet.mail.sender.address");
            }
        }

        public string MailDeliveryServer
        {
            get
            {
                return __cconfig.GetAppString("lotto.sheet.mail.delivery.server");
            }
        }

        public int MailDeliveryPort
        {
            get
            {
                return __cconfig.GetAppInteger("lotto.sheet.mail.delivery.port");   //587
            }
        }

        public string MailDeliveryUserName
        {
            get
            {
                return __cconfig.GetAppString("lotto.sheet.mail.delivery.user.name");
            }
        }

        public string MailDeliveryUserPassword
        {
            get
            {
                return __cconfig.GetAppString("lotto.sheet.mail.delivery.user.password");
            }
        }

        public async Task<(SmtpError error_code, string error_message)> TestMailServerConnectionAsync(TbLionMember member)
        {
            var _result = (error_code: SmtpError.GENERIC_SUCCESS, error_message: "");

            try
            {
                var _verify = await __smtp_direct.VerifyMailServerConnection2Async(MailSenderAddress, member.EmailAddress);
                if (_verify.error_code != SmtpError.GENERIC_SUCCESS)
                {
                    member.MailError = true;
                    _result = _verify;
                }
            }
            catch
            {
                member.MailError = true;
                _result.error_code = SmtpError.CONNECT_ERROR;
            }

            return _result;
        }

        public async Task SendLottoZipFile(string zip_file_path, TChoice choice, TbLionWinner winner, TbLionMember member, TbLionFactor curr_factor, TbLionFactor last_factor, TbLionJackpot last_jackpot)
        {
            var _message = new MimeMessage();
            {
                _message.From.Add(new MailboxAddress(MailSenderName, MailSenderAddress));
                _message.To.Add(new MailboxAddress(member.LoginName, member.EmailAddress));

                _message.Subject = $"[로또번호] {member.LoginName} 님을 위한 [{choice.sequence_no}]회차 로또645 예측 번호 제공 {(member.IsDirectSend.Value ? "D" : "I")}";
            }

            var _curr_no_combination = curr_factor.LNoCombination + curr_factor.RNoCombination;
            var _curr_no_extraction = curr_factor.LNoExtraction + curr_factor.RNoExtraction;
            var _last_no_extraction = last_factor.LNoExtraction + last_factor.RNoExtraction;

            var _builder = new BodyBuilder();
            {
                _builder.HtmlBody = $"<div>"
                                    + $"<h4>안녕하세요 '{member.LoginName}'님, '로또645 예측 번호 제공 서비스' 입니다.</h4>"
                                    + $"<p></p>"
                                    + $"<h4>A. [{last_jackpot.SequenceNo}]회 추첨 결과</h4>"
                                    + $"<p>당첨번호: <strong>{winner.Digit1}, {winner.Digit2}, {winner.Digit3}, {winner.Digit4}, {winner.Digit5}, {winner.Digit6} ({winner.Digit7})</strong></p>"
                                    + $"<p>예측결과: {_last_no_extraction:#,##0} 게임</p>"
                                    + $"<ul>"
                                    + $"<li>1등: {last_factor.NoJackpot1,10:#,##0} 게임, {last_factor.WinningAmount1,15:#,##0} 원</li>"
                                    + $"<li>2등: {last_factor.NoJackpot2,10:#,##0} 게임, {last_factor.WinningAmount2,15:#,##0} 원</li>"
                                    + $"<li>3등: {last_factor.NoJackpot3,10:#,##0} 게임, {last_factor.WinningAmount3,15:#,##0} 원</li>"
                                    + $"<li>4등: {last_factor.NoJackpot4,10:#,##0} 게임, {last_factor.WinningAmount4,15:#,##0} 원</li>"
                                    + $"<li>5등: {last_factor.NoJackpot5,10:#,##0} 게임, {last_factor.WinningAmount5,15:#,##0} 원</li>"
                                    + $"</ul>"
                                    + $"<p>회원결과: {last_jackpot.NoChoice} 게임</p>"
                                    + $"<ul>"
                                    + $"<li>지난 주 발송 된 번호 중 {last_jackpot.NoJackpot} 게임, <strong>{last_jackpot.WinningAmount:#,##0}원</strong>이 당첨 되었습니다.</li>"
                                    + $"</ul>"
                                    + $"<h4>B. [{choice.sequence_no}]회 예측 번호</h4>"
                                    + $"<ul>"
                                    + $"<li>전체 45개의 번호 중 <strong>{_curr_no_combination:#,##0}개의 게임</strong>을 조합 했습니다.</li>"
                                    + $"<li>그 중 확률적으로 의미 있는 <strong>{_curr_no_extraction:#,##0}개의 게임</strong>이 추출 되었습니다.</li>"
                                    + $"<li>'{member.LoginName}'님에게 추출 된 {_curr_no_extraction:#,##0}개의 게임 중, 엄선해서 <strong>{choice.no_choice:#,##0}개의 게임</strong>을 보내 드립니다.</li>"
                                    + $"<li>특별히 원하시는 번호(3개 까지)를 아래 [운영자] 메일로 알려 주시면, 해당 번호가 포함 되어 있는 게임만 받을 수 있습니다.</li>"
                                    + $"<li> <a href='https://play.google.com/store/apps/details?id=kr.co.odinsoftware.LION'>로또사자(Android)</a>, "
                                    + $"<a href='https://itunes.apple.com/us/app/%EB%A1%9C%EB%98%90%EC%82%AC%EC%9E%90-lottolion/id1236106275?l=ko&ls=1&mt=8'>로또사자(iOS)</a> "
                                    + $"앱을 설치 하시면, 당첨내역과 이력 조회 및 게임 수 변경, 번호 지정이 가능 합니다.</li>"
                                    + $"</ul>"
                                    + $"<h4>C. 출력 방법</h4>"
                                    + $"<ul>"
                                    + $"<li>이미지 파일을 A4 프린터로 출력 후 로또 용지 크기와 동일하게 절단 합니다.</li>"
                                    + $"<li>A4 한장 당 최대 로또 용지 3장이 출력 됩니다.</li>"
                                    + $"<li>절취선에 맞춰 절단 후 로또 판매점에서 별도로 작성 하지 않고, 출력하신 용지로 바로 구매 가능 합니다.</li>"
                                    + $"<li>절취시 잘못 절단 하면 로또 판매점의 리더기에서 오류가 발생 할 수 있습니다.</li>"
                                    + $"<li>실제 로또 용지와 크기가 동일하게 절단하는 것이 중요 합니다.</li>"
                                    + $"<li>길이(19cm) 및 높이(8.2cm)는 같거나 조금 작아야 오류가 없습니다.</li>"
                                    + $"</ul>"
                                    + $"<h4>D. 회원 가입</h4>"
                                    + $"<ul>"
                                    + $"<li>매주 무료 예측 번호를 원하시는 주변 분이 있는 경우 '메일주소'와 '닉네임'을 아래 [운영자] 메일로 보내 주세요.</li>"
                                    + $"<li>메일 수신을 원하지 않는 경우에도 [운영자]에게 제목을 '수신거부'로 메일 보내 주시면 바로 중단 해 드립니다.</li>"
                                    + $"</ul>"
                                    + $"<h4>E. 연락처</h4>"
                                    + $"<ul>"
                                    + $"<li>회사명: {ServiceProvider}</li>"
                                    + $"<li>연락처: {ServiceProviderPhone}</li>"
                                    + $"<li>사이트: <a href='{ServiceProviderHomePage}'>{ServiceProviderHomePage}</a></li>"
                                    + $"<li>운영자: <a href='mailto:{ServiceProviderMailTo}'>{ServiceProviderMailTo}</a></li>"
                                    + $"</ul>"
                                    + "</div>"
                                    ;

                _builder.Attachments.Add(zip_file_path);
            }

            _message.Body = _builder.ToMessageBody();

            if (member.IsDirectSend == false)
            {
                using (var _client = new SmtpClient())
                {
                    _client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    _client.Connect(MailDeliveryServer, MailDeliveryPort, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    _client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    _client.Authenticate(MailDeliveryUserName, MailDeliveryUserPassword);

                    _client.Send(_message);
                    _client.Disconnect(true);
                }
            }
            else
            {
                await __smtp_direct.SendMailAsync(_message);
            }
        }
    }
}