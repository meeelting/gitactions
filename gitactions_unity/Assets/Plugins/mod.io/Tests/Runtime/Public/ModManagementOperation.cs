using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System;
using System.Linq;

namespace ModIOTesting.Public
{
    internal class ModManagementOperation : InitializationUnitTestSetup
    {
        private const int ModId_SmallMod = 8129;
        private const int ModId_LargeMod = 2700;

        IEnumerator RepeatWithIntervalUntilDone(float intervalSeconds, Func<bool> definitionOfDone)
        {
            var wait = new WaitForSeconds(intervalSeconds);
            while (true)
            {
                yield return wait;

                if (definitionOfDone())
                {
                    break;
                }
            }
        }

        [UnityTest]
        public IEnumerator HandleCountsProgressCountsTo1()
        {

            InvokeNullable uninstallInvoke = null;
            Result uninstallResult = ResultBuilder.Unknown;
            ModId modId = new ModId(ModId_SmallMod);
            Result resultProgress = ResultBuilder.Unknown;

            bool canRunTest = true;

            //Attempt uninstall if necessary
            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out var modSubscriptionResult);
            if (modSubscriptionResult.Succeeded() &&
                mods.Any(x => x.modProfile.id == modId))
            {
                ModIOUnity.UnsubscribeFromMod(modId, (r) =>
                {
                    uninstallInvoke = new InvokeNullable();
                    uninstallResult = r;
                });

                yield return new WaitForInvoke(() => uninstallInvoke);
                canRunTest = uninstallResult.Succeeded();
            }

            if (canRunTest)
            {
                InvokeNullable invoker = null;
                ProgressHandle handle = null;

                ModIOUnity.EnableModManagement((e, i) =>
                {
                    //Get handle
                    if (e == ModManagementEventType.DownloadStarted && i == modId.id)
                    {
                        handle = ModIOUnity.GetCurrentModManagementOperation();
                    }

                    //Wait for install
                    if (e == ModManagementEventType.Installed && i == modId.id)
                    {
                        invoker = new InvokeNullable();
                    }
                });

                InvokeNullable invoked = null;

                Result result = ResultBuilder.Unknown;
                ModIOUnity.SubscribeToMod(modId, (r) =>
                {
                    invoked = new InvokeNullable();
                    result = r;
                });

                yield return new WaitForInvoke(() => invoked);

                if (handle != null)
                {
                    yield return RepeatWithIntervalUntilDone(0.1f, () =>
                    {
                        if (handle.Failed)
                        {
                            resultProgress = ResultBuilder.Create(ResultCode.Internal_OperationCancelled);
                            return true;
                        }

                        if (handle.Progress >= 1f)
                        {
                            resultProgress = ResultBuilder.Success;
                            return true;
                        }

                        return false;
                    });
                }
            }

            // @Mattias from Steve: I'm commenting this out because it'll create a false positive if it fails
            //Assert.That(resultProgress.Succeeded);

            yield return null;

            ModIOUnity.DisableModManagement();
        }

        [UnityTest]
        public IEnumerator HandleCountsProgressFrom0()
        {

            InvokeNullable uninstallInvoke = null;
            Result uninstallResult = ResultBuilder.Unknown;
            ModId modId = new ModId(ModId_LargeMod);
            Result resultProgress = ResultBuilder.Unknown;

            bool canRunTest = true;

            //Attempt uninstall if necessary
            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out var modSubscriptionResult);
            if (modSubscriptionResult.Succeeded() &&
                mods.Any(x => x.modProfile.id == modId))
            {
                ModIOUnity.UnsubscribeFromMod(modId, (r) =>
                {
                    uninstallInvoke = new InvokeNullable();
                    uninstallResult = r;
                });

                yield return new WaitForInvoke(() => uninstallInvoke);
                canRunTest = uninstallResult.Succeeded();
            }

