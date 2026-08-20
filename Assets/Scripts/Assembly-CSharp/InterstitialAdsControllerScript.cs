using System;
using AssemblyCSharp;
using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialAdsControllerScript : MonoBehaviour
{
	private InterstitialAd interstitial;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		GameManager.Instance.interstitialAds = this;
		RequestInterstitial();
	}

	private void RequestInterstitial()
	{
		string adMobAndroidID = StaticStrings.adMobAndroidID;
		interstitial = new InterstitialAd(adMobAndroidID);
		AdRequest request = new AdRequest.Builder().Build();
		interstitial.LoadAd(request);
		interstitial.OnAdLoaded += HandleOnAdLoaded;
		interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		interstitial.OnAdOpening += HandleOnAdOpened;
		interstitial.OnAdClosed += HandleOnAdClosed;
	}

	public void HandleOnAdLoaded(object sender, EventArgs args)
	{
		MonoBehaviour.print("OnAdLoaded event received.");
	}

	public void HandleOnAdOpened(object sender, EventArgs args)
	{
		MonoBehaviour.print("OnAdOpened event received.");
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		MonoBehaviour.print("OnAdClosed event received.");
		RequestInterstitial();
	}

	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print("Interstitial Failed to load: " + args.Message);
	}

	public void showInterstitial()
	{
		if (interstitial.IsLoaded())
		{
			interstitial.Show();
		}
	}
}
