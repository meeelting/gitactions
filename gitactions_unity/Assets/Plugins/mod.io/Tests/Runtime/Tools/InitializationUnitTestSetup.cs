using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting
{
    internal class InitializationUnitTestSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            // TODO @Jackson define symbols for platform specific setups
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // TODO @Jackson define symbols for platform specific teardown
        }

        [UnitySetUp]
        public IEnumerator AutoInitialize()
        {
            Debug.Log("<color=orange><b>- CLEANING LAST TEST -</b></color>");
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);
            TestingUtility.ClearAllData();
            ModIOUnity.SetLoggingDelegate(delegate {});
            Debug.Log("<color=red><b>- SETTING UP NEW TEST -</b></color>");

            InvokeNullable invoked = null;

            // Initialize
            ModIOUnity.InitialiseForUser(
                "TestSuite", TestingUtility.GetTestSettingsForReadingRequests(),
                new BuildSettings(), (r) => { invoked = new InvokeNullable(); });

            yield return new WaitForInvoke(() => invoked);

            Task taskUserCredentials = TestingUtility.SetupUserCredentials();
            yield return new WaitUntil(() => taskUserCredentials.IsCompleted);


            ModIOUnity.SetLoggingDelegate(TestingLogger.LoggingDelegate);
        }
    }
}
