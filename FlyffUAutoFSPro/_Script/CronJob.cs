using log4net;
using Sentry;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FlyffUAutoFSPro._Script
{
    public class CronJob
    {
        int Interval { get; set; }
        bool CanRunParallel { get; set; }
        bool CanRun { get; set; } = true;
        CronJobDelegate TaskToRun { get; set; }
        DispatcherTimer Timer { get; set; }

        public delegate Task CronJobDelegate();
        CancellationTokenSource cancelToken;

        public CronJob(int interval, CronJobDelegate task, bool canRunParallel = false)
        {
            log4net.Config.XmlConfigurator.Configure();

            Interval = interval;
            CanRunParallel = canRunParallel;
            TaskToRun = task;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(Interval);
            Timer.Tick += (sender, e) => {
                cancelToken = new CancellationTokenSource();
                var token = cancelToken.Token;

                Task.Run(async () => {
                   
                    if (CanRun == false && CanRunParallel == false) return;

                    try
                    {
                        CanRun = false;
                        await TaskToRun.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogService.LogError(ex);
                        SentrySdk.CaptureException(ex);
                    }
                    finally
                    {
                        CanRun = true;
                    }
                }, token).ConfigureAwait(false);

            };
            Timer.Start();
        }

        public void Dispose()
        {
            if(cancelToken != null)
                cancelToken.Cancel();
            Timer.Stop();
        }
    }
}
