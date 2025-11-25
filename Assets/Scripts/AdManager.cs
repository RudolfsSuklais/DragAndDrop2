using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAd = false;
    private bool firstAdShown = false;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAds = false;

    public static AdManager Instance { get; private set; }

    private void Awake()
    {
        if (adsInitializer == null)
            adsInitializer = FindFirstObjectByType<AdsInitializer>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        adsInitializer.OnAdsInitialized += HandleAdsInitialized;
    }

    private void HandleAdsInitialized()
    {
        Debug.Log("[AdManager] Ads initialized!");

        if (!turnOffInterstitialAd)
        {
            if (interstitialAd == null)
                interstitialAd = FindFirstObjectByType<InterstitialAd>();

            if (interstitialAd != null)
            {
               
                interstitialAd.LoadAd();
            }
        }

        if (!turnOffRewardedAds)
        {
            if (rewardedAds == null)
                rewardedAds = FindFirstObjectByType<RewardedAds>();

            if (rewardedAds != null)
                rewardedAds.LoadAd();
        }
    }

  

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private bool firstSceneLoad = false;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[AdManager] Scene loaded: {scene.name}");

        StartCoroutine(ShowInterstitialOnSceneChange());
    }

    private IEnumerator ShowInterstitialOnSceneChange()
    {
        yield return new WaitForSeconds(0.3f); // allow scene to settle

        if (turnOffInterstitialAd)
            yield break;

        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();

        if (interstitialAd == null)
        {
            Debug.LogWarning("[AdManager] InterstitialAd not found!");
            yield break;
        }

        if (interstitialAd.isReady)
        {
            interstitialAd.ShowAd();
        }
        else
        {
            interstitialAd.LoadAd();
        }
    }



}

