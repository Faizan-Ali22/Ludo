using System;
using System.Reflection;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	public class RewardedAdDummyClient : IRewardedAdClient
	{
		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<Reward> OnUserEarnedReward;

		public event EventHandler<AdValueEventArgs> OnPaidEvent;

		public RewardedAdDummyClient()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateRewardedAd(string adUnitId)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void LoadAd(AdRequest request)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			if (this.OnAdLoaded != null)
			{
				this.OnAdLoaded(this, EventArgs.Empty);
			}
		}

		public bool IsLoaded()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return true;
		}

		public void Show()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public string MediationAdapterClassName()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return null;
		}

		public Reward GetRewardItem()
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return null;
		}

		public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
		{
			DConsole.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}
	}
}
