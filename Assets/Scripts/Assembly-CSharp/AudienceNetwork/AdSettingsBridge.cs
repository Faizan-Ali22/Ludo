using UnityEngine;

namespace AudienceNetwork
{
	internal class AdSettingsBridge : IAdSettingsBridge
	{
		public static readonly IAdSettingsBridge Instance;

		internal AdSettingsBridge()
		{
		}

		static AdSettingsBridge()
		{
			Instance = CreateInstance();
		}

		private static IAdSettingsBridge CreateInstance()
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				return new AdSettingsBridgeAndroid();
			}
			return new AdSettingsBridge();
		}

		public virtual void AddTestDevice(string deviceID)
		{
		}

		public virtual void SetUrlPrefix(string urlPrefix)
		{
		}

		public virtual void SetMixedAudience(bool mixedAudience)
		{
		}

		public virtual void SetDataProcessingOptions(string[] dataProcessingOptions)
		{
		}

		public virtual void SetDataProcessingOptions(string[] dataProcessingOptions, int country, int state)
		{
		}

		public virtual string GetBidderToken()
		{
			return string.Empty;
		}
	}
}
