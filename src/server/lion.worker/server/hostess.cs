using LottoLion.BaseLib;
using Microsoft.Extensions.Configuration;
using OdinSdk.BaseLib.Configuration;
using OdinSdk.BaseLib.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LottoLion.Worker.Server
{
    /// <summary>
    ///
    /// </summary>
    public class Hostess : IDisposable
    {
        private static CLogger __clogger = new CLogger();
        private static LConfig __cconfig = new LConfig();

        private static List<Task> __tasks;

        /// <summary>
        ///
        /// </summary>
        public static List<Task> XTasks
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

        private static CancellationTokenSource __tokenSource;

        private CancellationTokenSource TokenSource
        {
            get
            {
                if (__tokenSource == null)
                    __tokenSource = new CancellationTokenSource();

                return __tokenSource;
            }
        }

        //private static IConfigurationBuilder __config_builder = null;

        //public IConfigurationBuilder ConfigBuilder
        //{
        //    get
        //    {
        //        if (__config_builder == null)
        //            __config_builder = new ConfigurationBuilder()
        //                    .SetBasePath(Directory.GetCurrentDirectory())
        //                    .AddJsonFile($"appsettings.json", true, true)
        //                    .AddEncryptedProvider()
        //                    .AddEnvironmentVariables();

        //        return __config_builder;
        //    }
        //}

        //private static IConfigurationRoot __config_root = null;

        //public IConfigurationRoot ConfigRoot
        //{
        //    get
        //    {
        //        if (__config_root == null)
        //            __config_root = ConfigBuilder.Build();

        //        return __config_root;
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="p_nothread"></param>
        public void Start(int p_nothread = 1)
        {
            try
            {
                __clogger.WriteDebug($"[hostess] starting service: {CRegistry.UserInteractive}...");

                //__cconfig.SetConfigRoot();

                if (CRegistry.UserInteractive == false)
                {
                    XTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.Start(TokenSource), TokenSource.Token));
                    XTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.Start(TokenSource), TokenSource.Token));
                    XTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Server.Worker.Start(TokenSource), TokenSource.Token));

                    Task.WaitAll(XTasks.ToArray(), TokenSource.Token);

                    __clogger.WriteDebug("[hostess] exit service...");
                }
                else
                {
                    XTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Receiver.Start(TokenSource), TokenSource.Token));
                    XTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Engine.Collector.Start(TokenSource), TokenSource.Token));
                    XTasks.Add(Task.Factory.StartNew(() => LottoLion.Worker.Server.Worker.Start(TokenSource), TokenSource.Token));
                }
            }
            catch (AggregateException ex)
            {
                ex.Handle(ix =>
                {
                    var tcex = ix as TaskCanceledException;
                    if (tcex == null)
                        __clogger.WriteLog(ix);

                    return true;
                });
            }
            catch (Exception ex)
            {
                __clogger.WriteLog(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Stop()
        {
            try
            {
                __clogger.WriteDebug("[hostess] stopping service...");

                if (CRegistry.UserInteractive == false)
                {
                    LottoLion.Worker.Engine.Receiver.Stop();
                    LottoLion.Worker.Engine.Collector.Stop();

                    TokenSource.Cancel();

                    Task.WaitAll(XTasks.ToArray());
                }
                else
                {
                    LottoLion.Worker.Engine.Receiver.Stop();
                    LottoLion.Worker.Engine.Collector.Stop();

                    TokenSource.Cancel();
                }
            }
            catch (AggregateException ex)
            {
                ex.Handle(ix =>
                {
                    var tcex = ix as TaskCanceledException;
                    if (tcex == null)
                        __clogger.WriteLog(ix);

                    return true;
                });
            }
            catch (Exception ex)
            {
                __clogger.WriteLog(ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
        }
    }
}