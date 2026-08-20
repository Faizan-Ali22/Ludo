using System;
using AssemblyCSharp;
using AudienceNetwork;
using GoogleMobileAds.Api;
using UnityEngine;
//using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour
{
	[HideInInspector]
	public enum DisplayType
	{
		SEQUENCE = 0,
		FALLBACK = 1
	}

	private class FacebookAudienceAdNetwork : adNetwork
	{
		private AudienceNetwork.InterstitialAd interstitialAd;

		private bool isLoaded;

		public override void init()
		{
			DConsole.Log("Init Audience Network");
			interstitialAd = new AudienceNetwork.InterstitialAd(FANAndroid_ID);
			interstitialAd.Register(go);
			interstitialAd.InterstitialAdDidLoad = delegate
			{
				DConsole.Log("Fb Interstitial ad loaded.");
				isLoaded = true;
			};
			interstitialAd.InterstitialAdDidFailWithError = delegate(string error)
			{
				DConsole.Log("Fb Interstitial ad failed to load with error: " + error);
			};
			interstitialAd.InterstitialAdWillLogImpression = delegate
			{
				DConsole.Log("Fb Interstitial ad logged impression.");
			};
			interstitialAd.InterstitialAdDidClick = delegate
			{
				DConsole.Log("Fb Interstitial ad clicked.");
			};
			interstitialAd.interstitialAdDidClose = delegate
			{
				DConsole.Log("Fb Interstitial ad closed.");
				interstitialAd.LoadAd();
			};
			interstitialAd.LoadAd();
		}

		public override void loadAd()
		{
			DConsole.Log("Loading Facebook Audience Network");
			if (isLoaded)
			{
				currentAdIndex = (currentAdIndex + 1) % activeNetworks;
				interstitialAd.Show();
				isLoaded = false;
			}
			else
			{
				interstitialAd.LoadAd();
				loadNextNetwork();
			}
		}

		public override void destroyAd()
		{
			if (interstitialAd != null)
			{
				interstitialAd.Dispose();
			}
		}
	}

	private class AdMobAdNetwork : adNetwork
	{
		private GoogleMobileAds.Api.InterstitialAd interstitialAd;

		private bool isLoaded;

		public override void init()
		{
			DConsole.Log("Init Admob");
			string adMobAndroid_ID = AdMobAndroid_ID;
			interstitialAd = new GoogleMobileAds.Api.InterstitialAd(adMobAndroid_ID);
			interstitialAd.OnAdLoaded += HandleOnAdLoaded;
			interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
			interstitialAd.OnAdClosed += HandleOnAdClosed;
			requestInterstitial();
		}

		public override void loadAd()
		{
			DConsole.Log("Loading AdMob");
			if (interstitialAd.IsLoaded())
			{
				interstitialAd.Show();
				currentAdIndex = (currentAdIndex + 1) % activeNetworks;
			}
			else
			{
				requestInterstitial();
				loadNextNetwork();
			}
		}

		private void requestInterstitial()
		{
			AdRequest request = new AdRequest.Builder().Build();
			interstitialAd.LoadAd(request);
		}

		private void HandleOnAdLoaded(object sender, EventArgs args)
		{
			DConsole.Log("Admob ad loaded");
		}

		private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
			DConsole.Log("Admob ad failed to load " + args.Message);
		}

		private void HandleOnAdClosed(object sender, EventArgs args)
		{
			requestInterstitial();
		}

		public override void destroyAd()
		{
			interstitialAd.Destroy();
		}
	}

	private abstract class adNetwork : IAdNetwork
	{
		public abstract void init();

		public abstract void loadAd();

		public abstract void destroyAd();

		public void loadNextNetwork()
		{
			displayAttempts++;
			if (displayAttempts >= activeNetworks)
			{
				return;
			}
			for (int i = 0; i < networks.Length; i++)
			{
				currentAdIndex = (currentAdIndex + 1) % activeNetworks;
				if (networks[currentAdIndex] != null)
				{
					networks[currentAdIndex].loadAd();
					break;
				}
			}
		}
	}

	private interface IAdNetwork
	{
		void init();

		void loadAd();

		void loadNextNetwork();
	}

	[Header("AD mediation Type")]
	[Header("SEQUENCE - display networks in sequence")]
	[Header("FALLBACK - Try to load network with order 1, if no fill then next network")]
	[Space(10f)]
	public DisplayType displayType;

	[Header("Order of networks. Set 0 to disable network")]
	[Space(10f)]
	[Range(0f, 3f)]
	public int AudienceNetworkOrder = 1;

	[Range(0f, 3f)]
	public int AdmobOrder = 2;

	private static int activeNetworks = 3;

	[Header("Networks IDs Android")]
	[Space(10f)]
	public string FANAndroidID;

	public string AdmobAndroidID;

	[Header("Networks IDs iOS")]
	[Space(10f)]
	public string FAN_IOS_ID;

	public string AdmobIOSID;

	[Header("Show Ads in locations")]
	[Space(10f)]
	public bool ShowAdOnMenuScene;

	[HideInInspector]
	public bool ShowAdOnGameOver;

	[HideInInspector]
	public bool ShowAdOnPause;

	[HideInInspector]
	public bool ShowAdOnLevelFinish;

	public bool ShowAdOnFacebookFriends;

	public bool ShowAdOnGameFinishWindow;

	public bool ShowAdOnStoreWindow;

	public bool ShowAdOnGamePropertiesWindow;

	[Header("Should Show Ad In Menu Scene after game start?")]
	[Space(10f)]
	public bool loadAdInMenuAfterStart = true;

	private int NetworksCount = 3;

	private static GameObject go;

	private static adNetwork[] networks;

	private adNetwork[] networksInit;

	private static int currentAdIndex = 0;

	private static int displayAttempts = 0;

	private int displayCount = 1;

	private static string AdMobAndroid_ID;

	public static string FANAndroid_ID;

	public static string FANIOS_ID;

	private int menuLoadCount;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (UnityEngine.Object.FindObjectsOfType(GetType()).Length > 1)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		GameManager.Instance.adsController = this;
		FANAndroid_ID = FANAndroidID;
		FANIOS_ID = FAN_IOS_ID;
		AdMobAndroid_ID = AdmobAndroidID;
		go = base.gameObject;
		networks = new adNetwork[NetworksCount];
		networksInit = new adNetwork[NetworksCount];
		networksInit[0] = new FacebookAudienceAdNetwork();
		networksInit[1] = new AdMobAdNetwork();
		for (int i = 0; i < networks.Length; i++)
		{
			if (networksInit[i] != null)
			{
				try
				{
					networksInit[i].init();
				}
				catch (Exception)
				{
				}
				networks[i] = networksInit[i];
			}
		}
		parseStringAndSortNetworks(AudienceNetworkOrder + ";" + AdmobOrder);
	}

	public void ShowVideoAd()
	{
		// if (Advertisement.IsReady("rewardedVideo"))
		// {
		// 	ShowOptions showOptions = new ShowOptions
		// 	{
		// 		resultCallback = HandleShowResult
		// 	};
		// 	Advertisement.Show("rewardedVideo", showOptions);
		// }
	}

	/* private void HandleShowResult(int result) {} */

	public void loadAd(AdLocation location)
	{
		if ((location == AdLocation.GameOver && !ShowAdOnGameOver) || (location == AdLocation.GameStart && !ShowAdOnMenuScene) || (location == AdLocation.Pause && !ShowAdOnPause) || (location == AdLocation.LevelComplete && !ShowAdOnLevelFinish) || (location == AdLocation.FacebookFriends && !ShowAdOnFacebookFriends) || (location == AdLocation.GameFinishWindow && !ShowAdOnGameFinishWindow) || (location == AdLocation.StoreWindow && !ShowAdOnStoreWindow) || (location == AdLocation.GamePropertiesWindow && !ShowAdOnGamePropertiesWindow))
		{
			return;
		}
		if (location == AdLocation.GameStart)
		{
			menuLoadCount++;
			if (!loadAdInMenuAfterStart && menuLoadCount < 2)
			{
				DConsole.Log("Skip AD on game start");
				return;
			}
			DConsole.Log("Load AD Game start");
		}
		if (PlayerPrefs.GetInt(StaticStrings.PrefsPlayerRemovedAds) == 0)
		{
			displayAttempts = 0;
			if (displayType == DisplayType.SEQUENCE)
			{
				networks[currentAdIndex].loadAd();
			}
			else if (displayType == DisplayType.FALLBACK)
			{
				currentAdIndex = 0;
				networks[currentAdIndex].loadAd();
			}
			displayCount++;
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < networks.Length; i++)
		{
			if (networks[i] != null)
			{
				networks[i].destroyAd();
			}
		}
		DConsole.Log("InterstitialAdTest was destroyed!");
	}

	public void parseStringAndSortNetworks(string sequence)
	{
		DConsole.Log("Parsing mediation networks");
		try
		{
			string[] array = sequence.ToLower().Split(';');
			if (array.Length != networks.Length)
			{
				return;
			}
			int num = networks.Length - 1;
			for (int i = 0; i < networks.Length; i++)
			{
				if (int.Parse(array[i]) > 0)
				{
					networks[int.Parse(array[i]) - 1] = networksInit[i];
					continue;
				}
				networks[num] = null;
				activeNetworks--;
				num--;
			}
		}
		catch (Exception ex)
		{
			DConsole.Log("Error parsing configuration file:\n" + ex.ToString());
		}
	}
}



