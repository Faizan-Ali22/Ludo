using System;
using AssemblyCSharp;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinnerControllerScript : MonoBehaviour
{
	public GameObject myObject;

	public GameObject opponentObject;

	public GameObject shareButton;

	public bool isGameScene;

	public Image myImage;

	public Image oppoImage;

	public Text myName;

	public Text oppoText;

	public GameObject myMessageBubble;

	public GameObject oppoMessageBubble;

	public GameObject rematchButton;

	public bool rematchRequest;

	public bool sentRematch;

	public GameObject ChatMessagesList;

	public GameObject ChatMessageButtonPrefab;

	public GameObject ChatMessagesObject;

	public GameObject prizeText;

	private AudioSource[] audioSources;

	public GameObject reardShareText;

	public bool messageDialogVisible;

	private void Start()
	{
		audioSources = GetComponents<AudioSource>();
		if (GameManager.Instance.playerDisconnected)
		{
			GameManager.Instance.playerDisconnected = false;
			if (!isGameScene)
			{
				rematchButton.SetActive(value: false);
			}
		}
		if (!isGameScene)
		{
			PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
			if (GameManager.Instance.payoutCoins > GameManager.Instance.myPlayerData.GetCoins())
			{
				rematchButton.SetActive(value: false);
			}
			if (reardShareText != null)
			{
				reardShareText.GetComponent<Text>().text = "+" + StaticStrings.rewardCoinsForShareViaFacebook;
			}
			if (!PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
			{
				shareButton.SetActive(value: false);
			}
			rematchRequest = false;
			sentRematch = false;
			if (GameManager.Instance.iWon)
			{
				myObject.GetComponent<Animator>().Play("WinnerOpponentAnimation");
				audioSources[0].Play();
				GameManager.Instance.playfabManager.addCoinsRequest(GameManager.Instance.payoutCoins * 2);
			}
			else if (GameManager.Instance.iDraw)
			{
				myObject.GetComponent<Animator>().Play("WinnerOpponentAnimation");
				opponentObject.GetComponent<Animator>().Play("WinnerOpponentAnimation");
				audioSources[0].Play();
				GameManager.Instance.playfabManager.addCoinsRequest(GameManager.Instance.payoutCoins);
			}
			else
			{
				opponentObject.GetComponent<Animator>().Play("WinnerOpponentAnimation");
				audioSources[1].Play();
			}
			if (GameManager.Instance.avatarMy != null)
			{
				myImage.sprite = GameManager.Instance.avatarMy;
			}
			if (GameManager.Instance.avatarOpponent != null)
			{
				oppoImage.sprite = GameManager.Instance.avatarOpponent;
			}
			myName.text = GameManager.Instance.nameMy;
			oppoText.text = GameManager.Instance.nameOpponent;
			int num = GameManager.Instance.payoutCoins * 2;
			if (num >= 1000)
			{
				if (num >= 1000000)
				{
					if ((float)num % 1000000f == 0f)
					{
						prizeText.GetComponent<Text>().text = ((float)num / 1000000f).ToString("0") + "M";
					}
					else
					{
						prizeText.GetComponent<Text>().text = ((float)num / 1000000f).ToString("0.0") + "M";
					}
				}
				else if ((float)num % 1000f == 0f)
				{
					prizeText.GetComponent<Text>().text = ((float)num / 1000f).ToString("0") + "k";
				}
				else
				{
					prizeText.GetComponent<Text>().text = ((float)num / 1000f).ToString("0.0") + "k";
				}
			}
			else
			{
				prizeText.GetComponent<Text>().text = string.Concat(num);
			}
		}
		for (int i = 0; i < StaticStrings.chatMessages.Length; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(ChatMessageButtonPrefab);
			obj.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.chatMessages[i];
			obj.transform.parent = ChatMessagesList.transform;
			obj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			string index = StaticStrings.chatMessages[i];
			obj.GetComponent<Button>().onClick.RemoveAllListeners();
			obj.GetComponent<Button>().onClick.AddListener(delegate
			{
				SendMessageEvent(index);
			});
		}
		for (int num2 = 0; num2 < StaticStrings.chatMessagesExtended.Length; num2++)
		{
			if (!GameManager.Instance.myPlayerData.GetChats().Contains("'" + num2 + "'"))
			{
				continue;
			}
			for (int num3 = 0; num3 < StaticStrings.chatMessagesExtended[num2].Length; num3++)
			{
				GameObject obj2 = UnityEngine.Object.Instantiate(ChatMessageButtonPrefab);
				obj2.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.chatMessagesExtended[num2][num3];
				obj2.transform.parent = ChatMessagesList.transform;
				obj2.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
				string index2 = StaticStrings.chatMessagesExtended[num2][num3];
				obj2.GetComponent<Button>().onClick.RemoveAllListeners();
				obj2.GetComponent<Button>().onClick.AddListener(delegate
				{
					SendMessageEvent(index2);
				});
			}
		}
	}

	public void share()
	{
		if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
		{
			FB.ShareLink(new Uri("https://play.google.com/store/apps/details?id=" + StaticStrings.AndroidPackageName), StaticStrings.facebookShareLinkTitle, "", null, ShareCallback);
		}
	}

	private void ShareCallback(IShareResult result)
	{
		if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
		{
			DConsole.Log("ShareLink Error: " + result.Error);
			return;
		}
		if (!string.IsNullOrEmpty(result.PostId))
		{
			DConsole.Log(result.PostId);
			return;
		}
		GameManager.Instance.playfabManager.addCoinsRequest(StaticStrings.rewardCoinsForShareViaFacebook);
		DConsole.Log("ShareLink success!");
	}

	private void OnDestroy()
	{
		removeOnEventCall();
	}

	public void SendMessageEvent(string index)
	{
		DConsole.Log("Button Clicked " + index);
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(193, index, sendReliable: true, null);
		}
		ChatMessagesObject.GetComponent<Animator>().Play("hideMessageDialog");
		messageDialogVisible = false;
		if (isGameScene)
		{
			myMessageBubble.SetActive(value: true);
			myMessageBubble.transform.GetChild(0).GetComponent<Text>().text = index;
			if (isGameScene)
			{
				CancelInvoke("hideMyMessageBubble");
				Invoke("hideMyMessageBubble", 6f);
			}
		}
	}

	public void loadMenuScene()
	{
		SceneManager.LoadScene("MenuScene");
		DConsole.Log("Timeout 6");
		PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(194, 1, sendReliable: true, null);
		}
		removeOnEventCall();
		GameManager.Instance.cueController.removeOnEventCall();
		PhotonNetwork.LeaveRoom();
		GameManager.Instance.playfabManager.roomOwner = false;
		GameManager.Instance.roomOwner = false;
		GameManager.Instance.resetAllData();
	}

	public void sendRematchRequest()
	{
		if (!rematchRequest)
		{
			sentRematch = true;
			DConsole.Log("Send message");
			if (!GameManager.Instance.offlineMode)
			{
				PhotonNetwork.RaiseEvent(195, 1, sendReliable: true, null);
			}
			myMessageBubble.SetActive(value: true);
			myMessageBubble.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.IWantPlayAgain;
			rematchButton.SetActive(value: false);
			return;
		}
		DConsole.Log("Send message");
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(195, 1, sendReliable: true, null);
		}
		rematchButton.SetActive(value: false);
		GameManager.Instance.resetAllData();
		GameManager.Instance.GameScene = "GameScene";
		if (!GameManager.Instance.gameSceneStarted)
		{
			SceneManager.LoadScene(GameManager.Instance.GameScene);
			GameManager.Instance.gameSceneStarted = true;
		}
		removeOnEventCall();
	}

	public void sendMessageButton()
	{
		ChatMessagesObject.GetComponent<Animator>().Play("showMessagesDialog");
		messageDialogVisible = true;
	}

	private void Awake()
	{
		PhotonNetwork.OnEventCall += OnEvent;
	}

	public void removeOnEventCall()
	{
		PhotonNetwork.OnEventCall -= OnEvent;
	}

	private void OnEvent(byte eventcode, object content, int senderid)
	{
		DConsole.Log("Received message");
		switch (eventcode)
		{
		case 195:
			if (sentRematch)
			{
				GameManager.Instance.resetAllData();
				GameManager.Instance.GameScene = "GameScene";
				if (!GameManager.Instance.gameSceneStarted)
				{
					SceneManager.LoadScene(GameManager.Instance.GameScene);
					GameManager.Instance.gameSceneStarted = true;
				}
				removeOnEventCall();
			}
			else
			{
				rematchRequest = true;
				if (GameManager.Instance.payoutCoins <= GameManager.Instance.myPlayerData.GetCoins())
				{
					oppoMessageBubble.SetActive(value: true);
					oppoMessageBubble.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.IWantPlayAgain;
				}
			}
			break;
		case 194:
			rematchButton.SetActive(value: false);
			oppoMessageBubble.SetActive(value: true);
			oppoMessageBubble.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.cantPlayRightNow;
			break;
		case 193:
		{
			string text = (string)content;
			DConsole.Log("INDEX: " + text);
			oppoMessageBubble.SetActive(value: true);
			oppoMessageBubble.transform.GetChild(0).GetComponent<Text>().text = text;
			if (isGameScene)
			{
				CancelInvoke("hideOppoMessageBubble");
				Invoke("hideOppoMessageBubble", 6f);
			}
			break;
		}
		}
	}

	public void hideOppoMessageBubble()
	{
		oppoMessageBubble.SetActive(value: false);
	}

	public void hideMyMessageBubble()
	{
		myMessageBubble.SetActive(value: false);
	}
}
