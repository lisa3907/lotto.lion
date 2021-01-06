using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdinSdk.BaseLib.Configuration;
using OdinSdk.BaseLib.Queue;
using OdinSdk.BaseLib.WebApi;
using System.Threading.Tasks;

namespace LottoLion.WebApi.Controllers
{
    [Route("api/[controller]")]
    public partial class MainController : Controller
    {
        private static ProductInfo __version = null;

        [Route("GetVersion")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetVersion()
        {
            return await CProxy.UsingAsync(() =>
            {
                if (__version == null)
                    __version = new ProductInfo("LottoLion-WebApi", "v1.0.0", "webapi service for LottoLion", ProductType.service);

                return new OkObjectResult(new
                {
                    success = true,
                    message = "",

                    result = __version
                });
            });
        }

        [Route("GetServerTime")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetServerTime()
        {
            return await CProxy.UsingAsync(() =>
            {
                return new OkObjectResult(new
                {
                    success = true,
                    message = "",

                    result = CUnixTime.LocalNow
                });
            });
        }
    }
}