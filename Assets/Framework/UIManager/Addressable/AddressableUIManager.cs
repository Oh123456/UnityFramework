using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityFramework.Addressable;
using UnityFramework.UI;
using UnityEngine.AddressableAssets;



#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
#else
using Cysharp.Threading.Tasks;
#endif


namespace UnityFramework.UI
{

    public struct UnmanagedUIResource<T> where T : UIBase
    {
        public AddressableResource<T> addressableResource;
        public T uiObject;
    }

    public struct UnmanagedUIResourceHandle<T> where T : UIBase
    {
        public AddressableResourceHandle<T> addressableResourceHandle;
        public T uiObject;
    }

    public partial class UIManager
    {
        private interface IUIAddressalbeHandle
        {
            T GetResource<T>() where T : UIBase;
            void Release();
        }
        private class UIAddressalbeHandle<T> : IUIAddressalbeHandle where T : UIBase
        {
            readonly AddressableResourceHandle<GameObject> addressableResourceHandle;
            T uiBase;

            public UIAddressalbeHandle(in AddressableResourceHandle<GameObject> addressableResourceHandle)
            {
                this.addressableResourceHandle = addressableResourceHandle;
                uiBase = addressableResourceHandle.GetResource().GetComponent<T>();
            }

            public TUIBase GetResource<TUIBase>() where TUIBase : UIBase
            {
                return uiBase as TUIBase;
            }

            public void Release()
            {
                uiBase = null;
                addressableResourceHandle.Release();
            }
        }


        public delegate void UnmanagedAddressableUICompleted<T>(in UnmanagedUIResource<T> unmanagedAddressableResource) where T : UIBase;

        private Dictionary<string, IUIAddressalbeHandle> unsafeLoads = new Dictionary<string, IUIAddressalbeHandle>();

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
            ExecuteUIContoller(ui);
            showComplete?.Invoke(ui);
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
            ExecuteUIContoller(ui);
            showComplete?.Invoke(ui);
        }

        public async void ShowUnmanagedAddressableUI<T>(object key, UnmanagedAddressableUICompleted<T> completed) where T : UIBase
        {
            if (!CheckType<T>())
                return;

            var unmanagedAddressableResource = new UnmanagedUIResource<T>();
            AddressableResource<T> addressableResource = AddressableManager.Instance.LoadAsset<T>(key);
            await addressableResource.Task;
            unmanagedAddressableResource.addressableResource = addressableResource;
            unmanagedAddressableResource.uiObject = GameObject.Instantiate(addressableResource.GetResource());
            completed(unmanagedAddressableResource);
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

        private void ExecuteUIContoller<T>(T ui) where T : UIBase
        {

            UIController uIController = GetUIController();
            uIController.Initialize(ui);
            uIController.Show();
            showUIStack.Push(uIController);
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
                AddressableResource<GameObject> addressableResource = AddressableManager.Instance.LoadAsset<GameObject>(key);
                await addressableResource.Task;
                GameObject gameObject = addressableResource.GetResource();
                return gameObject.GetComponent<T>();
            }
            else
            {
                string loadKey = (key is IKeyEvaluator evaluator ? evaluator.RuntimeKey : key) as string;
                if (!unsafeLoads.TryGetValue(loadKey, out IUIAddressalbeHandle uiAddressalbeHandle))
                {
                    AddressableResourceHandle<GameObject> addressableResourceHandle = AddressableManager.UnsafeLoadAsset<GameObject>(key);
                    await addressableResourceHandle.Task;
                    uiAddressalbeHandle = new UIAddressalbeHandle<T>(in addressableResourceHandle);
                    if (loadKey != null)
                        unsafeLoads.Add(loadKey, uiAddressalbeHandle);
                }

                return uiAddressalbeHandle.GetResource<T>();
            }
        }

    }

}