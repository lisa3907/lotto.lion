//#pragma warning disable 1591

using Lion.Share;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace OdinSdk.BaseLib.WebApi
{
    /// <summary>
    ///
    /// </summary>
    public class CProxy : IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f_worker"></param>
        /// <returns></returns>
        public static async ValueTask<CApiResult<T>> Using2<T>(Func<T> f_worker)
        {
            return await Task.Run(() =>
            {
                var _result = new CApiResult<T>();

                try
                {
                    _result.value = f_worker();

                    _result.status = 200;
                    _result.result = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    _result.status = 400;
                    _result.result = false;
                }
                finally
                {
                    _result.category = "U2";
                }

                return _result;
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="f_worker"></param>
        /// <returns></returns>
        public static async ValueTask<IActionResult> UsingAsync(Func<OkObjectResult> f_worker)
        {
            return await Task.Run(() =>
            {
                var _result = new OkObjectResult(null);

                try
                {
                    _result = f_worker();

                    _result.StatusCode = 200;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    _result = new OkObjectResult(new
                    {
                        success = false,
                        message = ex.InnerMessage(),

                        result = ""
                    });

                    _result.StatusCode = 400;
                }

                return _result;
            });
        }

        public static async ValueTask<IActionResult> UsingAsync(Func<ValueTask<OkObjectResult>> f_worker)
        {
            return await Task.Run(async () =>
            {
                var _result = new OkObjectResult(null);

                try
                {
                    _result = await f_worker();

                    _result.StatusCode = 200;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    _result = new OkObjectResult(new
                    {
                        success = false,
                        message = ex.InnerMessage(),

                        result = ""
                    });

                    _result.StatusCode = 400;
                }

                return _result;
            });
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
        }
    }
}