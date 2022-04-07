using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;

namespace ModIOTesting.Public
{
    internal class IsAuthenticated : InitializationUnitTestSetup
    {
        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Result result = ResultBuilder.Unknown;

            yield return CheckAuthentication((r) => result = r);

            Assert.That(result.IsInitialisationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenCredentialsAreInvalid()
        {
            TestingUtility.InvalidateCredentials();

            Result result = ResultBuilder.Unknown;

            yield return CheckAuthentication((r) => result = r);

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotAuthenticated()
        {
            TestingUtility.RemoveCredentials();

            Result result = ResultBuilder.Unknown;

            yield return CheckAuthentication((r) => result = r);

            Assert.That(result.IsAuthenticationError(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesAuthenticationCheckSucceed()
        {
            Result result = ResultBuilder.Unknown;

            yield return CheckAuthentication((r) => result = r);

            Assert.That(result.Succeeded, $"Failed with result: [{result.code}:{result.code_api}]");
        }

#region Utility

        IEnumerator CheckAuthentication(Action<Result> callback)
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.IsAuthenticated((r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            callback?.Invoke(result);
        }
#endregion
    }
}
