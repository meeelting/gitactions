using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using Logger = ModIO.Implementation.Logger;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{

    internal class ModManagement : InitializationUnitTestSetup
    {
        [UnityTest]
        public IEnumerator DoesDownloadSucceed()
        {
            InvokeNullable downloaded = null;
            Result downloadResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Downloaded && i == 41)
                {
                    downloadResult = ResultBuilder.Success;
                    downloaded = new InvokeNullable();
                }
            });

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod((ModId)41, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false,
                            $"failed to subscribe. Result [{result.code}:{result.code_api}]");
            }

            yield return new WaitForInvoke(() => downloaded);

            Assert.That(downloadResult.Succeeded);
        }

        [UnityTest]
        public IEnumerator DoesDownloadSucceedAfterReInitializing()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod((ModId)41, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to subscribe");
            }

            // re-initialize
            yield return InitializeWithPreviousData();

            // Fetch updates
            invoked = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.FetchUpdates((r) => {
                result = r;
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.AreEqual(ResultCode.Success, result.code);
                //Assert.That(false, "failed to fetch updates");
            }

            InvokeNullable downloaded = null;
            Result downloadedResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Downloaded && i == 41)
                {
                    downloadedResult = ResultBuilder.Success;
                    downloaded = new InvokeNullable();
                }
            });

            yield return new WaitForInvoke(() => downloaded);

            Assert.That(downloadedResult.Succeeded);
        }

        [UnityTest]
        public IEnumerator DoesInstallSucceed()
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

            ModIOUnity.SubscribeToMod((ModId)41, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to subscribe");
            }

            yield return new WaitForInvoke(() => installed);

            Assert.That(installedResult.Succeeded);
        }

        [UnityTest]
        public IEnumerator DoesInstallSucceedAfterReInitializing()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod((ModId)41, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to subscribe");
            }

            // re-initialize
            yield return InitializeWithPreviousData();

            // Fetch updates
            invoked = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.FetchUpdates((r) => {
                result = r;
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to fetch updates");
            }

            InvokeNullable installed = null;
            Result installedResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Installed && i == 41)
                {
                    installedResult = ResultBuilder.Success;
                    installed = new InvokeNullable();
                }
            });

            yield return new WaitForInvoke(() => installed);

            Assert.That(installedResult.Succeeded);
        }

        [UnityTest]
        public IEnumerator DoesUninstallSucceed()
        {
            InvokeNullable installed = null;
            InvokeNullable deleted = null;
            Result installedResult = ResultBuilder.Unknown;
            Result deletedResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Installed && i == 41)
                {
                    installedResult = ResultBuilder.Success;
                    installed = new InvokeNullable();
                }
                else if(e == ModManagementEventType.Uninstalled && i == 41)
                {
                    deletedResult = ResultBuilder.Success;
                    deleted = new InvokeNullable();
                }
            });

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod((ModId)41, (r) => {
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

            invoked = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.UnsubscribeFromMod((ModId)41, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to unsubscribe");
            }

            yield return new WaitForInvoke(() => deleted);

            Assert.That(deletedResult.Succeeded);
        }

        [UnityTest]
        public IEnumerator DoesUninstallSucceedAfterReInitializing()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod((ModId)41, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to subscribe");
            }

            InvokeNullable installed = null;
            Result installedResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Installed && i == 41)
                {
                    installedResult = ResultBuilder.Success;
                    installed = new InvokeNullable();
                }
            });

            yield return new WaitForInvoke(() => installed);

            if(!installedResult.Succeeded())
            {
                Assert.That(false, "failed to install");
            }

            // re-initialize
            yield return InitializeWithPreviousData();

            invoked = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.UnsubscribeFromMod((ModId)41, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to unsubscribe");
            }
            // Fetch updates
            invoked = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.FetchUpdates((r) => {
                result = r;
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to fetch updates");
            }

            InvokeNullable deleted = null;
            Result deletedResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Uninstalled && i == 41)
                {
                    deletedResult = ResultBuilder.Success;
                    deleted = new InvokeNullable();
                }
            });

            yield return new WaitForInvoke(() => deleted);

            Assert.That(deletedResult.Succeeded);
        }

        [UnityTest]
        public IEnumerator DoesUpdateSucceed()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod((ModId)5923, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to subscribe");
            }

            InvokeNullable installed = null;
            Result installedResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Installed && i == 5923)
                {
                    installedResult = ResultBuilder.Success;
                    installed = new InvokeNullable();
                }
            });

            yield return new WaitForInvoke(() => installed);

            if(!installedResult.Succeeded())
            {
                Assert.That(false, "failed to install");
            }

            yield return TryToAddModfile((ModId)5923, (r) => { result = r; });

            if(!installedResult.Succeeded())
            {
                Assert.That(false, "failed to add mod file");
            }

            // Fetch updates
            invoked = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.FetchUpdates((r) => {
                result = r;
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to fetch updates");
            }

            InvokeNullable updated = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Updated && i == 5923)
                {
                    result = ResultBuilder.Success;
                    updated = new InvokeNullable();
                }
            });

            yield return new WaitForInvoke(() => updated);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to update");
            }

            Assert.That(result.Succeeded(), "failed to update");
        }

        [UnityTest]
        public IEnumerator DoesUpdateSucceedAfterReInitializing()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubscribeToMod((ModId)5923, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to subscribe");
            }

            InvokeNullable installed = null;
            Result installedResult = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Installed && i == 5923)
                {
                    installedResult = ResultBuilder.Success;
                    installed = new InvokeNullable();
                }
            });

            yield return new WaitForInvoke(() => installed);

            if(!installedResult.Succeeded())
            {
                Assert.That(false, "failed to install");
            }

            yield return TryToAddModfile((ModId)5923, (r) => { result = r; });

            if(!installedResult.Succeeded())
            {
                Assert.That(false, "failed to add mod file");
            }

            // re-initialize
            yield return InitializeWithPreviousData();

            // Fetch updates
            invoked = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.FetchUpdates((r) => {
                result = r;
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to fetch updates");
            }

            InvokeNullable updated = null;
            result = ResultBuilder.Unknown;

            ModIOUnity.EnableModManagement((e, i) => {
                if(e == ModManagementEventType.Updated && i == 5923)
                {
                    result = ResultBuilder.Success;
                    updated = new InvokeNullable();
                }
            });

            yield return new WaitForInvoke(() => updated);

            if(!result.Succeeded())
            {
                Assert.That(false, "failed to update");
            }

            Assert.That(result.Succeeded(), "failed to update");
        }

