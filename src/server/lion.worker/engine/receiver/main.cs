using LottoLion.BaseLib;
using OdinSdk.BaseLib.Configuration;
using OdinSdk.BaseLib.Logger;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LottoLion.Worker.Engine
{
    /// <summary>
    /// BTCChina
    /// </summary>
    public partial class Receiver
    {
        private static CLogger __clogger = new CLogger();
        private static LConfig __cconfig = new LConfig();

        private static int NoMailDeliveryThread
        {
            get
            {
                return __cconfig.GetAppInteger("lotto.sheet.mail.delivery.no.thread");
            }
        }

        private static List<Task> __tasks;

        /// <summary>
        ///
        /// </summary>
        public static List<Task> RTasks
        {
            get
            {
                if (__tasks == null)
                    __tasks = new List<Task>();

                return __tasks;
            }
            set
            {
                __tasks = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tokenSource"></param>
        public static void Start(CancellationTokenSource tokenSource)
        {
            __clogger.WriteDebug("[receiver] starting thread...");

            if (CRegistry.UserInteractive == false)
            {
                RTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.StartWinnerQ(tokenSource), tokenSource.Token));
                RTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.StartAnalysisQ(tokenSource), tokenSource.Token));
                RTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.StartSelectorQ(tokenSource), tokenSource.Token));

                //for (int i = 0; i < NoMailDeliveryThread; i++)
                RTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.StartMemberQ(tokenSource), tokenSource.Token));

                Task.WaitAll(RTasks.ToArray(), tokenSource.Token);
            }
            else
            {
                RTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.StartWinnerQ(tokenSource), tokenSource.Token));
                RTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.StartAnalysisQ(tokenSource), tokenSource.Token));
                RTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.StartSelectorQ(tokenSource), tokenSource.Token));

                //for (int i = 0; i < NoMailDeliveryThread; i++)
                RTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.StartMemberQ(tokenSource), tokenSource.Token));
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static void Stop()
        {
            __clogger.WriteDebug("[receiver] stopping thread...");
        }
    }
}