using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    string _adUnitId;

    private UIManager uiManager;
    private Button _rewardedAdButton;

    void Awake()
    {
        _adUnitId = _androidAdUnitId;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Meklē UIManager jaunajā scenē
        uiManager = FindObjectOfType<UIManager>();

        // Meklē pogu pēc taga
        GameObject btnObj = GameObject.FindGameObjectWithTag("RewardedButton");
        if (btnObj != null)
        {
            Button b = btnObj.GetComponent<Button>();
            SetButton(b);
            Debug.Log("[RewardedAds] Rewarded button assigned.");
        }
        else
        {
            Debug.LogWarning("[RewardedAds] No rewarded ad button found in this scene.");
        }

        LoadAd();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
            return;

        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (_rewardedAdButton != null)
            _rewardedAdButton.interactable = true;

        Debug.Log("[RewardedAds] Ad loaded.");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning("Rewarded ad failed to load.");
        StartCoroutine(WaitAndLoad(3f));
    }

    public void ShowAd()
    {
        if (_rewardedAdButton != null)
            _rewardedAdButton.interactable = false;

        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("[RewardedAds] Ad clicked.");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning("Rewarded ad failed to show.");
        StartCoroutine(WaitAndLoad(3f));
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Time.timeScale = 1f;

        DestroyAllObstacles();

        // WIN SCREEN
        if (uiManager != null)
        {
            uiManager.ShowWinScreenWithStars(
                3,  // stars
                12, // placed vehicles
                12, // total vehicles
                -1  // ignore time
            );
        }

        StartCoroutine(WaitAndLoad(5f));
    }

    private IEnumerator WaitAndLoad(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadAd();
    }

    public void SetButton(Button button)
    {
        _rewardedAdButton = button;
        _rewardedAdButton.onClick.RemoveAllListeners();
        _rewardedAdButton.onClick.AddListener(ShowAd);
        _rewardedAdButton.interactable = false;
    }

    private void DestroyAllObstacles()
    {
        var obstacles = FindObjectsOfType<ObstaclesControllerScript>();
        foreach (var o in obstacles)
        {
            if (o != null)
                Destroy(o.gameObject);
        }

        Debug.Log("[RewardedAds] All obstacles destroyed.");
    }
}
