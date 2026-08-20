using AudienceNetwork;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterstitialAdScene : BaseScene
{
	private InterstitialAd interstitialAd;

	private bool isLoaded;

	private bool didClose;

	public Text statusLabel;

	private void Awake()
	{
		AudienceNetworkAds.Initialize();
		SettingsScene.InitializeSettings();
	}

	public void LoadInterstitial()
	{
		statusLabel.text = "Loading interstitial ad...";
		interstitialAd = new InterstitialAd("YOUR_PLACEMENT_ID");
		interstitialAd.Register(base.gameObject);
		interstitialAd.InterstitialAdDidLoad = delegate
		{
			DConsole.Log("Interstitial ad loaded.");
			isLoaded = true;
			didClose = false;
			string text = (interstitialAd.IsValid() ? "valid" : "invalid");
			statusLabel.text = "Ad loaded and is " + text + ". Click show to present!";
		};
		interstitialAd.InterstitialAdDidFailWithError = delegate(string error)
		{
			DConsole.Log("Interstitial ad failed to load with error: " + error);
			statusLabel.text = "Interstitial ad failed to load. Check console for details.";
		};
		interstitialAd.InterstitialAdWillLogImpression = delegate
		{
			DConsole.Log("Interstitial ad logged impression.");
		};
		interstitialAd.InterstitialAdDidClick = delegate
		{
			DConsole.Log("Interstitial ad clicked.");
		};
		interstitialAd.InterstitialAdDidClose = delegate
		{
			DConsole.Log("Interstitial ad did close.");
			didClose = true;
			if (interstitialAd != null)
			{
				interstitialAd.Dispose();
			}
		};
		interstitialAd.interstitialAdActivityDestroyed = delegate
		{
			if (!didClose)
			{
				DConsole.Log("Interstitial activity destroyed without being closed first.");
				DConsole.Log("Game should resume.");
			}
		};
		interstitialAd.LoadAd();
	}

	public void ShowInterstitial()
	{
		if (isLoaded)
		{
			interstitialAd.Show();
			isLoaded = false;
			statusLabel.text = "";
		}
		else
		{
			statusLabel.text = "Ad not loaded. Click load to request an ad.";
		}
	}

	private void OnDestroy()
	{
		if (interstitialAd != null)
		{
			interstitialAd.Dispose();
		}
		DConsole.Log("InterstitialAdTest was destroyed!");
	}

	public void NextScene()
	{
		SceneManager.LoadScene("AdViewScene");
	}
}
