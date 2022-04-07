using System;
using UnityEngine;

namespace ModIOTesting
{
    /// <summary>
    /// A CustomYieldInstruction that waits for a Func<bool> to return true. Has a maximum wait time
    /// of 5 seconds before finishing (give or take a few milliseconds based on framerate)
    /// </summary>
    public class WaitForInvoke : CustomYieldInstruction
    {
        static float MaximumTimeAllowedToWaitForInvoke = 10f;
        private float timeWaited;
        private Func<InvokeNullable> Predicate;

        public override bool keepWaiting
        {
            get {
                timeWaited += Time.deltaTime;
                if(timeWaited >= MaximumTimeAllowedToWaitForInvoke)
                {
                    return false;
                }
                return Predicate() == null;
            }
        }

        internal WaitForInvoke(Func<InvokeNullable> predicate)
        {
            Predicate = predicate;
        }
    }
}
