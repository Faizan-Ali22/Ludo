using System;
using System.Collections.Generic;
using AudienceNetwork.Utility;
using UnityEngine;

namespace AudienceNetwork
{
	internal class AdViewBridgeAndroid : AdViewBridge
	{
		private static Dictionary<int, AdViewContainer> adViews = new Dictionary<int, AdViewContainer>();

		private static int lastKey;

		private AndroidJavaObject AdViewForAdViewId(int uniqueId)
		{
			AdViewContainer value = null;
			if (adViews.TryGetValue(uniqueId, out value))
			{
				return value.bridgedAdView;
			}
			return null;
		}

		private AdViewContainer AdViewContainerForAdViewId(int uniqueId)
		{
			AdViewContainer value = null;
			if (adViews.TryGetValue(uniqueId, out value))
			{
				return value;
			}
			return null;
		}

		private string GetStringForAdViewId(int uniqueId, string method)
		{
			return AdViewForAdViewId(uniqueId)?.Call<string>(method, Array.Empty<object>());
		}

		private string GetImageURLForAdViewId(int uniqueId, string method)
		{
			AndroidJavaObject androidJavaObject = AdViewForAdViewId(uniqueId);
			if (androidJavaObject != null)
			{
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(method, Array.Empty<object>());
				if (androidJavaObject2 != null)
				{
					return androidJavaObject2.Call<string>("getUrl", Array.Empty<object>());
				}
			}
			return null;
		}

		private AndroidJavaObject JavaAdSizeFromAdSize(AdSize size)
		{
			AndroidJavaObject result = null;
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.facebook.ads.AdSize");
			switch (size)
			{
			case AdSize.BANNER_HEIGHT_50:
				result = androidJavaClass.GetStatic<AndroidJavaObject>("BANNER_HEIGHT_50");
				break;
			case AdSize.BANNER_HEIGHT_90:
				result = androidJavaClass.GetStatic<AndroidJavaObject>("BANNER_HEIGHT_90");
				break;
			case AdSize.RECTANGLE_HEIGHT_250:
				result = androidJavaClass.GetStatic<AndroidJavaObject>("RECTANGLE_HEIGHT_250");
				break;
			}
			return result;
		}

		public override int Create(string placementId, AdView adView, AdSize size)
		{
			AdUtility.Prepare();
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>());
			AndroidJavaObject bridgedAdView = new AndroidJavaObject("com.facebook.ads.AdView", androidJavaObject, placementId, JavaAdSizeFromAdSize(size));
			AdViewBridgeListenerProxy listenerProxy = new AdViewBridgeListenerProxy(adView, bridgedAdView);
			AdViewContainer value = new AdViewContainer(adView)
			{
				bridgedAdView = bridgedAdView,
				listenerProxy = listenerProxy
			};
			int num = lastKey;
			adViews.Add(num, value);
			lastKey++;
			return num;
		}

		public override int Load(int uniqueId)
		{
			AdUtility.Prepare();
			AdViewContainerForAdViewId(uniqueId)?.Load();
			return uniqueId;
		}

		public override int Load(int uniqueId, string bidPayload)
		{
			AdUtility.Prepare();
			AdViewContainerForAdViewId(uniqueId)?.Load(bidPayload);
			return uniqueId;
		}

		public override bool IsValid(int uniqueId)
		{
			AndroidJavaObject androidJavaObject = AdViewForAdViewId(uniqueId);
			if (androidJavaObject != null)
			{
				return !androidJavaObject.Call<bool>("isAdInvalidated", Array.Empty<object>());
			}
			return false;
		}

		public override bool Show(int uniqueId, double x, double y, double width, double height)
		{
			AndroidJavaObject adView = AdViewForAdViewId(uniqueId);
			if (adView == null)
			{
				return false;
			}
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
			{
				float num = activity.Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>()).Call<AndroidJavaObject>("getResources", Array.Empty<object>()).Call<AndroidJavaObject>("getDisplayMetrics", Array.Empty<object>())
					.Get<float>("density");
				AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.widget.LinearLayout$LayoutParams", (int)(width * (double)num), (int)(height * (double)num));
				AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.widget.LinearLayout", activity);
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.R$id");
				AndroidJavaObject androidJavaObject3 = activity.Call<AndroidJavaObject>("findViewById", new object[1] { androidJavaClass2.GetStatic<int>("content") });
				AndroidJavaObject androidJavaObject4 = adView.Call<AndroidJavaObject>("getParent", Array.Empty<object>());
				if (androidJavaObject4 != null)
				{
					if (AndroidJNI.GetMethodID(androidJavaObject4.GetRawClass(), "removeView", "(Landroid/view/View;)V") != IntPtr.Zero)
					{
						androidJavaObject4.Call("removeView", adView);
					}
					else
					{
						AndroidJNI.ExceptionClear();
					}
				}
				androidJavaObject.Call("setMargins", (int)(x * (double)num), (int)(y * (double)num), 0, 0);
				androidJavaObject2.Call("addView", adView, androidJavaObject);
				androidJavaObject3.Call("addView", androidJavaObject2);
			});
			return true;
		}

		public override void SetExtraHints(int uniqueId, ExtraHints extraHints)
		{
			AdViewForAdViewId(uniqueId)?.Call("setExtraHints", extraHints.GetAndroidObject());
		}

		public override void Release(int uniqueId)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject adView = AdViewForAdViewId(uniqueId);
			adViews.Remove(uniqueId);
			if (adView != null)
			{
				androidJavaObject.Call("runOnUiThread", (AndroidJavaRunnable)delegate
				{
					adView.Call("destroy");
					adView.Call<AndroidJavaObject>("getParent", Array.Empty<object>()).Call("removeView", adView);
				});
			}
		}

		public override void OnLoad(int uniqueId, FBAdViewBridgeCallback callback)
		{
		}

		public override void OnImpression(int uniqueId, FBAdViewBridgeCallback callback)
		{
		}

		public override void OnClick(int uniqueId, FBAdViewBridgeCallback callback)
		{
		}

		public override void OnError(int uniqueId, FBAdViewBridgeErrorCallback callback)
		{
		}

		public override void OnFinishedClick(int uniqueId, FBAdViewBridgeCallback callback)
		{
		}
	}
}
