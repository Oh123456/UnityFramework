using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityFramework.Addressable;
using UnityFramework.UI;


#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
#else
using Cysharp.Threading.Tasks;
#endif


namespace UnityFramework.UI
{
    public partial class UIManager
    {
        enum LoadType
        {
            Safe,
            UnSafe,
        }

        public async void ShowAddressableSceneUI<T>(object key, System.Action<T> showComplete = null) where T : UIBase
        {
            if (!CheckType<T>())
            {
                showComplete?.Invoke(null);
                return;
            }

            T ui = await GetCachedAddressableUI<T>(key, LoadType.Safe);
            ExecuteUIContoller(ui, showComplete);
            return;
        }

        public async void ShowAddressableUI<T>(object key, System.Action<T> showComplete = null) where T : UIBase
        {
            if (!CheckType<T>())
            {
                showComplete?.Invoke(null);
                return;
            }

            T ui = await GetCachedAddressableUI<T>(key, LoadType.UnSafe);
            ExecuteUIContoller(ui, showComplete);
        }


#if USE_ADDRESSABLE_TASK
        private async Task<T>
#else
        private async UniTask<T>
#endif
        GetCachedAddressableUI<T>(object key, LoadType loadType) where T : UIBase
        {
            T ui = null;
            if (!TryGetCachedUI(out ui))
            {
                T prb = await GetPrefab<T>(key, loadType);
                ui = GameObject.Instantiate<T>(prb);
                uis[typeof(T)] = ui;
            }

            return ui;
        }

        private void ExecuteUIContoller<T>(T ui, System.Action<T> showComplete) where T : UIBase
        {

            UIController uIController = GetUIController();
            uIController.Initialize(ui);
            uIController.Show();
            showUIStack.Push(uIController);
            showComplete?.Invoke(ui);
        }

#if USE_ADDRESSABLE_TASK
        private async Task<T>
#else
        private async UniTask<T>
#endif
        GetPrefab<T>(object key, LoadType loadType) where T : UIBase
        {
            if (loadType == LoadType.Safe)
            {
                AddressableResource<T> addressableResource = AddressableManager.Instance.LoadAsset<T>(key);
                await addressableResource.Task;
                return addressableResource.GetResource();
            }
            else
            {
                AddressableResourceHandle<T> addressableResource = AddressableManager.UnsafeLoadAsset<T>(key);
                await addressableResource.Task;
                return addressableResource.GetResource();
            }
        }

    }

}