using LottoLion.BaseLib.Models.Entity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OdinSdk.BaseLib.Cryption;
using OdinSdk.BaseLib.Logger;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LottoLion.BaseLib.Options
{
    public class UserManager
    {
        private static CLogger __clogger = new CLogger();
        private static CCryption __cryptor = new CCryption();

        private JwtIssuerOptions __jwtOptions;

        public UserManager(JwtIssuerOptions jwtOptions)
        {
            __jwtOptions = jwtOptions;
            ThrowIfInvalidOptions(__jwtOptions);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="login_id">로그인 사용자 아이디</param>
        /// <param name="mail_address">메일 주소</param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<string> GetEncodedJwt(ApplicationUser app_user, Claim identity)
        {
            var _claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, app_user.login_id),
                new Claim(JwtRegisteredClaimNames.Email, app_user.mail_address),
                new Claim(JwtRegisteredClaimNames.Jti, await __jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(__jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Role, "ValidUsers", __jwtOptions.Issuer),
                identity
            };

            // Create the JWT security token and encode it.
            var _json_web_token = new JwtSecurityToken
                (
                    issuer: __jwtOptions.Issuer,
                    audience: __jwtOptions.Audience,
                    claims: _claims,
                    notBefore: __jwtOptions.NotBefore,
                    expires: __jwtOptions.Expiration,
                    signingCredentials: __jwtOptions.SigningCredentials
                );

            return (new JwtSecurityTokenHandler()).WriteToken(_json_web_token);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="date"></param>
        ///<returns>Date converted to seconds since Unix epoch(Jan 1, 1970, midnight UTC).</returns>
        public long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        /// <summary>
        ///
        /// </summary>
        /// <param name="options"></param>
        public void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.JtiGenerator == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="password">암호</param>
        /// <returns></returns>
        public string ComupteHashString(string password)
        {
            return Convert.ToBase64String(__cryptor.ComputeHash(password));
        }

        /// <summary>
        /// 페북 토큰과 메일 주소를 확인 합니다.
        /// </summary>
        /// <param name="facebook_token"></param>
        /// <param name="facebook_id"></param>
        /// <param name="mail_address">메일 주소</param>
        /// <returns></returns>
        public async Task<bool> VerifyFacebookToken(string facebook_token, string facebook_id)
        {
            var _result = false;

            try
            {
                using (var _client = new HttpClient())
                {
                    var _response = await _client.GetStringAsync($"https://graph.facebook.com/me/?access_token={facebook_token}");

                    var _json = (JObject)JsonConvert.DeserializeObject(_response);

                    var _token = (JToken)null;
                    if (_json.TryGetValue("id", out _token) == true)
                    {
                        var _value = (JValue)_token;
                        if (_value.Value.ToString() == facebook_id)
                            _result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                __clogger.WriteLog(ex);
            }

            return _result;
        }

        public bool IsWebOrUnknownType(string device_type)
        {
            // "P": for webapp, "U": unknown(회원가입, 정보변경)
            return (device_type == "P" || device_type == "U");
        }

        public bool CheckDevice(string device_type, string device_id)
        {
            var _result = true;

            if (IsWebOrUnknownType(device_type) == false)
            {
                _result = false;

                if (String.IsNullOrEmpty(device_type) == false && String.IsNullOrEmpty(device_id) == false)
                {
                    // Iphone, Android, WindowsPhone
                    if (device_type == "I" || device_type == "A" || device_type == "W")
                        _result = true;
                }
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="app_user"></param>
        /// <returns></returns>
        public async Task<(Claim identity, string mail_address)> GetClaimsIdentityByFacebook(LottoLionContext ltctx, ApplicationUser app_user)
        {
            var _result = (identity: (Claim)null, mail_address: "");

            if (CheckDevice(app_user.device_type, app_user.device_id) == true)
            {
                var _member = ltctx.TbLionMember
                                        .Where(m => m.LoginId == app_user.facebook_id && m.IsAlive == true)
                                        .SingleOrDefault();

                if (_member != null)
                {
                    if (await VerifyFacebookToken(app_user.facebook_token, app_user.facebook_id) == true)
                    {
                        if (IsWebOrUnknownType(app_user.device_type) == false)
                        {
                            if (_member.DeviceType != app_user.device_type || _member.DeviceId != app_user.device_id)
                            {
                                _member.DeviceType = app_user.device_type;
                                _member.DeviceId = app_user.device_id;
                            }
                        }

                        _result.identity = new Claim("UserType", "Member");
                        _result.mail_address = _member.EmailAddress;
                    }

                    _member.UpdateTime = DateTime.Now;
                    ltctx.SaveChanges();
                }
            }

            return _result;
        }

        /// <summary>
        /// 메일 주소와 암호가 일치 하는지 확인 합니다.
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="app_user"></param>
        /// <returns></returns>
        public (Claim identity, string mail_address) GetClaimsIdentityByLoginId(LottoLionContext ltctx, ApplicationUser app_user)
        {
            var _result = (identity: (Claim)null, mail_address: "");

            if (CheckDevice(app_user.device_type, app_user.device_id) == true)
            {
                var _member = ltctx.TbLionMember
                                        .Where(m => m.LoginId == app_user.login_id && m.IsAlive == true)
                                        .SingleOrDefault();

                if (_member != null)
                {
                    if (IsWebOrUnknownType(app_user.device_type) == false)
                    {
                        if (_member.DeviceType != app_user.device_type || _member.DeviceId != app_user.device_id)
                        {
                            _member.DeviceType = app_user.device_type;
                            _member.DeviceId = app_user.device_id;
                        }
                    }

                    var _hash_value = Convert.FromBase64String(_member.LoginPassword);
                    {
                        var _pass_hash = __cryptor.VerifyHash(app_user.password, hash_value: _hash_value);
                        if (_pass_hash == true)
                        {
                            _result.identity = new Claim("UserType", "Member");
                            _result.mail_address = _member.EmailAddress;
                        }
                    }

                    _member.UpdateTime = DateTime.Now;
                    ltctx.SaveChanges();
                }
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="login_id">로그인 사용자 아이디</param>
        /// <param name="login_name">로그인 사용자 명</param>
        /// <param name="password">암호</param>
        /// <param name="device_type">장치구분("I"phone, "A"ndroid, "W"indowsPhone</param>
        /// <param name="device_id">장치 식별자</param>
        /// <param name="mail_address">메일 주소</param>
        public bool AddNewMember(LottoLionContext ltctx, ApplicationUser app_user)
        {
            var _result = false;

            if (CheckDevice(app_user.device_type, app_user.device_id) == true)
            {
                var _new_member = new TbLionMember
                {
                    LoginId = app_user.login_id,
                    LoginName = app_user.login_name,

                    LoginPassword = Convert.ToBase64String(__cryptor.ComputeHash(app_user.password)),

                    DeviceType = app_user.device_type,
                    DeviceId = app_user.device_id ?? "",

                    AccessToken = "",
                    IsAlive = true,

                    PhoneNumber = "",
                    EmailAddress = app_user.mail_address,

                    MailError = false,
                    IsMailSend = true,
                    IsDirectSend = false,

                    IsNumberChoice = true,
                    MaxSelectNumber = 30,

                    Digit1 = 0,
                    Digit2 = 0,
                    Digit3 = 0,

                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,

                    Remark = ""
                };

                ltctx.TbLionMember.Add(_new_member);
                ltctx.SaveChanges();

                _result = true;
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="login_id">로그인 사용자 아이디</param>
        /// <param name="login_name">로그인 사용자 명</param>
        /// <param name="password">암호</param>
        /// <param name="device_type">장치구분("I"phone, "A"ndroid, "W"indowsPhone, "U"nknown)</param>
        /// <param name="device_id">장치 식별자</param>
        /// <param name="mail_address">메일 주소</param>
        public bool ReUseMember(LottoLionContext ltctx, TbLionMember old_member, ApplicationUser app_user)
        {
            var _result = false;

            if (CheckDevice(app_user.device_type, app_user.device_id) == true)
            {
                if (old_member.LoginId != app_user.login_id)
                {
                    var _new_member = new TbLionMember
                    {
                        LoginId = app_user.login_id,
                        LoginName = app_user.login_name,

                        LoginPassword = Convert.ToBase64String(__cryptor.ComputeHash(app_user.password)),

                        DeviceType = app_user.device_type,
                        DeviceId = app_user.device_id ?? "",

                        AccessToken = "",
                        IsAlive = true,

                        PhoneNumber = "",
                        EmailAddress = app_user.mail_address,

                        MailError = false,
                        IsMailSend = true,
                        IsDirectSend = false,

                        IsNumberChoice = true,
                        MaxSelectNumber = 30,

                        Digit1 = 0,
                        Digit2 = 0,
                        Digit3 = 0,

                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,

                        Remark = ""
                    };

                    ltctx.TbLionMember.Remove(old_member);
                    ltctx.TbLionMember.Add(_new_member);
                }
                else
                {
                    old_member.LoginId = app_user.login_id;
                    old_member.LoginName = app_user.login_name;

                    old_member.LoginPassword = Convert.ToBase64String(__cryptor.ComputeHash(app_user.password));

                    old_member.DeviceType = app_user.device_type;
                    old_member.DeviceId = app_user.device_id ?? "";

                    old_member.AccessToken = "";
                    old_member.IsAlive = true;

                    old_member.PhoneNumber = "";
                    old_member.EmailAddress = app_user.mail_address;

                    old_member.MailError = false;
                    old_member.IsMailSend = true;
                    old_member.IsDirectSend = false;

                    old_member.IsNumberChoice = true;
                    old_member.MaxSelectNumber = 30;

                    old_member.Digit1 = 0;
                    old_member.Digit2 = 0;
                    old_member.Digit3 = 0;

                    //old_member.CreateTime = DateTime.Now;
                    old_member.UpdateTime = DateTime.Now;

                    old_member.Remark = "";
                }

                ltctx.SaveChanges();
                _result = true;
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetLoginId(HttpRequest request)
        {
            var _result = "";

            var _header_authorization = request.Headers["Authorization"];
            if (_header_authorization.Count() > 0)
            {
                var _token_string = _header_authorization[0].ToString().Split(' ');
                var _web_token = (new JwtSecurityTokenHandler()).ReadJwtToken(_token_string[1]);

                var _login_id = _web_token
                                        .Claims
                                        .Where(c => c.Type == JwtRegisteredClaimNames.Sub)
                                        .FirstOrDefault();
                if (_login_id != null)
                    _result = _login_id.Value;
            }

            return _result;
        }
    }
}