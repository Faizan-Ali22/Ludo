using System;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
	public class BannerClient : IBannerClient, IDisposable
	{
		internal delegate void GADUAdViewDidReceiveAdCallback(IntPtr bannerClient);

		internal delegate void GADUAdViewDidFailToReceiveAdWithErrorCallback(IntPtr bannerClient, string error);

		internal delegate void GADUAdViewWillPresentScreenCallback(IntPtr bannerClient);

		internal delegate void GADUAdViewDidDismissScreenCallback(IntPtr bannerClient);

		internal delegate void GADUAdViewWillLeaveApplicationCallback(IntPtr bannerClient);

		internal delegate void GADUAdViewPaidEventCallback(IntPtr bannerClient, int precision, long value, string currencyCode);

		private IntPtr bannerViewPtr;

		private IntPtr bannerClientPtr;

		private IntPtr BannerViewPtr
		{
			get
			{
				return bannerViewPtr;
			}
			set
			{
				Externs.GADURelease(bannerViewPtr);
				bannerViewPtr = value;
			}
		}

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<EventArgs> OnAdLeavingApplication;

		public event EventHandler<AdValueEventArgs> OnPaidEvent;

		public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
			bannerClientPtr = (IntPtr)GCHandle.Alloc(this);
			switch (adSize.AdType)
			{
			case AdSize.Type.SmartBanner:
				BannerViewPtr = Externs.GADUCreateSmartBannerView(bannerClientPtr, adUnitId, (int)position);
				break;
			case AdSize.Type.AnchoredAdaptive:
				BannerViewPtr = Externs.GADUCreateAnchoredAdaptiveBannerView(bannerClientPtr, adUnitId, adSize.Width, (int)adSize.Orientation, (int)position);
				break;
			case AdSize.Type.Standard:
				BannerViewPtr = Externs.GADUCreateBannerView(bannerClientPtr, adUnitId, adSize.Width, adSize.Height, (int)position);
				break;
			default:
				throw new ArgumentException("Invalid AdSize.Type provided.");
			}
			Externs.GADUSetBannerCallbacks(BannerViewPtr, AdViewDidReceiveAdCallback, AdViewDidFailToReceiveAdWithErrorCallback, AdViewWillPresentScreenCallback, AdViewDidDismissScreenCallback, AdViewWillLeaveApplicationCallback, AdViewPaidEventCallback);
		}

		public void CreateBannerView(string adUnitId, AdSize adSize, int x, int y)
		{
			bannerClientPtr = (IntPtr)GCHandle.Alloc(this);
			switch (adSize.AdType)
			{
			case AdSize.Type.SmartBanner:
				BannerViewPtr = Externs.GADUCreateSmartBannerViewWithCustomPosition(bannerClientPtr, adUnitId, x, y);
				break;
			case AdSize.Type.AnchoredAdaptive:
				BannerViewPtr = Externs.GADUCreateAnchoredAdaptiveBannerViewWithCustomPosition(bannerClientPtr, adUnitId, adSize.Width, (int)adSize.Orientation, x, y);
				break;
			case AdSize.Type.Standard:
				BannerViewPtr = Externs.GADUCreateBannerViewWithCustomPosition(bannerClientPtr, adUnitId, adSize.Width, adSize.Height, x, y);
				break;
			default:
				throw new ArgumentException("Invalid AdSize.Type provided.");
			}
			Externs.GADUSetBannerCallbacks(BannerViewPtr, AdViewDidReceiveAdCallback, AdViewDidFailToReceiveAdWithErrorCallback, AdViewWillPresentScreenCallback, AdViewDidDismissScreenCallback, AdViewWillLeaveApplicationCallback, AdViewPaidEventCallback);
		}

		public void LoadAd(AdRequest request)
		{
			IntPtr intPtr = Utils.BuildAdRequest(request);
			Externs.GADURequestBannerAd(BannerViewPtr, intPtr);
			Externs.GADURelease(intPtr);
		}

		public void ShowBannerView()
		{
			Externs.GADUShowBannerView(BannerViewPtr);
		}

		public void HideBannerView()
		{
			Externs.GADUHideBannerView(BannerViewPtr);
		}

		public void DestroyBannerView()
		{
			Externs.GADURemoveBannerView(BannerViewPtr);
			BannerViewPtr = IntPtr.Zero;
		}

		public float GetHeightInPixels()
		{
			return Externs.GADUGetBannerViewHeightInPixels(BannerViewPtr);
		}

		public float GetWidthInPixels()
		{
			return Externs.GADUGetBannerViewWidthInPixels(BannerViewPtr);
		}

		public void SetPosition(AdPosition adPosition)
		{
			Externs.GADUSetBannerViewAdPosition(BannerViewPtr, (int)adPosition);
		}

		public void SetPosition(int x, int y)
		{
			Externs.GADUSetBannerViewCustomPosition(BannerViewPtr, x, y);
		}

		public string MediationAdapterClassName()
		{
			return Utils.PtrToString(Externs.GADUMediationAdapterClassNameForBannerView(BannerViewPtr));
		}

		public void Dispose()
		{
			DestroyBannerView();
			((GCHandle)bannerClientPtr).Free();
		}

		~BannerClient()
		{
			Dispose();
		}

		[MonoPInvokeCallback(typeof(GADUAdViewDidReceiveAdCallback))]
		private static void AdViewDidReceiveAdCallback(IntPtr bannerClient)
		{
			BannerClient bannerClient2 = IntPtrToBannerClient(bannerClient);
			if (bannerClient2.OnAdLoaded != null)
			{
				bannerClient2.OnAdLoaded(bannerClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUAdViewDidFailToReceiveAdWithErrorCallback))]
		private static void AdViewDidFailToReceiveAdWithErrorCallback(IntPtr bannerClient, string error)
		{
			BannerClient bannerClient2 = IntPtrToBannerClient(bannerClient);
			if (bannerClient2.OnAdFailedToLoad != null)
			{
				AdFailedToLoadEventArgs e = new AdFailedToLoadEventArgs
				{
					Message = error
				};
				bannerClient2.OnAdFailedToLoad(bannerClient2, e);
			}
		}

		[MonoPInvokeCallback(typeof(GADUAdViewWillPresentScreenCallback))]
		private static void AdViewWillPresentScreenCallback(IntPtr bannerClient)
		{
			BannerClient bannerClient2 = IntPtrToBannerClient(bannerClient);
			if (bannerClient2.OnAdOpening != null)
			{
				bannerClient2.OnAdOpening(bannerClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUAdViewDidDismissScreenCallback))]
		private static void AdViewDidDismissScreenCallback(IntPtr bannerClient)
		{
			BannerClient bannerClient2 = IntPtrToBannerClient(bannerClient);
			if (bannerClient2.OnAdClosed != null)
			{
				bannerClient2.OnAdClosed(bannerClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUAdViewWillLeaveApplicationCallback))]
		private static void AdViewWillLeaveApplicationCallback(IntPtr bannerClient)
		{
			BannerClient bannerClient2 = IntPtrToBannerClient(bannerClient);
			if (bannerClient2.OnAdLeavingApplication != null)
			{
				bannerClient2.OnAdLeavingApplication(bannerClient2, EventArgs.Empty);
			}
		}

		[MonoPInvokeCallback(typeof(GADUAdViewPaidEventCallback))]
		private static void AdViewPaidEventCallback(IntPtr bannerClient, int precision, long value, string currencyCode)
		{
			BannerClient bannerClient2 = IntPtrToBannerClient(bannerClient);
			if (bannerClient2.OnPaidEvent != null)
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
				bannerClient2.OnPaidEvent(bannerClient2, e);
			}
		}

		private static BannerClient IntPtrToBannerClient(IntPtr bannerClient)
		{
			return ((GCHandle)bannerClient).Target as BannerClient;
		}
	}
}
