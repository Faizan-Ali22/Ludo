using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour
{
	public GameObject facebookLoginButton;

	public GameObject guestLoginButton;

	private PlayFabManager playFabManager;

	public string fbName;

	public Sprite sprite;

	private bool LoggedIn;

	private FacebookFriendsMenu facebookFriendsMenu;

	private bool alreadyGotFriends;

	public GameObject splashCanvas;

	public GameObject loginCanvas;

	public GameObject fbButton;

	public GameObject matchPlayersCanvas;

	public GameObject menuCanvas;

	public GameObject gameTitle;

	public GameObject idLoginDialog;

	public GameObject idRegisterDialog;

	public GameObject forgetPasswordDialog;

	public InputField loginEmail;

	public InputField loginPassword;

	public GameObject loginInvalidEmailorPassword;

	public InputField regiterEmail;

	public InputField registerPassword;

	public InputField registerNickname;

	public GameObject registerInvalidInput;

	public InputField resetPasswordEmail;

	public GameObject resetPasswordInformationText;

	private void Start()
	{
		DConsole.Log("FBManager start");
		GameManager.Instance.facebookManager = this;
		Screen.sleepTimeout = -1;
		facebookFriendsMenu = GameManager.Instance.facebookFriendsMenu;
	}

	private void Awake()
	{
		DConsole.Log("FBManager awake");
		GameManager.Instance.facebookManager = this;
		Object.DontDestroyOnLoad(base.transform.gameObject);
		playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();
		if (!GameManager.Instance.logged)
		{
			if (!FB.IsInitialized)
			{
				FB.Init(InitCallback, OnHideUnity);
			}
			else
			{
				FB.ActivateApp();
				initSession();
			}
			GameManager.Instance.logged = true;
		}
	}

	private void InitCallback()
	{
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
			initSession();
		}
		else
		{
			DConsole.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	public void startRandomGame()
	{
		GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
		GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().setBackButton(active: true);
		playFabManager.JoinRoomAndStartGame();
	}

	public void FBLogin()
	{
		if (!LoggedIn)
		{
			FB.LogInWithReadPermissions(new List<string> { "public_profile", "email", "user_friends" }, AuthCallback);
		}
		else
		{
			playFabManager.JoinRoomAndStartGame();
		}
	}

	public void FBLinkAccount()
	{
		GameManager.Instance.LinkFbAccount = true;
		FBLogin();
	}

	public void FBLoginWithoutLink()
	{
		GameManager.Instance.LinkFbAccount = false;
		FBLogin();
	}

	public void GuestLogin()
	{
		if (!LoggedIn)
		{
			playFabManager.Login();
		}
	}

	public void showRegisterDialog()
	{
		idLoginDialog.SetActive(value: false);
		idRegisterDialog.SetActive(value: true);
	}

	public void CloseLoginDialog()
	{
		loginInvalidEmailorPassword.SetActive(value: false);
		loginEmail.text = "";
		loginPassword.text = "";
		loginCanvas.SetActive(value: true);
		idLoginDialog.SetActive(value: false);
	}

	public void CloseRegisterDialog()
	{
		regiterEmail.text = "";
		registerPassword.text = "";
		registerNickname.text = "";
		registerInvalidInput.SetActive(value: false);
		loginCanvas.SetActive(value: true);
		idRegisterDialog.SetActive(value: false);
	}

	public void CloseForgetPasswordDialog()
	{
		resetPasswordEmail.text = "";
		resetPasswordInformationText.SetActive(value: false);
		forgetPasswordDialog.SetActive(value: false);
		loginCanvas.SetActive(value: true);
	}

	public void showForgetPasswordDialog()
	{
		forgetPasswordDialog.SetActive(value: true);
		idLoginDialog.SetActive(value: false);
	}

	public void IDLoginButtonPressed()
	{
		loginCanvas.SetActive(value: false);
		idLoginDialog.SetActive(value: true);
	}

	public void IDLogin()
	{
		if (!LoggedIn)
		{
			FB.LogInWithReadPermissions(new List<string> { "public_profile", "email", "user_friends" }, AuthCallback);
		}
	}

	private void AuthCallback(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{
			AccessToken currentAccessToken = AccessToken.CurrentAccessToken;
			GameManager.Instance.facebookIDMy = currentAccessToken.UserId;
			DConsole.Log(currentAccessToken.ToJson());
			foreach (string permission in currentAccessToken.Permissions)
			{
				DConsole.Log(permission);
			}
			PlayerPrefs.SetString("LoggedType", "Facebook");
			PlayerPrefs.Save();
			if (!GameManager.Instance.LinkFbAccount)
			{
				loginCanvas.SetActive(value: false);
				splashCanvas.SetActive(value: true);
			}
			initSession();
		}
		else
		{
			facebookLoginButton.GetComponent<Button>().interactable = true;
			guestLoginButton.GetComponent<Button>().interactable = true;
			DConsole.Log("User cancelled login");
		}
	}

	private void initSession()
	{
		DConsole.Log("FbManager init session");
		string text = PlayerPrefs.GetString("LoggedType");
		if (text.Equals("Facebook"))
		{
			GameManager.Instance.facebookIDMy = AccessToken.CurrentAccessToken.UserId;
			callApiToGetName();
			getMyProfilePicture(GameManager.Instance.facebookIDMy);
			LoggedIn = true;
		}
		else if (text.Equals("Guest"))
		{
			playFabManager.Login();
		}
		else if (text.Equals("EmailAccount"))
		{
			playFabManager.LoginWithEmailAccount();
		}
	}

	private void callApiToGetName()
	{
		FB.API("me?fields=name", HttpMethod.GET, APICallbackName);
	}

	private void APICallbackName(IResult response)
	{
		GameManager.Instance.nameMy = response.ResultDictionary["name"].ToString();
		DConsole.Log("My name " + GameManager.Instance.nameMy);
	}

	public void getMyProfilePicture(string userID)
	{
		FB.API("/me?fields=picture.width(200).height(200)", HttpMethod.GET, delegate(IGraphResult result)
		{
			if (result.Error == null)
			{
				Dictionary<string, object> dictionary = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
				if (dictionary == null)
				{
					DConsole.Log("JEST NULL");
				}
				else
				{
					DConsole.Log("nie null");
				}
				GameManager.Instance.avatarMyUrl = ((dictionary["picture"] as Dictionary<string, object>)["data"] as Dictionary<string, object>)["url"] as string;
				DConsole.Log("My avatar " + GameManager.Instance.avatarMyUrl);
				StartCoroutine(loadImageMy(GameManager.Instance.avatarMyUrl));
				if (GameManager.Instance.LinkFbAccount)
				{
					playFabManager.LinkFacebookAccount();
				}
				else
				{
					playFabManager.LoginWithFacebook();
				}
			}
			else
			{
				DConsole.Log("Error retreiving image: " + result.Error);
			}
		});
	}

	public IEnumerator loadImageMy(string url)
	{
		UnityWebRequest www = UnityWebRequest.Get(url);
		yield return www.SendWebRequest();
		Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
		GameManager.Instance.avatarMy = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 32f);
		GameManager.Instance.facebookAvatar = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 32f);
	}

	public void getOpponentProfilePicture(string userID)
	{
		FB.API("/" + userID + "/picture?type=square&height=92&width=92", HttpMethod.GET, delegate(IGraphResult result)
		{
			if (result.Texture != null)
			{
				GameManager.Instance.avatarMy = Sprite.Create(result.Texture, new Rect(0f, 0f, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32f);
				playFabManager.LoginWithFacebook();
			}
		});
	}

	public void getFacebookInvitableFriends()
	{
		if (alreadyGotFriends)
		{
			facebookFriendsMenu.showFriends();
			return;
		}
		FB.API("/me/invitable_friends?limit=5000&fields=id,name,picture.width(100).height(100)", HttpMethod.GET, delegate(IGraphResult result)
		{
			if (result.Error == null)
			{
				List<object> list = (Json.Deserialize(result.RawResult) as Dictionary<string, object>)["data"] as List<object>;
				DConsole.Log("Friends Count: " + list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					Dictionary<string, object> obj = list[i] as Dictionary<string, object>;
					string friendsNames = obj["name"] as string;
					string friendsIDs = obj["id"] as string;
					string friendsAvatars = ((obj["picture"] as Dictionary<string, object>)["data"] as Dictionary<string, object>)["url"] as string;
					GameManager.Instance.facebookFriendsMenu.AddFacebookFriend(friendsNames, friendsIDs, friendsAvatars);
				}
			}
			else
			{
				DConsole.Log("Something went wrong. " + result.Error + "  " + result.ToString());
			}
		});
	}

	public void destroy()
	{
		if (base.gameObject != null)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}

	public void showLoadingCanvas()
	{
		loginCanvas.SetActive(value: false);
		splashCanvas.SetActive(value: true);
	}
}
