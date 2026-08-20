using System;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
	public class InterstitialClient : IInterstitialClient
	{
		internal delegate void GADUInterstitialDidReceiveAdCallback(IntPtr interstitialClient);

		internal delegate void GADUInterstitialDidFailToReceiveAdWithErrorCallback(IntPtr interstitialClient, string error);

		internal delegate void GADUInterstitialWillPresentScreenCallback(IntPtr interstitialClient);

		internal delegate void GADUInterstitialDidDismissScreenCallback(IntPtr interstitialClient);

		internal delegate void GADUInterstitialWillLeaveApplicationCallback(IntPtr interstitialClient);

		internal delegate void GADUInterstitialPaidEventCallback(IntPtr interstitialClient, int precision, long value, string currencyCode);

		private IntPtr interstitialPtr;

		private IntPtr interstitialClientPtr;

		private IntPtr InterstitialPtr
		{
			get
			{
				return interstitialPtr;
			}
			set
			{
				Externs.GADURelease(interstitialPtr);
				interstitialPtr = value;
			}
		}

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<EventArgs> OnAdLeavingApplication;

		public event EventHandler<AdValueEventArgs> OnPaidEvent;

		public void CreateInterstitialAd(string adUnitId)
		{
			interstitialClientPtr = (IntPtr)GCHandle.Alloc(this);
			InterstitialPtr = Externs.GADUCreateInterstitial(interstitialClientPtr, adUnitId);
			Externs.GADUSetInterstitialCallbacks(InterstitialPtr, InterstitialDidReceiveAdCallback, InterstitialDidFailToReceiveAdWithErrorCallback, InterstitialWillPresentScreenCallback, InterstitialDidDismissScreenCallback, InterstitialWillLeaveApplicationCallback, InterstitialPaidEventCallback);
		}

		public void LoadAd(AdRequest request)
		{
			IntPtr intPtr = Utils.BuildAdRequest(request);
			Externs.GADURequestInterstitial(InterstitialPtr, intPtr);
			Externs.GADURelease(intPtr);
		}

		public bool IsLoaded()
		{
			return Externs.GADUInterstitialReady(InterstitialPtr);
		}

		public void ShowInterstitial()
		{
			Externs.GADUShowInterstitial(InterstitialPtr);
		}

		public void DestroyInterstitial()
		{
			InterstitialPtr = IntPtr.Zero;
		}

		public string MediationAdapterClassName()
		{
			return Utils.PtrToString(Externs.GADUMediationAdapterClassNameForInterstitial(InterstitialPtr));
		}

		public void Dispose()
		{
			DestroyInterstitial();
			((GCHandle)interstitialClientPtr).Free();
		}

		~InterstitialClient()
		{
			Dispose();
		}

		[MonoPInvokeCallback(typeof(GADUInterstitialDidReceiveAdCallback))]
		private static void InterstitialDidReceiveAdCallback(IntPtr interstitialClient)
		{
			InterstitialClient interstitialClient2 = IntPtrToInterstitialClient(interstitialClient);
			if (interstitialClient2.OnAdLoaded != null)
			{
				interstitialClient2.OnAdLoaded(interstitialClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUInterstitialDidFailToReceiveAdWithErrorCallback))]
		private static void InterstitialDidFailToReceiveAdWithErrorCallback(IntPtr interstitialClient, string error)
		{
			InterstitialClient interstitialClient2 = IntPtrToInterstitialClient(interstitialClient);
			if (interstitialClient2.OnAdFailedToLoad != null)
			{
				AdFailedToLoadEventArgs e = new AdFailedToLoadEventArgs
				{
					Message = error
				};
				interstitialClient2.OnAdFailedToLoad(interstitialClient2, e);
			}
		}

		[MonoPInvokeCallback(typeof(GADUInterstitialWillPresentScreenCallback))]
		private static void InterstitialWillPresentScreenCallback(IntPtr interstitialClient)
		{
			InterstitialClient interstitialClient2 = IntPtrToInterstitialClient(interstitialClient);
			if (interstitialClient2.OnAdOpening != null)
			{
				interstitialClient2.OnAdOpening(interstitialClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUInterstitialDidDismissScreenCallback))]
		private static void InterstitialDidDismissScreenCallback(IntPtr interstitialClient)
		{
			InterstitialClient interstitialClient2 = IntPtrToInterstitialClient(interstitialClient);
			if (interstitialClient2.OnAdClosed != null)
			{
				interstitialClient2.OnAdClosed(interstitialClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUInterstitialWillLeaveApplicationCallback))]
		private static void InterstitialWillLeaveApplicationCallback(IntPtr interstitialClient)
		{
			InterstitialClient interstitialClient2 = IntPtrToInterstitialClient(interstitialClient);
			if (interstitialClient2.OnAdLeavingApplication != null)
			{
				interstitialClient2.OnAdLeavingApplication(interstitialClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUInterstitialPaidEventCallback))]
		private static void InterstitialPaidEventCallback(IntPtr interstitialClient, int precision, long value, string currencyCode)
		{
			InterstitialClient interstitialClient2 = IntPtrToInterstitialClient(interstitialClient);
			if (interstitialClient2.OnPaidEvent != null)
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
				interstitialClient2.OnPaidEvent(interstitialClient2, e);
			}
		}

		private static InterstitialClient IntPtrToInterstitialClient(IntPtr interstitialClient)
		{
			return ((GCHandle)interstitialClient).Target as InterstitialClient;
		}
	}
}
