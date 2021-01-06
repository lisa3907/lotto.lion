using LottoLion.BaseLib.Models.Entity;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using OdinSdk.BaseLib.Net.Smtp;
using OdinSdk.BaseLib.WebApi;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace LottoLion.WebApi.Controllers
{
    public partial class UserController
    {
        private static ConcurrentDictionary<string, string> __verify_email = new ConcurrentDictionary<string, string>();
        private static SmtpDirectSender __smtp_direct = new SmtpDirectSender();

        private void SendMail(string mail_address, string subject, string html_body)
        {
            var _builder = new BodyBuilder();
            {
                _builder.HtmlBody = html_body;
            }

            var _mime_message = new MimeMessage();
            {
                _mime_message.From.Add(new MailboxAddress(MailSenderName, MailSenderAddress));
                _mime_message.To.Add(new MailboxAddress(mail_address, mail_address));

                _mime_message.Subject = subject;
                _mime_message.Body = _builder.ToMessageBody();
            }

            using (var _client = new SmtpClient())
            {
                _client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                _client.Connect(MailDeliveryServer, MailDeliveryPort, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                _client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                _client.Authenticate(MailDeliveryUserName, MailDeliveryUserPassword);

                _client.Send(_mime_message);
                _client.Disconnect(true);
            }
        }

        private (bool success, string message) SendMailCheckNumber(string mail_address)
        {
            var _result = (success: false, message: "ok");

            var _serial_number = "";
            __verify_email.TryRemove(mail_address, out _serial_number);

            var _generator = new Random();
            _serial_number = _generator.Next(0, 1000000).ToString("D6");

            var _validate = __verify_email.TryAdd(mail_address, _serial_number);
            if (_validate == false)
            {
                _result.message = "검증 번호 생성 중 오류가 발생 하였습니다";
            }
            else
            {
                var _html_body = $"<div>"
                                  + $"<h3>안녕하세요 </h3>"
                                  + $"<p>&nbsp;</p>"
                                  + $"<h2>메일주소 검증용 일련번호를 보내 드립니다.</h2>"
                                  + $"<p>&nbsp;</p>"
                                  + $"<h3>일련번호: '{_serial_number}'</h3>"
                                  + $"<p>&nbsp;</p>"
                                  + $"<p>&nbsp;</p>"
                                  + $"<p>회사명: {ServiceProviderName}</p>"
                                  + $"<p>연락처: {ServiceProviderPhone}</p>"
                                  + $"<p>사이트: <a href=\"{ServiceProviderHomePage}\">{ServiceProviderHomePage}</a></p>"
                                  + $"<p>&nbsp;</p>"
                                  + "</div>"
                                  ;

                SendMail(mail_address, $"[LottoLion] 메일 주소 검증", _html_body);

                _result.message = $"'{mail_address}'로 [검증번호]가 발송 되었습니다";
                _result.success = true;
            }

            return _result;
        }

        private (bool success, string message) SendMailRecoveryId(TbLionMember member, string mail_address, string login_id)
        {
            var _result = (success: false, message: "ok");

            var _generator = new Random();
            var _password = _generator.Next(0, 99999999).ToString("D8");

            member.LoginPassword = Convert.ToBase64String(__cryptor.ComputeHash(_password));
            __db_context.SaveChanges();

            var _html_body = $"<div>"
                              + $"<h3>안녕하세요 </h3>"
                              + $"<p>&nbsp;</p>"
                              + $"<h2>로그인 ID와 임시 비번을 보내 드립니다.</h2>"
                              + $"<p>&nbsp;</p>"
                              + $"<h3>로그인-ID: '{login_id}'</h3>"
                              + $"<h3>임시 비번: '{_password}'</h3>"
                              + $"<p>&nbsp;</p>"
                              + $"<p>&nbsp;</p>"
                              + $"<p>회사명: {ServiceProviderName}</p>"
                              + $"<p>연락처: {ServiceProviderPhone}</p>"
                              + $"<p>사이트: <a href=\"{ServiceProviderHomePage}\">{ ServiceProviderHomePage}</a></p>"
                              + $"<p>&nbsp;</p>"
                              + "</div>"
                              ;

            SendMail(mail_address, $"[LottoLion] 임시 인증 정보", _html_body);

            _result.message = $"'{mail_address}'로 [아이디], [임시비번]이 발송 되었습니다";
            _result.success = true;

            return _result;
        }

        [Route("SendMailToCheckMailAddress")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> SendMailToCheckMailAddress(string mail_address)
        {
            return await CProxy.UsingAsync(async () =>
            {
                var _result = (success: false, message: "ok");

                while (true)
                {
                    if (String.IsNullOrEmpty(mail_address) == true)
                    {
                        _result.message = "메일 주소가 필요 합니다";
                        break;
                    }

                    var _member = __db_context.TbLionMember
                                           .Where(m => m.EmailAddress == mail_address)
                                           .SingleOrDefault();

                    if (_member != null && _member.IsAlive == true)
                    {
                        _result.message = $"입력하신 메일주소({mail_address})는 이미 사용 중 입니다.";
                        break;
                    }

                    if (await __smtp_direct.TestMailServerConnection2Async(MailSenderAddress, mail_address) == false)
                    {
                        _result.message = $"검증 하려고 하는 메일주소({mail_address})는 수신 가능 상태가 아닙니다.";
                        break;
                    }

                    _result = SendMailCheckNumber(mail_address);
                    break;
                }

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = ""
                });
            });
        }

        [Route("SendMailToChangeMailAddress")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> SendMailToChangeMailAddress(string mail_address)
        {
            return await CProxy.UsingAsync(async () =>
            {
                var _result = (success: false, message: "ok");

                while (true)
                {
                    if (String.IsNullOrEmpty(mail_address) == true)
                    {
                        _result.message = "메일 주소가 필요 합니다";
                        break;
                    }

                    var _member = __db_context.TbLionMember
                                                .Where(m => m.EmailAddress == mail_address)
                                                .SingleOrDefault();

                    if (_member != null)
                    {
                        _result.message = $"입력하신 메일주소({mail_address})는 이미 사용 중 입니다.";
                        break;
                    }

                    if (await __smtp_direct.TestMailServerConnection2Async(MailSenderAddress, mail_address) == false)
                    {
                        _result.message = $"검증 하려고 하는 메일주소({mail_address})는 수신 가능 상태가 아닙니다.";
                        break;
                    }

                    _result = SendMailCheckNumber(mail_address);
                    break;
                }

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = ""
                });
            });
        }

        [Route("SendMailToRecoveryId")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> SendMailToRecoveryId(string mail_address)
        {
            return await CProxy.UsingAsync(async () =>
            {
                var _result = (success: false, message: "ok");

                while (true)
                {
                    if (String.IsNullOrEmpty(mail_address) == true)
                    {
                        _result.message = "메일 주소가 필요 합니다";
                        break;
                    }

                    var _member = __db_context.TbLionMember
                                                .Where(m => m.EmailAddress == mail_address)
                                                .SingleOrDefault();

                    if (_member == null)
                    {
                        _result.message = $"입력하신 메일주소({mail_address})는 회원 메일이 아닙니다.";
                        break;
                    }

                    if (_member.IsAlive == false)
                    {
                        _result.message = $"입력하신 메일주소({mail_address})는 가입 회원이 아닙니다.";
                        break;
                    }

                    if (await __smtp_direct.TestMailServerConnection2Async(MailSenderAddress, mail_address) == false)
                    {
                        _result.message = $"검증 하려고 하는 메일주소({mail_address})는 수신 가능 상태가 아닙니다.";
                        break;
                    }

                    _result = SendMailRecoveryId(_member, mail_address, _member.LoginId);
                    break;
                }

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = ""
                });
            });
        }

        [Route("CheckMailAddress")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> CheckMailAddress(string mail_address, string check_number)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                while (true)
                {
                    if (String.IsNullOrEmpty(mail_address) == true)
                    {
                        _result.message = "메일 주소가 필요 합니다";
                        break;
                    }

                    var _value = "";

                    _result.success = __verify_email.TryGetValue(mail_address, out _value);
                    if (_result.success == false)
                    {
                        _result.message = "메일 주소에 해당하는 검증 번호를 찾을 수 없습니다";
                        break;
                    }

                    _result.success = _value == check_number;
                    if (_result.success == false)
                    {
                        _result.message = "검증 번호가 일치하지 않습니다";
                        break;
                    }

                    _result.message = "검증 되었습니다";
                    break;
                }

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = ""
                });
            });
        }
    }
}