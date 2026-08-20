using System;
using UnityEngine;

namespace AudienceNetwork
{
	internal class InterstitialAdContainer
	{
		internal AndroidJavaProxy listenerProxy;

		internal AndroidJavaObject bridgedInterstitialAd;

		internal InterstitialAd interstitialAd { get; set; }

		internal FBInterstitialAdBridgeCallback onLoad { get; set; }

		internal FBInterstitialAdBridgeCallback onImpression { get; set; }

		internal FBInterstitialAdBridgeCallback onClick { get; set; }

		internal FBInterstitialAdBridgeErrorCallback onError { get; set; }

		internal FBInterstitialAdBridgeCallback onDidClose { get; set; }

		internal FBInterstitialAdBridgeCallback onWillClose { get; set; }

		internal FBInterstitialAdBridgeCallback onActivityDestroyed { get; set; }

		internal InterstitialAdContainer(InterstitialAd interstitialAd)
		{
			this.interstitialAd = interstitialAd;
		}

		public override string ToString()
		{
			return $"[InterstitialAdContainer: interstitialAd={interstitialAd}, onLoad={onLoad}]";
		}

		public static implicit operator bool(InterstitialAdContainer obj)
		{
			return obj != null;
		}

		internal AndroidJavaObject LoadAdConfig(string bidPayload)
		{
			AndroidJavaObject androidJavaObject = bridgedInterstitialAd.Call<AndroidJavaObject>("buildLoadAdConfig", Array.Empty<object>());
			androidJavaObject.Call<AndroidJavaObject>("withAdListener", new object[1] { listenerProxy });
			if (bidPayload != null)
			{
				androidJavaObject.Call<AndroidJavaObject>("withBid", new object[1] { bidPayload });
			}
			return androidJavaObject.Call<AndroidJavaObject>("build", Array.Empty<object>());
		}

		public void Load()
		{
			Load(null);
		}

		public void Load(string bidPayload)
		{
			AndroidJavaObject androidJavaObject = LoadAdConfig(bidPayload);
			bridgedInterstitialAd.Call("loadAd", androidJavaObject);
		}
	}
}
