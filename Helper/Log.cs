using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCIFramework.Helper
{
    public static class Log
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Log a Fatal error
        /// </summary>
        public static void LogFatal(string message)
        {
            log.Fatal(message);
        }

        /// <summary>
        /// Log a Fatal error with exception
        /// </summary>
        public static void LogFatal(string message, Exception ex)
        {
            log.Fatal(message, ex);
        }

        /// <summary>
        /// Log an error
        /// </summary>
        public static void LogError(string message)
        {
            log.Error(message);
        }

        /// <summary>
        /// Log an error with exception
        /// </summary>
        public static void LogError(string message, Exception ex)
        {
            log.Error(message, ex);
        }

        /// <summary>
        /// Log a warning
        /// </summary>
        public static void LogWarn(string message)
        {
            log.Warn(message);
        }

        /// <summary>
        /// Log an warning with exception
        /// </summary>
        public static void LogWarn(string message, Exception ex)
        {
            log.Warn(message, ex);
        }

        /// <summary>
        /// Log a Information
        /// </summary>
        public static void LogInfo(string message)
        {
            log.Info(message);
        }

        /// <summary>
        /// Log a Information with exception
        /// </summary>
        public static void LogInfo(string message, Exception ex)
        {
            log.Info(message, ex);
        }

        /// <summary>
        /// Log a Debug message with exception
        /// </summary>
        public static void LogDebug(string message)
        {
            log.Debug(message);
        }

        /// <summary>
        /// Log a Debug message with exception
        /// </summary>
        public static void LogDebug(string message, Exception ex)
        {
            log.Debug(message, ex);
        }
    }
}
