using GoogleMobileAds.Android;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.iOS;
using UnityEngine;

namespace GoogleMobileAds
{
	public class GoogleMobileAdsClientFactory
	{
		public static IBannerClient BuildBannerClient()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return new GoogleMobileAds.Android.BannerClient();
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return new GoogleMobileAds.iOS.BannerClient();
			}
			return new DummyClient();
		}

		public static IInterstitialClient BuildInterstitialClient()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return new GoogleMobileAds.Android.InterstitialClient();
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return new GoogleMobileAds.iOS.InterstitialClient();
			}
			return new DummyClient();
		}

		public static IRewardBasedVideoAdClient BuildRewardBasedVideoAdClient()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return new GoogleMobileAds.Android.RewardBasedVideoAdClient();
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return new GoogleMobileAds.iOS.RewardBasedVideoAdClient();
			}
			return new DummyClient();
		}

		public static IRewardedAdClient BuildRewardedAdClient()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return new GoogleMobileAds.Android.RewardedAdClient();
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return new GoogleMobileAds.iOS.RewardedAdClient();
			}
			return new RewardedAdDummyClient();
		}

		public static IAdLoaderClient BuildAdLoaderClient(AdLoader adLoader)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return new GoogleMobileAds.Android.AdLoaderClient(adLoader);
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return new GoogleMobileAds.iOS.AdLoaderClient(adLoader);
			}
			return new DummyClient();
		}

		public static IMobileAdsClient MobileAdsInstance()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return GoogleMobileAds.Android.MobileAdsClient.Instance;
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return GoogleMobileAds.iOS.MobileAdsClient.Instance;
			}
			return new DummyClient();
		}
	}
}
