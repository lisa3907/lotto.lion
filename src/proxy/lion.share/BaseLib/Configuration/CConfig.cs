using Microsoft.Extensions.Configuration;
using OdinSdk.BaseLib.Cryption;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#pragma warning disable 8632

namespace OdinSdk.BaseLib.Configuration
{
    /// <summary>
    ///
    /// </summary>
    public class CConfig
    {
        public CConfig()
        {
        }

        public CConfig(IConfiguration configuration)
        {
            __configuration_root = configuration;
        }

        private CCryption __cryptor = null;

        public CCryption Cryptor
        {
            get
            {
                if (__cryptor == null)
                {
                    var _key = this.GetAppString("aes_key");
                    var _iv = this.GetAppString("aes_iv");

                    __cryptor = new CCryption(_key, _iv);
                }

                return __cryptor;
            }
        }

        private bool? __is_encrypt_connection_string;

        public bool IsEncryptionConnectionString
        {
            get
            {
                if (__is_encrypt_connection_string == null)
                    __is_encrypt_connection_string = this.GetAppString("encrypt") == "true";

                return __is_encrypt_connection_string.Value;
            }
        }

        private IConfiguration? __configuration_root = null;

        public IConfiguration ConfigurationRoot
        {
            get
            {
                if (__configuration_root == null)
                {
                    var _config_builder = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile($"appsettings.json", true, true)
                                            .AddEncryptedProvider()
                                            .AddEnvironmentVariables();

                    __configuration_root = _config_builder.Build();
                }

                return __configuration_root;
            }
        }

        private IConfigurationSection __configuration_section = null;
        public IConfigurationSection ConfigAppSection
        {
            get
            {
                if (__configuration_section == null)
                    __configuration_section = this.ConfigurationRoot.GetSection("appsettings");
                return __configuration_section;
            }
        }

