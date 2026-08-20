using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdMobObjectController : MonoBehaviour
{
	private bool AdShowed = true;

	private bool myGame;

	private int showAttempts;

	public bool loadedAdmob;

	private string AndroidCallerID = "android";

	private UnityWebRequest www_image;

	private Texture2D texture;

	public string[] frames;

	public int[] adsShowed;

	private string APIMainURL = "http://houseadsserver.com/ServerPlay/";

	private string APIUrl = "";

	private bool canPushButtons = true;

	public GameObject admobAdsObject;

	public GameObject adView;

	public GameObject loadingPanel;

	public GameObject adsController;

	private AdsController adControl;

	private string storeAppID;

	private bool isVisible;

	private float volume;

	private void Start()
	{
	}

	public void Init()
	{
		APIUrl = APIMainURL + "/default.php?";
		adControl = adsController.GetComponent<AdsController>();
		AdsManager.Instance.adsScript = this;
		if (!AdMobObjectSingleton.Instance.houseAdDisplayed)
		{
			_ = AdShowed;
		}
	}

	public void destroy()
	{
		if (admobAdsObject != null)
		{
			Object.DestroyImmediate(admobAdsObject);
		}
	}

	private IEnumerator DownloadAdData()
	{
		string os = "android";
		string calling_app = AndroidCallerID;
		yield return new WaitForSeconds(2f);
		UnityWebRequest www = UnityWebRequest.Get(APIUrl + "os=" + os + "&calling_app=" + calling_app);
		yield return www.SendWebRequest();
		if (www.error == null && www.downloadHandler.text.Contains("API_DATA_BEGIN|"))
		{
			string text = www.downloadHandler.text.Substring(www.downloadHandler.text.IndexOf("API_DATA_BEGIN|") + "API_DATA_BEGIN|".Length, www.downloadHandler.text.Length - "API_DATA_BEGIN|".Length - "|API_DATA_END".Length - 1);
			string[] array = text.Split(';');
			frames = array[1].Split('-');
			adsShowed = new int[frames.Length];
			for (int i = 0; i < frames.Length; i++)
			{
				adsShowed[i] = int.Parse(frames[i]);
			}
			text = (storeAppID = array[0]);
			string uri = "";
			if (Screen.orientation == ScreenOrientation.LandscapeLeft)
			{
				uri = APIMainURL + "Android_PNG/Landscape/" + text + ".png";
			}
			else if (Screen.orientation == ScreenOrientation.Portrait)
			{
				uri = APIMainURL + "Android_PNG/Portrait/" + text + ".png";
			}
			www_image = UnityWebRequestTexture.GetTexture(uri);
			texture = ((DownloadHandlerTexture)www_image.downloadHandler).texture;
		}
	}

	public void ShowAd(AdLocation location)
	{
		if (!loadedAdmob || (location == AdLocation.GameOver && !adControl.ShowAdOnGameOver) || (location == AdLocation.GameStart && !adControl.ShowAdOnMenuScene) || (location == AdLocation.Pause && !adControl.ShowAdOnPause) || (location == AdLocation.LevelComplete && !adControl.ShowAdOnLevelFinish) || (location == AdLocation.FacebookFriends && !adControl.ShowAdOnFacebookFriends) || (location == AdLocation.GameFinishWindow && !adControl.ShowAdOnGameFinishWindow) || (location == AdLocation.StoreWindow && !adControl.ShowAdOnStoreWindow) || (location == AdLocation.GamePropertiesWindow && !adControl.ShowAdOnGamePropertiesWindow))
		{
			return;
		}
		showAttempts++;
		bool flag = false;
		for (int i = 0; i < adsShowed.Length; i++)
		{
			if (adsShowed[i] == showAttempts)
			{
				flag = true;
				break;
			}
		}
		if (myGame)
		{
			flag = false;
		}
		if (flag)
		{
			if (www_image != null && www_image.error == null && texture.width != 8 && texture.height != 8)
			{
				adView.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				loadingPanel.SetActive(value: true);
				AdMobObjectSingleton.Instance.houseAdDisplayed = true;
				Invoke("enableButtons", 2f);
				Screen.fullScreen = false;
				isVisible = true;
				volume = AudioListener.volume;
				AudioListener.volume = 0f;
			}
			else
			{
				GameManager.Instance.adsController.loadAd(location);
			}
		}
		else
		{
			GameManager.Instance.adsController.loadAd(location);
		}
	}

	public void LoadHouse()
	{
		if (PlayerPrefs.GetInt("HouseTryLoad", 0) >= 2 && adsShowed[0] == 1)
		{
			if (www_image != null && www_image.error == null && texture.width != 8 && texture.height != 8 && !(storeAppID == Application.identifier))
			{
				adView.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				loadingPanel.SetActive(value: true);
				AdMobObjectSingleton.Instance.houseAdDisplayed = true;
				Invoke("enableButtons", 2f);
				Screen.fullScreen = false;
				isVisible = true;
				volume = AudioListener.volume;
				AudioListener.volume = 0f;
			}
		}
		else
		{
			PlayerPrefs.SetInt("HouseTryLoad", PlayerPrefs.GetInt("HouseTryLoad", 0) + 1);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && isVisible)
		{
			loadingPanel.SetActive(value: false);
			Screen.fullScreen = true;
			isVisible = false;
			AudioListener.volume = volume;
		}
	}

	public void openAppStore()
	{
		if (canPushButtons)
		{
			DConsole.Log("Open store!!");
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", new object[1] { "market://details?id=" + storeAppID });
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.content.Intent", androidJavaClass.GetStatic<string>("ACTION_VIEW"), androidJavaObject);
			new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call("startActivity", androidJavaObject2);
		}
	}

	public void closeAd()
	{
		if (canPushButtons)
		{
			loadingPanel.SetActive(value: false);
			Screen.fullScreen = true;
			isVisible = false;
			AudioListener.volume = volume;
		}
	}

	private void enableButtons()
	{
		canPushButtons = true;
	}
}
