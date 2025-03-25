using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using UnityEngine;
using UnityEngine.UI;

using UnityFramework.AD;

public class Rnfwoa : MonoBehaviour
{
    RewardedAdController rewardedAdController = new RewardedAdController();
    RewardedInterstitialAdController rewardedInterstitialAdController = new RewardedInterstitialAdController();
    BannerController bannerController = new BannerController();

    [SerializeField] Button test1;
    [SerializeField] Button test2;
    [SerializeField] Text textTest;

    int reward = 0;


    Coroutine test1Co;
    Coroutine test2Co;

    private void Start()
    {
        RequestConfiguration requestConfiguration = new RequestConfiguration();
        requestConfiguration.TestDeviceIds.Add("192B511C6C1C1B4D392FFB27208F7775");

        MobileAds.SetRequestConfiguration(requestConfiguration);

        rewardedAdController.OnRewarded += OnRewarded;
        rewardedAdController.OnLoaded += (controller) => { controller.Show(); };

        rewardedInterstitialAdController.OnRewarded += OnRewarded;
        rewardedInterstitialAdController.OnLoaded += (controller) => { controller.Show(); };

        test1.onClick.AddListener(() => 
        {
            rewardedAdController.Load(); 
        }
        );
        test2.onClick.AddListener(() => 
        {
            rewardedInterstitialAdController.Load();
        });
        bannerController.Create();
        bannerController.Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            bannerController.Create();
            bannerController.Load();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            bannerController.Show();
        }
    }


    void OnRewarded(Reward reward)
    {
        this.reward += (int)reward.Amount;
        textTest.text = $"{reward.Type} : {this.reward}";
    }


    IEnumerator Wait(IAdController adController)
    {
        while (!adController.IsLoaded())
        {
            yield return null;
        }
        adController.Show();


        test1Co = null;
        test2Co = null;
    }

}
