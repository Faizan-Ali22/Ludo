using System;
using UnityEngine;

namespace AudienceNetwork
{
	internal class AdViewBridgeListenerProxy : AndroidJavaProxy
	{
		private AdView adView;

		private readonly AndroidJavaObject bridgedAdView;

		public AdViewBridgeListenerProxy(AdView adView, AndroidJavaObject bridgedAdView)
			: base("com.facebook.ads.AdListener")
		{
			this.adView = adView;
			this.bridgedAdView = bridgedAdView;
		}

		private void onError(AndroidJavaObject ad, AndroidJavaObject error)
		{
			string errorMessage = error.Call<string>("getErrorMessage", Array.Empty<object>());
			if (adView.AdViewDidFailWithError != null)
			{
				adView.ExecuteOnMainThread(delegate
				{
					adView.AdViewDidFailWithError(errorMessage);
				});
			}
		}

		private void onAdLoaded(AndroidJavaObject ad)
		{
			adView.LoadAdFromData();
		}

		private void onAdClicked(AndroidJavaObject ad)
		{
			if (adView.AdViewDidClick != null)
			{
				adView.ExecuteOnMainThread(delegate
				{
					adView.AdViewDidClick();
				});
			}
		}

		private void onLoggingImpression(AndroidJavaObject ad)
		{
			if (adView.AdViewWillLogImpression != null)
			{
				adView.ExecuteOnMainThread(delegate
				{
					adView.AdViewWillLogImpression();
				});
			}
		}
	}
}
