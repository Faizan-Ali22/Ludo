using AudienceNetwork;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardedVideoAdScene : BaseScene
{
	private RewardedVideoAd rewardedVideoAd;

	private bool isLoaded;

	private bool didClose;

	public Text statusLabel;

	private void Awake()
	{
		AudienceNetworkAds.Initialize();
		SettingsScene.InitializeSettings();
	}

	public void LoadRewardedVideo()
	{
		statusLabel.text = "Loading rewardedVideo ad...";
		rewardedVideoAd = new RewardedVideoAd("YOUR_PLACEMENT_ID");
		RewardData rewardData = new RewardData
		{
			UserId = "USER_ID",
			Currency = "REWARD_ID"
		};
		new RewardedVideoAd("YOUR_PLACEMENT_ID", rewardData);
		rewardedVideoAd.Register(base.gameObject);
		rewardedVideoAd.RewardedVideoAdDidLoad = delegate
		{
			DConsole.Log("RewardedVideo ad loaded.");
			isLoaded = true;
			didClose = false;
			string text = (rewardedVideoAd.IsValid() ? "valid" : "invalid");
			statusLabel.text = "Ad loaded and is " + text + ". Click show to present!";
		};
		rewardedVideoAd.RewardedVideoAdDidFailWithError = delegate(string error)
		{
			DConsole.Log("RewardedVideo ad failed to load with error: " + error);
			statusLabel.text = "RewardedVideo ad failed to load. Check console for details.";
		};
		rewardedVideoAd.RewardedVideoAdWillLogImpression = delegate
		{
			DConsole.Log("RewardedVideo ad logged impression.");
		};
		rewardedVideoAd.RewardedVideoAdDidClick = delegate
		{
			DConsole.Log("RewardedVideo ad clicked.");
		};
		rewardedVideoAd.RewardedVideoAdDidSucceed = delegate
		{
			DConsole.Log("Rewarded video ad validated by server");
		};
		rewardedVideoAd.RewardedVideoAdDidFail = delegate
		{
			DConsole.Log("Rewarded video ad not validated, or no response from server");
		};
		rewardedVideoAd.RewardedVideoAdDidClose = delegate
		{
			DConsole.Log("Rewarded video ad did close.");
			didClose = true;
			if (rewardedVideoAd != null)
			{
				rewardedVideoAd.Dispose();
			}
		};
		rewardedVideoAd.RewardedVideoAdActivityDestroyed = delegate
		{
			if (!didClose)
			{
				DConsole.Log("Rewarded video activity destroyed without being closed first.");
				DConsole.Log("Game should resume. User should not get a reward.");
			}
		};
		rewardedVideoAd.LoadAd();
	}

	public void ShowRewardedVideo()
	{
		if (isLoaded)
		{
			rewardedVideoAd.Show();
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
		if (rewardedVideoAd != null)
		{
			rewardedVideoAd.Dispose();
		}
		DConsole.Log("RewardedVideoAdTest was destroyed!");
	}

	public void NextScene()
	{
		SceneManager.LoadScene("InterstitialAdScene");
	}
}