#region Utility
        public IEnumerator InitializeWithPreviousData()
        {
            InvokeNullable shutdown = null;
            Logger.Log(LogLevel.Verbose, "SHUTTING DOWN");
            ModIOUnity.Shutdown(() => {
                Logger.Log(LogLevel.Verbose, "SHUTDOWN COMPLETE");
                shutdown = new InvokeNullable();
            });
            yield return new WaitForInvoke(() => shutdown);

            InvokeNullable invoked = null;

            // Initialize
            ModIOUnity.InitialiseForUser(
                "TestSuite", TestingUtility.GetTestSettingsForReadingRequests(),
                new BuildSettings(), (r) => { invoked = new InvokeNullable(); });

            yield return new WaitForInvoke(() => invoked);

            Task taskUserCredentials = TestingUtility.SetupUserCredentials();
            yield return new WaitUntil(() => taskUserCredentials.IsCompleted);

        }

        IEnumerator TryToAddModfile(ModId modId, Action<Result> callback)
        {
            // We're just using mod id 41 here for the directory of files to upload

            Result result = ResultBuilder.Unknown;

            ModfileDetails mod = new ModfileDetails();
            mod.modId = modId;
            mod.directory = GetDirectory((ModId)5923);

            yield return UploadModfile(mod, (r) => { result = r; });

            callback?.Invoke(result);
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
#endregion
    }
}
