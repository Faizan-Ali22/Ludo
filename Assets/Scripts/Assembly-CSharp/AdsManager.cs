public class AdsManager
{
	private static AdsManager _instance;

	public AdsController interstitialAds;

	public AdMobObjectController adsScript;

	public static AdsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AdsManager();
			}
			return _instance;
		}
	}

	public void showAd(AdLocation location)
	{
		adsScript.ShowAd(location);
	}
}