        public virtual string GetConnectionString(string name = "DefaultConnection")
        {
            return this.ConfigurationRoot.GetConnectionString(name);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> AllAppString()
        {
            var _result = new Dictionary<string, string>();

            var _config_section = this.ConfigAppSection.GetChildren().ToList();
            foreach (ConfigurationSection _c in _config_section)
                _result.Add(_c.Key, _c.Value);

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string GetAppString(string appkey, string defaultValue = "")
        {
            return GetAppString(this.ConfigAppSection, appkey, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string GetAppSection(string sectionName, string appkey, string defaultValue = "")
        {
            var _config_section = this.ConfigAppSection.GetSection(sectionName);
            return GetAppString(_config_section, appkey, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSection"></param>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual string GetAppString(IConfigurationSection configSection, string appkey, string defaultValue)
        {
            var _result = "";

            if (String.IsNullOrEmpty(appkey) == false)
            {
                if (this.IsWindows == true)
                {
                    _result = configSection[appkey + ".debug"];

                    if (_result == null)
                        _result = configSection[appkey];
                }
                else
                    _result = configSection[appkey];
            }

            if (String.IsNullOrEmpty(_result) == true)
                _result = defaultValue;

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appkey"></param>
        /// <returns></returns>
        public virtual DateTime GetAppDateTime(string appkey)
        {
            var _value = GetAppString(appkey);
            return String.IsNullOrEmpty(_value) ? CUnixTime.UtcNow : Convert.ToDateTime(_value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual int GetAppInteger(string appkey, int defaultValue = 0)
        {
            var _value = GetAppString(appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : Convert.ToInt32(_value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual bool GetAppBoolean(string appkey, bool defaultValue = false)
        {
            var _value = GetAppString(appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : _value.ToLower() == "true";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual long GetAppInteger64(string appkey, int defaultValue = 0)
        {
            var _value = GetAppString(appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : Convert.ToInt64(_value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual decimal GetAppDecimal(string appkey, decimal defaultValue = 0)
        {
            var _value = GetAppString(appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : Convert.ToDecimal(_value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual int GetHexInteger(string appkey, int defaultValue = 0)
        {
            var _value = GetAppString(appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : Convert.ToInt32(_value, 16);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="appSection"></param>
        /// <param name="appkey"></param>
        /// <returns></returns>
        public virtual DateTime GetAppDateTime(string appSection, string appkey)
        {
            var _value = GetAppSection(appSection, appkey);
            return String.IsNullOrEmpty(_value) ? CUnixTime.UtcNow : Convert.ToDateTime(_value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appSection"></param>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual int GetAppInteger(string appSection, string appkey, int defaultValue = 0)
        {
            var _value = GetAppSection(appSection, appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : Convert.ToInt32(_value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appSection"></param>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual bool GetAppBoolean(string appSection, string appkey, bool defaultValue = false)
        {
            var _value = GetAppSection(appSection, appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : _value.ToLower() == "true";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appSection"></param>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual long GetAppInteger64(string appSection, string appkey, int defaultValue = 0)
        {
            var _value = GetAppSection(appSection, appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : Convert.ToInt64(_value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="appSection"></param>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual decimal GetAppDecimal(string appSection, string appkey, decimal defaultValue = 0)
        {
            var _value = GetAppSection(appSection, appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : Convert.ToDecimal(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSection"></param>
        /// <param name="appkey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual int GetHexInteger(string appSection, string appkey, int defaultValue = 0)
        {
            var _value = GetAppSection(appSection, appkey);
            return String.IsNullOrEmpty(_value) ? defaultValue : Convert.ToInt32(_value, 16);
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------------------------------
        private string? __product_name = null;

        /// <summary>
        ///
        /// </summary>
        public string ProductName
        {
            get
            {
                if (__product_name == null)
                    __product_name = GetAppString("product.name");

                return __product_name;
            }
        }

        private string? __application_folder = null;

        /// <summary>
        ///
        /// </summary>
        public string ApplicationDataFolder
        {
            get
            {
                if (__application_folder == null)
                    __application_folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ProductName);

                return __application_folder;
            }
        }

        private string? __working_folder = null;

        /// <summary>
        ///
        /// </summary>
        public string WorkingFolder
        {
            get
            {
                if (__working_folder == null)
                {
                    __working_folder = GetAppString("working.folder");

                    if (String.IsNullOrEmpty(__working_folder) == true)
                        __working_folder = Path.Combine(ApplicationDataFolder, "Working");
                }

                return __working_folder;
            }
        }

        private string? __logging_folder = null;

        /// <summary>
        ///
        /// </summary>
        public string LoggingFolder
        {
            get
            {
                if (__logging_folder == null)
                {
                    __logging_folder = GetAppString("logging.folder");

                    if (String.IsNullOrEmpty(__logging_folder) == true)
                        __logging_folder = Path.Combine(ApplicationDataFolder, "Logging");
                }

                return __logging_folder;
            }
        }

        public string CreateFolder(params string[] folders)
        {
            var _result = Path.Combine(folders);

            if (System.IO.Directory.Exists(_result) == false)
                System.IO.Directory.CreateDirectory(_result);

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="folders"></param>
        /// <returns></returns>
        public string GetWorkingFolder(params string[] folders)
        {
            return CreateFolder(WorkingFolder, Path.Combine(folders));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="folders"></param>
        /// <returns></returns>
        public string GetWorkingFolderWithDateTime(params string[] folders)
        {
            return GetWorkingFolder(Path.Combine(folders), String.Format("{0:yyyyMM}", CUnixTime.UtcNow));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="folders"></param>
        /// <returns></returns>
        public string GetDownloadFolder(params string[] folders)
        {
            return CreateFolder(WorkingFolder, "download", Path.Combine(folders));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="folders"></param>
        /// <returns></returns>
        public string GetLoggingFolder(params string[] folders)
        {
            return CreateFolder(WorkingFolder, "logging", Path.Combine(folders));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="folders"></param>
        /// <returns></returns>
        public string GetApplicationFolder(params string[] folders)
        {
            return CreateFolder(ApplicationDataFolder, Path.Combine(folders));
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string[] GetIPAddresses()
        {
            return GetIPAddresses(Dns.GetHostName());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="host_name">name of host PC</param>
        /// <returns></returns>
        public string[] GetIPAddresses(string host_name)
        {
            string[] _result;

            if (IsValidIP(host_name) == false)
            {
                var _ipadrs = Dns.GetHostEntryAsync(host_name)
                                            .GetAwaiter()
                                            .GetResult()
                                            .AddressList;

                _result = new string[_ipadrs.Length];

                var i = 0;
                foreach (var _ip in _ipadrs)
                {
                    if (_ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        _result[i++] = _ip.ToString();
                }

                Array.Resize(ref _result, i);
            }
            else
            {
                _result = new string[] { host_name };
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {
            return GetIPAddress(Dns.GetHostName());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="host_name">name of host PC</param>
        /// <returns></returns>
        public string GetIPAddress(string host_name)
        {
            var _result = host_name;

            if (IsValidIP(host_name) == false)
            {
                var _ipHostInfo = Dns.GetHostEntryAsync(host_name)
                                        .GetAwaiter()
                                        .GetResult();

                foreach (var _ip in _ipHostInfo.AddressList)
                {
                    if (_ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        _result = _ip.ToString();
                        break;
                    }
                }
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string GetIP6Address()
        {
            return GetIP6Address(Dns.GetHostName());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="host_name">name of host PC</param>
        /// <returns></returns>
        public string GetIP6Address(string host_name)
        {
            var _result = host_name;

            if (IsValidIP(host_name) == false)
            {
                var _ipHostInfo = Dns.GetHostEntryAsync(host_name)
                                            .GetAwaiter()
                                            .GetResult();

                foreach (var _ip in _ipHostInfo.AddressList)
                {
                    if (_ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        _result = _ip.ToString().Split('%')[0];
                        break;
                    }
                }
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string GetMacAddress()
        {
            var _nics = NetworkInterface.GetAllNetworkInterfaces();

            var _mac_address = "";
            foreach (NetworkInterface _adapter in _nics)
            {
                // only return MAC Address from first card
                if (String.IsNullOrEmpty(_mac_address) == true)
                {
                    //IPInterfaceProperties properties = adapter.GetIPProperties(); Line is not required
                    _mac_address = _adapter.GetPhysicalAddress().ToString();
                }
            }

            return _mac_address;
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        ///
        /// </summary>
        /// <param name="host_name">name of host PC</param>
        /// <returns></returns>
        public string GetLocalIpAddress(string host_name)
        {
            return IsLocalMachine(host_name) ? IPAddress : host_name;
        }

        /// <summary>
        /// method to validate an IP address
        /// using regular expressions. The pattern
        /// being used will validate an ip address
        /// with the range of 1.0.0.0 to 255.255.255.255
        /// </summary>
        /// <param name="ip_address">Address to validate</param>
        /// <returns></returns>
        public bool IsValidIP(string ip_address)
        {
            //create our match pattern
            const string _pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";

            //create our Regular Expression object
            var _regCheck = new Regex(_pattern);

            //boolean variable to hold the status
            var _result = false;

            //check to make sure an ip address was provided
            if (ip_address == "")
            {
                //no address provided so return false
                _result = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                _result = _regCheck.IsMatch(ip_address, 0);
            }

            //return the results
            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="host_name">name of host PC</param>
        /// <returns></returns>
        public bool IsLocalMachine(string host_name)
        {
            var _result = false;

            if (String.IsNullOrEmpty(host_name) == true ||
                host_name.ToLower() == "localhost" || host_name == "127.0.0.1" ||
                host_name.ToLower() == MachineName || host_name == "::1"
                )
            {
                _result = true;
            }
            else
            {
                string[] _serverIPs = GetIPAddresses(host_name);
                if (_serverIPs.Length > 0)
                {
                    string[] _ipadrs = GetIPAddresses();
                    foreach (string _ip in _ipadrs)
                    {
                        if (_ip == _serverIPs[0])
                        {
                            _result = true;
                            break;
                        }
                    }
                }
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="local_host_name"></param>
        /// <param name="remote_host_name"></param>
        /// <returns></returns>
        public bool IsSameMachine(string local_host_name, string remote_host_name)
        {
            var _result = false;

            if (local_host_name == remote_host_name)
            {
                _result = true;
            }
            else
            {
                string[] _localIPs = GetIPAddresses(local_host_name);
                if (_localIPs.Length > 0)
                {
                    string[] _remoteIPs = GetIPAddresses(remote_host_name);
                    foreach (string _ip in _remoteIPs)
                    {
                        if (_ip == _localIPs[0])
                        {
                            _result = true;
                            break;
                        }
                    }
                }
            }

            return _result;
        }

        /// <summary>
        ///
        /// </summary>
        public string MachineName
        {
            get
            {
                return Environment.MachineName.ToLower();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string UserDomainName
        {
            get
            {
                return this.ConfigurationRoot["USERDOMAIN"].ToLower();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string UserName
        {
            get
            {
                if (IsWindows == true)
                    return this.ConfigurationRoot["USERNAME"].ToLower();
                else
                    return this.ConfigurationRoot["USER"].ToLower();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsUnix
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsWindows
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsOSX
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            }
        }

        public bool Is64BitOperatingSystem
        {
            get
            {
                return RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.Arm64;
            }
        }

        private string[] __ip_addresses;

        /// <summary>
        ///
        /// </summary>
        public string[] IPAddresses
        {
            get
            {
                if (__ip_addresses == null)
                    __ip_addresses = GetIPAddresses();

                return __ip_addresses;
            }
        }

        private string __ip_address;

        /// <summary>
        ///
        /// </summary>
        public string IPAddress
        {
            get
            {
                if (__ip_address == null)
                    __ip_address = GetIPAddress();

                return __ip_address;
            }
        }

        private string __ip6_address;

        /// <summary>
        ///
        /// </summary>
        public string IP6Address
        {
            get
            {
                if (__ip6_address == null)
                    __ip6_address = GetIP6Address();

                return __ip6_address;
            }
        }

        private string __mac_address;

        /// <summary>
        ///
        /// </summary>
        public string MacAddress
        {
            get
            {
                if (__mac_address == null)
                    __mac_address = GetMacAddress();

                return __mac_address;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="assembly_string"></param>
        /// <returns></returns>
        public string GetVersion(string assembly_string)
        {
            string[] _splitNames = assembly_string.Split(',');
            var _version = "";

            foreach (string f in _splitNames)
            {
                if (f.Trim().StartsWith("Version="))
                    _version = f.Trim().Replace("Version=", "");
            }

            return _version;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="assembly_string"></param>
        /// <returns></returns>
        public string GetPublicKeyToken(string assembly_string)
        {
            string[] _splitNames = assembly_string.Split(',');
            var _keyToken = "";

            foreach (string f in _splitNames)
            {
                if (f.Trim().StartsWith("PublicKeyToken="))
                    _keyToken = f.Trim().Replace("PublicKeyToken=", "").ToLower();
            }

            return _keyToken;
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        // 영어(미국), 일본어(일본), 중국어(중국), 중국어(간체), 중국어(번체), 한국어(대한민국), 태국어(태국). 필리핀어(필리핀)
        //-----------------------------------------------------------------------------------------------------------------------------
        private ArrayList __culture { get; } = new ArrayList(new string[] { "ko-kr", "en-us", "ja-jp", "zh-cn", "zh-chs", "zh-cht", "th-th", "fil-ph" });

        /// <summary>
        ///
        /// </summary>
        public string GetDefaultCulture
        {
            get
            {
                return __culture[0].ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public bool ContainsCultureName(string culture)
        {
            return __culture.Contains(culture.ToLower());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetCultureName(string culture)
        {
            var _result = culture;

            for (int i = 0; i < __culture.Count; i++)
            {
                if (_result.ToLower().Equals(__culture[i].ToString()) == true)
                    return _result;
            }

            switch (_result.ToLower().Substring(0, 2))
            {
                case "ko":
                    _result = __culture[0].ToString();
                    break;

                case "en":
                    _result = __culture[1].ToString();
                    break;

                case "ja":
                    _result = __culture[2].ToString();
                    break;

                case "zh":
                    _result = __culture[3].ToString();
                    break;

                default:
                    _result = __culture[1].ToString();
                    break;
            }

            return _result;
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------------------------------
        private bool? _runningFromNUnit = null;

        /// <summary>
        /// Check that Unit Test is in progress.
        /// </summary>
        public bool IsRunningFromNunit
        {
            get
            {
                if (_runningFromNUnit == null)
                {
                    _runningFromNUnit = false;

                    const string _testAssemblyName = "Microsoft.VisualStudio.QualityTools.UnitTestFramework";
                    foreach (Assembly _assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (_assembly.FullName.StartsWith(_testAssemblyName))
                        {
                            _runningFromNUnit = true;
                            break;
                        }
                    }
                }

                return _runningFromNUnit.Value;
            }
        }

        /// <summary>
        /// SQL ConnectionString에 application name을 추가 합니다.
        /// </summary>
        /// <param name="connection_string"></param>
        /// <param name="appname"></param>
        /// <returns></returns>
        public string AppendAppNameInConnectionString(string connection_string, string appname)
        {
            var _result = "";

            const string _cApplication = "application name";

            string[] _nodes = connection_string.Split(';');
            foreach (string _node in _nodes)
            {
                if (_node.Split('=')[0].ToLower() != _cApplication)
                    _result += _node + ";";
            }

            _result += String.Format("{0}={1};", _cApplication, appname);

            return _result;
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool IsAdministrator()
        {
            var _result = false;
#if net48
            var _identity = WindowsIdentity.GetCurrent();
            if (_identity != null)
            {
                var _principal = new WindowsPrincipal(_identity);
                _result = _principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
#endif
            return _result;
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}