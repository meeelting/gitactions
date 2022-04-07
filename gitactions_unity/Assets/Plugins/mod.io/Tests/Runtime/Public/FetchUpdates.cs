using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class FetchUpdates
    {
        [UnitySetUp]
        public IEnumerator AutoInitialize()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);
            TestingUtility.ClearAllData();
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
        public IEnumerator CallbackHasCorrectResultWhenCredentialsAreInvalid()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            // INVALIDATE AUTH TOKEN
            TestingUtility.InvalidateCredentials();

            ModIOUnity.FetchUpdates((r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsAuthenticationError());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.FetchUpdates((r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsInitialisationError());
        }

        [UnityTest]
        public IEnumerator DoesInitialAndRecurringRequestSucceed()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.FetchUpdates((r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if (!result.Succeeded())
            {
                Assert.That(false, $"Result [{result.code}:{result.code_api}]");
            }

            invoked = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.FetchUpdates((r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(), $"Result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesInitialRequestSucceedWithOfflineQueuedUnsubscribe()
        {
            ModId modId = new ModId(41);
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false,
                            $"failed to subscribe. Result [{result.code}:{result.code_api}]");
            }

            // add a queued unsubscribe as if it were offline
            ModCollectionManager.RemoveModFromUserSubscriptions(modId, true);

            invoked = null;

            ModIOUnity.FetchUpdates((r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false,
                            $"failed to fetch updates. Result [{result.code}:{result.code_api}]");
            }

            // Double check by getting subscribedMods
            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out Result getsubsResult);
            if(getsubsResult.Succeeded())
            {
                foreach(SubscribedMod mod in mods)
                {
                    if(mod.modProfile.id.Equals(modId))
                    {
                        Assert.That(false, "mod id still exists in subscribed mods");
                    }
                }
            }
            else
            {
                Assert.That(
                    false,
                    $"Failed to get subscribed mods for validation. Result [{getsubsResult.code}:{getsubsResult.code_api}]");
            }

            Assert.That(true);
        }
    }
}
