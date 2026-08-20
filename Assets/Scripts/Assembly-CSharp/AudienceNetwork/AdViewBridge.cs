using UnityEngine;

namespace AudienceNetwork
{
	internal class AdViewBridge : IAdViewBridge
	{
		public static readonly IAdViewBridge Instance;

		internal AdViewBridge()
		{
		}

		static AdViewBridge()
		{
			Instance = CreateInstance();
		}

		private static IAdViewBridge CreateInstance()
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				return new AdViewBridgeAndroid();
			}
			return new AdViewBridge();
		}

		public virtual int Create(string placementId, AdView AdView, AdSize size)
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

		public virtual bool Show(int uniqueId, double x, double y, double width, double height)
		{
			return true;
		}

		public virtual void SetExtraHints(int uniqueId, ExtraHints extraHints)
		{
		}

		public virtual void Release(int uniqueId)
		{
		}

		public virtual void OnLoad(int uniqueId, FBAdViewBridgeCallback callback)
		{
		}

		public virtual void OnImpression(int uniqueId, FBAdViewBridgeCallback callback)
		{
		}

		public virtual void OnClick(int uniqueId, FBAdViewBridgeCallback callback)
		{
		}

		public virtual void OnError(int uniqueId, FBAdViewBridgeErrorCallback callback)
		{
		}

		public virtual void OnFinishedClick(int uniqueId, FBAdViewBridgeCallback callback)
		{
		}
	}
}
