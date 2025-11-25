using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidAdUnitId = "Banner_Android";

    void Start()
    {
        // Baneris tiek ielādēts un parādīts automātiski
        LoadBanner();
    }

    public void LoadBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(_androidAdUnitId, options);
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded.");
        Advertisement.Banner.Show(_androidAdUnitId);
    }

    void OnBannerError(string message)
    {
        Debug.LogWarning("Banner load failed: " + message);
        Invoke(nameof(LoadBanner), 3f); // mēģina atkārtoti pēc 3s
    }
}
