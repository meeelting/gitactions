using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModIOTesting.Public
{
    internal class AddMod : InitializationUnitTestSetup
    {
        [UnityTest]
        public IEnumerator DoesAddingModProfileSucceed()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.Succeeded, $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.IsInitialisationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotAuthenticated()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            TestingUtility.RemoveCredentials();

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWithInvalidCredentials()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            TestingUtility.InvalidateCredentials();

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWithBadCreationToken()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            yield return TryToAddMod(mod, new CreationToken(), (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_BadCreationToken,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenMetadataExceedsMaxSize()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            mod.metadata = new string(new char[50001]);

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModMetadataTooLarge,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenLogoNotSet()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            mod.logo = null;

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModProfileRequiredFieldsNotSet,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNameNotSet()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            mod.name = null;

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModProfileRequiredFieldsNotSet,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenSummaryNotSet()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            mod.summary = null;

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModProfileRequiredFieldsNotSet,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenSummaryExceeds250Characters()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            mod.summary = new string(new char[251]);

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModSummaryTooLarge,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenLogoExceeds8MiB()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            mod.logo = GetTooLargeTexture();

            yield return TryToAddMod(mod, ModIOUnity.GenerateCreationToken(),
                                     (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModLogoTooLarge,
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
        Texture2D GetTooLargeTexture()
        {
            Texture2D tex = new Texture2D(5120, 2880);
            Color[] colors = tex.GetPixels();
            for(int i = 0; i < colors.Length; i++)
            {
                colors[i].r = Random.Range(0.0f, 1.0f);
                colors[i].g = Random.Range(0.0f, 1.0f);
                colors[i].b = Random.Range(0.0f, 1.0f);
            }
            tex.SetPixels(0, 0, 5120, 2880, colors);
            tex.Apply();
            Debug.Log(tex.EncodeToPNG().Length);
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
#endregion
    }
}
