using System;
using System.Collections;
using System.IO;
using UnityEngine.TestTools;
using NUnit.Framework;

using ModIO;
using ModIO.Implementation;
using ModIO.Implementation.Platform;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Platform
{
    internal class TestSuite_SystemIO
    {
        readonly static string TestDirectory =
            $@"{System.IO.Directory.GetCurrentDirectory()}/mod.io/test";

        const string ReadFile_FileName = "createReadFileStream.txt";
        const string WriteFile_FileName = "createWriteFileStream.txt";

        const long FileSize_1MiB = 1024 * 1024;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            // create Test Directory
            if(Directory.Exists(TestSuite_SystemIO.TestDirectory))
            {
                Directory.Delete(TestSuite_SystemIO.TestDirectory, true);
            }
            Directory.CreateDirectory(TestSuite_SystemIO.TestDirectory);


            yield break;
        }

        [UnityTest]
        public IEnumerator CreateReadStream()
        {
            string filePath =
                $@"{TestSuite_SystemIO.TestDirectory}/{TestSuite_SystemIO.ReadFile_FileName}";

            // create ReadFile
            System.IO.File.WriteAllBytes(filePath, new byte[0]);

            // test
            Result result = ResultBuilder.Unknown;

            using(ModIOFileStream stream = SystemIOWrapper.OpenReadStream(filePath, out result))
            {
                Assert.AreEqual(ResultCode.Success, result.code, $"Unexpected result code");
                Assert.IsNotNull(stream, $"Stream was null");
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator ReadFromFileSync()
        {
            string filePath =
                $@"{TestSuite_SystemIO.TestDirectory}/{TestSuite_SystemIO.ReadFile_FileName}";

            // create ReadFile
            int seed = (int)DateTime.UtcNow.ToFileTime();
            byte[] fileContent = new byte[TestSuite_SystemIO.FileSize_1MiB];
            var random = new System.Random(seed);
            random.NextBytes(fileContent);
            System.IO.File.WriteAllBytes(filePath, fileContent);

            // test
            Result result = ResultBuilder.Unknown;

            using(ModIOFileStream stream = SystemIOWrapper.OpenReadStream(filePath, out result))
            {
                long fileSize = stream.Length;
                byte[] buffer = new byte[fileSize];
                stream.Read(buffer, 0, (int)fileSize);

                Assert.AreEqual(ResultCode.Success, result.code,
                                $"Unexpected result code [Seed={seed}]");
                Assert.AreEqual(fileContent, buffer,
                                $"File contents read != randomised byte array [Seed={seed}]");
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator ReadFromFileAsync()
        {
            string filePath =
                $@"{TestSuite_SystemIO.TestDirectory}/{TestSuite_SystemIO.ReadFile_FileName}";

            // create ReadFile
            int seed = (int)DateTime.UtcNow.ToFileTime();
            byte[] fileContent = new byte[TestSuite_SystemIO.FileSize_1MiB];
            var random = new System.Random(seed);
            random.NextBytes(fileContent);
            System.IO.File.WriteAllBytes(filePath, fileContent);

            // test
            Result result = ResultBuilder.Unknown;

            using(ModIOFileStream stream = SystemIOWrapper.OpenReadStream(filePath, out result))
            {
                var readTask = stream.ReadAllBytesAsync();
                while(!readTask.IsCompleted) { yield return null; }

                Assert.AreEqual(ResultCode.Success, result.code,
                                $"Unexpected result code [Seed={seed}]");
                Assert.AreEqual(fileContent, readTask.GetValue(),
                                $"File contents read != randomised byte array [Seed={seed}]");
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator CreateWriteStream()
        {
            string filePath =
                $@"{TestSuite_SystemIO.TestDirectory}/{TestSuite_SystemIO.WriteFile_FileName}";

            // test
            Result result = ResultBuilder.Unknown;

            using(ModIOFileStream stream = SystemIOWrapper.OpenWriteStream(filePath, out result))
            {
                Assert.AreEqual(ResultCode.Success, result.code, $"Unexpected result code");
                Assert.IsNotNull(stream, $"Stream was null");
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator WriteToFileSync()
        {
            string filePath =
                $@"{TestSuite_SystemIO.TestDirectory}/{TestSuite_SystemIO.WriteFile_FileName}";

            // create data
            int seed = (int)DateTime.UtcNow.ToFileTime();
            byte[] fileContent = new byte[TestSuite_SystemIO.FileSize_1MiB];
            var random = new System.Random(seed);
            random.NextBytes(fileContent);

            // test
            Result result = ResultBuilder.Unknown;

            using(ModIOFileStream stream = SystemIOWrapper.OpenWriteStream(filePath, out result))
            {
                stream.Write(fileContent, 0, fileContent.Length);
                Assert.AreEqual(ResultCode.Success, result.code,
                                $"Unexpected result code [Seed={seed}]");
            }

            byte[] buffer = File.ReadAllBytes(filePath);
            Assert.AreEqual(fileContent, buffer,
                            $"File contents written != randomised byte array [Seed={seed}]");

            yield break;
        }

        [UnityTest]
        public IEnumerator WriteToFileAsync()
        {
            string filePath =
                $@"{TestSuite_SystemIO.TestDirectory}/{TestSuite_SystemIO.WriteFile_FileName}";

            // create data
            int seed = (int)DateTime.UtcNow.ToFileTime();
            byte[] fileContent = new byte[TestSuite_SystemIO.FileSize_1MiB];
            var random = new System.Random(seed);
            random.NextBytes(fileContent);

            // test
            Result result = ResultBuilder.Unknown;

            using(ModIOFileStream stream = SystemIOWrapper.OpenWriteStream(filePath, out result))
            {
                var writeTask = stream.WriteAllBytesAsync(fileContent);
                while(!writeTask.IsCompleted) { yield return null; }

                Assert.AreEqual(ResultCode.Success, result.code,
                                $"Unexpected result code [Seed={seed}]");
            }

            byte[] buffer = File.ReadAllBytes(filePath);
            Assert.AreEqual(fileContent, buffer,
                            $"File contents written != randomised byte array [Seed={seed}]");

            yield break;
        }

        [UnityTest]
        public IEnumerator MultipleReadWriteFileAttemptsDontThrowExceptions()
        {
            /*
                Using IsAuthenticated() and GetCurrentUser() for this test. because both methods 
                save the user data it retrieved.
                NOTE: Should edit this if IsAuthenticated() method changes to synchronous
             */
            
            yield return Initialise();
            
            InvokeNullable invokedAuthCallback = null;
            InvokeNullable invokedUserCallback = null;
            Result resultAuth = ResultBuilder.Unknown;
            Result resultUser = ResultBuilder.Unknown;

            // Try auth check
            ModIOUnity.IsAuthenticated((r) => {
                invokedAuthCallback = new InvokeNullable();
                resultAuth = r;
            });

            // Try get user
            ModIOUnity.GetCurrentUser((r) => {
                invokedUserCallback = new InvokeNullable();
                resultUser = r.result;
            });

            yield return new WaitForInvoke(() => invokedAuthCallback);
            yield return new WaitForInvoke(() => invokedUserCallback);
            
            Assert.True(resultAuth.Succeeded() && resultUser.Succeeded(), 
                $"One or both of the write operations failed."
                + $"\nAuth: {resultAuth.Succeeded()}"
                + $"\nUser: {resultUser.Succeeded()}");

            yield break;
        }
        
        IEnumerator Initialise()
        {
            Debug.Log("<color=orange><b>- CLEANING LAST TEST -</b></color>");
            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);
            TestingUtility.ClearAllData();
            ModIOUnity.SetLoggingDelegate(delegate {});
            Debug.Log("<color=red><b>- SETTING UP NEW TEST -</b></color>");

            InvokeNullable invoked = null;

            // Initialize
            ModIOUnity.InitialiseForUser(
                "TestSuite", TestingUtility.GetTestSettingsForReadingRequests(),
                new BuildSettings(), (r) => { invoked = new InvokeNullable(); });

            yield return new WaitForInvoke(() => invoked);

            Task taskUserCredentials = TestingUtility.SetupUserCredentials();
            yield return new WaitUntil(() => taskUserCredentials.IsCompleted);

            ModIOUnity.SetLoggingDelegate(TestingLogger.LoggingDelegateWithErrors);
        }
    }
}
