using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
	internal class InitializationStatusClient : IInitializationStatusClient
	{
		private IntPtr status;

		public InitializationStatusClient(IntPtr status)
		{
			this.status = status;
		}

		public AdapterStatus getAdapterStatusForClassName(string className)
		{
			string description = Utils.PtrToString(Externs.GADUGetInitDescription(status, className));
			int latency = Externs.GADUGetInitLatency(status, className);
			return new AdapterStatus((AdapterState)Externs.GADUGetInitState(status, className), description, latency);
		}

		public Dictionary<string, AdapterStatus> getAdapterStatusMap()
		{
			Dictionary<string, AdapterStatus> dictionary = new Dictionary<string, AdapterStatus>();
			foreach (string adapterClassName in GetAdapterClassNames())
			{
				dictionary.Add(adapterClassName, getAdapterStatusForClassName(adapterClassName));
			}
			return dictionary;
		}

		public List<string> GetAdapterClassNames()
		{
			IntPtr arrayPtr = Externs.GADUGetInitAdapterClasses(status);
			int numOfAssets = Externs.GADUGetInitNumberOfAdapterClasses(status);
			return Utils.PtrArrayToManagedList(arrayPtr, numOfAssets);
		}

		public void Dispose()
		{
			Externs.GADURelease(status);
		}

		~InitializationStatusClient()
		{
			Dispose();
		}
	}
}
