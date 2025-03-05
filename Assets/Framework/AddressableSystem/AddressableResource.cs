using System;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;



#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
#else
using Cysharp.Threading.Tasks;
#endif
namespace UnityFramework.Addressable
{
    interface IAddressableResource
    {
#if USE_ADDRESSABLE_TASK
    Task Task { get;}
#else
        UniTask Task { get; }
#endif
        void Release();

        bool GetResource(out object resource);
    }

    public struct AddressableResource<T> : IDisposable, IAddressableResource
    {
        private AsyncOperationHandle<T> asyncOperationHandle;

        public AddressableResource(AsyncOperationHandle<T> asyncOperationHandle)
        {
            this.asyncOperationHandle = asyncOperationHandle;
        }

#if USE_ADDRESSABLE_TASK
    public Task Task { get => this.asyncOperationHandle.Task;}
#else
        public UniTask Task { get => this.asyncOperationHandle.ToUniTask(); }
#endif

        public T GetResource()
        {
            if (!GetResource(out object resource))
                return default(T);
            return (T)resource;
        }

        public bool GetResource(out object resource)
        {

            if (this.asyncOperationHandle.IsValid()
              && this.asyncOperationHandle.IsDone)
            {
                resource = this.asyncOperationHandle.Result;
                return true;
            }

            AddressableManager.AddressableLog("UnLoad Asset", Color.yellow);
            resource = null;
            return false;
        }

        public void Dispose()
        {
            Release();
        }

        public void Release()
        {
            if (this.asyncOperationHandle.IsValid())
                Addressables.Release(this.asyncOperationHandle);
        }

        public T WaitForCompletion()
        {
            if (!this.asyncOperationHandle.IsValid())
            {
                AddressableManager.AddressableLog("UnLoad Asset", Color.yellow);
                return default(T);
            }
            return this.asyncOperationHandle.WaitForCompletion();
        }

    }

}