using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;

namespace ModIOTesting.Public
{
    internal class _Initialization
    {
        [UnityTest]
        public IEnumerator DoesInitialize()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);
            ModIOUnity.SetLoggingDelegate(null);
            ModIOUnity.SetLoggingDelegate(TestingLogger.LoggingDelegate);
            Debug.Log("<color=red><b>- NEW TEST SETUP -</b></color>");

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.InitialiseForUser("TestSuite", (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(), $"Failed with result: [{result.code}]");
        }

        [UnityTest]
        public IEnumerator DoesInitializeWithCustomSettings()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);
            ModIOUnity.SetLoggingDelegate(null);
            ModIOUnity.SetLoggingDelegate(TestingLogger.LoggingDelegate);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.InitialiseForUser("TestSuite",
                                              TestingUtility.GetTestSettingsForReadingRequests(),
                                              new BuildSettings(), (r) => {
                                                  invoked = new InvokeNullable();
                                                  result = r;
                                              });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(), $"Failed with result: [{result.code}]");
        }
    }
}
