using System;
using System.Reflection;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	public class DummyClient : IBannerClient, IInterstitialClient, IRewardBasedVideoAdClient, IAdLoaderClient, IMobileAdsClient
	{
		public string UserId
		{
			get
			{
				DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
				return "UserId";
			}
			set
			{
				DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
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

		public event EventHandler<AdValueEventArgs> OnPaidEvent;

		public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

		public DummyClient()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void Initialize(string appId)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void Initialize(Action<InitializationStatus> initCompleteAction)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			initCompleteAction(null);
		}

		public void SetApplicationMuted(bool muted)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetApplicationVolume(float volume)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetiOSAppPauseOnBackground(bool pause)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public float GetDeviceScale()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return 0f;
		}

		public int GetDeviceSafeWidth()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return 0;
		}

		public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateBannerView(string adUnitId, AdSize adSize, int positionX, int positionY)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void LoadAd(AdRequest request)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void ShowBannerView()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void HideBannerView()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void DestroyBannerView()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public float GetHeightInPixels()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return 0f;
		}

		public float GetWidthInPixels()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return 0f;
		}

		public void SetPosition(AdPosition adPosition)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetPosition(int x, int y)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateInterstitialAd(string adUnitId)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public bool IsLoaded()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return true;
		}

		public void ShowInterstitial()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void DestroyInterstitial()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateRewardBasedVideoAd()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetUserId(string userId)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void LoadAd(AdRequest request, string adUnitId)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void DestroyRewardBasedVideoAd()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void ShowRewardBasedVideoAd()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateAdLoader(AdLoader.Builder builder)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void Load(AdRequest request)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetAdSize(AdSize adSize)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public string MediationAdapterClassName()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return null;
		}
	}
}
