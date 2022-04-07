using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;

namespace ModIOTesting.Public
{
    internal class ArchiveMod : InitializationUnitTestSetup
    {
        [UnityTest]
        public IEnumerator DoesArchiveModProfileSucceed()
        {
            Result result = ResultBuilder.Unknown;
            ModId modId = new ModId(0);

            ModProfileDetails mod = GetValidModProfileDetails();

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(), (r, id) => {
                modId = id;
                result = r;
            });

            if(!result.Succeeded())
            {
                Assert.That(false, "Failed to add mod");
            }

            yield return TryToArchiveMod(modId, (r) => { result = r; });

            Assert.That(result.Succeeded, $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            Result result = ResultBuilder.Unknown;
            ModId modId = new ModId(41);

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            yield return TryToArchiveMod(modId, (r) => { result = r; });

            Assert.That(result.IsInitialisationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotAuthenticated()
        {
            Result result = ResultBuilder.Unknown;
            ModId modId = new ModId(41);

            TestingUtility.RemoveCredentials();

            yield return TryToArchiveMod(modId, (r) => { result = r; });

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWithInvalidCredentials()
        {
            Result result = ResultBuilder.Unknown;
            ModId modId = new ModId(41);

            TestingUtility.InvalidateCredentials();

            yield return TryToArchiveMod(modId, (r) => { result = r; });

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWithInvalidPermissions()
        {
            Result result = ResultBuilder.Unknown;
            ModId modId = new ModId(41);

            yield return TryToArchiveMod(modId, (r) => { result = r; });

            Assert.That(result.IsPermissionError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

#region Utility

        ModProfileDetails GetValidModProfileDetails()
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

        IEnumerator TryToAddMod(ModProfileDetails mod, CreationToken token,
                                Action<Result, ModId> callback)
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModId modId = new ModId(0);

            ModIOUnity.CreateModProfile(token, mod, r => {
                invoked = new InvokeNullable();
                result = r.result;
                modId = r.value;
            });

            yield return new WaitForInvoke(() => invoked);

            callback?.Invoke(result, modId);
        }

        IEnumerator TryToArchiveMod(ModId modId, Action<Result> callback)
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.ArchiveModProfile(modId, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            callback?.Invoke(result);
        }
#endregion
    }
}
