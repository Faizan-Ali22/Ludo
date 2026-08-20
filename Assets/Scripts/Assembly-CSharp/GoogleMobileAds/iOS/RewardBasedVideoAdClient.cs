using System;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
	public class RewardBasedVideoAdClient : IRewardBasedVideoAdClient, IDisposable
	{
		internal delegate void GADURewardBasedVideoAdDidReceiveAdCallback(IntPtr rewardBasedVideoAdClient);

		internal delegate void GADURewardBasedVideoAdDidFailToReceiveAdWithErrorCallback(IntPtr rewardBasedVideoClient, string error);

		internal delegate void GADURewardBasedVideoAdDidOpenCallback(IntPtr rewardBasedVideoAdClient);

		internal delegate void GADURewardBasedVideoAdDidStartCallback(IntPtr rewardBasedVideoAdClient);

		internal delegate void GADURewardBasedVideoAdDidCloseCallback(IntPtr rewardBasedVideoAdClient);

		internal delegate void GADURewardBasedVideoAdDidRewardCallback(IntPtr rewardBasedVideoAdClient, string rewardType, double rewardAmount);

		internal delegate void GADURewardBasedVideoAdWillLeaveApplicationCallback(IntPtr rewardBasedVideoAdClient);

		internal delegate void GADURewardBasedVideoAdDidCompleteCallback(IntPtr rewardBasedVideoAdClient);

		private IntPtr rewardBasedVideoAdPtr;

		private IntPtr rewardBasedVideoAdClientPtr;

		private IntPtr RewardBasedVideoAdPtr
		{
			get
			{
				return rewardBasedVideoAdPtr;
			}
			set
			{
				Externs.GADURelease(rewardBasedVideoAdPtr);
				rewardBasedVideoAdPtr = value;
			}
		}

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdStarted;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<Reward> OnAdRewarded;

		public event EventHandler<EventArgs> OnAdLeavingApplication;

		public event EventHandler<EventArgs> OnAdCompleted;

		public void CreateRewardBasedVideoAd()
		{
			rewardBasedVideoAdClientPtr = (IntPtr)GCHandle.Alloc(this);
			RewardBasedVideoAdPtr = Externs.GADUCreateRewardBasedVideoAd(rewardBasedVideoAdClientPtr);
			Externs.GADUSetRewardBasedVideoAdCallbacks(RewardBasedVideoAdPtr, RewardBasedVideoAdDidReceiveAdCallback, RewardBasedVideoAdDidFailToReceiveAdWithErrorCallback, RewardBasedVideoAdDidOpenCallback, RewardBasedVideoAdDidStartCallback, RewardBasedVideoAdDidCloseCallback, RewardBasedVideoAdDidRewardUserCallback, RewardBasedVideoAdWillLeaveApplicationCallback, RewardBasedVideoAdDidCompleteCallback);
		}

		public void LoadAd(AdRequest request, string adUnitId)
		{
			IntPtr intPtr = Utils.BuildAdRequest(request);
			Externs.GADURequestRewardBasedVideoAd(RewardBasedVideoAdPtr, intPtr, adUnitId);
			Externs.GADURelease(intPtr);
		}

		public void ShowRewardBasedVideoAd()
		{
			Externs.GADUShowRewardBasedVideoAd(RewardBasedVideoAdPtr);
		}

		public void SetUserId(string userId)
		{
			Externs.GADUSetRewardBasedVideoAdUserId(RewardBasedVideoAdPtr, userId);
		}

		public bool IsLoaded()
		{
			return Externs.GADURewardBasedVideoAdReady(RewardBasedVideoAdPtr);
		}

		public string MediationAdapterClassName()
		{
			return Utils.PtrToString(Externs.GADUMediationAdapterClassNameForRewardedVideo(RewardBasedVideoAdPtr));
		}

		public void DestroyRewardedVideoAd()
		{
			RewardBasedVideoAdPtr = IntPtr.Zero;
		}

		public void Dispose()
		{
			DestroyRewardedVideoAd();
			((GCHandle)rewardBasedVideoAdClientPtr).Free();
		}

		~RewardBasedVideoAdClient()
		{
			Dispose();
		}

		[MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidReceiveAdCallback))]
		private static void RewardBasedVideoAdDidReceiveAdCallback(IntPtr rewardBasedVideoAdClient)
		{
			RewardBasedVideoAdClient rewardBasedVideoAdClient2 = IntPtrToRewardBasedVideoClient(rewardBasedVideoAdClient);
			if (rewardBasedVideoAdClient2.OnAdLoaded != null)
			{
				rewardBasedVideoAdClient2.OnAdLoaded(rewardBasedVideoAdClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidFailToReceiveAdWithErrorCallback))]
		private static void RewardBasedVideoAdDidFailToReceiveAdWithErrorCallback(IntPtr rewardBasedVideoAdClient, string error)
		{
			RewardBasedVideoAdClient rewardBasedVideoAdClient2 = IntPtrToRewardBasedVideoClient(rewardBasedVideoAdClient);
			if (rewardBasedVideoAdClient2.OnAdFailedToLoad != null)
			{
				AdFailedToLoadEventArgs e = new AdFailedToLoadEventArgs
				{
					Message = error
				};
				rewardBasedVideoAdClient2.OnAdFailedToLoad(rewardBasedVideoAdClient2, e);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidOpenCallback))]
		private static void RewardBasedVideoAdDidOpenCallback(IntPtr rewardBasedVideoAdClient)
		{
			RewardBasedVideoAdClient rewardBasedVideoAdClient2 = IntPtrToRewardBasedVideoClient(rewardBasedVideoAdClient);
			if (rewardBasedVideoAdClient2.OnAdOpening != null)
			{
				rewardBasedVideoAdClient2.OnAdOpening(rewardBasedVideoAdClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidStartCallback))]
		private static void RewardBasedVideoAdDidStartCallback(IntPtr rewardBasedVideoAdClient)
		{
			RewardBasedVideoAdClient rewardBasedVideoAdClient2 = IntPtrToRewardBasedVideoClient(rewardBasedVideoAdClient);
			if (rewardBasedVideoAdClient2.OnAdStarted != null)
			{
				rewardBasedVideoAdClient2.OnAdStarted(rewardBasedVideoAdClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidCloseCallback))]
		private static void RewardBasedVideoAdDidCloseCallback(IntPtr rewardBasedVideoAdClient)
		{
			RewardBasedVideoAdClient rewardBasedVideoAdClient2 = IntPtrToRewardBasedVideoClient(rewardBasedVideoAdClient);
			if (rewardBasedVideoAdClient2.OnAdClosed != null)
			{
				rewardBasedVideoAdClient2.OnAdClosed(rewardBasedVideoAdClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidRewardCallback))]
		private static void RewardBasedVideoAdDidRewardUserCallback(IntPtr rewardBasedVideoAdClient, string rewardType, double rewardAmount)
		{
			RewardBasedVideoAdClient rewardBasedVideoAdClient2 = IntPtrToRewardBasedVideoClient(rewardBasedVideoAdClient);
			if (rewardBasedVideoAdClient2.OnAdRewarded != null)
			{
				Reward e = new Reward
				{
					Type = rewardType,
					Amount = rewardAmount
				};
				rewardBasedVideoAdClient2.OnAdRewarded(rewardBasedVideoAdClient2, e);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardBasedVideoAdWillLeaveApplicationCallback))]
		private static void RewardBasedVideoAdWillLeaveApplicationCallback(IntPtr rewardBasedVideoAdClient)
		{
			RewardBasedVideoAdClient rewardBasedVideoAdClient2 = IntPtrToRewardBasedVideoClient(rewardBasedVideoAdClient);
			if (rewardBasedVideoAdClient2.OnAdLeavingApplication != null)
			{
				rewardBasedVideoAdClient2.OnAdLeavingApplication(rewardBasedVideoAdClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardBasedVideoAdDidCompleteCallback))]
		private static void RewardBasedVideoAdDidCompleteCallback(IntPtr rewardBasedVideoAdClient)
		{
			RewardBasedVideoAdClient rewardBasedVideoAdClient2 = IntPtrToRewardBasedVideoClient(rewardBasedVideoAdClient);
			if (rewardBasedVideoAdClient2.OnAdCompleted != null)
			{
				rewardBasedVideoAdClient2.OnAdCompleted(rewardBasedVideoAdClient2, EventArgs.Empty);
			}
		}

		private static RewardBasedVideoAdClient IntPtrToRewardBasedVideoClient(IntPtr rewardBasedVideoAdClient)
		{
			return ((GCHandle)rewardBasedVideoAdClient).Target as RewardBasedVideoAdClient;
		}
	}
}
