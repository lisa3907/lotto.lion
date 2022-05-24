using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdinSdk.BaseLib.Configuration;
using OdinSdk.BaseLib.WebApi;

namespace Lion.WebApi.Controllers
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
                    __version = new ProductInfo("Lion.WebApi", "v3.1.0", "WebApi Service for Lotto-Lion", ProductType.service);

                return Ok(new
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
                return Ok(new
                {
                    success = true,
                    message = "",

                    result = CUnixTime.LocalNow
                });
            });
        }
    }
}