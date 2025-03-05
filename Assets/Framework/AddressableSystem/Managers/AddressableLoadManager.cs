using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityFramework.Addressable
{
	public partial class AddressableManager
	{
        public static AddressableResource<T> UnsafeLoadAsset<T>(object key)
        {
            return new AddressableResource<T>(Addressables.LoadAssetAsync<T>(key));
        }


        public AddressableResource<T> LoadAsset<T>(object key)
        {
            return addressableDataManager.Value.LoadResource<T>(key);  
        }

    } 
}
