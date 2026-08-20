using System;
using System.Collections.Generic;
using AudienceNetwork.Utility;
using UnityEngine;

namespace AudienceNetwork
{
	internal class RewardedVideoAdBridgeAndroid : RewardedVideoAdBridge
	{
		private static Dictionary<int, RewardedVideoAdContainer> rewardedVideoAds = new Dictionary<int, RewardedVideoAdContainer>();

		private static int lastKey;

		private AndroidJavaObject RewardedVideoAdForUniqueId(int uniqueId)
		{
			RewardedVideoAdContainer value = null;
			if (rewardedVideoAds.TryGetValue(uniqueId, out value))
			{
				return value.bridgedRewardedVideoAd;
			}
			return null;
		}

		private RewardedVideoAdContainer RewardedVideoAdContainerForUniqueId(int uniqueId)
		{
			RewardedVideoAdContainer value = null;
			if (rewardedVideoAds.TryGetValue(uniqueId, out value))
			{
				return value;
			}
			return null;
		}

		private string GetStringForuniqueId(int uniqueId, string method)
		{
			return RewardedVideoAdForUniqueId(uniqueId)?.Call<string>(method, Array.Empty<object>());
		}

		private string GetImageURLForuniqueId(int uniqueId, string method)
		{
			AndroidJavaObject androidJavaObject = RewardedVideoAdForUniqueId(uniqueId);
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

		public override int Create(string placementId, RewardData rewardData, RewardedVideoAd rewardedVideoAd)
		{
			AdUtility.Prepare();
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>());
			AndroidJavaObject bridgedRewardedVideoAd = new AndroidJavaObject("com.facebook.ads.RewardedVideoAd", androidJavaObject, placementId);
			RewardedVideoAdBridgeListenerProxy listenerProxy = new RewardedVideoAdBridgeListenerProxy(rewardedVideoAd, bridgedRewardedVideoAd);
			AndroidJavaObject rewardData2 = null;
			if (rewardData != null)
			{
				rewardData2 = new AndroidJavaObject("com.facebook.ads.RewardData", rewardData.UserId, rewardData.Currency);
			}
			RewardedVideoAdContainer value = new RewardedVideoAdContainer(rewardedVideoAd)
			{
				bridgedRewardedVideoAd = bridgedRewardedVideoAd,
				listenerProxy = listenerProxy,
				rewardData = rewardData2
			};
			int num = lastKey;
			rewardedVideoAds.Add(num, value);
			lastKey++;
			return num;
		}

		public override int Load(int uniqueId)
		{
			AdUtility.Prepare();
			RewardedVideoAdContainerForUniqueId(uniqueId)?.Load();
			return uniqueId;
		}

		public override int Load(int uniqueId, string bidPayload)
		{
			AdUtility.Prepare();
			RewardedVideoAdContainerForUniqueId(uniqueId)?.Load(bidPayload);
			return uniqueId;
		}

		public override bool IsValid(int uniqueId)
		{
			AndroidJavaObject androidJavaObject = RewardedVideoAdForUniqueId(uniqueId);
			if (androidJavaObject != null)
			{
				return !androidJavaObject.Call<bool>("isAdInvalidated", Array.Empty<object>());
			}
			return false;
		}

		public override bool Show(int uniqueId)
		{
			RewardedVideoAdContainer rewardedVideoAdContainer = RewardedVideoAdContainerForUniqueId(uniqueId);
			AndroidJavaObject rewardedVideoAd = RewardedVideoAdForUniqueId(uniqueId);
			rewardedVideoAdContainer.rewardedVideoAd.ExecuteOnMainThread(delegate
			{
				if (rewardedVideoAd != null)
				{
					rewardedVideoAd.Call<bool>("show", Array.Empty<object>());
				}
			});
			return true;
		}

		public override void SetExtraHints(int uniqueId, ExtraHints extraHints)
		{
			AdUtility.Prepare();
			RewardedVideoAdForUniqueId(uniqueId)?.Call("setExtraHints", extraHints.GetAndroidObject());
		}

		public override void Release(int uniqueId)
		{
			RewardedVideoAdForUniqueId(uniqueId)?.Call("destroy");
			rewardedVideoAds.Remove(uniqueId);
		}

		public override void OnLoad(int uniqueId, FBRewardedVideoAdBridgeCallback callback)
		{
		}

		public override void OnImpression(int uniqueId, FBRewardedVideoAdBridgeCallback callback)
		{
		}

		public override void OnClick(int uniqueId, FBRewardedVideoAdBridgeCallback callback)
		{
		}

		public override void OnError(int uniqueId, FBRewardedVideoAdBridgeErrorCallback callback)
		{
		}

		public override void OnWillClose(int uniqueId, FBRewardedVideoAdBridgeCallback callback)
		{
		}

		public override void OnDidClose(int uniqueId, FBRewardedVideoAdBridgeCallback callback)
		{
		}

		public override void OnActivityDestroyed(int uniqueId, FBRewardedVideoAdBridgeCallback callback)
		{
		}
	}
}
