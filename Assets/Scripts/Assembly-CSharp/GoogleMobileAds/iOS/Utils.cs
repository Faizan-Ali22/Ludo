using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation;

namespace GoogleMobileAds.iOS
{
	internal class Utils
	{
		public static IntPtr BuildAdRequest(AdRequest request)
		{
			IntPtr intPtr = Externs.GADUCreateRequest();
			foreach (string keyword in request.Keywords)
			{
				Externs.GADUAddKeyword(intPtr, keyword);
			}
			foreach (string testDevice in request.TestDevices)
			{
				Externs.GADUAddTestDevice(intPtr, testDevice);
			}
			if (request.Birthday.HasValue)
			{
				DateTime valueOrDefault = request.Birthday.GetValueOrDefault();
				Externs.GADUSetBirthday(intPtr, valueOrDefault.Year, valueOrDefault.Month, valueOrDefault.Day);
			}
			if (request.Gender.HasValue)
			{
				Externs.GADUSetGender(intPtr, (int)request.Gender.GetValueOrDefault());
			}
			if (request.TagForChildDirectedTreatment.HasValue)
			{
				Externs.GADUTagForChildDirectedTreatment(intPtr, request.TagForChildDirectedTreatment == true);
			}
			foreach (KeyValuePair<string, string> extra in request.Extras)
			{
				Externs.GADUSetExtra(intPtr, extra.Key, extra.Value);
			}
			Externs.GADUSetExtra(intPtr, "is_unity", "1");
			foreach (MediationExtras mediationExtra in request.MediationExtras)
			{
				IntPtr intPtr2 = Externs.GADUCreateMutableDictionary();
				if (!(intPtr2 != IntPtr.Zero))
				{
					continue;
				}
				foreach (KeyValuePair<string, string> extra2 in mediationExtra.Extras)
				{
					Externs.GADUMutableDictionarySetValue(intPtr2, extra2.Key, extra2.Value);
				}
				Externs.GADUSetMediationExtras(intPtr, intPtr2, mediationExtra.IOSMediationExtraBuilderClassName);
			}
			Externs.GADUSetRequestAgent(intPtr, "unity-5.0.0");
			return intPtr;
		}

		public static IntPtr BuildServerSideVerificationOptions(ServerSideVerificationOptions options)
		{
			IntPtr intPtr = Externs.GADUCreateServerSideVerificationOptions();
			Externs.GADUServerSideVerificationOptionsSetUserId(intPtr, options.UserId);
			Externs.GADUServerSideVerificationOptionsSetCustomRewardString(intPtr, options.CustomData);
			return intPtr;
		}

		public static string PtrToString(IntPtr stringPtr)
		{
			string result = Marshal.PtrToStringAnsi(stringPtr);
			Marshal.FreeHGlobal(stringPtr);
			return result;
		}

		public static List<string> PtrArrayToManagedList(IntPtr arrayPtr, int numOfAssets)
		{
			IntPtr[] array = new IntPtr[numOfAssets];
			string[] array2 = new string[numOfAssets];
			Marshal.Copy(arrayPtr, array, 0, numOfAssets);
			for (int i = 0; i < numOfAssets; i++)
			{
				array2[i] = Marshal.PtrToStringAuto(array[i]);
				Marshal.FreeHGlobal(array[i]);
			}
			Marshal.FreeHGlobal(arrayPtr);
			return new List<string>(array2);
		}
	}
}
