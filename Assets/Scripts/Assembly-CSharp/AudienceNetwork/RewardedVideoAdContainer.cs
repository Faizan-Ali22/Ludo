using System;
using UnityEngine;

namespace AudienceNetwork
{
	internal class RewardedVideoAdContainer
	{
		internal AndroidJavaProxy listenerProxy;

		internal AndroidJavaObject bridgedRewardedVideoAd;

		internal AndroidJavaObject rewardData;

		internal RewardedVideoAd rewardedVideoAd { get; set; }

		internal FBRewardedVideoAdBridgeCallback onLoad { get; set; }

		internal FBRewardedVideoAdBridgeCallback onImpression { get; set; }

		internal FBRewardedVideoAdBridgeCallback onClick { get; set; }

		internal FBRewardedVideoAdBridgeErrorCallback onError { get; set; }

		internal FBRewardedVideoAdBridgeCallback onDidClose { get; set; }

		internal FBRewardedVideoAdBridgeCallback onWillClose { get; set; }

		internal FBRewardedVideoAdBridgeCallback onComplete { get; set; }

		internal FBRewardedVideoAdBridgeCallback onDidSucceed { get; set; }

		internal FBRewardedVideoAdBridgeCallback onDidFail { get; set; }

		internal RewardedVideoAdContainer(RewardedVideoAd rewardedVideoAd)
		{
			this.rewardedVideoAd = rewardedVideoAd;
		}

		public override string ToString()
		{
			return $"[RewardedVideoAdContainer: rewardedVideoAd={rewardedVideoAd}, onLoad={onLoad}]";
		}

		public static implicit operator bool(RewardedVideoAdContainer obj)
		{
			return obj != null;
		}

		internal AndroidJavaObject LoadAdConfig(string bidPayload)
		{
			AndroidJavaObject androidJavaObject = bridgedRewardedVideoAd.Call<AndroidJavaObject>("buildLoadAdConfig", Array.Empty<object>());
			androidJavaObject.Call<AndroidJavaObject>("withAdListener", new object[1] { listenerProxy });
			if (rewardData != null)
			{
				androidJavaObject.Call<AndroidJavaObject>("withRewardData", new object[1] { rewardData });
			}
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
			bridgedRewardedVideoAd.Call("loadAd", androidJavaObject);
		}
	}
}
