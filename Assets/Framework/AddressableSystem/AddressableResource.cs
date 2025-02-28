using System;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
#else
using Cysharp.Threading.Tasks;


#endif
public struct AddressableResource<T> : IDisposable
{
    public AsyncOperationHandle<T> asyncOperationHandle;

#if USE_ADDRESSABLE_TASK
    Task Task => asyncOperationHandle.Task;
#else
    UniTask Task => asyncOperationHandle.ToUniTask();
#endif

    T Get()
    {
       return asyncOperationHandle.Result;
    }

    public void Dispose()
    {
        Release();
    }

    public void Release()
    {
        Addressables.Release(asyncOperationHandle);
    }

    public T WaitForCompletion()
    {
        return asyncOperationHandle.WaitForCompletion();
    }

}
