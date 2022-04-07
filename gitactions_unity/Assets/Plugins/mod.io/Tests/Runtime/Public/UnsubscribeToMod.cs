using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class UnsubscribeToMod
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
            ModId modId = new ModId(41);

            ModIOUnity.UnsubscribeFromMod(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsInitialisationError());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenCredentialsAreInvalid()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            TestingUtility.InvalidateCredentials();

            ModId modId = new ModId(41);

            ModIOUnity.UnsubscribeFromMod(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsAuthenticationError);
        }

        [UnityTest]
        public IEnumerator RequestSucceedsWithValidModID()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModId modId = new ModId(41);

            // Ensure we are subscribed already
            // Ignore the result here, we may already be subscribed
            ModIOUnity.SubscribeToMod(modId, (r) => {
                // ignore
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);
            invoked = null;

            ModIOUnity.UnsubscribeFromMod(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        // [UnityTest]
        // public IEnumerator RequestResponseIs403WithInvalidModId()
        // {
        //     InvokeNullable invoked = null;
        //     Result result = ResultBuilder.Unknown;
        //
        //     ModId modId = new ModId(0);
        //
        //     ModIOUnity.UnsubscribeFromMod(modId, (r) => {
        //         invoked = new InvokeNullable();
        //         result = r;
        //     });
        //
        //     yield return new WaitForInvoke(() => invoked);
        //
        //     Assert.That(result.code == 403);
        // }

        [UnityTest]
        public IEnumerator RequestResponseIs404WithInvalidModId()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModId modId = new ModId(long.MaxValue);

            ModIOUnity.UnsubscribeFromMod(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.code == 404);
        }

        [UnityTest]
        public IEnumerator RequestSucceedsWhenAlreadyUnsubscribed()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModId modId = new ModId(41);

            ModIOUnity.UnsubscribeFromMod(modId, (r) => {
                // Ignore
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);
            invoked = null;

            // Clear Cache
            ModIO.Implementation.API.ResponseCache.ClearCache();

            ModIOUnity.UnsubscribeFromMod(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }
    }
}
