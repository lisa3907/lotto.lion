using Lion.Share.Data;
using Lion.Share.Data.Models;
using Lion.Share.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OdinSdk.BaseLib.WebApi;

namespace Lion.WebApi.Controllers
{
    [Route("api/[controller]")]
    public partial class NotifyController : Controller
    {
        private readonly UserManager __usermgr;
        private readonly AppDbContext __db_context;

        public NotifyController(IOptions<JwtIssuerOptions> jwtOptions, AppDbContext dbContext)
        {
            __usermgr = new UserManager(jwtOptions.Value);
            __db_context = dbContext;
        }

        [Route("GetMessages")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> GetMessages(DateTime notify_time, int limit = 10, bool is_read = false)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _notifies = new List<mNotify>();

                var _login_id = __usermgr.GetLoginId(Request);
                if (String.IsNullOrEmpty(_login_id) == false)
                {
                    _notifies = __db_context.tb_lion_notify
                                            .Where(n => n.LoginId == _login_id && (n.IsRead == false || is_read == true) && n.NotifyTime < notify_time)
                                            .OrderByDescending(n => n.NotifyTime)
                                            .Take(limit)
                                            .ToList();

                    _result.success = true;
                }
                else
                    _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";

                return Ok(new
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
                    _no_notify = __db_context.tb_lion_notify
                                             .Where(n => n.LoginId == _login_id && n.IsRead == false)
                                             .Count();

                    _result.success = true;
                }
                else
                    _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";

                return Ok(new
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
                    var _notify = __db_context.tb_lion_notify
                                              .Where(n => n.LoginId == _login_id && n.IsRead == false)
                                              .SingleOrDefault();
                    if (_notify != null)
                    {
                        _notify.IsRead = true;
                        __db_context.SaveChanges();

                        _no_notify++;
                    }

                    _result.success = true;
                }
                else
                    _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = _no_notify
                });
            });
        }
    }
}