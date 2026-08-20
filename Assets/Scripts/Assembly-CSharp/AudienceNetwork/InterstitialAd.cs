using System;
using UnityEngine;

namespace AudienceNetwork
{
	public sealed class InterstitialAd : IDisposable
	{
		private readonly int uniqueId;

		private bool isLoaded;

		private AdHandler handler;

		public FBInterstitialAdBridgeCallback interstitialAdDidLoad;

		public FBInterstitialAdBridgeCallback interstitialAdWillLogImpression;

		public FBInterstitialAdBridgeErrorCallback interstitialAdDidFailWithError;

		public FBInterstitialAdBridgeCallback interstitialAdDidClick;

		public FBInterstitialAdBridgeCallback interstitialAdWillClose;

		public FBInterstitialAdBridgeCallback interstitialAdDidClose;

		public FBInterstitialAdBridgeCallback interstitialAdActivityDestroyed;

		public string PlacementId { get; private set; }

		public FBInterstitialAdBridgeCallback InterstitialAdDidLoad
		{
			internal get
			{
				return interstitialAdDidLoad;
			}
			set
			{
				interstitialAdDidLoad = value;
				InterstitialAdBridge.Instance.OnLoad(uniqueId, interstitialAdDidLoad);
			}
		}

		public FBInterstitialAdBridgeCallback InterstitialAdWillLogImpression
		{
			internal get
			{
				return interstitialAdWillLogImpression;
			}
			set
			{
				interstitialAdWillLogImpression = value;
				InterstitialAdBridge.Instance.OnImpression(uniqueId, interstitialAdWillLogImpression);
			}
		}

		public FBInterstitialAdBridgeErrorCallback InterstitialAdDidFailWithError
		{
			internal get
			{
				return interstitialAdDidFailWithError;
			}
			set
			{
				interstitialAdDidFailWithError = value;
				InterstitialAdBridge.Instance.OnError(uniqueId, interstitialAdDidFailWithError);
			}
		}

		public FBInterstitialAdBridgeCallback InterstitialAdDidClick
		{
			internal get
			{
				return interstitialAdDidClick;
			}
			set
			{
				interstitialAdDidClick = value;
				InterstitialAdBridge.Instance.OnClick(uniqueId, interstitialAdDidClick);
			}
		}

		public FBInterstitialAdBridgeCallback InterstitialAdWillClose
		{
			internal get
			{
				return interstitialAdWillClose;
			}
			set
			{
				interstitialAdWillClose = value;
				InterstitialAdBridge.Instance.OnWillClose(uniqueId, interstitialAdWillClose);
			}
		}

		public FBInterstitialAdBridgeCallback InterstitialAdDidClose
		{
			internal get
			{
				return interstitialAdDidClose;
			}
			set
			{
				interstitialAdDidClose = value;
				InterstitialAdBridge.Instance.OnDidClose(uniqueId, interstitialAdDidClose);
			}
		}

		public FBInterstitialAdBridgeCallback InterstitialAdActivityDestroyed
		{
			internal get
			{
				return interstitialAdActivityDestroyed;
			}
			set
			{
				interstitialAdActivityDestroyed = value;
				InterstitialAdBridge.Instance.OnActivityDestroyed(uniqueId, interstitialAdActivityDestroyed);
			}
		}

		public InterstitialAd(string placementId)
		{
			AudienceNetworkAds.Initialize();
			PlacementId = placementId;
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				uniqueId = InterstitialAdBridge.Instance.Create(placementId, this);
				InterstitialAdBridge.Instance.OnLoad(uniqueId, InterstitialAdDidLoad);
				InterstitialAdBridge.Instance.OnImpression(uniqueId, InterstitialAdWillLogImpression);
				InterstitialAdBridge.Instance.OnClick(uniqueId, InterstitialAdDidClick);
				InterstitialAdBridge.Instance.OnError(uniqueId, InterstitialAdDidFailWithError);
				InterstitialAdBridge.Instance.OnWillClose(uniqueId, InterstitialAdWillClose);
				InterstitialAdBridge.Instance.OnDidClose(uniqueId, InterstitialAdDidClose);
				InterstitialAdBridge.Instance.OnActivityDestroyed(uniqueId, InterstitialAdActivityDestroyed);
			}
		}

		~InterstitialAd()
		{
			Dispose(iAmBeingCalledFromDisposeAndNotFinalize: false);
		}

		public void Dispose()
		{
			Dispose(iAmBeingCalledFromDisposeAndNotFinalize: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool iAmBeingCalledFromDisposeAndNotFinalize)
		{
			DConsole.Log("Interstitial Ad Disposed.");
			InterstitialAdBridge.Instance.Release(uniqueId);
		}

		public override string ToString()
		{
			return $"[InterstitialAd: PlacementId={PlacementId}, InterstitialAdDidLoad={InterstitialAdDidLoad}, InterstitialAdWillLogImpression={InterstitialAdWillLogImpression}, InterstitialAdDidFailWithError={InterstitialAdDidFailWithError}, InterstitialAdDidClick={InterstitialAdDidClick}, InterstitialAdWillClose={InterstitialAdWillClose}, InterstitialAdDidClose={InterstitialAdDidClose}], InterstitialAdActivityDestroyed={InterstitialAdActivityDestroyed}]";
		}

		public void Register(GameObject gameObject)
		{
			handler = gameObject.AddComponent<AdHandler>();
		}

		public void LoadAd()
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				InterstitialAdBridge.Instance.Load(uniqueId);
			}
			else
			{
				InterstitialAdDidLoad();
			}
		}

		public void LoadAd(string bidPayload)
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				InterstitialAdBridge.Instance.Load(uniqueId, bidPayload);
			}
			else
			{
				InterstitialAdDidLoad();
			}
		}

		public bool IsValid()
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				if (isLoaded)
				{
					return InterstitialAdBridge.Instance.IsValid(uniqueId);
				}
				return false;
			}
			return true;
		}

		internal void LoadAdFromData()
		{
			isLoaded = true;
			if (InterstitialAdDidLoad != null)
			{
				handler.ExecuteOnMainThread(delegate
				{
					InterstitialAdDidLoad();
				});
			}
		}

		public bool Show()
		{
			return InterstitialAdBridge.Instance.Show(uniqueId);
		}

		public void SetExtraHints(ExtraHints extraHints)
		{
			InterstitialAdBridge.Instance.SetExtraHints(uniqueId, extraHints);
		}

		internal void ExecuteOnMainThread(Action action)
		{
			if ((bool)handler)
			{
				handler.ExecuteOnMainThread(action);
			}
		}

		public static implicit operator bool(InterstitialAd obj)
		{
			return obj != null;
		}
	}
}
