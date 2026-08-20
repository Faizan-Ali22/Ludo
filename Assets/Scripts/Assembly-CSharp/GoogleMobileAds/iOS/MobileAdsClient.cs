using System;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
	public class MobileAdsClient : IMobileAdsClient
	{
		internal delegate void GADUInitializationCompleteCallback(IntPtr mobileAdsClient, IntPtr initStatusClient);

		private static MobileAdsClient instance = new MobileAdsClient();

		private Action<InitializationStatus> initCompleteAction;

		private IntPtr mobileAdsClientPtr;

		public static MobileAdsClient Instance => instance;

		private MobileAdsClient()
		{
			mobileAdsClientPtr = (IntPtr)GCHandle.Alloc(this);
		}

		public void Initialize(string appId)
		{
			Externs.GADUInitialize(appId);
		}

		public void Initialize(Action<InitializationStatus> initCompleteAction)
		{
			this.initCompleteAction = initCompleteAction;
			Externs.GADUInitializeWithCallback(mobileAdsClientPtr, InitializationCompleteCallback);
		}

		public void SetApplicationVolume(float volume)
		{
			Externs.GADUSetApplicationVolume(volume);
		}

		public void SetApplicationMuted(bool muted)
		{
			Externs.GADUSetApplicationMuted(muted);
		}

		public void SetiOSAppPauseOnBackground(bool pause)
		{
			Externs.GADUSetiOSAppPauseOnBackground(pause);
		}

		public float GetDeviceScale()
		{
			return Externs.GADUDeviceScale();
		}

		public int GetDeviceSafeWidth()
		{
			return Externs.GADUDeviceSafeWidth();
		}

		[MonoPInvokeCallback(typeof(GADUInitializationCompleteCallback))]
		private static void InitializationCompleteCallback(IntPtr mobileAdsClient, IntPtr initStatus)
		{
			MobileAdsClient mobileAdsClient2 = IntPtrToMobileAdsClient(mobileAdsClient);
			if (mobileAdsClient2.initCompleteAction != null)
			{
				InitializationStatus obj = new InitializationStatus(new InitializationStatusClient(initStatus));
				mobileAdsClient2.initCompleteAction(obj);
			}
		}

		private static MobileAdsClient IntPtrToMobileAdsClient(IntPtr mobileAdsClient)
		{
			return ((GCHandle)mobileAdsClient).Target as MobileAdsClient;
		}

		public void Dispose()
		{
			((GCHandle)mobileAdsClientPtr).Free();
		}

		~MobileAdsClient()
		{
			Dispose();
		}
	}
}
