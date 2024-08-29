using FlyffUniverseFarmBot;
using log4net;
using System;

namespace FlyffUAutoFSPro._Script
{
    public static class LogService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        public static void Initalize()
        {
            log4net.Config.XmlConfigurator.Configure();
        }


        public static void Log(string message)
        {
#if DEBUG
            log.Info(message);
#endif
        }

        public static void LogError(Exception ex)
        {
#if DEBUG
            log.Error(ex);
#endif
        }
    }
}
