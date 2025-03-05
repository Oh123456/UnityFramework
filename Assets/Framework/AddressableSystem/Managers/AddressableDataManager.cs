using System;
using System.Collections;
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
            if (key is AssetReference assetReference)            
                assetKey = assetReference.RuntimeKey;
            else
                assetKey = key;

            IAddressableResource addressableResource = default(IAddressableResource);

            // 없으면 추가
            if (!loadedResource.TryGetValue(assetKey, out addressableResource))
            {
                addressableResource = new AddressableResource<T>(Addressables.LoadAssetAsync<T>(key));
                loadedResource.Add(assetKey, addressableResource);
            }

            return (AddressableResource<T>)addressableResource;
        }


        public void Release()
        {
            foreach (var pair in loadedResource)
            {
                pair.Value.Release();
            }

            loadedResource.Clear(); 
        }
    } 
}