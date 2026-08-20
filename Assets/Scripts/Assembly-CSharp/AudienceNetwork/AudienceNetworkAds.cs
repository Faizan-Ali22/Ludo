using System;
using UnityEngine;

namespace AudienceNetwork
{
	public static class AudienceNetworkAds
	{
		private static bool isInitialized;

		internal static void Initialize()
		{
			if (!IsInitialized())
			{
				PlayerPrefs.SetString("an_isUnitySDK", SdkVersion.Build);
				AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>());
				new AndroidJavaClass("com.facebook.ads.AudienceNetworkAds").CallStatic("initialize", androidJavaObject);
				isInitialized = true;
			}
		}

		internal static bool IsInitialized()
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>());
			return new AndroidJavaClass("com.facebook.ads.AudienceNetworkAds").CallStatic<bool>("isInitialized", new object[1] { androidJavaObject });
		}
	}
}
