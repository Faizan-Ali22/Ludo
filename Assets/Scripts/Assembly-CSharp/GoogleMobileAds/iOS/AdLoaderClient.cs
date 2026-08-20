using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
	public class AdLoaderClient : IAdLoaderClient, IDisposable
	{
		internal delegate void GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback(IntPtr adLoader, IntPtr nativeCustomTemplateAd, string templateID);

		internal delegate void GADUAdLoaderDidFailToReceiveAdWithErrorCallback(IntPtr AdLoader, string error);

		private IntPtr adLoaderPtr;

		private IntPtr adLoaderClientPtr;

		private NativeAdTypes adTypes;

		private Dictionary<string, Action<CustomNativeTemplateAd, string>> customNativeTemplateCallbacks;

		private IntPtr AdLoaderPtr
		{
			get
			{
				return adLoaderPtr;
			}
			set
			{
				Externs.GADURelease(adLoaderPtr);
				adLoaderPtr = value;
			}
		}

		public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public AdLoaderClient(AdLoader unityAdLoader)
		{
			adLoaderClientPtr = (IntPtr)GCHandle.Alloc(this);
			customNativeTemplateCallbacks = unityAdLoader.CustomNativeTemplateClickHandlers;
			string[] array = new string[unityAdLoader.TemplateIds.Count];
			unityAdLoader.TemplateIds.CopyTo(array);
			adTypes = default(NativeAdTypes);
			bool returnUrlsForImageAssets = false;
			if (unityAdLoader.AdTypes.Contains(NativeAdType.CustomTemplate))
			{
				returnUrlsForImageAssets = false;
				adTypes.CustomTemplateAd = 1;
			}
			AdLoaderPtr = Externs.GADUCreateAdLoader(adLoaderClientPtr, unityAdLoader.AdUnitId, array, array.Length, ref adTypes, returnUrlsForImageAssets);
			Externs.GADUSetAdLoaderCallbacks(AdLoaderPtr, AdLoaderDidReceiveNativeCustomTemplateAdCallback, AdLoaderDidFailToReceiveAdWithErrorCallback);
		}

		public void LoadAd(AdRequest request)
		{
			IntPtr intPtr = Utils.BuildAdRequest(request);
			Externs.GADURequestNativeAd(AdLoaderPtr, intPtr);
			Externs.GADURelease(intPtr);
		}

		public void DestroyAdLoader()
		{
			AdLoaderPtr = IntPtr.Zero;
		}

		public void Dispose()
		{
			DestroyAdLoader();
			((GCHandle)adLoaderClientPtr).Free();
		}

		~AdLoaderClient()
		{
			Dispose();
		}

		[MonoPInvokeCallback(typeof(GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback))]
		private static void AdLoaderDidReceiveNativeCustomTemplateAdCallback(IntPtr adLoader, IntPtr nativeCustomTemplateAd, string templateID)
		{
			AdLoaderClient adLoaderClient = IntPtrToAdLoaderClient(adLoader);
			Action<CustomNativeTemplateAd, string> clickHandler = (adLoaderClient.customNativeTemplateCallbacks.ContainsKey(templateID) ? adLoaderClient.customNativeTemplateCallbacks[templateID] : null);
			if (adLoaderClient.OnCustomNativeTemplateAdLoaded != null)
			{
				CustomNativeEventArgs e = new CustomNativeEventArgs
				{
					nativeAd = new CustomNativeTemplateAd(new CustomNativeTemplateClient(nativeCustomTemplateAd, clickHandler))
				};
				adLoaderClient.OnCustomNativeTemplateAdLoaded(adLoaderClient, e);
			}
		}

		[MonoPInvokeCallback(typeof(GADUAdLoaderDidFailToReceiveAdWithErrorCallback))]
		private static void AdLoaderDidFailToReceiveAdWithErrorCallback(IntPtr adLoader, string error)
		{
			AdLoaderClient adLoaderClient = IntPtrToAdLoaderClient(adLoader);
			if (adLoaderClient.OnAdFailedToLoad != null)
			{
				AdFailedToLoadEventArgs e = new AdFailedToLoadEventArgs
				{
					Message = error
				};
				adLoaderClient.OnAdFailedToLoad(adLoaderClient, e);
			}
		}

		private static AdLoaderClient IntPtrToAdLoaderClient(IntPtr adLoader)
		{
			return ((GCHandle)adLoader).Target as AdLoaderClient;
		}
	}
}
