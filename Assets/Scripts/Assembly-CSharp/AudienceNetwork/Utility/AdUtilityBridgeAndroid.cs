using System;
using UnityEngine;

namespace AudienceNetwork.Utility
{
	internal class AdUtilityBridgeAndroid : AdUtilityBridge
	{
		private T GetPropertyOfDisplayMetrics<T>(string property)
		{
			return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>()).Call<AndroidJavaObject>("getResources", Array.Empty<object>())
				.Call<AndroidJavaObject>("getDisplayMetrics", Array.Empty<object>())
				.Get<T>(property);
		}

		private double Density()
		{
			return GetPropertyOfDisplayMetrics<float>("density");
		}

		public override double DeviceWidth()
		{
			return GetPropertyOfDisplayMetrics<int>("widthPixels");
		}

		public override double DeviceHeight()
		{
			return GetPropertyOfDisplayMetrics<int>("heightPixels");
		}

		public override double Width()
		{
			return Convert(Screen.width);
		}

		public override double Height()
		{
			return Convert(Screen.height);
		}

		public override double Convert(double deviceSize)
		{
			return deviceSize / Density();
		}

		public override void Prepare()
		{
			try
			{
				new AndroidJavaClass("android.os.Looper").CallStatic("prepare");
			}
			catch (Exception)
			{
			}
		}
	}
}
