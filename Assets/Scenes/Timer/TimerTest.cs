using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class TimerTest : MonoBehaviour
{
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    // Start is called before the first frame update
    void Start()
    {
        
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
            cancellationTokenSource.Cancel();
        }

        Debug.Log(cancellationTokenSource.IsCancellationRequested);
    }


    async void Test()
    {
        Debug.Log("시작");
        await UniTask.Delay(5000, cancellationToken: cancellationTokenSource.Token, cancelImmediately: false);
        Debug.Log("끝남");
    }
}
