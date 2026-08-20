using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
	internal class CustomNativeTemplateClient : ICustomNativeTemplateClient, IDisposable
	{
		internal delegate void GADUNativeCustomTemplateDidReceiveClick(IntPtr nativeCustomTemplateAd, string error);

		private IntPtr customNativeAdPtr;

		private IntPtr customNativeTemplateAdClientPtr;

		private Action<CustomNativeTemplateAd, string> clickHandler;

		private IntPtr CustomNativeAdPtr
		{
			get
			{
				return customNativeAdPtr;
			}
			set
			{
				Externs.GADURelease(customNativeAdPtr);
				customNativeAdPtr = value;
			}
		}

		public CustomNativeTemplateClient(IntPtr customNativeAd, Action<CustomNativeTemplateAd, string> clickHandler)
		{
			customNativeAdPtr = customNativeAd;
			this.clickHandler = clickHandler;
			customNativeTemplateAdClientPtr = (IntPtr)GCHandle.Alloc(this);
			Externs.GADUSetNativeCustomTemplateAdUnityClient(customNativeAd, customNativeTemplateAdClientPtr);
			Externs.GADUSetNativeCustomTemplateAdCallbacks(customNativeAd, NativeCustomTemplateDidReceiveClickCallback);
		}

		public List<string> GetAvailableAssetNames()
		{
			IntPtr arrayPtr = Externs.GADUNativeCustomTemplateAdAvailableAssetKeys(CustomNativeAdPtr);
			int numOfAssets = Externs.GADUNativeCustomTemplateAdNumberOfAvailableAssetKeys(CustomNativeAdPtr);
			return Utils.PtrArrayToManagedList(arrayPtr, numOfAssets);
		}

		public string GetTemplateId()
		{
			return Externs.GADUNativeCustomTemplateAdTemplateID(CustomNativeAdPtr);
		}

		public byte[] GetImageByteArray(string key)
		{
			string text = Externs.GADUNativeCustomTemplateAdImageAsBytesForKey(CustomNativeAdPtr, key);
			if (text == null)
			{
				return null;
			}
			return Convert.FromBase64String(text);
		}

		public string GetText(string key)
		{
			return Externs.GADUNativeCustomTemplateAdStringForKey(CustomNativeAdPtr, key);
		}

		public void PerformClick(string assetName)
		{
			bool customClickAction = clickHandler != null;
			Externs.GADUNativeCustomTemplateAdPerformClickOnAssetWithKey(CustomNativeAdPtr, assetName, customClickAction);
		}

		public void RecordImpression()
		{
			Externs.GADUNativeCustomTemplateAdRecordImpression(CustomNativeAdPtr);
		}

		public void DestroyCustomNativeTemplateAd()
		{
			CustomNativeAdPtr = IntPtr.Zero;
		}

		public void Dispose()
		{
			DestroyCustomNativeTemplateAd();
			((GCHandle)customNativeTemplateAdClientPtr).Free();
		}

		~CustomNativeTemplateClient()
		{
			Dispose();
		}

		[MonoPInvokeCallback(typeof(GADUNativeCustomTemplateDidReceiveClick))]
		private static void NativeCustomTemplateDidReceiveClickCallback(IntPtr nativeCustomAd, string assetName)
		{
			CustomNativeTemplateClient customNativeTemplateClient = IntPtrToAdLoaderClient(nativeCustomAd);
			if (customNativeTemplateClient.clickHandler != null)
			{
				CustomNativeTemplateAd arg = new CustomNativeTemplateAd(customNativeTemplateClient);
				customNativeTemplateClient.clickHandler(arg, assetName);
			}
		}

		private static CustomNativeTemplateClient IntPtrToAdLoaderClient(IntPtr customNativeTemplateAd)
		{
			return ((GCHandle)customNativeTemplateAd).Target as CustomNativeTemplateClient;
		}
	}
}
