using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class ControlAvatars : MonoBehaviour
{
	public AudioSource playerJoin;

	public AudioSource playerLeft;

	public GameObject FailedToJoinRoomWindow;

	public GameObject FailedToJoinRoomText;

	public GameObject CancelButton;

	public GameObject startButtonPrivate;

	public GameObject RoomIDObject;

	public GameObject RoomIDText;

	public GameObject[] OppoProgressBar;

	public GameObject[] OppoAvatar;

	public GameObject[] OppoAvatarImage;

	public GameObject[] InviteToJoinButtons;

	public GameObject prefab;

	private List<GameObject> avatars;

	private GameObject lastAvatar;

	public bool foundPlayer;

	public bool playerRejected;

	public float speed;

	private float speed1;

	public bool foundCancel;

	public Sprite prite;

	private GameObject OpponentAvatar;

	public Text opponentNameText;

	public Sprite noAvatarSprite;

	public GameObject cancelGameButton;

	public GameObject menuCanvas;

	public GameObject titleCanvas;

	public GameObject matchPlayersCanvas;

	public GameObject AvatarFrameMy;

	public GameObject AvatarFrameOpponent;

	public GameObject vsText;

	public GameObject centerCoins;

	public GameObject leftcoins;

	public GameObject rightCoins;

	public GameObject oppontentCoinImage;

	public GameObject myCoinImage;

	public GameObject oppontentPayoutCoins;

	public GameObject myPayoutCoins;

	public GameObject centerPayoutCoins;

	private AudioSource[] audioSources;

	public GameObject cantPlayNowOppo;

	public GameObject longTimeMessage;

	public float waitingOpponentTime;

	public GameObject messageBubbleText;

	public GameObject messageBubble;

	public bool opponentActive = true;

	public bool GameSceneLoaded;

	private bool changedAvatar;

	private bool startedGame;

	private Text oppontent;

	private Text my;

	private Text center;

	private Image opImage;

	private Image myImage;

	private Image lc;

	private void Awake()
	{
		audioSources = GetComponents<AudioSource>();
	}

	private void Start()
	{
		GameManager.Instance.controlAvatars = this;
	}

	public void CancelWaitingForPlayer()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void ShowJoinFailed(string error)
	{
		FailedToJoinRoomWindow.SetActive(value: true);
	}

	public void reset()
	{
		startButtonPrivate.GetComponent<Button>().interactable = false;
		if (GameManager.Instance.type == MyGameType.Private && !GameManager.Instance.JoinedByID)
		{
			DConsole.Log("Timeout infinity");
			PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
		}
		else
		{
			DConsole.Log("Timeout 0.2s");
			PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeout;
		}
		GameSceneLoaded = false;
		if (GameManager.Instance.type == MyGameType.TwoPlayer)
		{
			GameManager.Instance.requiredPlayers = 2;
		}
		else if (StaticStrings.isFourPlayerModeEnabled)
		{
			GameManager.Instance.requiredPlayers = 4;
		}
		else
		{
			GameManager.Instance.requiredPlayers = 2;
		}
		RoomIDObject.SetActive(value: false);
		if (GameManager.Instance.type == MyGameType.Private && !GameManager.Instance.JoinedByID)
		{
			RoomIDObject.SetActive(value: true);
			RoomIDText.GetComponent<Text>().text = "Fetching...";
		}
		else
		{
			RoomIDObject.SetActive(value: false);
		}
		CancelButton.SetActive(value: false);
		for (int i = 0; i < InviteToJoinButtons.Length; i++)
		{
			if (GameManager.Instance.type != MyGameType.Private || GameManager.Instance.JoinedByID)
			{
				InviteToJoinButtons[i].SetActive(value: false);
			}
			else
			{
				InviteToJoinButtons[i].SetActive(value: true);
			}
		}
		for (int j = 0; j < OppoAvatar.Length; j++)
		{
			OppoAvatar[j].SetActive(value: false);
		}
		if (!StaticStrings.isFourPlayerModeEnabled)
		{
			OppoAvatar[1].SetActive(value: false);
			OppoAvatar[2].SetActive(value: false);
			InviteToJoinButtons[1].SetActive(value: false);
			InviteToJoinButtons[2].SetActive(value: false);
		}
		matchPlayersCanvas.SetActive(value: true);
		if (GameManager.Instance.requiredPlayers == 2)
		{
			for (int k = 1; k < OppoProgressBar.Length; k++)
			{
				OppoProgressBar[k].SetActive(value: false);
			}
		}
	}

	public void ShareCode()
	{
		NativeShare nativeShare = new NativeShare();
		string text = StaticStrings.SharePrivateLinkMessage + " " + RoomIDText.GetComponent<Text>().text + "\n\n" + StaticStrings.SharePrivateLinkMessage2 + " ";
		text = text + "https://play.google.com/store/apps/details?id=" + StaticStrings.AndroidPackageName;
		nativeShare.Share(text, null, null, "Share via");
	}

	public void setCancelButton()
	{
		if (GameManager.Instance.type == MyGameType.Private)
		{
			CancelButton.SetActive(value: true);
		}
	}

	public void updateRoomID(string id)
	{
		GameManager.Instance.privateRoomID = id;
		RoomIDText.GetComponent<Text>().text = id;
	}

	public void PlayerJoined(int index, string id)
	{
		DConsole.Log("PLAYJOINED");
		GameManager.Instance.currentPlayersCount++;
		if (!GameManager.Instance.opponentsIDs.Contains(id))
		{
			return;
		}
		playerJoin.Play();
		InviteToJoinButtons[index].SetActive(value: false);
		OppoAvatar[index].SetActive(value: true);
		if (GameManager.Instance.opponentsAvatars[index] != null)
		{
			OppoAvatarImage[index].GetComponent<Image>().sprite = GameManager.Instance.opponentsAvatars[index];
		}
		DConsole.Log("Current players count: " + GameManager.Instance.currentPlayersCount);
		if (GameManager.Instance.currentPlayersCount >= GameManager.Instance.requiredPlayers)
		{
			if (PhotonNetwork.isMasterClient)
			{
				GameManager.Instance.playfabManager.StartGame();
			}
		}
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.Instance.playfabManager.WaitForNewPlayer();
			DConsole.Log("INVOKE PLAYJOINED");
		}
	}

	public void PlayerDisconnected(int index)
	{
		playerLeft.Play();
		GameManager.Instance.currentPlayersCount--;
		GameManager.Instance.opponentsIDs[index] = null;
		GameManager.Instance.opponentsNames[index] = null;
		GameManager.Instance.opponentsAvatars[index] = null;
		if (GameManager.Instance.type == MyGameType.Private && !GameManager.Instance.JoinedByID)
		{
			InviteToJoinButtons[index].SetActive(value: true);
		}
		OppoAvatar[index].SetActive(value: false);
		DConsole.Log("Current players count: " + GameManager.Instance.currentPlayersCount);
	}

	public void showLongTimeMessage()
	{
		if (!foundPlayer && base.gameObject.activeSelf)
		{
			longTimeMessage.SetActive(value: true);
		}
	}

	public void hideLongTimeMessage()
	{
		longTimeMessage.SetActive(value: false);
	}

	private void Update()
	{
		if (startedGame || !foundPlayer)
		{
			return;
		}
		if (speed1 > 3f)
		{
			speed1 -= speed / 200f;
		}
		if (speed1 < speed * 0.7f && !changedAvatar)
		{
			changedAvatar = true;
			OpponentAvatar = lastAvatar;
			if (GameManager.Instance.avatarOpponent != null)
			{
				lastAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarOpponent;
			}
			else
			{
				lastAvatar.GetComponent<Image>().sprite = noAvatarSprite;
			}
		}
		if (speed1 <= 0f)
		{
			speed1 = speed / 100f;
		}
		if (!(OpponentAvatar != null) || !(OpponentAvatar.GetComponent<RectTransform>().anchoredPosition.y <= 0f))
		{
			return;
		}
		speed1 = 0f;
		foreach (GameObject avatar in avatars)
		{
			avatar.SetActive(value: false);
		}
		audioSources[1].Play();
		OpponentAvatar.SetActive(value: true);
		OpponentAvatar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
		opponentNameText.text = GameManager.Instance.nameOpponent;
		startedGame = true;
		if (PhotonNetwork.playerList.Length < 2 || playerRejected)
		{
			playerDisconnected();
		}
		else
		{
			coinsAnimate();
			AvatarFrameMy.GetComponent<Animator>().Play("MySelectorMoveOut");
			AvatarFrameOpponent.GetComponent<Animator>().Play("OpponentFrameMoveOut");
			vsText.GetComponent<Animator>().Play("VsTextAnim");
			centerCoins.GetComponent<Animator>().Play("CoinsCenter");
			leftcoins.GetComponent<Animator>().Play("OppontentCoins");
			rightCoins.GetComponent<Animator>().Play("MyCoinsA");
			StartCoroutine(countDownCoins(GameManager.Instance.payoutCoins));
		}
		GameManager.Instance.readyToAnimateCoins = true;
	}

	private void coinsAnimate()
	{
		oppontent = oppontentPayoutCoins.GetComponent<Text>();
		my = myPayoutCoins.GetComponent<Text>();
		center = centerPayoutCoins.GetComponent<Text>();
		opImage = oppontentCoinImage.GetComponent<Image>();
		myImage = myCoinImage.GetComponent<Image>();
		lc = leftcoins.GetComponent<Image>();
		my.color = new Color(my.color.r, my.color.g, my.color.b, 1f);
		oppontent.color = new Color(oppontent.color.r, oppontent.color.g, oppontent.color.b, 1f);
		opImage.color = new Color(opImage.color.r, opImage.color.g, opImage.color.b, 1f);
		myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, 1f);
		if (GameManager.Instance.payoutCoins >= 1000)
		{
			if (GameManager.Instance.payoutCoins >= 1000000)
			{
				if ((float)GameManager.Instance.payoutCoins % 1000000f == 0f)
				{
					my.text = ((float)GameManager.Instance.payoutCoins / 1000000f).ToString("0") + "M";
					oppontent.text = ((float)GameManager.Instance.payoutCoins / 1000000f).ToString("0") + "M";
				}
				else
				{
					my.text = ((float)GameManager.Instance.payoutCoins / 1000000f).ToString("0.0") + "M";
					oppontent.text = ((float)GameManager.Instance.payoutCoins / 1000000f).ToString("0.0") + "M";
				}
			}
			else if ((float)GameManager.Instance.payoutCoins % 1000f == 0f)
			{
				my.text = ((float)GameManager.Instance.payoutCoins / 1000f).ToString("0") + "k";
				oppontent.text = ((float)GameManager.Instance.payoutCoins / 1000f).ToString("0") + "k";
			}
			else
			{
				my.text = ((float)GameManager.Instance.payoutCoins / 1000f).ToString("0.0") + "k";
				oppontent.text = ((float)GameManager.Instance.payoutCoins / 1000f).ToString("0.0") + "k";
			}
		}
		else
		{
			oppontent.text = string.Concat(GameManager.Instance.payoutCoins);
			my.text = string.Concat(GameManager.Instance.payoutCoins);
		}
		center.text = "0";
	}

	private IEnumerator countDownCoins(int count)
	{
		DConsole.Log("STAET");
		StartCoroutine(waitSecs(5f));
		DConsole.Log("END");
		int loops = 50;
		int minus = count / loops;
		int current = count;
		int centerCurrent = 0;
		float minusAlpha = 1f / (float)loops;
		yield return new WaitForSeconds(2f);
		audioSources[2].Play();
		for (int i = 0; i < loops; i++)
		{
			my.color = new Color(my.color.r, my.color.g, my.color.b, my.color.a - minusAlpha);
			oppontent.color = new Color(oppontent.color.r, oppontent.color.g, oppontent.color.b, oppontent.color.a - minusAlpha);
			opImage.color = new Color(opImage.color.r, opImage.color.g, opImage.color.b, opImage.color.a - minusAlpha);
			myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, myImage.color.a - minusAlpha);
			current -= minus;
			centerCurrent += minus * 2;
			if (count >= 1000)
			{
				if (count >= 1000000)
				{
					my.text = ((float)current / 1000000f).ToString("0.0") + "M";
					oppontent.text = ((float)current / 1000000f).ToString("0.0") + "M";
					center.text = ((float)centerCurrent / 1000000f).ToString("0.0") + "M";
				}
				else
				{
					my.text = ((float)current / 1000f).ToString("0.0") + "k";
					oppontent.text = ((float)current / 1000f).ToString("0.0") + "k";
					if (centerCurrent >= 1000000)
					{
						center.text = ((float)centerCurrent / 1000000f).ToString("0.0") + "M";
					}
					else
					{
						center.text = ((float)centerCurrent / 1000f).ToString("0.0") + "k";
					}
				}
			}
			else
			{
				my.text = string.Concat(current);
				oppontent.text = string.Concat(current);
				if (centerCurrent >= 1000)
				{
					center.text = ((float)centerCurrent / 1000f).ToString("0.0") + "k";
				}
				else
				{
					center.text = string.Concat(centerCurrent);
				}
			}
			if (current > 0)
			{
				yield return new WaitForSeconds(0.04f);
			}
		}
		if (centerCurrent >= 1000)
		{
			if (centerCurrent >= 1000000)
			{
				if ((float)centerCurrent % 1000000f == 0f)
				{
					center.text = ((float)centerCurrent / 1000000f).ToString("0") + "M";
				}
			}
			else if ((float)centerCurrent % 1000f == 0f)
			{
				center.text = ((float)centerCurrent / 1000f).ToString("0") + "k";
			}
		}
		float alpha = 1f;
		for (int i = 0; i < 20; i++)
		{
			alpha -= 0.05f;
			lc.color = new Color(1f, 1f, 1f, alpha);
			Color color = rightCoins.GetComponent<Image>().color;
			color.a = alpha;
			rightCoins.GetComponent<Image>().color = color;
			yield return new WaitForSeconds(0.01f);
		}
		startGame();
	}

	private IEnumerator waitSecs(float milis)
	{
		yield return new WaitForSeconds(milis);
	}

	public void playerDisconnected()
	{
		StopAllCoroutines();
		rightCoins.SetActive(value: false);
		leftcoins.SetActive(value: false);
		cantPlayNowOppo.SetActive(value: true);
		PhotonNetwork.LeaveRoom();
		Invoke("cancelGame", 5f);
	}

	private void cancelGame()
	{
		cantPlayNowOppo.SetActive(value: false);
		matchPlayersCanvas.SetActive(value: false);
		PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
		DConsole.Log("Timeout 1");
	}

	public void StartGamePrivate()
	{
		GameManager.Instance.playfabManager.StartGame();
	}

	private void startGame()
	{
		GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>().imReady = true;
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(199, GameManager.Instance.cueIndex + "-" + GameManager.Instance.cueTime, sendReliable: true, null);
		}
		if (PhotonNetwork.playerList.Length < 2)
		{
			playerDisconnected();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			PhotonNetwork.SendOutgoingCommands();
			DConsole.Log("Application pause");
		}
		else
		{
			PhotonNetwork.SendOutgoingCommands();
			DConsole.Log("Application resume");
		}
	}

	public void hideMessageBubble()
	{
		messageBubble.GetComponent<Animator>().Play("HideBubble");
	}

	public IEnumerator updateMessageBubbleText()
	{
		yield return new WaitForSeconds(2f);
		waitingOpponentTime -= 1f;
		if (!GameManager.Instance.opponentDisconnected)
		{
			messageBubbleText.GetComponent<Text>().text = StaticStrings.waitingForOpponent + " " + waitingOpponentTime;
		}
		if (waitingOpponentTime > 0f && !opponentActive && !GameManager.Instance.opponentDisconnected)
		{
			StartCoroutine(updateMessageBubbleText());
		}
	}

	public void test()
	{
	}

	public void cancelMatching()
	{
		cancelGameButton.SetActive(value: false);
		Invoke("cancelGameInvoke", 3f);
	}

	public void cancelGameInvoke()
	{
		DConsole.Log("Length: " + PhotonNetwork.otherPlayers.Length);
		if (!foundPlayer && PhotonNetwork.otherPlayers.Length == 0)
		{
			PhotonNetwork.LeaveRoom();
			matchPlayersCanvas.SetActive(value: false);
			PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
			DConsole.Log("Timeout 2");
			GameManager.Instance.playfabManager.imReady = false;
		}
	}
}
