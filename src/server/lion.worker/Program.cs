using LottoLion.BaseLib;
using LottoLion.Worker.Server;
using OdinSdk.BaseLib.Configuration;
using System;
using System.Text;
using System.Threading;

namespace LottoLion.Worker
{
    internal class Program
    {
        private static LConfig __cconfig = new LConfig();

        private static void Main(string[] args)
        {
            var provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            //__cconfig.SetConfigRoot();

            using (var _hostess = new Hostess())
            {
                _hostess.Start();

                if (CRegistry.UserInteractive == true)
                {
                    while (Console.ReadLine() != "quit")
                        Console.WriteLine("Enter 'quit' to stop the services and end the process...");
                }

                _hostess.Stop();
            }

            Console.WriteLine("[program] all services stopped.");

            // Keep the console alive for a second to allow the user to see the message.
            Thread.Sleep(1000);
        }
    }
}