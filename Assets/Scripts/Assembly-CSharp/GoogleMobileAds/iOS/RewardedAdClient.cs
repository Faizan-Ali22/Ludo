using System;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
	public class RewardedAdClient : IRewardedAdClient, IDisposable
	{
		internal delegate void GADURewardedAdDidReceiveAdCallback(IntPtr rewardedAdClient);

		internal delegate void GADURewardedAdDidFailToReceiveAdWithErrorCallback(IntPtr rewardedAdClient, string error);

		internal delegate void GADURewardedAdDidFailToShowAdWithErrorCallback(IntPtr rewardedAdClient, string error);

		internal delegate void GADURewardedAdDidOpenCallback(IntPtr rewardedAdClient);

		internal delegate void GADURewardedAdDidCloseCallback(IntPtr rewardedAdClient);

		internal delegate void GADUUserEarnedRewardCallback(IntPtr rewardedAdClient, string rewardType, double rewardAmount);

		internal delegate void GADURewardedAdPaidEventCallback(IntPtr rewardedAdClient, int precision, long value, string currencyCode);

		private IntPtr rewardedAdPtr;

		private IntPtr rewardedAdClientPtr;

		private IntPtr RewardedAdPtr
		{
			get
			{
				return rewardedAdPtr;
			}
			set
			{
				Externs.GADURelease(rewardedAdPtr);
				rewardedAdPtr = value;
			}
		}

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdStarted;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<Reward> OnUserEarnedReward;

		public event EventHandler<AdValueEventArgs> OnPaidEvent;

		public void CreateRewardedAd(string adUnitId)
		{
			rewardedAdClientPtr = (IntPtr)GCHandle.Alloc(this);
			RewardedAdPtr = Externs.GADUCreateRewardedAd(rewardedAdClientPtr, adUnitId);
			Externs.GADUSetRewardedAdCallbacks(RewardedAdPtr, RewardedAdDidReceiveAdCallback, RewardedAdDidFailToReceiveAdWithErrorCallback, RewardedAdDidFailToShowAdWithErrorCallback, RewardedAdDidOpenCallback, RewardedAdDidCloseCallback, RewardedAdUserDidEarnRewardCallback, RewardedAdPaidEventCallback);
		}

		public void LoadAd(AdRequest request)
		{
			IntPtr intPtr = Utils.BuildAdRequest(request);
			Externs.GADURequestRewardedAd(RewardedAdPtr, intPtr);
			Externs.GADURelease(intPtr);
		}

		public void Show()
		{
			Externs.GADUShowRewardedAd(RewardedAdPtr);
		}

		public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
		{
			IntPtr intPtr = Utils.BuildServerSideVerificationOptions(serverSideVerificationOptions);
			Externs.GADURewardedAdSetServerSideVerificationOptions(RewardedAdPtr, intPtr);
			Externs.GADURelease(intPtr);
		}

		public bool IsLoaded()
		{
			return Externs.GADURewardedAdReady(RewardedAdPtr);
		}

		public Reward GetRewardItem()
		{
			string type = Externs.GADURewardedAdGetRewardType(RewardedAdPtr);
			double amount = Externs.GADURewardedAdGetRewardAmount(RewardedAdPtr);
			return new Reward
			{
				Type = type,
				Amount = amount
			};
		}

		public string MediationAdapterClassName()
		{
			return Utils.PtrToString(Externs.GADUMediationAdapterClassNameForRewardedAd(RewardedAdPtr));
		}

		public void DestroyRewardedAd()
		{
			RewardedAdPtr = IntPtr.Zero;
		}

		public void Dispose()
		{
			DestroyRewardedAd();
			((GCHandle)rewardedAdClientPtr).Free();
		}

		~RewardedAdClient()
		{
			Dispose();
		}

		[MonoPInvokeCallback(typeof(GADURewardedAdDidReceiveAdCallback))]
		private static void RewardedAdDidReceiveAdCallback(IntPtr rewardedAdClient)
		{
			RewardedAdClient rewardedAdClient2 = IntPtrToRewardedAdClient(rewardedAdClient);
			if (rewardedAdClient2.OnAdLoaded != null)
			{
				rewardedAdClient2.OnAdLoaded(rewardedAdClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardedAdDidFailToReceiveAdWithErrorCallback))]
		private static void RewardedAdDidFailToReceiveAdWithErrorCallback(IntPtr rewardedAdClient, string error)
		{
			RewardedAdClient rewardedAdClient2 = IntPtrToRewardedAdClient(rewardedAdClient);
			if (rewardedAdClient2.OnAdFailedToLoad != null)
			{
				AdErrorEventArgs e = new AdErrorEventArgs
				{
					Message = error
				};
				rewardedAdClient2.OnAdFailedToLoad(rewardedAdClient2, e);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardedAdDidFailToShowAdWithErrorCallback))]
		private static void RewardedAdDidFailToShowAdWithErrorCallback(IntPtr rewardedAdClient, string error)
		{
			RewardedAdClient rewardedAdClient2 = IntPtrToRewardedAdClient(rewardedAdClient);
			if (rewardedAdClient2.OnAdFailedToShow != null)
			{
				AdErrorEventArgs e = new AdErrorEventArgs
				{
					Message = error
				};
				rewardedAdClient2.OnAdFailedToShow(rewardedAdClient2, e);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardedAdDidOpenCallback))]
		private static void RewardedAdDidOpenCallback(IntPtr rewardedAdClient)
		{
			RewardedAdClient rewardedAdClient2 = IntPtrToRewardedAdClient(rewardedAdClient);
			if (rewardedAdClient2.OnAdOpening != null)
			{
				rewardedAdClient2.OnAdOpening(rewardedAdClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardedAdDidCloseCallback))]
		private static void RewardedAdDidCloseCallback(IntPtr rewardedAdClient)
		{
			RewardedAdClient rewardedAdClient2 = IntPtrToRewardedAdClient(rewardedAdClient);
			if (rewardedAdClient2.OnAdClosed != null)
			{
				rewardedAdClient2.OnAdClosed(rewardedAdClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUUserEarnedRewardCallback))]
		private static void RewardedAdUserDidEarnRewardCallback(IntPtr rewardedAdClient, string rewardType, double rewardAmount)
		{
			RewardedAdClient rewardedAdClient2 = IntPtrToRewardedAdClient(rewardedAdClient);
			if (rewardedAdClient2.OnUserEarnedReward != null)
			{
				Reward e = new Reward
				{
					Type = rewardType,
					Amount = rewardAmount
				};
				rewardedAdClient2.OnUserEarnedReward(rewardedAdClient2, e);
			}
		}

		[MonoPInvokeCallback(typeof(GADURewardedAdPaidEventCallback))]
		private static void RewardedAdPaidEventCallback(IntPtr rewardedAdClient, int precision, long value, string currencyCode)
		{
			RewardedAdClient rewardedAdClient2 = IntPtrToRewardedAdClient(rewardedAdClient);
			if (rewardedAdClient2.OnPaidEvent != null)
			{
				AdValue adValue = new AdValue
				{
					Precision = (AdValue.PrecisionType)precision,
					Value = value,
					CurrencyCode = currencyCode
				};
				AdValueEventArgs e = new AdValueEventArgs
				{
					AdValue = adValue
				};
				rewardedAdClient2.OnPaidEvent(rewardedAdClient2, e);
			}
		}

		private static RewardedAdClient IntPtrToRewardedAdClient(IntPtr rewardedAdClient)
		{
			return ((GCHandle)rewardedAdClient).Target as RewardedAdClient;
		}
	}
}
