using DnsClient;
using MimeKit;
using OdinSdk.BaseLib.Configuration;
using OdinSdk.BaseLib.Net.Dns;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

#pragma warning disable 8632

namespace OdinSdk.BaseLib.Net.Smtp
{
    public enum SmtpResponse : int
    {
        CONNECT_SUCCESS = 220,          // <domain> Service ready
        QUIT_SUCCESS = 221,             // <domain> Service closing transmission channel

        AUTH_SUCCESS = 235,             // Authentication successful
        AUTH_RESPONSE = 334,            // Authentication response

        GENERIC_SUCCESS = 250,          // Requested mail action okay, completed
        WILL_FORWARD = 251,             // User not local; will forward to <forward-path>
        DATA_SUCCESS = 354,             // Start mail input; end with <CRLF>.<CRLF>

        GENERIC_FAILURE = 999,          // Requested mail action okay, completed
    }

    public enum SmtpError : int
    {
        CONNECT_ERROR = 421,            // <domain> Service not available, closing transmission channel
        HELO_ERROR = 521,               // <domain> does not accept mail [rfc1846]
        MAIL_ERROR = 451,               // Requested action aborted: local error in processing
        RCPT_ERROR = 450,               // Requested mail action not taken: mailbox unavailable

        GENERIC_SUCCESS = 250,          // Requested mail action okay, completed
        NO_SUCH_USER = 550,             // No such user here
    }

    /// <summary>
    /// provides methods to send email via smtp direct to mail server
    /// </summary>
    public class SmtpDirectSender : IDisposable
    {
        private string __mail_host_name = "";

        public SmtpDirectSender(string host_fqdn_name = "")
        {
            if (String.IsNullOrEmpty(host_fqdn_name) == true)
                host_fqdn_name = System.Net.Dns.GetHostName() + ".localhost";

            __mail_host_name = host_fqdn_name;
        }

        private static List<IPAddress> __badDnsServers = null;

        private static List<IPAddress> BadDnsServers
        {
            get
            {
                if (__badDnsServers == null)
                    __badDnsServers = new List<IPAddress>();
                return __badDnsServers;
            }
        }

        private void SendData(Socket socket, string data)
        {
            if (socket != null)
            {
                var _bytes = Encoding.UTF8.GetBytes(data);
                socket.Send(_bytes, 0, _bytes.Length, SocketFlags.None);
            }
        }

        private void SendData2(TcpClient client, string data)
        {
            if (client != null && client.Connected)
            {
                var _bytes = Encoding.UTF8.GetBytes(data);

                var _stream = client.GetStream();
                _stream.Write(_bytes, 0, _bytes.Length);
            }
        }

        private async ValueTask<(int response, string message)> CheckResponse2Async(TcpClient client)
        {
            var _result = (response: (int)SmtpResponse.GENERIC_FAILURE, message: "");

            if (client != null)
            {
                for (var _t = 0; _t < 10; _t++)
                {
                    if (client.Available > 0)
                        break;

                    await Task.Delay(100);
                }

                if (client.Connected && client.Available > 0)
                {
                    var _stream = client.GetStream();

                    var _response_buff = new byte[1024];
                    await _stream.ReadAsync(_response_buff, 0, _response_buff.Length);

                    var _response_data = Encoding.UTF8.GetString(_response_buff);
                    {
                        _result.response = Convert.ToInt32(_response_data.Substring(0, 3));
                        _result.message = _response_data;
                    }
                }
            }

            return _result;
        }

        /// <summary>
        /// https://docs.microsoft.com/ko-kr/exchange/mail-flow/test-smtp-with-telnet?view=exchserver-2019
        /// </summary>
        /// <param name="client"></param>
        /// <param name="expected_code"></param>
        /// <returns></returns>
        private async ValueTask<(bool success, bool expected, string message)> CheckResponse2Async(TcpClient client, SmtpResponse expected_code)
        {
            var _response = await CheckResponse2Async(client);

            var _success = _response.response / 100 == 2;
            var _expected = _response.response == (int)expected_code;

            return (_success, _expected, _response.message);
        }

