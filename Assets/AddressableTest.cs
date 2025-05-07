using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityFramework.Addressable;

public class AddressableTest : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] string imageKeys;
    [SerializeField] AssetReference assetReference;
    [SerializeField] AssetReference assetReferenceGO;
    [SerializeField] AssetReference assetScene;
    [SerializeField] Button button;

    AddressableResourceHandle<Texture> addressableResource;
    void Start()
    {
        button.onClick.AddListener(ButtonClick);
        button.interactable =false;
        Load();
    }

    async void Load()
    {
        long size = await AddressableManager.Instance.CheckDownLoadBundle();
        AddressableManager.Instance.OnAllCompletedLoad += () =>
        {
            button.interactable = true;
            Debug.Log("다운로드 완료 ");
        };
        AddressableManager.Instance.DownLoadBundle();   
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
            LoadAsetReference();
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

        if (Input.GetKeyDown(KeyCode.I))
        {
            LoadScene();
        }

    }

    void ButtonClick()
    {
        button.interactable = false;
        _ = LoadScene();
    }

    async UniTask LoadScene()
    {
        AddressableManager.Instance.LoadScene(assetScene, LoadSceneMode.Single);
        Debug.Log("TT");
    }


    private void LoadAsetReference()
    {
        

        AddressableResource<Texture> addressableResource = AddressableManager.Instance.LoadAsset<Texture>(assetReference);
        image.texture = addressableResource.WaitForCompletion();
    }
}
