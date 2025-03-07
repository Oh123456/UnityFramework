using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityFramework.Addressable.Managing;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;
#endif


#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
#else
using Cysharp.Threading.Tasks;
#endif

namespace UnityFramework.Addressable
{
    public partial class AddressableManager
	{
        public event System.Action<float> OnLoadScenePercent;
        public event System.Action<SceneInstance> OnSceneLoadCompleted;

        System.Lazy<AddressableDataManager> addressableDataManager = new System.Lazy<AddressableDataManager>(() => new AddressableDataManager());

        public static AddressableResourceHandle<T> UnsafeLoadAsset<T>(object key)
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
#if UNITY_EDITOR
            Editor.AddressableManagingDataManager.TrackEditorLoad(handle, Editor.AddressableManagingDataManager.LoadType.UnsafeLoad, key);
#endif
            return new AddressableResourceHandle<T>(handle);
        }

        public AddressableResource<T> LoadAsset<T>(object key)
        {
            return this.addressableDataManager.Value.LoadResource<T>(key);  
        }

        public async void LoadScene(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            AsyncOperationHandle<SceneInstance> asyncOperationHandle = Addressables.LoadSceneAsync(sceneName, loadSceneMode);

            while (!asyncOperationHandle.IsDone)
            {
                this.OnLoadScenePercent?.Invoke(asyncOperationHandle.PercentComplete);

                #if USE_ADDRESSABLE_TASK
                await Task.Yield();
                #else
                await UniTask.Yield();
                #endif
            }

            this.OnSceneLoadCompleted?.Invoke(asyncOperationHandle.Result);
            this.OnSceneLoadCompleted = null;
            this.OnLoadScenePercent = null;

            Addressables.Release(asyncOperationHandle);
        }


        public AddressableDataManager GetAddressableDataManager()
        {
            return this.addressableDataManager.Value;
        }


    } 
}
