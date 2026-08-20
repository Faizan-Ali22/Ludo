using System;
using UnityEngine;

namespace AudienceNetwork
{
	internal class AdSettingsBridgeAndroid : AdSettingsBridge
	{
		public override void AddTestDevice(string deviceID)
		{
			GetAdSettingsObject().CallStatic("addTestDevice", deviceID);
		}

		public override void SetUrlPrefix(string urlPrefix)
		{
			GetAdSettingsObject().CallStatic("setUrlPrefix", urlPrefix);
		}

		public override void SetMixedAudience(bool mixedAudience)
		{
			GetAdSettingsObject().CallStatic("setMixedAudience", mixedAudience);
		}

		public override void SetDataProcessingOptions(string[] dataProcessingOptions)
		{
			GetAdSettingsObject().CallStatic("setDataProcessingOptions", new object[1] { dataProcessingOptions });
		}

		public override void SetDataProcessingOptions(string[] dataProcessingOptions, int country, int state)
		{
			GetAdSettingsObject().CallStatic("setDataProcessingOptions", dataProcessingOptions, country, state);
		}

		public override string GetBidderToken()
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>());
			return new AndroidJavaClass("com.facebook.ads.BidderTokenProvider").CallStatic<string>("getBidderToken", new object[1] { androidJavaObject });
		}

		private AndroidJavaClass GetAdSettingsObject()
		{
			return new AndroidJavaClass("com.facebook.ads.AdSettings");
		}
	}
}
