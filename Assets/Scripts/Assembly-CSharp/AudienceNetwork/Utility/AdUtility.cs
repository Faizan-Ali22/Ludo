using UnityEngine;

namespace AudienceNetwork.Utility
{
	public static class AdUtility
	{
		internal static double Width()
		{
			return AdUtilityBridge.Instance.Width();
		}

		internal static double Height()
		{
			return AdUtilityBridge.Instance.Height();
		}

		internal static double Convert(double deviceSize)
		{
			return AdUtilityBridge.Instance.Convert(deviceSize);
		}

		internal static void Prepare()
		{
			AdUtilityBridge.Instance.Prepare();
		}

		internal static bool IsLandscape()
		{
			if (Screen.orientation != ScreenOrientation.LandscapeLeft && Screen.orientation != ScreenOrientation.LandscapeLeft)
			{
				return Screen.orientation == ScreenOrientation.LandscapeRight;
			}
			return true;
		}
	}
}
