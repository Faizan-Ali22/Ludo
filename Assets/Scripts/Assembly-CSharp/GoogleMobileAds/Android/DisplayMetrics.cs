using System;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class DisplayMetrics
	{
		public float Density { get; protected set; }

		public int HeightPixels { get; protected set; }

		public int WidthPixels { get; protected set; }

		public DisplayMetrics()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (new AndroidJavaClass("android.util.DisplayMetrics"))
				{
					using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.util.DisplayMetrics"))
					{
						using (AndroidJavaObject androidJavaObject2 = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
						{
							using (AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getWindowManager", Array.Empty<object>()))
							{
								using (AndroidJavaObject androidJavaObject4 = androidJavaObject3.Call<AndroidJavaObject>("getDefaultDisplay", Array.Empty<object>()))
								{
									androidJavaObject4.Call("getMetrics", androidJavaObject);
									Density = androidJavaObject.Get<float>("density");
									HeightPixels = androidJavaObject.Get<int>("heightPixels");
									WidthPixels = androidJavaObject.Get<int>("widthPixels");
								}
							}
						}
					}
				}
			}
		}
	}
}
