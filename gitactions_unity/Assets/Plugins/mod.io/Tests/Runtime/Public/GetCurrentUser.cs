using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class GetCurrentUser
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
        public IEnumerator CallbackHasCorrectResultWhenCredentialsAreInvalid()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            TestingUtility.InvalidateCredentials();

            ModIOUnity.GetCurrentUser(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsAuthenticationError());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetCurrentUser(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsInitialisationError());
        }

        [UnityTest]
        public IEnumerator DoesRequestSucceed()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetCurrentUser(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }


        [UnityTest]
        public IEnumerator DoesRemoveUserDataRemoveUserData()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.RemoveUserData();

            ModIOUnity.GetCurrentUser((r) => {
                invoked = new InvokeNullable();
                result = r.result;
            });
            yield return new WaitForInvoke(() => invoked);

            //We should not be able to succeed in fetching a user,
            //when the user has been removed.
            Assert.AreEqual(result.Succeeded(), false);
        }
    }
}
