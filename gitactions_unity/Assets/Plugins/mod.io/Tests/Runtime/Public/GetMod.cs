using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class GetMod
    {
        [UnitySetUp]
        public IEnumerator AutoInitialize()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);
            ModIOUnity.SetLoggingDelegate(delegate {});
            Debug.Log("<color=red><b>- NEW TEST SETUP -</b></color>");

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

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetMod(new ModId(1), r => { result = r.result; });
            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsInitialisationError());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenCredentialsAreInvalid()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            TestingUtility.InvalidateCredentials();

            // Try GetMod
            ModIOUnity.GetMod(new ModId(1), r => { result = r.result; });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsAuthenticationError);
        }

        [UnityTest]
        public IEnumerator RequestSucceedsWithValidModID()
        {
            ModId id = new ModId(41);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            // Try GetMod
            ModIOUnity.GetMod(id, r => { result = r.result; });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        // [UnityTest]
        // public IEnumerator RequestResponseIs403WithInvalidModId()
        // {
        //     ModId id = new ModId(0);
        //
        //     InvokeNullable invoked = null;
        //     Result result = ResultBuilder.Unknown;
        //
        //     // Try GetMod
        //     ModIOUnity.GetMod(id, r => { result = r.result; });
        //
        //     yield return new WaitForInvoke(() => invoked);
        //
        //     Assert.That(result.code == 403);
        // }

        [UnityTest]
        public IEnumerator RequestResponseIs404WithInvalidModId()
        {
            ModId id = new ModId(long.MaxValue);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            // Try GetMod
            ModIOUnity.GetMod(id, r => { result = r.result; });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.code == 404);
        }
    }
}
