using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using UnityFramework.Singleton;

namespace UnityFramework.Timer
{
    using Extensions;

    public class TimerManager : LazySingleton<TimerManager>
    {
        /// <summary>
        /// UniTask 기반 Timer
        /// </summary>
        /// <param name="time"> 1.0 == 1초  </param>
        /// <param name="ignoreTimeScale"> 타임 스케일 무시여부 </param>
        /// <param name="delayTiming"> 딜레이타이밍 </param>
        /// <param name="cancelImmediately">캔슬후 바로 반환 될건지 </param>
        public void SetTimer(float time, out TimerHandle timerHandle, System.Action callback ,bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, bool cancelImmediately = false)
        {
            CancellationToken cancellationToken = new CancellationToken();

            UniTask uniTask = UniTask.Delay((int)(time * 1000), ignoreTimeScale, delayTiming, cancellationToken, cancelImmediately);
            UniTask.Awaiter awaiter = uniTask.GetAwaiter();

            timerHandle = new TimerHandle(in awaiter, in cancellationToken);
            _= uniTask.ContinueWith(cancellationToken,callback);            
        }

    }

    namespace Extensions
    {
        public static class TimerUniTaskExtensions
        {
            public static async UniTask ContinueWith(this UniTask uniTask, CancellationToken cancellationToken, System.Action continuationFunction)
            {
                await uniTask;
                if (cancellationToken.IsCancellationRequested)
                    return;
                continuationFunction();
            }
        } 
    }

}