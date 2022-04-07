using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;

namespace ModIOTesting.Public
{
    internal class EditMod : InitializationUnitTestSetup
    {
        [UnityTest]
        public IEnumerator DoesEditingModProfileSucceed()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            yield return TryToEditMod(mod, (r) => { result = r; });

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

            yield return TryToEditMod(mod, (r) => { result = r; });

            Assert.That(result.IsInitialisationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotAuthenticated()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            TestingUtility.RemoveCredentials();

            yield return TryToEditMod(mod, (r) => { result = r; });

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWithInvalidCredentials()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            TestingUtility.InvalidateCredentials();

            yield return TryToEditMod(mod, (r) => { result = r; });

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenMetadataExceedsMaxSize()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            mod.metadata = new string(new char[50001]);

            yield return TryToEditMod(mod, (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModMetadataTooLarge,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenSummaryExceeds250Characters()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();

            mod.summary = new string(new char[251]);

            yield return TryToEditMod(mod, (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModSummaryTooLarge,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenModIdIsUnassigned()
        {
            Result result = ResultBuilder.Unknown;

            ModProfileDetails mod = GetValidModProfileDetails();
            mod.modId = null;

            yield return TryToEditMod(mod, (r) => { result = r; });

            Assert.That(result.code == ResultCode.InvalidParameter_ModProfileRequiredFieldsNotSet,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

#region Utility

        IEnumerator TryToEditMod(ModProfileDetails mod, Action<Result> callback)
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.EditModProfile(mod, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            callback?.Invoke(result);
        }

        ModProfileDetails GetValidModProfileDetails()
        {
            ModId modId = new ModId(5923);

            ModProfileDetails mod = new ModProfileDetails();
            mod.modId = modId;
            mod.name = "Unity v2 Test mod";
            mod.summary =
                "Summary for a test mod submitted via the Test Runner of the Unity v2 plugin";
            mod.description =
                "A longer description for a test mod submitted via the Test Runner of the Unity v2 plugin";
            mod.contentWarning =
                ContentWarnings.Alcohol | ContentWarnings.Drugs | ContentWarnings.Explicit;
            mod.tags = new[] { "Weapon", "Novice" };
            mod.metadata = "Test metadata submitted by the Unity v2 Test Runner";

            return mod;
        }

#endregion
    }
}
