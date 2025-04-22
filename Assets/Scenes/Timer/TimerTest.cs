using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using UnityFramework.Timer;

public class TimerTest : MonoBehaviour
{
    [SerializeField]
    float _timeScale = 1.0f;
    public float timeScale {  set { _timeScale = Mathf.Clamp(value, 0.1f, 2.0f); Time.timeScale = _timeScale;  } }



    TimerHandle timerHandle;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = _timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Test();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("시작");
            TimerManager.Instance.SetTimer(2.0f, out timerHandle ,() =>
            {
                Debug.Log("asdf");
            });
            Debug.Log("끝남");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            timerHandle.Cancel();
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("시작");
            TimerManager.Instance.SetCoroutineTimer(this,2.0f,() =>
            {
                Debug.Log("코루틴");
            });
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("시작");
            TimerManager.Instance.SetCoroutineTimer(this, 2.0f, out timerHandle ,() =>
            {
                Debug.Log("코루틴");
            });
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("시작");
            TimerManager.Instance.SetCoroutineTimer(this, 2.0f, () =>
            {
                Debug.Log("코루틴");
            }, true);
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
            timeScale = _timeScale + 0.1f;
        if (Input.GetKeyDown(KeyCode.PageDown))
            timeScale = _timeScale - 0.1f;
    }


    void Test()
    {
        Debug.Log("시작");
        TimerManager.Instance.SetTimer(2.0f, () =>
        {
            Debug.Log("asdf");
        });
        Debug.Log("끝남");
    }
}
