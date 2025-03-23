using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFramework.Singleton;

namespace UnityFramework.Coroutine
{

    public class CoroutineManager : LazySingleton<CoroutineManager>
    {
#if !PREALLOC_YIELD_OBJECTS
        WaitForEndOfFrame waitForEndOfFrame = null;
        WaitForFixedUpdate waitForFixedUpdate = null;
#else
        readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
#endif
        // ���� �� �Ŵ����� ����ϴ� �ָ����̱⿡ �ٷ� �����Ѵ�
        Dictionary<float, WaitForSeconds> waitForSecondDictionary = new Dictionary<float, WaitForSeconds>();
        //�̰Ŵ� ���� ���� ���� ��� ���ϱ⿡ Lazy�� �ʵ� �޸𸮸� ��´�
        System.Lazy<Dictionary<float, WaitForSecondsRealtime>> waitForSecondsRealtimeDictionary = new System.Lazy<Dictionary<float, WaitForSecondsRealtime>>(() => new Dictionary<float, WaitForSecondsRealtime>());

        public WaitForEndOfFrame WaitForEndOfFrame
        {
            get
            {
                CreateWaitForEndOfFrame();
                return waitForEndOfFrame;
            }
        }

        public WaitForFixedUpdate WaitForFixedUpdate
        {
            get
            {
                CreateWaitForFixedUpdate();
                return waitForFixedUpdate;
            }
        }

        /// <summary>
        /// ����� WaitForSeconds �� �����ɴϴ�.
        /// </summary>        
        public void WaitForSecond(float time, out WaitForSeconds waitForSeconds)
        {
            if (!waitForSecondDictionary.TryGetValue(time, out waitForSeconds))
            {
                waitForSeconds = new WaitForSeconds(time);
                waitForSecondDictionary.Add(time, waitForSeconds);
            }
        }

        /// <summary>
        /// ����� WaitForSecondsRealtime �� �����ɴϴ�.
        /// </summary>
        public void WaitForSecondsRealtime(float time, out WaitForSecondsRealtime waitForSecondsRealtime)
        {
            if (!waitForSecondsRealtimeDictionary.Value.TryGetValue(time, out waitForSecondsRealtime))
            {
                waitForSecondsRealtime = new WaitForSecondsRealtime(time);
                waitForSecondsRealtimeDictionary.Value.Add(time, waitForSecondsRealtime);
            }
        }

        public void GetWaitForEndOfFrame(out WaitForEndOfFrame waitForEndOfFrame)
        {
            CreateWaitForEndOfFrame();
            waitForEndOfFrame = this.waitForEndOfFrame;
        }

        public void GetWaitForFixedUpdate(WaitForFixedUpdate waitForFixedUpdate)
        {
            CreateWaitForFixedUpdate();
            waitForFixedUpdate = this.waitForFixedUpdate;

        }

        [System.Diagnostics.Conditional("PREALLOC_YIELD_OBJECTS")]
        private void CreateWaitForFixedUpdate()
        {
#if !PREALLOC_YIELD_OBJECTS
            if (waitForFixedUpdate == null)
                waitForFixedUpdate = new WaitForFixedUpdate();
#endif
        }

        [System.Diagnostics.Conditional("PREALLOC_YIELD_OBJECTS")]
        private void CreateWaitForEndOfFrame()
        {
#if !PREALLOC_YIELD_OBJECTS
            if (waitForEndOfFrame == null)
                waitForEndOfFrame = new WaitForEndOfFrame();
#endif
        }

    }

}