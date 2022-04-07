using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class Authentication
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
        public IEnumerator DoesEmailRequestSucceed()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            // TODO @Steve Dont use my email lol
            ModIOUnity.RequestAuthenticationEmail("stephen.lucerne@mod.io", (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.RequestAuthenticationEmail("notanemailaddress", (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsInitialisationError());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWithInvalidEmailAddress()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.RequestAuthenticationEmail("notanemailaddress", (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.code == ResultCode.User_InvalidEmailAddress);
        }

        // Can Run this Unit Test on its own, just fill in the security code at the top of method
        /*[UnityTest]
         public IEnumerator DoesEmailExchangeSucceed()
         {
            string securityCode = "SDQQW";

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.SubmitEmailSecurityCode(securityCode, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
         }*/

        [UnityTest]
        public IEnumerator DoesSteamAuthenticationSucceed()
        {
            TestingUtility.RemoveCredentials();
            
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.AuthenticateUserViaSteam(
                "CAEQiJPmmggYACA6KnB3qJB1v1pBTCAYB8nWsAjfZbl15LlW4lWtDcnRlB7qK7+OJKfne6Np/uClveuQF4WZaeK4PNFymXgxd7BuF+PBaUN93wRxr1ggs0J1C8GLAltAyN3fuDUZ0J7YYs6Zp1ng4togdMpr4j8w2S7GSxFI",
                null, default, (r) => {
                    invoked = new InvokeNullable();
                    result = r;
                });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetTermsOfUseSucceed_Steam()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetTermsOfUse(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetTermsOfUseSucceed_Epic()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetTermsOfUse(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetTermsOfUseSucceed_GOG()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetTermsOfUse(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetTermsOfUseSucceed_Itchio()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetTermsOfUse(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetTermsOfUseSucceed_Switch()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetTermsOfUse(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetTermsOfUseSucceed_Oculus()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetTermsOfUse(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetTermsOfUseSucceed_Xbox()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetTermsOfUse(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }

        [UnityTest]
        public IEnumerator DoesGetTermsOfUseSucceed_Discord()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIOUnity.GetTermsOfUse(r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }
    }
}
