using System.Collections.Generic;
using UnityEngine;

namespace ModIO.Implementation
{
    /// <summary>
    /// This class is responsible for outputting all of the logs that pertain to the ModIO Plugin
    /// </summary>
    internal static class Logger
    {
        internal const string ModioLogPrefix = "[mod.io]";

        static Dictionary<int, string> ErrorCodeLibrary = new Dictionary<int, string>();

        static LogMessageDelegate LogDelegate = UnityLogDelegate;

        internal static void SetLoggingDelegate(LogMessageDelegate loggingDelegate)
        {
            Logger.LogDelegate = loggingDelegate ?? UnityLogDelegate;
        }

        internal static void ResetLoggingDelegate()
        {
            Logger.LogDelegate = UnityLogDelegate;
        }

        internal static void UnityLogDelegate(LogLevel logLevel, string logMessge)
        {
            switch(logLevel)
            {
                case LogLevel.Error:
                    Debug.LogWarning(logMessge);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(logMessge);
                    break;
                case LogLevel.Message:
                    Debug.Log(logMessge);
                    break;
                case LogLevel.Verbose:
                    Debug.Log(logMessge);
                    break;
            }
        }

        internal static void Log(LogLevel logLevel, string logMessage)
        {
            logMessage = $"{ModioLogPrefix} {logMessage}";
            Logger.LogDelegate?.Invoke(logLevel, logMessage);
        }
    }
}
