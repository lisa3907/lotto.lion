using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OdinSdk.BaseLib.WebApi;

namespace Lion.WebApi.Controllers
{
    public partial class UserController
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [EnableCors("CorsPolicy")]
        [Route("GetWinners2")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetWinners2()
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = __db_context.tb_lion_winner
                                    .OrderByDescending(w => w.SequenceNo)
                                    .ToList();

                return Ok(new
                {
                    success = true,
                    message = "",

                    result = _result
                });
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [EnableCors("CorsPolicy")]
        [Route("GetWinnersWithPage")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetWinnersWithPage(int pageIndex, int pageRows)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = __db_context.tb_lion_winner
                                    .OrderByDescending(w => w.SequenceNo)
                                    .Skip((pageIndex - 1) * pageRows).Take(pageRows)
                                    .ToList();

                return Ok(new
                {
                    success = true,
                    message = "",

                    result = _result
                });
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [Route("GetWinners")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> GetWinners()
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = __db_context.tb_lion_winner
                                    .OrderByDescending(w => w.SequenceNo)
                                    .ToList();

                return Ok(new
                {
                    success = true,
                    message = "",

                    result = _result
                });
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [Route("GetAnalysis")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> GetAnalysis()
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = __db_context.tb_lion_analysis
                                    .OrderByDescending(w => w.SequenceNo)
                                    .ToList();

                return Ok(new
                {
                    success = true,
                    message = "",

                    result = _result
                });
            });
        }
    }
}