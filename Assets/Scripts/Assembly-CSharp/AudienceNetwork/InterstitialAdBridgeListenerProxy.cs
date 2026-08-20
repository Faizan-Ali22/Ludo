using System;
using UnityEngine;

namespace AudienceNetwork
{
	internal class InterstitialAdBridgeListenerProxy : AndroidJavaProxy
	{
		private InterstitialAd interstitialAd;

		private readonly AndroidJavaObject bridgedInterstitialAd;

		public InterstitialAdBridgeListenerProxy(InterstitialAd interstitialAd, AndroidJavaObject bridgedInterstitialAd)
			: base("com.facebook.ads.InterstitialAdExtendedListener")
		{
			this.interstitialAd = interstitialAd;
			this.bridgedInterstitialAd = bridgedInterstitialAd;
		}

		private void onError(AndroidJavaObject ad, AndroidJavaObject error)
		{
			string errorMessage = error.Call<string>("getErrorMessage", Array.Empty<object>());
			if (interstitialAd.InterstitialAdDidFailWithError != null)
			{
				interstitialAd.ExecuteOnMainThread(delegate
				{
					interstitialAd.InterstitialAdDidFailWithError(errorMessage);
				});
			}
		}

		private void onAdLoaded(AndroidJavaObject ad)
		{
			interstitialAd.LoadAdFromData();
		}

		private void onAdClicked(AndroidJavaObject ad)
		{
			if (interstitialAd.InterstitialAdDidClick != null)
			{
				interstitialAd.ExecuteOnMainThread(delegate
				{
					interstitialAd.InterstitialAdDidClick();
				});
			}
		}

		private void onInterstitialDisplayed(AndroidJavaObject ad)
		{
		}

		private void onInterstitialDismissed(AndroidJavaObject ad)
		{
			if (interstitialAd.InterstitialAdDidClose != null)
			{
				interstitialAd.ExecuteOnMainThread(delegate
				{
					interstitialAd.InterstitialAdDidClose();
				});
			}
		}

		private void onLoggingImpression(AndroidJavaObject ad)
		{
			if (interstitialAd.InterstitialAdWillLogImpression != null)
			{
				interstitialAd.ExecuteOnMainThread(delegate
				{
					interstitialAd.InterstitialAdWillLogImpression();
				});
			}
		}

		private void onInterstitialActivityDestroyed()
		{
			if (interstitialAd.InterstitialAdActivityDestroyed != null)
			{
				interstitialAd.ExecuteOnMainThread(delegate
				{
					interstitialAd.InterstitialAdActivityDestroyed();
				});
			}
		}
	}
}
