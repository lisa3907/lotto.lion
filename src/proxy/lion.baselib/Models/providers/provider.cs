using LottoLion.BaseLib.Models.Entity;
using Microsoft.Extensions.Configuration;
using OdinSdk.BaseLib.Cryption;
using OdinSdk.BaseLib.Logger;
using System;

namespace LottoLion.BaseLib.Models
{
    /// <summary>
    ///
    /// </summary>
    public partial class LTCX : IDisposable
    {
        private static CLogger __clogger = new CLogger();
        private static LConfig __cconfig = new LConfig();

        private static CCryption __cryptor = null;

        private static CCryption Cryptor
        {
            get
            {
                if (__cryptor == null)
                {
                    var _key = __cconfig.ConfigurationRoot["aes_key"];
                    var _iv = __cconfig.ConfigurationRoot["aes_iv"];

                    __cryptor = new CCryption(_key, _iv);
                }

                return __cryptor;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static LottoLionContext GetNewContext(string connection_name = "DefaultConnection", bool enctyprion = true)
        {
            var _connection_string = __cconfig.ConfigurationRoot.GetConnectionString(connection_name);
            if (enctyprion == true)
                _connection_string = Cryptor.ChiperToPlain(_connection_string);

            return new LottoLionContext(_connection_string);
        }

        public void Dispose()
        {
        }
    }
}