using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using ModIO;

namespace ModIOTesting.Public
{
    internal class SetLoggingDelegate
    {
        [UnityTest]
        public IEnumerator DoesLoggingDelegateGetInvokedForError()
        {
            InvokeNullable invoked = null;

            ModIOUnity.SetLoggingDelegate((l, m) => { invoked = new InvokeNullable(); });

            ModIO.Implementation.Logger.Log(LogLevel.Error, "Testing");

            yield return new WaitForInvoke(() => invoked);

            Assert.That(invoked != null);
        }

        [UnityTest]
        public IEnumerator DoesLoggingDelegateGetInvokedForWarning()
        {
            InvokeNullable invoked = null;

            ModIOUnity.SetLoggingDelegate((l, m) => { invoked = new InvokeNullable(); });

            ModIO.Implementation.Logger.Log(LogLevel.Warning, "Testing");

            yield return new WaitForInvoke(() => invoked);

            Assert.That(invoked != null);
        }

        [UnityTest]
        public IEnumerator DoesLoggingDelegateGetInvokedForMessage()
        {
            InvokeNullable invoked = null;

            ModIOUnity.SetLoggingDelegate((l, m) => { invoked = new InvokeNullable(); });

            ModIO.Implementation.Logger.Log(LogLevel.Message, "Testing");

            yield return new WaitForInvoke(() => invoked);

            Assert.That(invoked != null);
        }

        [UnityTest]
        public IEnumerator DoesLoggingDelegateGetInvokedForNone()
        {
            InvokeNullable invoked = null;

            ModIOUnity.SetLoggingDelegate((l, m) => { invoked = new InvokeNullable(); });

            ModIO.Implementation.Logger.Log(LogLevel.None, "Testing");

            yield return new WaitForInvoke(() => invoked);

            Assert.That(invoked != null);
        }

        [UnityTest]
        public IEnumerator DoesLoggingDelegateGetInvokedForVerbose()
        {
            InvokeNullable invoked = null;

            ModIOUnity.SetLoggingDelegate((l, m) => { invoked = new InvokeNullable(); });

            ModIO.Implementation.Logger.Log(LogLevel.Verbose, "Testing");

            yield return new WaitForInvoke(() => invoked);

            Assert.That(invoked != null);
        }
    }
}
