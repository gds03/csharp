using CustomComponents.Core.Interfaces;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Log4net.Types
{
    public class Log4NetLogger : ILogger
    {
        const string APPENDER_NAME = "AdoNetAppender_SqlServer";
        const string LOGGER_NAME = "DatabaseLogger";
        const string FILE_CONFIG_NAME = "log4net-config.xml";

        // Concrete log
        static readonly ILog s_databaseLog;

        // Get the Hierarchy object that organizes the loggers
        static readonly Hierarchy s_appenderHierarchy;

        // Singleton access
        public static readonly Log4NetLogger Singleton = new Log4NetLogger();


        private Log4NetLogger() { }








        // type constructor
        static Log4NetLogger()
        {
            // configure internal structures from xml
            XmlConfigurator.Configure(new FileInfo(FILE_CONFIG_NAME));

            // obtain logger and hierarchy with appenders
            s_databaseLog = LogManager.GetLogger(LOGGER_NAME);
            s_appenderHierarchy = (Hierarchy)LogManager.GetLoggerRepository();
            
        }


        private AdoNetAppender CurrentAppender
        {
            get
            {
                return (AdoNetAppender)s_appenderHierarchy.GetLogger(LOGGER_NAME, s_appenderHierarchy.LoggerFactory).GetAppender(APPENDER_NAME);
            }
        }

        public string ConnectionString
        {
            get
            {
                if (s_appenderHierarchy != null)
                {
                    AdoNetAppender adoAppender = CurrentAppender;

                    if (adoAppender != null)
                    {
                        return adoAppender.ConnectionString;
                    }
                }
                
                throw new InvalidOperationException();
            }
            set
            {
                if (s_appenderHierarchy != null)
                {
                    AdoNetAppender adoAppender = CurrentAppender;

                    if (adoAppender != null)
                    {
                        adoAppender.ConnectionString = value;
                        adoAppender.ActivateOptions();
                    }
                }

            }
        }



        public void Fatal(string message)
        {
            if (!s_databaseLog.IsFatalEnabled)
                throw new InvalidOperationException("Fatal level is not enabled");

            s_databaseLog.Fatal(message);
        }

        public void Error(string message)
        {
            if (!s_databaseLog.IsErrorEnabled)
                throw new InvalidOperationException("Error level is not enabled");

            s_databaseLog.Error(message);
        }

        public void Warn(string message)
        {
            if (!s_databaseLog.IsWarnEnabled)
                throw new InvalidOperationException("Warn level is not enabled");

            s_databaseLog.Warn(message);
        }

        public void Info(string message)
        {
            if (!s_databaseLog.IsInfoEnabled)
                throw new InvalidOperationException("Info level is not enabled");

            s_databaseLog.Info(message);
        }

        public void Debug(string message)
        {
            if (!s_databaseLog.IsDebugEnabled)
                throw new InvalidOperationException("Debug level is not enabled");

            s_databaseLog.Debug(message);
        }
    }
}
