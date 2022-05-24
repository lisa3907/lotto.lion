namespace Lion.WebApi.Controllers
{
    public partial class UserController
    {
        private IConfigurationSection __config_section = null;
        public IConfigurationSection ConfigSection
        {
            get
            {
                if (__config_section == null)
                    __config_section = __configuration.GetSection("appsettings");
                return __config_section;
            }
        }

        public string ServiceProviderName
        {
            get
            {
                return ConfigSection["lotto.sender.service.provider.name"];
            }
        }

        public string ServiceProviderPhone
        {
            get
            {
                return ConfigSection["lotto.sender.service.provider.phone"];
            }
        }

        public string ServiceProviderHomePage
        {
            get
            {
                return ConfigSection["lotto.sender.service.provider.homepage"];
            }
        }

        public string MailSenderName
        {
            get
            {
                return ConfigSection["lotto.sheet.mail.sender.name"];
            }
        }

        public string MailSenderAddress
        {
            get
            {
                return ConfigSection["lotto.sheet.mail.sender.address"];
            }
        }

        public string MailDeliveryServer
        {
            get
            {
                return ConfigSection["lotto.sheet.mail.delivery.server"];
            }
        }

        public int MailDeliveryPort
        {
            get
            {
                return Convert.ToInt32(ConfigSection["lotto.sheet.mail.delivery.port"]);   //587
            }
        }

        public string MailDeliveryUserName
        {
            get
            {
                return ConfigSection["lotto.sheet.mail.delivery.user.name"];
            }
        }

        public string MailDeliveryUserPassword
        {
            get
            {
                return ConfigSection["lotto.sheet.mail.delivery.user.password"];
            }
        }
    }
}