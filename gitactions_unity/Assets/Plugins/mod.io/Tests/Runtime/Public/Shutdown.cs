using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class Shutdown
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
        public IEnumerator DoesShutdownCompleteWithoutExceptions()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            yield return null;

            Assert.That(true);
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetMod()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetMod((ModId)41, r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetMods()
        {
            Result result = ResultBuilder.Unknown;
            SearchFilter filter = new SearchFilter();
            filter.SetPageIndex(0);
            filter.SetPageSize(10);

            ModIOUnity.GetMods(filter, (r, p) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTags()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTagCategories(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetCurrentUser()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetCurrentUser(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_Initialize()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Result result = ResultBuilder.Unknown;
            ModIOUnity.InitialiseForUser("Testsuite", (r) => { result = r; });

            shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_RequestAuthenticationEmail()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.RequestAuthenticationEmail("stephen.lucerne@mod.io", (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_SubmitEmailSecurityCode()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.SubmitEmailSecurityCode("12345", (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTermsOfUse_Steam()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTermsOfUse(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTermsOfUse_GOG()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTermsOfUse(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTermsOfUse_Itchio()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTermsOfUse(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTermsOfUse_Discord()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTermsOfUse(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTermsOfUse_Google()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTermsOfUse(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTermsOfUse_Oculus()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTermsOfUse(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTermsOfUse_Switch()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTermsOfUse(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_GetTermsOfUse_Xbox()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.GetTermsOfUse(r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AuthenticateViaSteam()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.AuthenticateUserViaSteam("12345", "stephen.lucerne@mod.io", null,
                                                (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AuthenticateViaDiscord()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.AuthenticateUserViaDiscord("12345", "stephen.lucerne@mod.io", null,
                                                  (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AuthenticateViaSwitch()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.AuthenticateUserViaSwitch("12345", "stephen.lucerne@mod.io", null,
                                                 (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AuthenticateViaGOG()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.AuthenticateUserViaGOG("12345", "stephen.lucerne@mod.io", null,
                                              (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AuthenticateViaXbox()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.AuthenticateUserViaXbox("12345", "stephen.lucerne@mod.io", null,
                                               (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AuthenticateViaGoogle()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.AuthenticateUserViaGoogle("12345", "stephen.lucerne@mod.io", null,
                                                 (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AuthenticateViaItchio()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.AuthenticateUserViaItch("12345", "stephen.lucerne@mod.io", null,
                                               (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AuthenticateViaOculus()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.AuthenticateUserViaOculus(OculusDevice.Quest, "12345", 12345, "12345",
                                                 "stephen.lucerne@mod.io", null,
                                                 (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_RateMod()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.RateMod((ModId)41, ModRating.Positive, (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_SubscribeTo()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.SubscribeToMod((ModId)41, (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_UnubscribeFrom()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.UnsubscribeFromMod((ModId)41, (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_FetchUpdates()
        {
            Result result = ResultBuilder.Unknown;
            ModIOUnity.FetchUpdates((r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_DownloadTexture()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;

            ModIOUnity.DownloadTexture(profile.logoImage_320x180, r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AddMod()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetailsForAdd();

            ModIOUnity.CreateModProfile(ModIOUnity.GenerateCreationToken(), mod, r => { result = r.result; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_EditMod()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetailsForEdit();

            ModIOUnity.EditModProfile(mod, (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_ArchiveMod()
        {
            Result result = ResultBuilder.Unknown;

            ModId modId = new ModId(41);

            ModIOUnity.ArchiveModProfile(modId, (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesOperationCancelOnShutdown_AddModfile()
        {
            Result result = ResultBuilder.Unknown;

            yield return InstallMod((ModId)41);

            ModfileDetails mod = new ModfileDetails();
            mod.modId = (ModId)41;
            mod.directory = GetDirectory((ModId)41);

            ModIOUnity.UploadModfile(mod, (r) => { result = r; });

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Assert.That(result.code == ResultCode.Internal_OperationCancelled,
                        $"result [{result.code}:{result.code_api}]");
        }

#region Utility

        ModProfileDetails GetValidModProfileDetailsForEdit()
        {
            ModProfileDetails mod = new ModProfileDetails();
            mod.modId = new ModId(41);
            mod.summary =
                "Summary for a test mod submitted via the Test Runner of the Unity v2 plugin";
            mod.name = "Unity v2 Test mod";
            return mod;
        }

        ModProfileDetails GetValidModProfileDetailsForAdd()
        {
            ModProfileDetails mod = new ModProfileDetails();
            mod.logo = GetTexture();
            mod.summary =
                "Summary for a test mod submitted via the Test Runner of the Unity v2 plugin";
            mod.name = "Unity v2 Test mod";
            return mod;
        }

        Texture2D GetTexture()
        {
            Texture2D tex = new Texture2D(512, 288);
            tex.SetPixels(0, 0, 512, 288, new Color[147456]);
            tex.Apply();
            return tex;
        }

        IEnumerator TryToAddMod(ModProfileDetails mod, CreationToken token, Action<Result> callback)
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.CreateModProfile(token, mod, r => { result = r.result; });

            yield return new WaitForInvoke(() => invoked);

            callback?.Invoke(result);
        }

        string GetDirectory(ModId modId)
        {
            string dir = "";

            if(ModCollectionManager.Registry.mods.ContainsKey(modId))
            {
                ModCollectionEntry entry = ModCollectionManager.Registry.mods[modId];
                DataStorage.TryGetInstallationDirectory(modId, entry.currentModfile.id, out dir);
            }

            return dir;
        }

        IEnumerator InstallMod(ModId modId)
        {
            InvokeNullable installed = null;
            Result installedResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Installed && i == 41)
                {
                    installedResult = ResultBuilder.Success;
                    installed = new InvokeNullable();
                }
            });

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to subscribe");
            }

            yield return new WaitForInvoke(() => installed);

            if(!installedResult.Succeeded())
            {
                Assert.That(false, "failed to install");
            }
        }
#endregion // Utility
    }
}
