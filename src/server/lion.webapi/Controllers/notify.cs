using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using OdinSdk.BaseLib.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LottoLion.WebApi.Controllers
{
    [Route("api/[controller]")]
    public partial class NotifyController : Controller
    {
        private IConfiguration __configuration;
        private UserManager __usermgr;
        private LottoLionContext __db_context;

        public NotifyController(IOptions<JwtIssuerOptions> jwtOptions, IConfiguration configuration, LottoLionContext db_context)
        {
            __usermgr = new UserManager(jwtOptions.Value);

            __configuration = configuration;
            __db_context = db_context;
        }

        [Route("GetMessages")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> GetMessages(DateTime notify_time, int limit = 10, bool is_read = false)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _notifies = new List<TbLionNotify>();

                var _login_id = __usermgr.GetLoginId(Request);
                if (String.IsNullOrEmpty(_login_id) == false)
                {
                    _notifies = __db_context.TbLionNotify
                                        .Where(n => n.LoginId == _login_id && (n.IsRead == false || is_read == true) && n.NotifyTime.ToUniversalTime() < notify_time.ToUniversalTime())
                                        .OrderByDescending(n => n.NotifyTime)
                                        .Take(limit)
                                        .ToList();

                    _result.success = true;
                }
                else
                    _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = _notifies
                });
            });
        }

        [Route("GetCount")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> GetCount()
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _no_notify = 0;

                var _login_id = __usermgr.GetLoginId(Request);
                if (String.IsNullOrEmpty(_login_id) == false)
                {
                    _no_notify = __db_context.TbLionNotify
                                        .Where(n => n.LoginId == _login_id && n.IsRead == false)
                                        .Count();

                    _result.success = true;
                }
                else
                    _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = _no_notify
                });
            });
        }

        [Route("Clear")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _no_notify = 0;

                var _login_id = __usermgr.GetLoginId(Request);
                if (String.IsNullOrEmpty(_login_id) == false)
                {
                    _no_notify = __db_context.ExecuteCommand(
                                            "UPDATE tb_lion_notify SET is_read='t' WHERE login_id=@login_id AND is_read='f'",
                                            new NpgsqlParameter("@login_id", NpgsqlDbType.Varchar) { Value = _login_id }
                                        );

                    _result.success = true;
                }
                else
                    _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";

                return new OkObjectResult(new
                {
                    _result.success,
                    _result.message,

                    result = _no_notify
                });
            });
        }
    }
}