using UnityEngine;

namespace AudienceNetwork.Utility
{
	internal class AdUtilityBridge : IAdUtilityBridge
	{
		public static readonly IAdUtilityBridge Instance;

		internal AdUtilityBridge()
		{
		}

		static AdUtilityBridge()
		{
			Instance = CreateInstance();
		}

		private static IAdUtilityBridge CreateInstance()
		{
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				return new AdUtilityBridgeAndroid();
			}
			return new AdUtilityBridge();
		}

		public virtual double DeviceWidth()
		{
			return 2208.0;
		}

		public virtual double DeviceHeight()
		{
			return 1242.0;
		}

		public virtual double Width()
		{
			return 1104.0;
		}

		public virtual double Height()
		{
			return 621.0;
		}

		public virtual double Convert(double deviceSize)
		{
			return 2.0;
		}

		public virtual void Prepare()
		{
		}
	}
}
