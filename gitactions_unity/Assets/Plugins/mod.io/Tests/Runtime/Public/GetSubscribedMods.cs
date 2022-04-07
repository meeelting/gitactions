using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class GetSubsribedMods
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
        public IEnumerator DoesGetSubscribedModsSucceed()
        {
            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out Result result);

            yield return null;

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetSubscribedModsContainSessionSubscriptions()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModId modId = new ModId(41);

            ModIOUnity.SubscribeToMod(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "Failed to subscribe");
            }

            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out result);

            yield return null;

            if(!result.Succeeded())
            {
                Assert.That(false, "Failed to get subscribed mods");
            }

            foreach(SubscribedMod mod in mods)
            {
                if(mod.modProfile.id.id == 41)
                {
                    Assert.That(true);
                    yield break;
                }
            }

            Assert.That(false, $"Doesn't contain modId. Returned results: {mods.Length}");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotAuthenticated()
        {
            TestingUtility.RemoveCredentials();

            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out Result result);

            yield return null;

            Assert.That(result.IsAuthenticationError());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenCredentialsAreInvalid()
        {
            TestingUtility.InvalidateCredentials();

            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out Result result);

            yield return null;

            Assert.That(result.IsAuthenticationError());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out Result result);

            yield return null;

            Assert.That(result.IsInitialisationError());
        }
    }
}
