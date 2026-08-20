using System;
using UnityEngine;

namespace AudienceNetwork
{
	internal class AdViewContainer
	{
		internal AndroidJavaProxy listenerProxy;

		internal AndroidJavaObject bridgedAdView;

		internal AdView adView { get; set; }

		internal FBAdViewBridgeCallback onLoad { get; set; }

		internal FBAdViewBridgeCallback onImpression { get; set; }

		internal FBAdViewBridgeCallback onClick { get; set; }

		internal FBAdViewBridgeErrorCallback onError { get; set; }

		internal FBAdViewBridgeCallback onFinishedClick { get; set; }

		internal AdViewContainer(AdView adView)
		{
			this.adView = adView;
		}

		public override string ToString()
		{
			return $"[AdViewContainer: adView={adView}, onLoad={onLoad}]";
		}

		public static implicit operator bool(AdViewContainer obj)
		{
			return obj != null;
		}

		internal AndroidJavaObject LoadAdConfig(string bidPayload)
		{
			AndroidJavaObject androidJavaObject = bridgedAdView.Call<AndroidJavaObject>("buildLoadAdConfig", Array.Empty<object>());
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
			bridgedAdView.Call("loadAd", androidJavaObject);
		}
	}
}
