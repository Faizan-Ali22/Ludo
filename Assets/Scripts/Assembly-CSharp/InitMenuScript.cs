using System.Collections.Generic;
using AssemblyCSharp;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
//using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitMenuScript : MonoBehaviour
{
	public GameObject rateWindow;

	public GameObject FacebookLinkReward;

	public GameObject rewardDialogText;

	public GameObject FacebookLinkButton;

	public GameObject playerName;

	public GameObject videoRewardText;

	public GameObject playerAvatar;

	public GameObject fbFriendsMenu;

	public GameObject matchPlayer;

	public GameObject backButtonMatchPlayers;

	public GameObject MatchPlayersCanvas;

	public GameObject menuCanvas;

	public GameObject tablesCanvas;

	public GameObject gameTitle;

	public GameObject changeDialog;

	public GameObject inputNewName;

	public GameObject tooShortText;

	public GameObject coinsText;

	public GameObject coinsTextShop;

	public GameObject coinsTab;

	public GameObject TheMillButton;

	public GameObject dialog;

	public GameObject GameConfigurationScreen;

	public GameObject FourPlayerMenuButton;

	private void Start()
	{
		if (PlayerPrefs.GetInt(StaticStrings.SoundsKey, 0) == 0)
		{
			AudioListener.volume = 1f;
		}
		else
		{
			AudioListener.volume = 0f;
		}
		FacebookLinkReward.GetComponent<Text>().text = "+ " + StaticStrings.CoinsForLinkToFacebook;
		if (!StaticStrings.isFourPlayerModeEnabled)
		{
			FourPlayerMenuButton.SetActive(value: false);
		}
		GameManager.Instance.FacebookLinkButton = FacebookLinkButton;
		GameManager.Instance.dialog = dialog;
		videoRewardText.GetComponent<Text>().text = "+" + StaticStrings.rewardForVideoAd;
		GameManager.Instance.tablesCanvas = tablesCanvas;
		GameManager.Instance.facebookFriendsMenu = fbFriendsMenu.GetComponent<FacebookFriendsMenu>();
		GameManager.Instance.matchPlayerObject = matchPlayer;
		GameManager.Instance.backButtonMatchPlayers = backButtonMatchPlayers;
		playerName.GetComponent<Text>().text = GameManager.Instance.nameMy;
		GameManager.Instance.MatchPlayersCanvas = MatchPlayersCanvas;
		if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
		{
			FacebookLinkButton.SetActive(value: false);
		}
		if (GameManager.Instance.avatarMy != null)
		{
			playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
		}
		GameManager.Instance.myAvatarGameObject = playerAvatar;
		GameManager.Instance.myNameGameObject = playerName;
		GameManager.Instance.coinsTextMenu = coinsText;
		GameManager.Instance.coinsTextShop = coinsTextShop;
		GameManager.Instance.initMenuScript = this;
		if (StaticStrings.hideCoinsTabInShop)
		{
			coinsTab.SetActive(value: false);
		}
		rewardDialogText.GetComponent<Text>().text = "1 Video = " + StaticStrings.rewardForVideoAd + " Coins";
		DConsole.Log("Load ad menu");
		AdsManager.Instance.adsScript.ShowAd(AdLocation.GameStart);
		if (PlayerPrefs.GetInt("GamesPlayed", 1) % 8 == 0 && PlayerPrefs.GetInt("GameRated", 0) == 0)
		{
			rateWindow.SetActive(value: true);
			PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 1) + 1);
		}
	}

	public void QuitApp()
	{
		PlayerPrefs.SetInt("GameRated", 1);
		Application.OpenURL("market://details?id=" + StaticStrings.AndroidPackageName);
	}

	public void LinkToFacebook()
	{
		GameManager.Instance.facebookManager.FBLinkAccount();
	}

	public void ShowGameConfiguration(int index)
	{
		switch (index)
		{
		case 0:
			GameManager.Instance.type = MyGameType.TwoPlayer;
			break;
		case 1:
			GameManager.Instance.type = MyGameType.FourPlayer;
			break;
		case 2:
			GameManager.Instance.type = MyGameType.Private;
			break;
		}
		GameConfigurationScreen.SetActive(value: true);
		AdsManager.Instance.adsScript.ShowAd(AdLocation.GamePropertiesWindow);
	}

	public void TakeScreenshot()
	{
		ScreenCapture.CaptureScreenshot("TestScreenshot.png");
	}

	private void Update()
	{
	}

	public void showAdStore()
	{
		AdsManager.Instance.adsScript.ShowAd(AdLocation.StoreWindow);
	}

	public void backToMenuFromTableSelect()
	{
		GameManager.Instance.offlineMode = false;
		tablesCanvas.SetActive(value: false);
		menuCanvas.SetActive(value: true);
		gameTitle.SetActive(value: true);
	}

	public void showSelectTableScene(bool challengeFriend)
	{
		if (!challengeFriend)
		{
			GameManager.Instance.inviteFriendActivated = false;
		}
		AdsManager.Instance.adsScript.ShowAd(AdLocation.GameStart);
		if (GameManager.Instance.offlineMode)
		{
			TheMillButton.SetActive(value: false);
		}
		else
		{
			TheMillButton.SetActive(value: true);
		}
		menuCanvas.SetActive(value: false);
		tablesCanvas.SetActive(value: true);
		gameTitle.SetActive(value: false);
	}

	public void playOffline()
	{
		GameManager.Instance.offlineMode = true;
		GameManager.Instance.roomOwner = true;
		showSelectTableScene(challengeFriend: false);
	}

	public void switchUser()
	{
		GameManager.Instance.playfabManager.destroy();
		GameManager.Instance.facebookManager.destroy();
		GameManager.Instance.connectionLost.destroy();
		GameManager.Instance.avatarMy = null;
		PhotonNetwork.Disconnect();
		PlayerPrefs.DeleteAll();
		GameManager.Instance.resetAllData();
		LocalNotification.ClearNotifications();
		SceneManager.LoadScene("LoginSplash");
	}

	public void showChangeDialog()
	{
		changeDialog.SetActive(value: true);
	}

	public void changeUserName()
	{
		DConsole.Log("Change Nickname");
		string newName = inputNewName.GetComponent<Text>().text;
		if (newName.Equals(StaticStrings.addCoinsHackString))
		{
			GameManager.Instance.playfabManager.addCoinsRequest(1000000);
			changeDialog.SetActive(value: false);
		}
		else if (newName.Length > 0)
		{
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = GameManager.Instance.playfabManager.PlayFabId
			}, delegate
			{
				Dictionary<string, string> data = new Dictionary<string, string> { { "PlayerName", newName } };
				PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
				{
					Data = data,
					Permission = UserDataPermission.Public
				}, delegate
				{
					DConsole.Log("Data updated successfull ");
					DConsole.Log("Title Display name updated successfully");
					PlayerPrefs.SetString("GuestPlayerName", newName);
					PlayerPrefs.Save();
					GameManager.Instance.nameMy = newName;
					playerName.GetComponent<Text>().text = newName;
				}, delegate(PlayFabError error1)
				{
					DConsole.Log("Data updated error " + error1.ErrorMessage);
				});
			}, delegate(PlayFabError error)
			{
				DConsole.Log("Title Display name updated error: " + error.Error);
			});
			changeDialog.SetActive(value: false);
		}
		else
		{
			tooShortText.SetActive(value: true);
		}
	}

	public void startQuickGame()
	{
		GameManager.Instance.facebookManager.startRandomGame();
	}

	public void startQuickGameTableNumer(int tableNumer, int fee)
	{
		GameManager.Instance.payoutCoins = fee;
		GameManager.Instance.tableNumber = tableNumer;
		GameManager.Instance.facebookManager.startRandomGame();
	}

	public void showFacebookFriends()
	{
		AdsManager.Instance.adsScript.ShowAd(AdLocation.FacebookFriends);
		GameManager.Instance.playfabManager.GetPlayfabFriends();
	}

	public void setTableNumber()
	{
		GameManager.Instance.tableNumber = int.Parse(GameObject.Find("TextTableNumber").GetComponent<Text>().text);
	}

	public void ShowRewardedAd()
	{
		// if (Advertisement.IsReady("rewardedVideo"))
		// {
		// 	ShowOptions showOptions = new ShowOptions
		// 	{
		// 		resultCallback = HandleShowResult
		// 	};
		// 	Advertisement.Show("rewardedVideo", showOptions);
		// }
	}

	/* private void HandleShowResult(int result) {} */
}



