using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;

namespace ModIOTesting.Public
{
    internal class AddModfile : InitializationUnitTestSetup
    {
        [UnityTest]
        public IEnumerator DoesAddingModfileSucceed()
        {
            Result result = ResultBuilder.Unknown;
            ModId modId = new ModId(5923);

            yield return TryToAddModfile(modId, (r) => { result = r; });

            Assert.That(result.Succeeded, $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            Result result = ResultBuilder.Unknown;

            yield return InstallMod((ModId)41);

            ModfileDetails mod = new ModfileDetails();
            mod.modId = new ModId(5923);
            mod.directory = GetDirectory((ModId)41);

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            yield return UploadModfile(mod, (r) => { result = r; });

            Assert.That(result.IsInitialisationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWithInvalidCredentials()
        {
            Result result = ResultBuilder.Unknown;

            yield return InstallMod((ModId)41);

            ModfileDetails mod = new ModfileDetails();
            mod.modId = new ModId(5923);
            mod.directory = GetDirectory((ModId)41);

            TestingUtility.InvalidateCredentials();

            yield return UploadModfile(mod, (r) => { result = r; });

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotAuthenticated()
        {
            Result result = ResultBuilder.Unknown;

            yield return InstallMod((ModId)41);

            ModfileDetails mod = new ModfileDetails();
            mod.modId = new ModId(5923);
            mod.directory = GetDirectory((ModId)41);

            TestingUtility.RemoveCredentials();

            yield return UploadModfile(mod, (r) => { result = r; });

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenUserDoesntHavePermission()
        {
            Result result = ResultBuilder.Unknown;

            yield return InstallMod((ModId)41);

            ModfileDetails mod = new ModfileDetails();
            mod.modId = new ModId(41);
            mod.directory = GetDirectory((ModId)41);

            yield return UploadModfile(mod, (r) => { result = r; });

            Assert.That(result.IsPermissionError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenDirectoryDoesntExist()
        {
            Result result = ResultBuilder.Unknown;

            ModfileDetails mod = new ModfileDetails();
            mod.modId = new ModId(41);
            mod.directory = GetDirectory((ModId)41);

            yield return UploadModfile(mod, (r) => { result = r; });

            Assert.That(result.code == ResultCode.IO_DirectoryDoesNotExist,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenMetadataExceedsMaxCharacters()
        {
            Result result = ResultBuilder.Unknown;

            yield return InstallMod((ModId)41);

            ModfileDetails mod = new ModfileDetails();
            mod.modId = new ModId(5923);
            mod.directory = GetDirectory((ModId)41);
            mod.metadata = new string(new char[500001]);

            yield return UploadModfile(mod, (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModMetadataTooLarge,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultChangelogExceedsMaxCharacters()
        {
            Result result = ResultBuilder.Unknown;

            yield return InstallMod((ModId)41);

            ModfileDetails mod = new ModfileDetails();
            mod.modId = new ModId(5923);
            mod.directory = GetDirectory((ModId)41);
            mod.changelog = new string(new char[500001]);

            yield return UploadModfile(mod, (r) => { result = r; });

            Assert.AreEqual(ResultCode.InvalidParameter_ChangeLogTooLarge, result.code, 
                $"Changelog should exceed max characters. Result: [{result.code}:{result.code_api}]");
        }

        #region Utility

        IEnumerator TryToAddModfile(ModId modId, Action<Result> callback)
        {
            // We're just using mod id 41 here for the directory of files to upload

            Result result = ResultBuilder.Unknown;

            yield return InstallMod((ModId)41);

            ModfileDetails mod = new ModfileDetails();
            mod.modId = modId;
            mod.directory = GetDirectory((ModId)41);

            yield return UploadModfile(mod, (r) => { result = r; });

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

        IEnumerator UploadModfile(ModfileDetails mod, Action<Result> callback)
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.UploadModfile(mod, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            callback?.Invoke(result);
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
#endregion
    }
}
