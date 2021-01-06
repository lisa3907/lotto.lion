using OdinSdk.BaseLib.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LottoLion.BaseLib
{
    /// <summary>
    ///
    /// </summary>
    public class LConfig : CConfig
    {
        #region Common

        public string LionVersion
        {
            get
            {
                return this.GetAppString("lion.version");
            }
        }

        #endregion Common

        #region Creator

        //private IConfigurationBuilder __config_builder = null;
        //public IConfigurationBuilder ConfigBuilder
        //{
        //    get
        //    {
        //        if (__config_builder == null)
        //        {
        //            __config_builder = new ConfigurationBuilder()
        //                                .SetBasePath(Directory.GetCurrentDirectory())
        //                                .AddJsonFile($"appsettings.json", true, true)
        //                                .AddEncryptedProvider()
        //                                .AddEnvironmentVariables();
        //        }

        //        return __config_builder;
        //    }
        //}

        //private IConfigurationRoot __config_root = null;

        //public override IConfiguration ConfigRoot
        //{
        //    get
        //    {
        //        if (__config_root == null)
        //            __config_root = ConfigBuilder.Build();

        //        return __config_root;
        //    }
        //}

        //public void SetConfigRoot()
        //{
        //    this.SetConfigRoot(ConfigRoot);
        //}

        #endregion Creator
    }
}