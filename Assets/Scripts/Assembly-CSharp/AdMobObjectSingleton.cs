public class AdMobObjectSingleton
{
	private static AdMobObjectSingleton instance;

	public bool houseAdDisplayed;

	public static AdMobObjectSingleton Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AdMobObjectSingleton();
			}
			return instance;
		}
	}

	private AdMobObjectSingleton()
	{
	}
}
