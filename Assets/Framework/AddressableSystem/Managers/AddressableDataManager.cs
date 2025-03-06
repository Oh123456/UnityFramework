using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;


namespace UnityFramework.Addressable.Managing
{
    public class AddressableDataManager
    {
        Dictionary<object, IAddressableResource> loadedResource = new Dictionary<object, IAddressableResource>();

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
                addressableResource = new AddressableResource<T>(new AddressableResourceHandle<T>(Addressables.LoadAssetAsync<T>(key)));
                loadedResource.Add(assetKey, addressableResource);
            }

            return addressableResource as AddressableResource<T>;
        }

        public void Release()
        {
            foreach (var pair in loadedResource)
            {
                IAddressableResource addressableResource = pair.Value;
                if (!addressableResource.IsValid())
                    continue;

                if (addressableResource is IAddressableReleaseAble releaseAble)
                {
                    releaseAble.Release();
                    continue;
                }

                AddressableManager.AddressableLog($"{pair.Key} Not Relese");
            }

            loadedResource.Clear(); 
        }
    } 
}