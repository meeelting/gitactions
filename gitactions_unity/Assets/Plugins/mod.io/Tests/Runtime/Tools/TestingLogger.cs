using ModIO;

namespace ModIOTesting
{
    internal class TestingLogger
    {
        internal const string TestingPrefix =
            "<b><color=red>[[[</color><color=#ffffff>TESTING</color><color=red>]]]</color></b>";

        internal static void LoggingDelegate(LogLevel logLevel, string logMessage)
        {
            logMessage = $"{TestingPrefix} {logMessage}";

            switch(logLevel)
            {
                case LogLevel.Error:
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(logMessage);
                    break;
                default:
                    UnityEngine.Debug.Log(logMessage);
                    break;
            }
        }

        /// <summary>
        /// Functionally the same although it allows errors to be fired which can normally intefere
        /// with a test by triggering a negative result.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logMessage"></param>
        internal static void LoggingDelegateWithErrors(LogLevel logLevel, string logMessage)
        {
            logMessage = $"{TestingPrefix} {logMessage}";

            switch(logLevel)
            {
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(logMessage);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(logMessage);
                    break;
                default:
                    UnityEngine.Debug.Log(logMessage);
                    break;
            }
        }
    }
}
