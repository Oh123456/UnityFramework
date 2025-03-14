using System;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityFramework.Addressable.Managing;





#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
#else
using Cysharp.Threading.Tasks;
#endif

/*
 * 1011100 1110101 1100011 110101 110100 110100 1011100 1110101 110000 110000 110010 110000 1011100 1110101 1100010 110010 1100011 111000 1011100 1110101 1100010 1100010 1100110 111000 1011100 1110101 110000 110000 110010 110000 1011100 1110101 1100011 110010 1100100 1100011 1011100 1110101 1100010 1100011 110001 1100011
 */


namespace UnityFramework.Addressable
{

    interface IAddressableReleaseAble
    {
        /// <summary>
        /// Addressable Release
        /// </summary>
        /// <returns> ture is Complete Release</returns>
        bool Release();
    }

    public interface IAddressableResource
    {
#if USE_ADDRESSABLE_TASK
        Task Task { get;}
#else
        UniTask Task { get; }
#endif
        bool IsValid();

        bool GetResource(out object resource);
    }

    public sealed class AddressableResource<T> : IAddressableResource 
    {
        AddressableResourceHandle<T> addressableResourceHandler;

        public AddressableResource(AddressableResourceHandle<T> addressableResourceHandler)
        {
            this.addressableResourceHandler = addressableResourceHandler;
            AddressableManager.Instance.GetAddressableDataManager().OnRelease += Release;
        }

        public AddressableResource(AddressableResourceHandle<T> addressableResourceHandler, AddressableDataManager addressableDataManager)
        {
            this.addressableResourceHandler = addressableResourceHandler;
            addressableDataManager.OnRelease += Release;
        }

#if USE_ADDRESSABLE_TASK
        public Task Task { get => addressableResource.Task; }
#else
        public UniTask Task { get => addressableResourceHandler.Task; }
#endif

        public T GetResource()
        {
            if (!addressableResourceHandler.GetResource(out object resource))
                return default(T);
            return (T)resource;
        }

        public bool GetResource(out object resource)
        {
            return addressableResourceHandler.GetResource(out resource);
        }

        public bool IsValid()
        {
            return addressableResourceHandler.IsValid();    
        }

        public T WaitForCompletion()
        {
            return addressableResourceHandler.WaitForCompletion();
        }

        private bool Release()
        {
            return addressableResourceHandler.Release();
        }
    }

    public struct AddressableResourceHandle<T> : IDisposable, IAddressableResource, IAddressableReleaseAble 
    {

        private AsyncOperationHandle<T> asyncOperationHandle;

        public AddressableResourceHandle(AsyncOperationHandle<T> asyncOperationHandle)
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

        public bool Release()
        {
            if (this.asyncOperationHandle.IsValid())
            {
#if UNITY_EDITOR
                Editor.AddressableManagingDataManager.TrackRelease(this.asyncOperationHandle);
#endif
                AddressableManager.AddressableLog($"{this.asyncOperationHandle.DebugName} Release!!", Color.blue);
                Addressables.Release(this.asyncOperationHandle);
                return !this.asyncOperationHandle.IsValid();
            }

            return true;
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

        public bool IsValid()
        {
            return this.asyncOperationHandle.IsValid(); 
        }
    }

}