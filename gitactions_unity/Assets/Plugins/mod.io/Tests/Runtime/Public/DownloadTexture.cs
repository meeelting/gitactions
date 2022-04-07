using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;
using ModIO.Implementation;
using UnityEngine;
using System.Threading.Tasks;

namespace ModIOTesting.Public
{
    internal class DownloadTexture
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
        public IEnumerator RequestSucceedsForGalleryImage_Original()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.galleryImages_Original[0], r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForGalleryImage_320x180()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.galleryImages_Original[0], r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForLogoImage_Original()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.logoImage_Original, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForLogoImage_320x180()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.logoImage_320x180, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForLogoImage_640x360()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.logoImage_640x360, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForLogoImage_1280x720()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.logoImage_1280x720, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForCreatorAvatar_Original()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.creatorAvatar_Original, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForCreatorAvatar_50x50()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.creatorAvatar_50x50, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForCreatorAvatar_100x100()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.creatorAvatar_100x100, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenNotInitialized()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            InvokeNullable shutdown = null;
            ModIOUnity.Shutdown(() => shutdown = new InvokeNullable());
            yield return new WaitForInvoke(() => shutdown);

            ModIOUnity.DownloadTexture(profile.logoImage_320x180, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsInitialisationError(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForGuestSession()
        {
            TestingUtility.RemoveCredentials();

            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.logoImage_320x180, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator CallbackHasCorrectResultWhenCredentialsAreInvalid()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            ModProfile profile = default;

            ModIOUnity.GetMod(id, r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            TestingUtility.InvalidateCredentials();

            ModIOUnity.DownloadTexture(profile.logoImage_320x180, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.IsAuthenticationError,
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForUserAvatar_Original()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            UserProfile profile = default;

            ModIOUnity.GetCurrentUser(r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.avatar_original, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForUserAvatar_50x50()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            UserProfile profile = default;

            ModIOUnity.GetCurrentUser(r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.avatar_50x50, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }

        [UnityTest]
        public IEnumerator RequestSucceedsForUserAvatar_100x100()
        {
            ModId id = new ModId(891);

            InvokeNullable invoked = null;
            Result result = ResultBuilder.Unknown;
            UserProfile profile = default;

            ModIOUnity.GetCurrentUser(r => {
                invoked = new InvokeNullable();
                profile = r.value;
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            if(!result.Succeeded())
            {
                Assert.That(false, $"Failed with Result code[{result.code}:{result.code_api}]");
            }

            result = ResultBuilder.Unknown;
            invoked = null;

            ModIOUnity.DownloadTexture(profile.avatar_100x100, r => {
                invoked = new InvokeNullable();
                result = r.result;
            });

            yield return new WaitForInvoke(() => invoked);

            Assert.That(result.Succeeded(),
                        $"Failed with Result code[{result.code}:{result.code_api}]");
        }
    }
}