        private async ValueTask<(int response, string message)> CheckResponseAsync(Socket socket)
        {
            var _result = (response: (int)SmtpResponse.GENERIC_FAILURE, message: "");

            if (socket != null)
            {
                for (var _t = 0; _t < 10; _t++)
                {
                    if (socket.Available != 0)
                        break;

                    await Task.Delay(100);
                }

                if (socket.Available != 0)
                {
                    var _response_buff = new byte[1024];
                    socket.Receive(_response_buff, 0, socket.Available, SocketFlags.None);

                    var _response_data = Encoding.UTF8.GetString(_response_buff);
                    {
                        _result.response = Convert.ToInt32(_response_data.Substring(0, 3));
                        _result.message = _response_data;
                    }
                }
            }

            return _result;
        }

        private async ValueTask<(bool success, bool expected, string message)> CheckResponseAsync(Socket socket, SmtpResponse expected_code)
        {
            var _response = await CheckResponseAsync(socket);

            var _success = _response.response / 100 == 2;
            var _expected = _response.response == (int)expected_code;

            return (_success, _expected, _response.message);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public IPAddress? GetMxDomainAddressOld(string domain)
        {
            var _result = (IPAddress?)null;

            var _networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in _networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var ipProperties = networkInterface.GetIPProperties();
                    var dnsAddresses = ipProperties.DnsAddresses;

                    foreach (var dnsAdress in dnsAddresses)
                    {
                        if (BadDnsServers.Contains(dnsAdress) == true)
                            continue;

                        try
                        {
                            var _mx_domain = Resolver.MXLookup(domain, dnsAdress);
                            if (_mx_domain != null)
                            {
                                var _name = _mx_domain.FirstOrDefault().DomainName;

                                var _addresses = System.Net.Dns.GetHostAddressesAsync(_name).GetAwaiter().GetResult();
                                _result = _addresses.FirstOrDefault();
                                break;
                            }
                        }
                        catch (NoResponseException)
                        {
                            BadDnsServers.Add(dnsAdress);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    break;
                }
            }

            //throw new InvalidOperationException("Unable to find DNS Address");
            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public IPAddress? GetMxDomainAddress(string domain)
        {
            var _result = (IPAddress?)null;

            var _lookup_client = new LookupClient();
            {
                var _query_result = _lookup_client.Query(domain, QueryType.MX);
                if (_query_result.Answers.Count > 0)
                {
                    var _mx_record = (DnsClient.Protocol.MxRecord)_query_result.Answers.ToArray()[0];
                    var _server_name = _mx_record.Exchange.Value;

                    var _server_ipadrs = System.Net.Dns.GetHostAddressesAsync(_server_name).GetAwaiter().GetResult();
                    if (_server_ipadrs.Count() > 0)
                        _result = _server_ipadrs[0];
                }
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async ValueTask<bool> SendMailAsync(MimeMessage message)
        {
            var _result = false;

            if (message.To.Mailboxes.Count() > 0)
            {
                var _domain_uri = new Uri($"mailto:{message.To.Mailboxes.ToArray()[0].Address}");

                var _server_ip_address = GetMxDomainAddress(_domain_uri.Host);
                if (_server_ip_address != null)
                    _result = await SendMailAsync(message, _server_ip_address);
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ip_address"></param>
        /// <param name="smtp_port"></param>
        /// <returns></returns>
        public async ValueTask<bool> SendMailAsync(MimeMessage message, IPAddress ip_address, int smtp_port = 25)
        {
            return await SendMailAsync(message, new IPEndPoint(ip_address, smtp_port));
        }

        private const string __end_of_line = "\r\n";

        public async ValueTask<bool> ValidateUserAsync(string mail_address, string password, IPEndPoint end_point)
        {
            using (var _socket = new Socket(end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                _socket.Connect(end_point);

                var _checker = await CheckResponseAsync(_socket, SmtpResponse.CONNECT_SUCCESS);
                if (!_checker.expected)
                    return false;

                SendData(_socket, $"EHLO {System.Net.Dns.GetHostName()}" + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.GENERIC_SUCCESS);
                if (!_checker.success)
                    return false;

                //SendData(_socket, $"VRFY {mail_address}" + __end_of_line);
                //if (!CheckResponse(_socket, SmtpResponse.GENERIC_SUCCESS) || !CheckResponse(_socket, SmtpResponse.WILL_FORWARD))
                //    return false;

                //SendData(_socket, $"STARTTLS" + __end_of_line);
                //if (await CheckResponseAsync(_socket, SmtpResponse.CONNECT_SUCCESS) == false)
                //    return false;

                SendData(_socket, $"AUTH {mail_address}" + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.AUTH_RESPONSE);
                if (!_checker.expected)
                    return false;

                SendData(_socket, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{mail_address}")) + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.AUTH_RESPONSE);
                if (!_checker.expected)
                    return false;

                SendData(_socket, Convert.ToBase64String(Encoding.UTF8.GetBytes($"{password}")) + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.AUTH_SUCCESS);
                if (!_checker.expected)
                    return false;
            }

            return true;
        }

        public async ValueTask<bool> TestMailServerConnection2Async(string from_address, string to_address)
        {
            return (await VerifyMailServerConnection2Async(from_address, to_address)).error_code == SmtpError.GENERIC_SUCCESS;
        }

        public async ValueTask<(SmtpError error_code, string error_message)> VerifyMailServerConnection2Async(string from_address, string to_address)
        {
            var _domain_uri = new Uri($"mailto:{to_address}");

            var _server_ip_address = GetMxDomainAddress(_domain_uri.Host);
            return await VerifyMailServerConnection2Async(from_address, to_address, _server_ip_address);
        }

        public async ValueTask<(SmtpError error_code, string error_message)> VerifyMailServerConnection2Async(string from_address, string to_address, IPAddress? ip_address, int smtp_port = 25)
        {
            return await VerifyMailServerConnection2Async(from_address, to_address, new IPEndPoint(ip_address, smtp_port));
        }

        public async ValueTask<(SmtpError error_code, string error_message)> VerifyMailServerConnection2Async(string from_address, string to_address, IPEndPoint end_point)
        {
            var _result = (error_code: SmtpError.GENERIC_SUCCESS, error_message: "");

            try
            {
                using (var _client = new TcpClient(end_point.AddressFamily))
                {
                    var _connector = _client.ConnectAsync(end_point.Address, end_point.Port);
                    while (_connector.Wait(10000) == true)
                    {
                        var _checker = await CheckResponse2Async(_client, SmtpResponse.CONNECT_SUCCESS);
                        if (!_checker.expected)
                        {
                            _result.error_code = SmtpError.CONNECT_ERROR;
                            _result.error_message = _checker.message;
                            break;
                        }

                        // send HELO and test the response for code 250 = proper response
                        SendData2(_client, $"HELO {__mail_host_name}" + __end_of_line);

                        _checker = await CheckResponse2Async(_client, SmtpResponse.GENERIC_SUCCESS);
                        if (!_checker.success)
                        {
                            _result.error_code = SmtpError.HELO_ERROR;
                            _result.error_message = _checker.message;
                            break;
                        }

                        SendData2(_client, $"MAIL FROM: <{from_address}>" + __end_of_line);

                        _checker = await CheckResponse2Async(_client, SmtpResponse.GENERIC_SUCCESS);
                        if (!_checker.success)
                        {
                            _result.error_code = SmtpError.MAIL_ERROR;
                            _result.error_message = _checker.message;
                            break;
                        }

                        SendData2(_client, $"RCPT TO: <{to_address}>" + __end_of_line);

                        _checker = await CheckResponse2Async(_client, SmtpResponse.GENERIC_SUCCESS);
                        if (!_checker.success)
                        {
                            _result.error_code = SmtpError.RCPT_ERROR;
                            _result.error_message = _checker.message;
                            break;
                        }

                        SendData2(_client, "QUIT" + __end_of_line);
                        await CheckResponse2Async(_client, SmtpResponse.QUIT_SUCCESS);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _result.error_code = SmtpError.CONNECT_ERROR;
                _result.error_message = ex.Message;
            }

            // if we got here it's that we can connect to the smtp server
            return _result;
        }

        public async ValueTask<bool> TestMailServerConnectionAsync(string from_address, string to_address)
        {
            return await VerifyMailServerConnectionAsync(from_address, to_address) == SmtpError.GENERIC_SUCCESS;
        }

        public async ValueTask<SmtpError> VerifyMailServerConnectionAsync(string from_address, string to_address)
        {
            var _domain_uri = new Uri($"mailto:{to_address}");

            var _server_ip_address = GetMxDomainAddress(_domain_uri.Host);
            return await VerifyMailServerConnectionAsync(from_address, to_address, _server_ip_address);
        }

        private async ValueTask<SmtpError> VerifyMailServerConnectionAsync(string from_address, string to_address, IPAddress? ip_address, int smtp_port = 25)
        {
            return await VerifyMailServerConnectionAsync(from_address, to_address, new IPEndPoint(ip_address, smtp_port));
        }

        private async ValueTask<SmtpError> VerifyMailServerConnectionAsync(string from_address, string to_address, IPEndPoint end_point)
        {
            using (var _socket = new Socket(end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                //try to connect and test the rsponse for code 220 = success
                _socket.Connect(end_point);

                var _checker = await CheckResponseAsync(_socket, SmtpResponse.CONNECT_SUCCESS);
                if (!_checker.expected)
                    return SmtpError.CONNECT_ERROR;

                // send HELO and test the response for code 250 = proper response
                SendData(_socket, $"HELO {__mail_host_name}" + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.CONNECT_SUCCESS);
                if (!_checker.success)
                    return SmtpError.HELO_ERROR;

                SendData(_socket, $"MAIL FROM: <{from_address}>" + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.CONNECT_SUCCESS);
                if (!_checker.success)
                    return SmtpError.MAIL_ERROR;

                SendData(_socket, $"RCPT TO: <{to_address}>" + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.CONNECT_SUCCESS);
                if (!_checker.success)
                    return SmtpError.RCPT_ERROR;

                SendData(_socket, "QUIT" + __end_of_line);
                await CheckResponseAsync(_socket, SmtpResponse.QUIT_SUCCESS);
            }

            // if we got here it's that we can connect to the smtp server
            return SmtpError.GENERIC_SUCCESS;
        }

        private string GetBodyData(MimeMessage message)
        {
            var _result = new StringBuilder();
            {
                // from
                var _from_address = message.From.Mailboxes.ToArray()[0].Address;
                _result.Append($"From: <{_from_address}>" + __end_of_line);

                var _i = 0;

                // to
                _result.Append("To: ");

                var _tos = message.To ?? new InternetAddressList();
                foreach (var _to in _tos.Mailboxes)
                {
                    _result.Append(_i > 0 ? "," : "");
                    _result.Append($"<{_to.Address}>");

                    _i++;
                }
                _result.Append(__end_of_line);

                _i = 0;

                // cc
                _result.Append("Cc: ");

                var _ccs = message.Cc ?? new InternetAddressList();
                foreach (var _cc in _ccs.Mailboxes)
                {
                    _result.Append(_i > 0 ? "," : "");
                    _result.Append($"<{_cc.Address}>");

                    _i++;
                }
                _result.Append(__end_of_line);

                // ???
                _result.Append("Content-Type: text/html; charset=UTF-8" + __end_of_line);

                _result.Append("Date: ");
                _result.Append(CUnixTime.UtcNow.ToString("ddd, d M y H:m:s z"));
                _result.Append(__end_of_line);

                // subject
                _result.Append("Subject: " + message.Subject + __end_of_line);
                _result.Append("X-Mailer: OdinSdk.Direct.SMTPMail V1.0" + __end_of_line);
            }

            var _message_body = message.Body.ToString();
            if (_message_body.EndsWith(__end_of_line) == false)
                _message_body += __end_of_line;

            if (message.Attachments.Count() > 0)
            {
                _result.Append("MIME-Version: 1.0" + __end_of_line);
                _result.Append("Content-Type: multipart/mixed; boundary=unique-boundary-1" + __end_of_line);
                _result.Append(__end_of_line);
                _result.Append("This is a multi-part message in MIME format." + __end_of_line);

                var _body = new StringBuilder();
                {
                    _body.Append("--unique-boundary-1" + __end_of_line);
                    _body.Append("Content-Type: text/plain" + __end_of_line);
                    _body.Append("Content-Transfer-Encoding: 7Bit" + __end_of_line);
                    _body.Append(__end_of_line);
                    _body.Append(_message_body + __end_of_line);
                    _body.Append(__end_of_line);
                }

                foreach (var _a in message.Attachments)
                {
                    var _attchment = _a as MimeKit.MimePart;
                    if (_attchment == null)
                        continue;

                    _body.Append("--unique-boundary-1" + __end_of_line);
                    _body.Append("Content-Type: application/octet-stream; file=" + _attchment.FileName + __end_of_line);
                    _body.Append("Content-Transfer-Encoding: base64" + __end_of_line);
                    _body.Append("Content-Disposition: attachment; filename=" + _attchment.FileName + __end_of_line);
                    _body.Append(__end_of_line);

                    using (var _ms = new MemoryStream())
                    {
                        _attchment.WriteTo(_ms);

                        var _binary_data = _ms.ToArray();
                        var _base64_string = Convert.ToBase64String(_binary_data, 0, _binary_data.Length);

                        for (var i = 0; i < _base64_string.Length;)
                        {
                            var _next_chunk = 100;
                            if (_base64_string.Length - (i + _next_chunk) < 0)
                                _next_chunk = _base64_string.Length - i;

                            _body.Append(_base64_string.Substring(i, _next_chunk));
                            _body.Append(__end_of_line);

                            i += _next_chunk;
                        }

                        _body.Append(__end_of_line);
                    }
                }

                _message_body = _body.ToString();
            }

            _result.Append(__end_of_line);
            _result.Append(_message_body);
            _result.Append("." + __end_of_line);
            _result.Append(__end_of_line);
            _result.Append(__end_of_line);

            return _result.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="end_point"></param>
        /// <returns></returns>
        public async ValueTask<bool> SendMailAsync(MimeMessage message, IPEndPoint end_point)
        {
            using (var _socket = new Socket(end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                _socket.Connect(end_point);

                var _checker = await CheckResponseAsync(_socket, SmtpResponse.CONNECT_SUCCESS);
                if (!_checker.expected)
                    return false;

                SendData(_socket, $"HELO {__mail_host_name}" + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.GENERIC_SUCCESS);
                if (!_checker.success)
                    return false;

                var _from_address = message.From.Mailboxes.ToArray()[0].Address;
                SendData(_socket, $"MAIL FROM: <{_from_address}>" + __end_of_line);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.GENERIC_SUCCESS);
                if (!_checker.success)
                    return false;

                var _tos = message.To ?? new InternetAddressList();
                foreach (var _to in _tos.Mailboxes)
                {
                    SendData(_socket, $"RCPT TO: <{_to.Address}>" + __end_of_line);

                    _checker = await CheckResponseAsync(_socket, SmtpResponse.GENERIC_SUCCESS);
                    if (!_checker.success)
                        return false;
                }

                var _ccs = message.Cc ?? new InternetAddressList();
                foreach (var _cc in _ccs.Mailboxes)
                {
                    SendData(_socket, $"RCPT TO: <{_cc.Address}>" + __end_of_line);

                    _checker = await CheckResponseAsync(_socket, SmtpResponse.GENERIC_SUCCESS);
                    if (!_checker.success)
                        return false;
                }

                SendData(_socket, ("DATA" + __end_of_line));

                _checker = await CheckResponseAsync(_socket, SmtpResponse.DATA_SUCCESS);
                if (!_checker.expected)
                    return false;

                var _body_data = GetBodyData(message);

                SendData(_socket, _body_data);

                _checker = await CheckResponseAsync(_socket, SmtpResponse.GENERIC_SUCCESS);
                if (!_checker.success)
                    return false;

                SendData(_socket, "QUIT" + __end_of_line);
                await CheckResponseAsync(_socket, SmtpResponse.QUIT_SUCCESS);

                //_socket.Close();
            }

            return true;
        }

        ~SmtpDirectSender()
        {
            this.Dispose(false);
        }

        private bool disposed;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {             
            }

            this.disposed = true;
        }
    }
}