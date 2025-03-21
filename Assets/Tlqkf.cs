using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
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
        AddressableManager.Instance.OnAllCompletedLoad += () =>
        {
            Debug.Log("다운로드 완료 ");
        };
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            addressableResource.Release();
            addressableResource = AddressableManager.UnsafeLoadAsset<Texture>(imageKeys);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            dkslaltlqkfdlrpdhodksejhkaghfsdjkghsdfjkghjfdakghadkfghadf();
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadSceneAsync("NewScene");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            var addressableResource = AddressableManager.UnsafeLoadAsset<GameObject>(assetReferenceGO);
            var go = addressableResource.WaitForCompletion();
            Instantiate(go);
        }

    }

    private void dkslaltlqkfdlrpdhodksejhkaghfsdjkghsdfjkghjfdakghadkfghadf()
    {
        AddressableResource<Texture> addressableResource = AddressableManager.Instance.LoadAsset<Texture>(assetReference);
        image.texture = addressableResource.WaitForCompletion();
    }
}
