using System;
using AudienceNetwork.Utility;
using UnityEngine;

namespace AudienceNetwork
{
	public sealed class AdView : IDisposable
	{
		private readonly int uniqueId;

		private bool isLoaded;

		private readonly AdSize size;

		private AdHandler handler;

		public FBAdViewBridgeCallback adViewDidLoad;

		public FBAdViewBridgeCallback adViewWillLogImpression;

		public FBAdViewBridgeErrorCallback adViewDidFailWithError;

		public FBAdViewBridgeCallback adViewDidClick;

		public FBAdViewBridgeCallback adViewDidFinishClick;

		public string PlacementId { get; private set; }

		public FBAdViewBridgeCallback AdViewDidLoad
		{
			internal get
			{
				return adViewDidLoad;
			}
			set
			{
				adViewDidLoad = value;
				AdViewBridge.Instance.OnLoad(uniqueId, adViewDidLoad);
			}
		}

		public FBAdViewBridgeCallback AdViewWillLogImpression
		{
			internal get
			{
				return adViewWillLogImpression;
			}
			set
			{
				adViewWillLogImpression = value;
				AdViewBridge.Instance.OnImpression(uniqueId, adViewWillLogImpression);
			}
		}

		public FBAdViewBridgeErrorCallback AdViewDidFailWithError
		{
			internal get
			{
				return adViewDidFailWithError;
			}
			set
			{
				adViewDidFailWithError = value;
				AdViewBridge.Instance.OnError(uniqueId, adViewDidFailWithError);
			}
		}

		public FBAdViewBridgeCallback AdViewDidClick
		{
			internal get
			{
				return adViewDidClick;
			}
			set
			{
				adViewDidClick = value;
				AdViewBridge.Instance.OnClick(uniqueId, adViewDidClick);
			}
		}

		public FBAdViewBridgeCallback AdViewDidFinishClick
		{
			internal get
			{
				return adViewDidFinishClick;
			}
			set
			{
				adViewDidFinishClick = value;
				AdViewBridge.Instance.OnFinishedClick(uniqueId, adViewDidFinishClick);
			}
		}

		public AdView(string placementId, AdSize size)
		{
			AudienceNetworkAds.Initialize();
			PlacementId = placementId;
			this.size = size;
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				uniqueId = AdViewBridge.Instance.Create(placementId, this, size);
				AdViewBridge.Instance.OnLoad(uniqueId, AdViewDidLoad);
				AdViewBridge.Instance.OnImpression(uniqueId, AdViewWillLogImpression);
				AdViewBridge.Instance.OnClick(uniqueId, AdViewDidClick);
				AdViewBridge.Instance.OnError(uniqueId, AdViewDidFailWithError);
				AdViewBridge.Instance.OnFinishedClick(uniqueId, AdViewDidFinishClick);
			}
		}

		~AdView()
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
			DConsole.Log("Banner Ad Disposed.");
			AdViewBridge.Instance.Release(uniqueId);
		}

		public override string ToString()
		{
			return $"[AdView: PlacementId={PlacementId}, AdViewDidLoad={AdViewDidLoad}, AdViewWillLogImpression={AdViewWillLogImpression}, AdViewDidFailWithError={AdViewDidFailWithError}, AdViewDidClick={AdViewDidClick}, adViewDidFinishClick={adViewDidFinishClick}]";
		}

		public void Register(GameObject gameObject)
		{
			handler = gameObject.AddComponent<AdHandler>();
		}

		public void LoadAd()
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				AdViewBridge.Instance.Load(uniqueId);
			}
			else
			{
				AdViewDidLoad();
			}
		}

		public void LoadAd(string bidPayload)
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				AdViewBridge.Instance.Load(uniqueId, bidPayload);
			}
			else
			{
				AdViewDidLoad();
			}
		}

		public bool IsValid()
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				if (isLoaded)
				{
					return AdViewBridge.Instance.IsValid(uniqueId);
				}
				return false;
			}
			return true;
		}

		internal void LoadAdFromData()
		{
			isLoaded = true;
			if (AdViewDidLoad != null)
			{
				handler.ExecuteOnMainThread(delegate
				{
					AdViewDidLoad();
				});
			}
		}

		private static double HeightFromType(AdView instance, AdSize size)
		{
			switch (size)
			{
			case AdSize.BANNER_HEIGHT_50:
				return 50.0;
			case AdSize.BANNER_HEIGHT_90:
				return 90.0;
			case AdSize.RECTANGLE_HEIGHT_250:
				return 250.0;
			default:
				return 0.0;
			}
		}

		public bool Show(AdPosition position)
		{
			double y = 0.0;
			switch (position)
			{
			case AdPosition.BOTTOM:
				y = AdUtility.Height() - HeightFromType(this, size);
				break;
			case AdPosition.CUSTOM:
				DConsole.LogWarning("Use Show(double y) instead");
				break;
			}
			return Show(y);
		}

		public bool Show(double y)
		{
			return Show(0.0, y);
		}

		public bool Show(double x, double y)
		{
			return Show(x, y, AdUtility.Width(), HeightFromType(this, size));
		}

		private bool Show(double x, double y, double width, double height)
		{
			return AdViewBridge.Instance.Show(uniqueId, x, y, width, height);
		}

		public void SetExtraHints(ExtraHints extraHints)
		{
			AdViewBridge.Instance.SetExtraHints(uniqueId, extraHints);
		}

		internal void ExecuteOnMainThread(Action action)
		{
			if ((bool)handler)
			{
				handler.ExecuteOnMainThread(action);
			}
		}

		public static implicit operator bool(AdView obj)
		{
			return obj != null;
		}
	}
}
