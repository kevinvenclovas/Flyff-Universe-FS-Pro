using FlyffUAutoFSPro._Script.Bot.FS;
using FlyffUAutoFSPro._Script.Bot.Main;
using System;
using System.Threading.Tasks;

namespace FlyffUAutoFSPro._Script.Bot
{
    public class BotController : IDisposable
    {
      
        private int _runningTime = 0;

        public FSBotController FSController;
        public MainBotController MainBotController;

        public BotController()
        {
        }

        public void Startup()
        {
            FSController = new FSBotController(this);
            MainBotController = new MainBotController(this);
            var runningTimeCronjob = new CronJob(1000, IncreaseRunningTime);
           
        }

        private async Task IncreaseRunningTime()
        {
            // Nur wenn ein Bot auch läuft laufzeit erhöhen
            if (FSController.IsRunning(true))
            {
                _runningTime++;
                FSController.IncreaseRunningTime();
            }
        }

        public void Dispose()
        {
            FSController.Dispose();
        }
    }
}
