using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net;
using System.Text;

namespace LottoLion.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var _provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(_provider);

            //BuildWebHost(args).Run();

            CreateHostBuilder(args).Run();
        }

        public static IHost CreateHostBuilder(string[] args) =>
                        Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder
                                    .ConfigureKestrel(serverOptions =>
                                    {
                                        serverOptions.Listen(IPAddress.Loopback, 5127);
                                    })
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseIISIntegration()
                                    .UseStartup<Startup>();
                            })
                            .Build();

        public static IWebHost BuildWebHost(string[] args)
        {
            var _config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("hosting.json", true)
                    .Build();

            return new WebHostBuilder()
                    .UseKestrel()
                    .UseConfiguration(_config)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .Build();
        }
    }
}