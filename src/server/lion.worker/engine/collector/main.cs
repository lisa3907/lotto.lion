using OdinSdk.BaseLib.Configuration;
using OdinSdk.BaseLib.Logger;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LottoLion.Worker.Engine
{
    /// <summary>
    ///
    /// </summary>
    public partial class Collector
    {
        private static CLogger __clogger = new CLogger();

        private static List<Task> __tasks;

        /// <summary>
        ///
        /// </summary>
        public static List<Task> CTasks
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
            __clogger.WriteDebug("[collector] starting thread...");

            if (CRegistry.UserInteractive == false)
            {
                CTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.StartWinner(tokenSource), tokenSource.Token));
                CTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.StartAnalysis(tokenSource), tokenSource.Token));
                CTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.StartSelector(tokenSource), tokenSource.Token));
                CTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.StartMember(tokenSource), tokenSource.Token));

                Task.WaitAll(CTasks.ToArray(), tokenSource.Token);
            }
            else
            {
                CTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.StartWinner(tokenSource), tokenSource.Token));
                CTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.StartAnalysis(tokenSource), tokenSource.Token));
                CTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.StartSelector(tokenSource), tokenSource.Token));
                CTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.StartMember(tokenSource), tokenSource.Token));
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static void Stop()
        {
            __clogger.WriteDebug("[collector] stopping thread...");
        }
    }
}