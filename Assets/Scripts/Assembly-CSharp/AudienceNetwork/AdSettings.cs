namespace AudienceNetwork
{
	public static class AdSettings
	{
		public static void AddTestDevice(string deviceID)
		{
			AdSettingsBridge.Instance.AddTestDevice(deviceID);
		}

		public static void SetUrlPrefix(string urlPrefix)
		{
			AdSettingsBridge.Instance.SetUrlPrefix(urlPrefix);
		}

		public static void SetMixedAudience(bool mixedAudience)
		{
			AdSettingsBridge.Instance.SetMixedAudience(mixedAudience);
		}

		public static void SetDataProcessingOptions(string[] dataProcessingOptions)
		{
			AdSettingsBridge.Instance.SetDataProcessingOptions(dataProcessingOptions);
		}

		public static void SetDataProcessingOptions(string[] dataProcessingOptions, int country, int state)
		{
			AdSettingsBridge.Instance.SetDataProcessingOptions(dataProcessingOptions, country, state);
		}

		public static string GetBidderToken()
		{
			return AdSettingsBridge.Instance.GetBidderToken();
		}
	}
}
