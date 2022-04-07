using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class GetUserSubscriptions
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
        public IEnumerator CallbackHasCorrectResultWithoutPagination()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            SearchFilter filter = new SearchFilter();
            ModIOUnityImplementation.GetUserSubscriptions(filter, (r, p, t) => {
                result = r;
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.code == ResultCode.InvalidParameter_PaginationParams);
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWithInvalidCredentials()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            // INVALIDATE AUTH TOKEN
            TestingUtility.InvalidateCredentials();

            SearchFilter filter = new SearchFilter();
            filter.SetPageIndex(0);
            filter.SetPageSize(10);

            ModIOUnityImplementation.GetUserSubscriptions(filter, (r, p, t) => {
                result = r;
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsAuthenticationError());
        }

        [UnityTest]
        public IEnumerator DoesRequestFailWithoutAuthentication()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            TestingUtility.RemoveCredentials();

            SearchFilter filter = new SearchFilter();
            filter.SetPageIndex(0);
            filter.SetPageSize(10);

            ModIOUnityImplementation.GetUserSubscriptions(filter, (r, p, t) => {
                result = r;
                invoked = new InvokeNullable();
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(!result.Succeeded());
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            SearchFilter filter = new SearchFilter();
            filter.SetPageIndex(0);
            filter.SetPageSize(10);

            ModIOUnityImplementation.GetUserSubscriptions(filter, (r, p, t) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsInitialisationError());
        }

        [UnityTest]
        public IEnumerator DoesRequestSucceed()
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            // Try GetMods
            SearchFilter filter = new SearchFilter();
            filter.SetPageIndex(0);
            filter.SetPageSize(10);

            ModIOUnityImplementation.GetUserSubscriptions(filter, (r, p, t) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded());
        }
    }
}
