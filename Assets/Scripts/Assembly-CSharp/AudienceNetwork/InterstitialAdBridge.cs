using UnityEngine;

namespace AudienceNetwork
{
	internal class InterstitialAdBridge : IInterstitialAdBridge
	{
		public static readonly IInterstitialAdBridge Instance;

		internal InterstitialAdBridge()
		{
		}

		static InterstitialAdBridge()
		{
			Instance = CreateInstance();
		}

		private static IInterstitialAdBridge CreateInstance()
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				return new InterstitialAdBridgeAndroid();
			}
			return new InterstitialAdBridge();
		}

		public virtual int Create(string placementId, InterstitialAd InterstitialAd)
		{
			return 123;
		}

		public virtual int Load(int uniqueId)
		{
			return 123;
		}

		public virtual int Load(int uniqueId, string bidPayload)
		{
			return 123;
		}

		public virtual bool IsValid(int uniqueId)
		{
			return true;
		}

		public virtual bool Show(int uniqueId)
		{
			return true;
		}

		public virtual void SetExtraHints(int uniqueId, ExtraHints extraHints)
		{
		}

		public virtual void Release(int uniqueId)
		{
		}

		public virtual void OnLoad(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public virtual void OnImpression(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public virtual void OnClick(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public virtual void OnError(int uniqueId, FBInterstitialAdBridgeErrorCallback callback)
		{
		}

		public virtual void OnWillClose(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public virtual void OnDidClose(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}

		public virtual void OnActivityDestroyed(int uniqueId, FBInterstitialAdBridgeCallback callback)
		{
		}
	}
}
