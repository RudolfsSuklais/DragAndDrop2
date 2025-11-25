using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    private string _adUnitId;

    public bool isReady = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _adUnitId = _androidAdUnitId;
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Interstitial: Tried to load before Unity Ads init!");
            return;
        }

        Debug.Log("Interstitial: Loading...");
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        if (isReady)
        {
            Debug.Log("Interstitial: SHOW");
            Advertisement.Show(_adUnitId, this);
            isReady = false;
        }
        else
        {
            Debug.Log("Interstitial: Not ready, loading again.");
            LoadAd();
        }
    }

    // CALLBACKS ---------------------------------------

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial: LOADED.");
        isReady = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning("Interstitial: FAILED TO LOAD → retry");
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Interstitial: Show Start");
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Interstitial: CLICK");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Interstitial: Completed → reload next");
        Time.timeScale = 1f;
        LoadAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning("Interstitial: Show FAILED → reload");
        Time.timeScale = 1f;
        LoadAd();
    }
}
