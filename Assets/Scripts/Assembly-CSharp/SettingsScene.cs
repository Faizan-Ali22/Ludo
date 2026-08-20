using AudienceNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScene : MonoBehaviour
{
	private static string URL_PREFIX_KEY = "URL_PREFIX";

	public InputField urlPrefixInput;

	public Text sdkVersionText;

	private string urlPrefix;

	public static void InitializeSettings()
	{
		AdSettings.SetUrlPrefix(PlayerPrefs.GetString(URL_PREFIX_KEY, ""));
	}

	private void Start()
	{
		urlPrefix = PlayerPrefs.GetString(URL_PREFIX_KEY, "");
		urlPrefixInput.text = urlPrefix;
		sdkVersionText.text = SdkVersion.Build;
	}

	public void OnEditEnd(string prefix)
	{
		urlPrefix = prefix;
		SaveSettings();
	}

	public void SaveSettings()
	{
		PlayerPrefs.SetString(URL_PREFIX_KEY, urlPrefix);
		AdSettings.SetUrlPrefix(urlPrefix);
	}

	public void AdViewScene()
	{
		SceneManager.LoadScene("AdViewScene");
	}

	public void InterstitialAdScene()
	{
		SceneManager.LoadScene("InterstitialAdScene");
	}

	public void RewardedVideoAdScene()
	{
		SceneManager.LoadScene("RewardedVideoAdScene");
	}
}
