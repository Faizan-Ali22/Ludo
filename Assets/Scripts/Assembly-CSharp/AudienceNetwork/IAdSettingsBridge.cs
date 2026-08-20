namespace AudienceNetwork
{
	internal interface IAdSettingsBridge
	{
		void AddTestDevice(string deviceID);

		void SetUrlPrefix(string urlPrefix);

		void SetMixedAudience(bool mixedAudience);

		void SetDataProcessingOptions(string[] dataProcessingOptions);

		void SetDataProcessingOptions(string[] dataProcessingOptions, int country, int state);

		string GetBidderToken();
	}
}
