using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public partial class AddressableManager
{
    public static AddressableResource<T> UnsafeLoadAsset<T>(object key)
    {
        return new AddressableResource<T>()
        {
            asyncOperationHandle = Addressables.LoadAssetAsync<T>(key) 
        };
    }
}
