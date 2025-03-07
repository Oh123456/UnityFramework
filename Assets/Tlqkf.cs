using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

using UnityFramework.Addressable;

public class Tlqkf : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] string imageKeys;
    [SerializeField] AssetReference assetReference;
    [SerializeField] AssetReference assetReferenceGO;


    AddressableResourceHandle<Texture> addressableResource;
    void Start()
    {
        AddressableManager.Instance.DownLoad();
        AddressableManager.Instance.OnCompletedLoad += () =>
        {
            Debug.Log("다운로드 완료 ");
        };
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            AddressableResource<Texture> addressableResource = AddressableManager.Instance.LoadAsset<Texture>(assetReference);
            image.texture = addressableResource.WaitForCompletion();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddressableResource<Texture> addressableResource = AddressableManager.Instance.LoadAsset<Texture>(imageKeys);
            image.texture = addressableResource.WaitForCompletion();            
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddressableManager.Instance.GetAddressableDataManager().Release();  
        }
    }

}
