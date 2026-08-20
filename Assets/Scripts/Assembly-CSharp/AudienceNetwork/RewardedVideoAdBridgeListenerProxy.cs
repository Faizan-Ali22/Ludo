using System;
using UnityEngine;

namespace AudienceNetwork
{
	internal class RewardedVideoAdBridgeListenerProxy : AndroidJavaProxy
	{
		private RewardedVideoAd rewardedVideoAd;

		private readonly AndroidJavaObject bridgedRewardedVideoAd;

		public RewardedVideoAdBridgeListenerProxy(RewardedVideoAd rewardedVideoAd, AndroidJavaObject bridgedRewardedVideoAd)
			: base("com.facebook.ads.S2SRewardedVideoAdExtendedListener")
		{
			this.rewardedVideoAd = rewardedVideoAd;
			this.bridgedRewardedVideoAd = bridgedRewardedVideoAd;
		}

		private void onError(AndroidJavaObject ad, AndroidJavaObject error)
		{
			string errorMessage = error.Call<string>("getErrorMessage", Array.Empty<object>());
			if (rewardedVideoAd.RewardedVideoAdDidFailWithError != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdDidFailWithError(errorMessage);
				});
			}
		}

		private void onAdLoaded(AndroidJavaObject ad)
		{
			rewardedVideoAd.LoadAdFromData();
		}

		private void onAdClicked(AndroidJavaObject ad)
		{
			if (rewardedVideoAd.RewardedVideoAdDidClick != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdDidClick();
				});
			}
		}

		private void onRewardedVideoDisplayed(AndroidJavaObject ad)
		{
			if (rewardedVideoAd.RewardedVideoAdWillLogImpression != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdWillLogImpression();
				});
			}
		}

		private void onRewardedVideoClosed()
		{
			if (rewardedVideoAd.RewardedVideoAdDidClose != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdDidClose();
				});
			}
		}

		private void onRewardedVideoCompleted()
		{
			if (rewardedVideoAd.RewardedVideoAdComplete != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdComplete();
				});
			}
		}

		private void onRewardServerSuccess()
		{
			if (rewardedVideoAd.RewardedVideoAdDidSucceed != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdDidSucceed();
				});
			}
		}

		private void onRewardServerFailed()
		{
			if (rewardedVideoAd.RewardedVideoAdDidFail != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdDidFail();
				});
			}
		}

		private void onLoggingImpression(AndroidJavaObject ad)
		{
			if (rewardedVideoAd.RewardedVideoAdWillLogImpression != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdWillLogImpression();
				});
			}
		}

		private void onRewardedVideoActivityDestroyed()
		{
			if (rewardedVideoAd.RewardedVideoAdActivityDestroyed != null)
			{
				rewardedVideoAd.ExecuteOnMainThread(delegate
				{
					rewardedVideoAd.RewardedVideoAdActivityDestroyed();
				});
			}
		}
	}
}
