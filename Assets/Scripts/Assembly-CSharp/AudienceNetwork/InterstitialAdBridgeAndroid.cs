using System;
using System.Collections.Generic;
using AudienceNetwork.Utility;
using UnityEngine;

namespace AudienceNetwork
{
	internal class InterstitialAdBridgeAndroid : InterstitialAdBridge
	{
		private static Dictionary<int, InterstitialAdContainer> interstitialAds = new Dictionary<int, InterstitialAdContainer>();

		private static int lastKey;

		private AndroidJavaObject InterstitialAdForuniqueId(int uniqueId)
		{
			InterstitialAdContainer value = null;
			if (interstitialAds.TryGetValue(uniqueId, out value))
			{
				return value.bridgedInterstitialAd;
			}
			return null;
		}

		private InterstitialAdContainer InterstitialAdContainerForuniqueId(int uniqueId)
		{
			InterstitialAdContainer value = null;
			if (interstitialAds.TryGetValue(uniqueId, out value))
			{
				return value;
			}
			return null;
		}

		private string GetStringForuniqueId(int uniqueId, string method)
		{
			return InterstitialAdForuniqueId(uniqueId)?.Call<string>(method, Array.Empty<object>());
		}

		private string GetImageURLForuniqueId(int uniqueId, string method)
		{
			AndroidJavaObject androidJavaObject = InterstitialAdForuniqueId(uniqueId);
			if (androidJavaObject != null)
			{
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(method, Array.Empty<object>());
				if (androidJavaObject2 != null)
				{
					return androidJavaObject2.Call<string>("getUrl", Array.Empty<object>());
				}
			}
			return null;
		}

		public override int Create(string placementId, InterstitialAd interstitialAd)
		{
			AdUtility.Prepare();
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>());
			AndroidJavaObject bridgedInterstitialAd = new AndroidJavaObject("com.facebook.ads.InterstitialAd", androidJavaObject, placementId);
			InterstitialAdBridgeListenerProxy listenerProxy = new InterstitialAdBridgeListenerProxy(interstitialAd, bridgedInterstitialAd);
			InterstitialAdContainer value = new InterstitialAdContainer(interstitialAd)
			{
				bridgedInterstitialAd = bridgedInterstitialAd,
				listenerProxy = listenerProxy
			};
			int num = lastKey;
			interstitialAds.Add(num, value);
			lastKey++;
			return num;
		}

		public override int Load(int uniqueId)
		{
			AdUtility.Prepare();
			InterstitialAdContainerForuniqueId(uniqueId)?.Load();
			return uniqueId;
		}

		public override int Load(int uniqueId, string bidPayload)
		{
			AdUtility.Prepare();
			InterstitialAdContainerForuniqueId(uniqueId)?.Load(bidPayload);
			return uniqueId;
		}

		public override bool IsValid(int uniqueId)
		{
			AndroidJavaObject androidJavaObject = InterstitialAdForuniqueId(uniqueId);
			if (androidJavaObject != null)
			{
				return !androidJavaObject.Call<bool>("isAdInvalidated", Array.Empty<object>());
			}
			return false;
		}

		public override bool Show(int uniqueId)
		{
			return InterstitialAdForuniqueId(uniqueId)?.Call<bool>("show", Array.Empty<object>()) ?? false;
		}

		public override void Release(int uniqueId)
		{
			InterstitialAdForuniqueId(uniqueId)?.Call("destroy");
			interstitialAds.Remove(uniqueId);
		}

		public override void SetExtraHints(int uniqueId, ExtraHints extraHints)
		{
			AdUtility.Prepare();
			InterstitialAdForuniqueId(uniqueId)?.Call("setExtraHints", extraHints.GetAndroidObject());
		}

		public override void OnLoad(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public override void OnImpression(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public override void OnClick(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public override void OnError(int uniqueId, FBInterstitialAdBridgeErrorCallback callback)
		{
		}

		public override void OnWillClose(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public override void OnDidClose(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public override void OnActivityDestroyed(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}
	}
}
