using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Pipe;
using Microsoft.EntityFrameworkCore;
using OdinSdk.BaseLib.Net.Smtp;
using System.Runtime.Versioning;
using System.Text;

namespace Lion.Worker
{
    [SupportedOSPlatform("windows")]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureLogging((hostContext, builder) =>
                {
                    builder.ClearProviders()
                            .AddConsole()
                            .AddFile(
                                outputTemplate: "[{Timestamp:yyyy/MM/dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
#if DEBUG
                                minimumLevel: LogLevel.Debug,
#else
                                minimumLevel: LogLevel.Information,
#endif
                                pathFormat: "D:/project-data/LottoLION/Logger/Worker/log-{Date}.txt"
                            );
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContextFactory<AppDbContext>(options =>
                    {
                        options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                    });

                    services.AddSingleton<MailSenderLottoLion>();
                    services.AddSingleton<SmtpDirectSender>();
                    services.AddSingleton<PrintOutLottoLion>();
                    services.AddSingleton<NotifyPushLottoLion>();

                    services.AddSingleton<WinnerReader>();
                    services.AddSingleton<WinnerSelector>();
                    services.AddSingleton<WinnerMember>();
                    services.AddSingleton<WinnerScoring>();
                    services.AddSingleton<WinnerAnalysis>();
                    services.AddSingleton<WinnerPercent>();
                    services.AddSingleton<PipeClient>();

                    services.AddHostedService<Engine.Purger>();
                    services.AddHostedService<Engine.Receiver>();

                    services.AddHostedService<Engine.Processor.Winner>();
                    services.AddHostedService<Engine.Processor.Selector>();
                    services.AddHostedService<Engine.Processor.Choicer>();
                    services.AddHostedService<Engine.Processor.Analyst>();

                    services.AddHostedService<Engine.Collector.Winner>();
                    services.AddHostedService<Engine.Collector.Selector>();
                    services.AddHostedService<Engine.Collector.Choicer>();
                    services.AddHostedService<Engine.Collector.Analyst>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}