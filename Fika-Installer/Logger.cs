using Fika_Installer.Utils;

namespace Fika_Installer
{
    public interface ILogger
    {
        void Log(string message);
        void Success(string message);
        void Warning(string message);
        void Error(string message);
    }

    public interface IPageLogger : ILogger
    {
        void Confirm(string message, bool confirm = false);
        void Success(string message, bool confirm = false);
        void Error(string message, bool confirm = false);
    }

    public class FileLogger(string dir) : ILogger
    {
        private readonly string _logFilePath = Path.Combine(dir, "fika-installer.log");

        public void Log(string message)
        {
            WriteLog(message, "INFO");
        }

        public void Success(string message)
        {
            Log(message);
        }

        public void Warning(string message)
        {
            WriteLog(message, "WARN");
        }

        public void Error(string message)
        {
            WriteLog(message, "ERROR");
        }

        private void WriteLog(string message, string severity)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logEntry = $"{timestamp} [{severity}] {message}\r\n";

                File.AppendAllText(_logFilePath, logEntry);
            }
            catch { }
        }
    }

    public class PageLogger : IPageLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Confirm(string message)
        {
            Confirm(message, false);
        }

        public void Confirm(string message, bool confirm = false)
        {
            ConUtils.WriteConfirm(message, confirm);
        }

        public void Success(string message)
        {
            Success(message, false);
        }

        public void Success(string message, bool confirm = false)
        {
            ConUtils.WriteSuccess(message, confirm);
        }

        public void Warning(string message)
        {
            ConUtils.WriteWarning(message);
        }

        public void Error(string message)
        {
            Error(message, false);
        }

        public void Error(string message, bool confirm = false)
        {
            ConUtils.WriteError(message, confirm);
        }
    }

    public static class Logger
    {
        private static readonly List<ILogger> _loggers = [];

        public static void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        public static void Log(string message)
        {
            foreach (var logger in _loggers)
            {
                if (logger is IPageLogger pageLogger)
                {
                    pageLogger.Log(message);
                }
                else
                {
                    logger.Log(message);
                }
            }
        }

        public static void Confirm(string message)
        {
            foreach (var logger in _loggers)
            {
                if (logger is IPageLogger pageLogger)
                {
                    pageLogger.Confirm(message);
                }
                else
                {
                    logger.Log(message);
                }
            }
        }

        public static void Success(string message)
        {
            Success(message, false);
        }

        public static void Success(string message, bool confirm = false)
        {
            foreach (var logger in _loggers)
            {
                if (logger is IPageLogger pageLogger)
                {
                    pageLogger.Success(message, confirm);
                }
                else
                {
                    logger.Success(message);
                }
            }
        }

        public static void Warning(string message)
        {
            foreach (var logger in _loggers)
            {
                if (logger is IPageLogger pageLogger)
                {
                    pageLogger.Warning(message);
                }
                else
                {
                    logger.Warning(message);
                }
            }
        }

        public static void Error(string message)
        {
            Error(message, false);
        }

        public static void Error(string message, bool confirm = false)
        {
            foreach (var logger in _loggers)
            {
                if (logger is IPageLogger pageLogger)
                {
                    pageLogger.Error(message, confirm);
                }
                else
                {
                    logger.Error(message);
                }
            }
        }
    }
}
