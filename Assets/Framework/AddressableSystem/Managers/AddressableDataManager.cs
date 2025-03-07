using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;


namespace UnityFramework.Addressable.Managing
{
    public class AddressableDataManager
    {
        Dictionary<object, IAddressableResource> loadedResource = new Dictionary<object, IAddressableResource>();

        private event System.Func<bool> OnRelease;

        public AddressableResource<T> LoadResource<T>(object key)
        {
            object assetKey = null;

            if (key is IKeyEvaluator keyEvaluator)
                assetKey = keyEvaluator.RuntimeKey;
            else
                assetKey = key;

            IAddressableResource addressableResource = null;

            // 없으면 추가
            if (!loadedResource.TryGetValue(assetKey, out addressableResource))
            {
                AddressableManager.AddressableLog($"Loaded Addressable resource KeyCode : {assetKey}", Color.yellow);
                var handle = Addressables.LoadAssetAsync<T>(key);
#if UNITY_EDITOR
                Editor.AddressableManagingDataManager.TrackEditorLoad(handle, Editor.AddressableManagingDataManager.LoadType.SafeLoad, key);
#endif
                AddressableResourceHandle<T> addressableResourceHandle = new AddressableResourceHandle<T>(handle);
                OnRelease += addressableResourceHandle.Release;
                addressableResource = new AddressableResource<T>(addressableResourceHandle);
                loadedResource.Add(assetKey, addressableResource);
            }

            return addressableResource as AddressableResource<T>;
        }

        public void Release()
        {
            foreach (var pair in loadedResource)
            {
                IAddressableResource addressableResource = pair.Value;

                bool isReleaseComplete = false;
                while (!isReleaseComplete) 
                {
                   bool? releaseComplete = OnRelease?.Invoke();
                   isReleaseComplete = releaseComplete == null ? true : releaseComplete.Value;
                }
                
            }
            AddressableManager.AddressableLog($"LoadedResource Release!!", Color.blue);
            loadedResource.Clear();
        }
    }
}