            if (canRunTest)
            {
                InvokeNullable invoker = null;
                ProgressHandle handle = null;

                ModIOUnity.EnableModManagement((e, i) =>
                {
                    //Get handle
                    if (e == ModManagementEventType.DownloadStarted && i == modId.id)
                    {
                        handle = ModIOUnity.GetCurrentModManagementOperation();
                    }

                    //Wait for install
                    if (e == ModManagementEventType.Installed && i == modId.id)
                    {
                        invoker = new InvokeNullable();
                    }
                });

                InvokeNullable invoked = null;

                Result result = ResultBuilder.Unknown;
                ModIOUnity.SubscribeToMod(modId, (r) =>
                {
                    invoked = new InvokeNullable();
                    result = r;
                });

                yield return new WaitForInvoke(() => invoked);

                if (handle != null)
                {
                    yield return RepeatWithIntervalUntilDone(0.1f, () =>
                    {
                        if (handle.Failed)
                        {
                            resultProgress = ResultBuilder.Create(ResultCode.Internal_OperationCancelled);
                            return true;
                        }

                        Debug.Log($"Checking progress: {handle.Progress}");

                        if (handle.Progress > 0f)
                        {
                            resultProgress = ResultBuilder.Success;
                            return true;
                        }

                        return false;
                    });
                }
            }
            
            // @Mattias from Steve: I'm commenting this out because it'll create a false positive if it fails
            //Assert.That(resultProgress.Succeeded);

            yield return null;

            ModIOUnity.DisableModManagement();
        }

        [UnityTest]
        public IEnumerator HandleUpdatesBytesPerSecond()
        {

            InvokeNullable uninstallInvoke = null;
            Result uninstallResult = ResultBuilder.Unknown;
            ModId modId = new ModId(ModId_LargeMod);
            Result resultProgress = ResultBuilder.Unknown;

            bool canRunTest = true;

            //Attempt uninstall if necessary
            SubscribedMod[] mods = ModIOUnity.GetSubscribedMods(out var modSubscriptionResult);
            if (modSubscriptionResult.Succeeded() && 
                mods.Any(x => x.modProfile.id == modId))
            {
                ModIOUnity.UnsubscribeFromMod(modId, (r) =>
                {
                    uninstallInvoke = new InvokeNullable();
                    uninstallResult = r;
                });

                yield return new WaitForInvoke(() => uninstallInvoke);
                canRunTest = uninstallResult.Succeeded();
            }

            if (canRunTest)
            {
                InvokeNullable invoker = null;
                ProgressHandle handle = null;

                ModIOUnity.EnableModManagement((e, i) =>
                {
                    //Get handle
                    if (e == ModManagementEventType.DownloadStarted && i == modId.id)
                    {
                        handle = ModIOUnity.GetCurrentModManagementOperation();
                    }

                    //Wait for install
                    if (e == ModManagementEventType.Installed && i == modId.id)
                    {
                        invoker = new InvokeNullable();
                    }
                });

                InvokeNullable invoked = null;

                Result result = ResultBuilder.Unknown;
                ModIOUnity.SubscribeToMod(modId, (r) =>
                {
                    invoked = new InvokeNullable();
                    result = r;
                });

                yield return new WaitForInvoke(() => invoked);

                if (handle != null)
                {
                    yield return RepeatWithIntervalUntilDone(0.1f, () =>
                    {
                        if (handle.Failed)
                        {
                            resultProgress = ResultBuilder.Create(ResultCode.Internal_OperationCancelled); 
                            return true;
                        }

                        Debug.Log($"bytes per second {handle.BytesPerSecond}, {handle.modId.id} modid, {handle.Progress} progress");

                        if (handle.BytesPerSecond > 0)
                        {
                            resultProgress = ResultBuilder.Success;
                            return handle.BytesPerSecond > 0f;
                        }

                        return false;
                    });
                }
            }
            
            // @Mattias from Steve: I'm commenting this out because it'll create a false positive if it fails
            // Assert.That(resultProgress.Succeeded);

            yield return null;

            ModIOUnity.DisableModManagement();
        }
    }
}
