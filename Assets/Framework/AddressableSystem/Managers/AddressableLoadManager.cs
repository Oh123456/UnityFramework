using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;


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

        public static AddressableResourceHandle<T> UnsafeLoadAsset<T>(object key)
        {
            return new AddressableResourceHandle<T>(Addressables.LoadAssetAsync<T>(key));
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

    } 
}
