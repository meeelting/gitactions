using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class GetInstalledMods
    {
        [UnitySetUp]
        public IEnumerator AutoInitialize()
        {
            ModIOUnity.SetLoggingDelegate(delegate {});
            ModIOUnity.RemoveUserData();
            TestingUtility.ClearAllData();
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);
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
        public IEnumerator DoesGetInstalledModsSucceed()
        {
            InstalledMod[] mods = ModIOUnity.GetSystemInstalledMods(out Result result);

            yield return null;

            Assert.That(result.Succeeded(), $"Result code: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesGetInstalledModsSucceedWhenNotAuthenticated()
        {
            TestingUtility.RemoveCredentials();

            InstalledMod[] mods = ModIOUnity.GetSystemInstalledMods(out Result result);

            yield return null;

            Assert.That(result.Succeeded(), $"Result code: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesGetInstalledModsExcludeSessionInstalls()
        {
            InvokeNullable installed = null;
            ModIOUnity.EnableModManagement((eventType, id) => {
                if(eventType == ModManagementEventType.Installed)
                {
                    installed = new InvokeNullable();
                }
            });

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

            // wait for mod management to install (use delegate to check above)
            yield return new WaitForInvoke(() => installed);

            InstalledMod[] installedMods = ModIOUnity.GetSystemInstalledMods(out result);

            yield return null;

            if(!result.Succeeded())
            {
                Assert.That(false, "Failed to get installed mods");
            }

            foreach(InstalledMod mod in installedMods)
            {
                if(mod.modProfile.id.id == 41)
                {
                    Assert.That(false, "GetInstalledMods should not include the subscribed mods for the current user");
                    yield break;
                }
            }
            
            SubscribedMod[] subbedMods = ModIOUnity.GetSubscribedMods(out result);

            if(!result.Succeeded())
            {
                Assert.That(false, "Failed to get subscribed mods");
            }
            
            foreach(SubscribedMod mod in subbedMods)
            {
                if(mod.modProfile.id.id == 41)
                {
                    Assert.That(true);
                    yield break;
                }
            }

            Assert.That(false, $"Doesn't contain modId. Result count: {subbedMods.Length}");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenCredentialsAreInvalid()
        {
            TestingUtility.InvalidateCredentials();

            InstalledMod[] mods = ModIOUnity.GetSystemInstalledMods(out Result result);

            yield return null;

            Assert.That(result.IsAuthenticationError());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            InstalledMod[] mods = ModIOUnity.GetSystemInstalledMods(out Result result);

            yield return null;

            Assert.That(result.IsInitialisationError());
        }
    }
}
