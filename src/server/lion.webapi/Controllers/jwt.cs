using LottoLion.BaseLib.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdinSdk.BaseLib.WebApi;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LottoLion.WebApi.Controllers
{
    public partial class UserController
    {
        [Route("GetGuestToken")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetGuestToken()
        {
            return await CProxy.UsingAsync(async () =>
            {
                var _token = await __usermgr.GetEncodedJwt(
                                    new ApplicationUser()
                                    {
                                        login_id = "guest",
                                        mail_address = ""
                                    },
                                    new Claim("UserType", "Guest")
                                );

                return new OkObjectResult(new
                {
                    success = true,
                    message = "",

                    result = _token
                });
            });
        }

        [Route("GetTokenByLoginId")]
        [Authorize(Policy = "LottoLionGuest")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetTokenByLoginId([FromForm] ApplicationUser app_user)
        {
            return await CProxy.UsingAsync(async () =>
            {
                var _result = (success: false, message: "ok");

                var _user_token = "";

                if (String.IsNullOrEmpty(app_user.login_id) == false && String.IsNullOrEmpty(app_user.password) == false)
                {
                    var _claim = __usermgr.GetClaimsIdentityByLoginId(__db_context, app_user);
                    if (_claim.identity != null)
                    {
                        app_user.mail_address = _claim.mail_address;

                        _user_token = await __usermgr.GetEncodedJwt(app_user, _claim.identity);
                        _result.success = true;
                    }
                    else
                        _result.message = "인증에 실패 하였습니다";
                }
                else
                    _result.message = "회원ID와 암호를 입력 바랍니다";

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = _user_token
                });
            });
        }

        [Route("GetTokenByFacebook")]
        [Authorize(Policy = "LottoLionGuest")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetTokenByFacebook([FromForm] ApplicationUser app_user)
        {
            return await CProxy.UsingAsync(async () =>
            {
                var _result = (success: false, message: "ok");

                var _user_token = "";

                if (String.IsNullOrEmpty(app_user.facebook_id) == false && String.IsNullOrEmpty(app_user.facebook_token) == false)
                {
                    var _claim = await __usermgr.GetClaimsIdentityByFacebook(__db_context, app_user);
                    if (_claim.identity != null)
                    {
                        app_user.login_id = app_user.facebook_id;
                        app_user.mail_address = _claim.mail_address;

                        _user_token = await __usermgr.GetEncodedJwt(app_user, _claim.identity);
                        _result.success = true;
                    }
                    else
                        _result.message = "인증에 실패 하였습니다";
                }
                else
                    _result.message = "facebook-id와 facebook-token이 필요 합니다";

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = _user_token
                });
            });
        }
    }
}