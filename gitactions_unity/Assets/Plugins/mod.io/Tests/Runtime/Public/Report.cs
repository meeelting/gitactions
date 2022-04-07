using System;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class Report
    {
        [UnitySetUp]
        public IEnumerator AutoInitialize()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);
            TestingUtility.ClearAllData();
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
        public IEnumerator DoesReportSucceed_Generic()
        {
            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.Generic, (Result r) => result = r);

            Assert.That(result.Succeeded(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesReportSucceed_DMCA()
        {
            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.DMCA, (Result r) => result = r);

            Assert.That(result.Succeeded(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesReportSucceed_Other()
        {
            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.Other, (Result r) => result = r);

            Assert.That(result.Succeeded(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesReportSucceed_FalseInformation()
        {
            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.FalseInformation, (Result r) => result = r);

            Assert.That(result.Succeeded(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesReportSucceed_IllegalContent()
        {
            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.IllegalContent, (Result r) => result = r);

            Assert.That(result.Succeeded(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesReportSucceed_NotWorking()
        {
            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.NotWorking, (Result r) => result = r);

            Assert.That(result.Succeeded(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesReportSucceed_RudeContent()
        {
            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.RudeContent, (Result r) => result = r);

            Assert.That(result.Succeeded(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator DoesReportSucceed_StolenContent()
        {
            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.StolenContent, (Result r) => result = r);

            Assert.That(result.Succeeded(),
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            Result result = ResultBuilder.Unknown;
            yield return SendReport(ReportType.Generic, (Result r) => result = r);

            Assert.That(result.IsInitialisationError,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenReportIsInvalid()
        {
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIO.Report report = new ModIO.Report((ModId)41, ReportType.Generic, null, "Test User",
                                                   "stephen.lucerne@mod.io");

            ModIOUnity.Report(report, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.code == ResultCode.InvalidParameter_ReportNotReady,
                        $"Failed with result: [{result.code}:{result.code_api}]");
        }

#region Utility

        IEnumerator SendReport(ReportType type, Action<Result> callback)
        {
            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;

            ModIO.Report report = new ModIO.Report(
                (ModId)41, type, "This is a test report being sent via the Unity v2 Test Runner",
                "Test User", "stephen.lucerne@mod.io");

            ModIOUnity.Report(report, (r) => {
                invoked = new InvokeNullable();
                result = r;
            });

            yield return new WaitForInvoke(() => invoked);

            callback?.Invoke(result);
        }
#endregion
    }
}
