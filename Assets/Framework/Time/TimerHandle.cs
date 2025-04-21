using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace UnityFramework.Timer
{
    public struct TimerHandle
    {
        private readonly CancellationToken cancellationToken;
        private readonly UniTask.Awaiter awaiter;

        public TimerHandle(in UniTask.Awaiter awaiter , in CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.awaiter = awaiter;
        }

        /// <summary>
        /// 해당 타미어가 완료 됬는지
        /// </summary>
        public bool IsCompleted => awaiter.IsCompleted;

        public void Cancel()
        {
            if (!cancellationToken.CanBeCanceled)
                return;
            if (cancellationToken.IsCancellationRequested)
                return;
            

            //cancellationToken.
        }
    }
}
