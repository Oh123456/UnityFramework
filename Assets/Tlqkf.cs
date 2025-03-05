using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

using UnityFramework.Addressable;

public class Tlqkf : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] AssetReference assetReference;
    [SerializeField] AssetReference assetReferenceGO;


    AddressableResource<Texture> addressableResource;
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
            addressableResource.Release();
            addressableResource = AddressableManager.UnsafeLoadAsset<Texture>(assetReference);
            image.texture = addressableResource.WaitForCompletion();
        }
        if (Input.GetKeyDown(KeyCode.B))
            Instantiate( Addressables.LoadAssetAsync<GameObject>(assetReferenceGO).WaitForCompletion());
    }

}
