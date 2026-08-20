using System;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using Facebook.Unity;
using Photon;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameGUIController : PunBehaviour
{
	[Serializable]
	public class MultiDimensionalGameObject
	{
		public GameObject[] objectsArray;
	}

	public GameObject TIPButtonObject;

	public GameObject TIPObject;

	public GameObject firstPrizeObject;

	public GameObject SecondPrizeObject;

	public GameObject firstPrizeText;

	public GameObject secondPrizeText;

	public AudioSource WinSound;

	public AudioSource myTurnSource;

	public AudioSource oppoTurnSource;

	private bool AllPlayersReady;

	public MultiDimensionalGameObject[] PlayersPawns;

	public GameObject[] PlayersDices;

	public GameObject[] HomeLockObjects;

	public GameObject ludoBoard;

	public GameObject[] diceBackgrounds;

	public MultiDimensionalGameObject[] playersPawnsColors;

	public MultiDimensionalGameObject[] playersPawnsMultiple;

	private Color colorRed = new Color(50f / 51f, 4f / 85f, 4f / 85f);

	private Color colorBlue = new Color(0f, 0.3372549f, 1f);

	private Color colorYellow = new Color(1f, 0.6392157f, 0f);

	private Color colorGreen = new Color(0.03137255f, 58f / 85f, 0.11764706f);

	public GameObject GameFinishWindow;

	public GameObject ScreenShotController;

	public GameObject invitiationDialog;

	public GameObject addedFriendWindow;

	public GameObject PlayerInfoWindow;

	public GameObject ChatWindow;

	public GameObject ChatButton;

	private bool SecondPlayerOnDiagonal = true;

	private List<string> PlayersIDs;

	public GameObject[] Players;

	public GameObject[] PlayersTimers;

	public GameObject[] PlayersChatBubbles;

	public GameObject[] PlayersChatBubblesText;

	public GameObject[] PlayersChatBubblesImage;

	private GameObject[] ActivePlayers;

	public GameObject[] PlayersAvatarsButton;

	private List<Sprite> avatars;

	private List<string> names;

	private List<PlayerObject> playerObjects;

	private int myIndex;

	private string myId;

	private int currentPlayerIndex;

	private int ActivePlayersInRoom;

	private Sprite[] emojiSprites;

	private string CurrentPlayerID;

	private List<PlayerObject> playersFinished = new List<PlayerObject>();

	private bool iFinished;

	private bool FinishWindowActive;

	private int firstPlacePrize;

	private int secondPlacePrize;

	private int requiredToStart;

	private void Start()
	{
		requiredToStart = GameManager.Instance.requiredPlayers;
		if (GameManager.Instance.type == MyGameType.Private)
		{
			requiredToStart = 2;
		}
		PhotonNetwork.RaiseEvent(179, 0, sendReliable: true, null);
		int num = UnityEngine.Random.Range(0, 4);
		Color[] array = null;
		switch (num)
		{
		case 0:
			array = new Color[4] { colorYellow, colorGreen, colorRed, colorBlue };
			break;
		case 1:
			array = new Color[4] { colorBlue, colorYellow, colorGreen, colorRed };
			ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, -90f);
			break;
		case 2:
			array = new Color[4] { colorRed, colorBlue, colorYellow, colorGreen };
			ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, -180f);
			break;
		default:
			array = new Color[4] { colorGreen, colorRed, colorBlue, colorYellow };
			ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, -270f);
			break;
		}
		for (int i = 0; i < diceBackgrounds.Length; i++)
		{
			diceBackgrounds[i].GetComponent<Image>().color = array[i];
		}
		for (int j = 0; j < playersPawnsColors.Length; j++)
		{
			for (int k = 0; k < playersPawnsColors[j].objectsArray.Length; k++)
			{
				playersPawnsColors[j].objectsArray[k].GetComponent<Image>().color = array[j];
				playersPawnsMultiple[j].objectsArray[k].GetComponent<Image>().color = array[j];
			}
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() - GameManager.Instance.payoutCoins).ToString());
		dictionary.Add(MyPlayerData.GamesPlayedKey, (GameManager.Instance.myPlayerData.GetPlayedGamesCount() + 1).ToString());
		GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
		currentPlayerIndex = 0;
		emojiSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emoji;
		myId = GameManager.Instance.playfabManager.PlayFabId;
		playerObjects = new List<PlayerObject>();
		avatars = GameManager.Instance.opponentsAvatars;
		avatars.Insert(0, GameManager.Instance.avatarMy);
		names = GameManager.Instance.opponentsNames;
		names.Insert(0, GameManager.Instance.nameMy);
		PlayersIDs = new List<string>();
		for (int l = 0; l < GameManager.Instance.opponentsIDs.Count; l++)
		{
			if (GameManager.Instance.opponentsIDs[l] != null)
			{
				PlayersIDs.Add(GameManager.Instance.opponentsIDs[l]);
			}
		}
		PlayersIDs.Insert(0, GameManager.Instance.playfabManager.PlayFabId);
		for (int m = 0; m < PlayersIDs.Count; m++)
		{
			playerObjects.Add(new PlayerObject(names[m], PlayersIDs[m], avatars[m]));
		}
		for (int n = 0; n < PlayersIDs.Count; n++)
		{
			for (int num2 = 0; num2 < PlayersIDs.Count - 1; num2++)
			{
				if (string.Compare(playerObjects[num2].id, playerObjects[num2 + 1].id) == 1)
				{
					PlayerObject value = playerObjects[num2 + 1];
					playerObjects[num2 + 1] = playerObjects[num2];
					playerObjects[num2] = value;
				}
			}
		}
		for (int num3 = 0; num3 < PlayersIDs.Count; num3++)
		{
			DConsole.Log(playerObjects[num3].id);
		}
		ActivePlayersInRoom = PlayersIDs.Count;
		if (PlayersIDs.Count == 2)
		{
			if (SecondPlayerOnDiagonal)
			{
				Players[1].SetActive(value: false);
				Players[3].SetActive(value: false);
				ActivePlayers = new GameObject[2];
				ActivePlayers[0] = Players[0];
				ActivePlayers[1] = Players[2];
				for (int num4 = 0; num4 < PlayersPawns[1].objectsArray.Length; num4++)
				{
					PlayersPawns[1].objectsArray[num4].SetActive(value: false);
				}
				for (int num5 = 0; num5 < PlayersPawns[3].objectsArray.Length; num5++)
				{
					PlayersPawns[3].objectsArray[num5].SetActive(value: false);
				}
			}
			else
			{
				for (int num6 = 0; num6 < PlayersPawns[21].objectsArray.Length; num6++)
				{
					PlayersPawns[2].objectsArray[num6].SetActive(value: false);
				}
				for (int num7 = 0; num7 < PlayersPawns[3].objectsArray.Length; num7++)
				{
					PlayersPawns[3].objectsArray[num7].SetActive(value: false);
				}
				Players[2].SetActive(value: false);
				Players[3].SetActive(value: false);
				ActivePlayers = new GameObject[2];
				ActivePlayers[0] = Players[0];
				ActivePlayers[1] = Players[1];
			}
		}
		else
		{
			ActivePlayers = Players;
		}
		int num8 = 0;
		for (int num9 = 0; num9 < playerObjects.Count; num9++)
		{
			if (playerObjects[num9].id == GameManager.Instance.playfabManager.PlayFabId)
			{
				num8 = num9;
				break;
			}
		}
		int num10 = 0;
		bool flag = false;
		myIndex = num8;
		GameManager.Instance.myPlayerIndex = myIndex;
		int num11 = num8;
		while (!(num11 == num8 && flag))
		{
			if (PlayersIDs.Count == 2 && SecondPlayerOnDiagonal)
			{
				if (flag)
				{
					playerObjects[num11].timer = PlayersTimers[2];
					playerObjects[num11].ChatBubble = PlayersChatBubbles[2];
					playerObjects[num11].ChatBubbleText = PlayersChatBubblesText[2];
					playerObjects[num11].ChatbubbleImage = PlayersChatBubblesImage[2];
					string id = playerObjects[num11].id;
					PlayersAvatarsButton[2].GetComponent<Button>().onClick.RemoveAllListeners();
					PlayersAvatarsButton[2].GetComponent<Button>().onClick.AddListener(delegate
					{
						ButtonClick(id);
					});
					playerObjects[num11].dice = PlayersDices[2];
					playerObjects[num11].pawns = PlayersPawns[2].objectsArray;
					for (int num12 = 0; num12 < playerObjects[num11].pawns.Length; num12++)
					{
						playerObjects[num11].pawns[num12].GetComponent<LudoPawnController>().setPlayerIndex(num11);
					}
					playerObjects[num11].homeLockObjects = HomeLockObjects[2];
				}
				else
				{
					GameManager.Instance.myPlayerIndex = num11;
					playerObjects[num11].timer = PlayersTimers[num10];
					playerObjects[num11].ChatBubble = PlayersChatBubbles[num10];
					playerObjects[num11].ChatBubbleText = PlayersChatBubblesText[num10];
					playerObjects[num11].ChatbubbleImage = PlayersChatBubblesImage[num10];
					playerObjects[num11].dice = PlayersDices[num10];
					playerObjects[num11].pawns = PlayersPawns[num10].objectsArray;
					for (int num13 = 0; num13 < playerObjects[num11].pawns.Length; num13++)
					{
						playerObjects[num11].pawns[num13].GetComponent<LudoPawnController>().setPlayerIndex(num11);
					}
					playerObjects[num11].homeLockObjects = HomeLockObjects[num10];
				}
			}
			else
			{
				playerObjects[num11].timer = PlayersTimers[num10];
				playerObjects[num11].ChatBubble = PlayersChatBubbles[num10];
				playerObjects[num11].ChatBubbleText = PlayersChatBubblesText[num10];
				playerObjects[num11].ChatbubbleImage = PlayersChatBubblesImage[num10];
				playerObjects[num11].dice = PlayersDices[num10];
				playerObjects[num11].pawns = PlayersPawns[num10].objectsArray;
				for (int num14 = 0; num14 < playerObjects[num11].pawns.Length; num14++)
				{
					playerObjects[num11].pawns[num14].GetComponent<LudoPawnController>().setPlayerIndex(num11);
				}
				playerObjects[num11].homeLockObjects = HomeLockObjects[num10];
				string id2 = playerObjects[num11].id;
				if (num10 != 0)
				{
					PlayersAvatarsButton[num10].GetComponent<Button>().onClick.RemoveAllListeners();
					PlayersAvatarsButton[num10].GetComponent<Button>().onClick.AddListener(delegate
					{
						ButtonClick(id2);
					});
				}
			}
			playerObjects[num11].AvatarObject = ActivePlayers[num10];
			ActivePlayers[num10].GetComponent<PlayerAvatarController>().Name.GetComponent<Text>().text = playerObjects[num11].name;
			if (playerObjects[num11].avatar != null)
			{
				ActivePlayers[num10].GetComponent<PlayerAvatarController>().Avatar.GetComponent<Image>().sprite = playerObjects[num11].avatar;
			}
			num10++;
			num11 = ((num11 < PlayersIDs.Count - 1) ? (num11 + 1) : 0);
			flag = true;
		}
		currentPlayerIndex = GameManager.Instance.firstPlayerInGame;
		GameManager.Instance.currentPlayer = playerObjects[currentPlayerIndex];
		GameManager.Instance.playerObjects = playerObjects;
		if (ActivePlayersInRoom == 2)
		{
			firstPlacePrize = 2 * GameManager.Instance.payoutCoins;
			secondPlacePrize = 0;
		}
		else if (ActivePlayersInRoom == 3)
		{
			firstPlacePrize = 2 * GameManager.Instance.payoutCoins;
			secondPlacePrize = GameManager.Instance.payoutCoins;
		}
		else if (ActivePlayersInRoom == 4)
		{
			firstPlacePrize = 3 * GameManager.Instance.payoutCoins;
			secondPlacePrize = GameManager.Instance.payoutCoins;
		}
		else
		{
			firstPlacePrize = GameManager.Instance.payoutCoins;
		}
		firstPrizeText.GetComponent<Text>().text = string.Concat(firstPlacePrize);
		secondPrizeText.GetComponent<Text>().text = string.Concat(secondPlacePrize);
		if (secondPlacePrize == 0)
		{
			SecondPrizeObject.SetActive(value: false);
			firstPrizeObject.GetComponent<RectTransform>().anchoredPosition = SecondPrizeObject.GetComponent<RectTransform>().anchoredPosition;
		}
		if (GameManager.Instance.mode == MyGameMode.Quick || GameManager.Instance.mode == MyGameMode.Master)
		{
			for (int num15 = 0; num15 < GameManager.Instance.playerObjects.Count; num15++)
			{
				GameManager.Instance.playerObjects[num15].homeLockObjects.SetActive(value: true);
			}
			GameManager.Instance.needToKillOpponentToEnterHome = true;
		}
		else
		{
			GameManager.Instance.needToKillOpponentToEnterHome = false;
		}
		for (int num16 = 0; num16 < playerObjects.Count; num16++)
		{
			if (playerObjects[num16].id.Contains("_BOT"))
			{
				GameManager.Instance.readyPlayersCount++;
			}
		}
		GameManager.Instance.playerObjects = playerObjects;
		for (int num17 = 0; num17 < playerObjects.Count; num17++)
		{
			bool flag2 = false;
			if (playerObjects[num17].id.Contains("_BOT"))
			{
				continue;
			}
			for (int num18 = 0; num18 < PhotonNetwork.playerList.Length; num18++)
			{
				if (PhotonNetwork.playerList[num18].NickName.Equals(playerObjects[num17].id))
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				GameManager.Instance.readyPlayersCount++;
				DConsole.Log("Ready players: " + GameManager.Instance.readyPlayersCount);
				setPlayerDisconnected(num17);
			}
		}
		CheckPlayersIfShouldFinishGame();
		StartCoroutine(waitForPlayersToStart());
	}

	private IEnumerator waitForPlayersToStart()
	{
		DConsole.Log("Waiting for players " + GameManager.Instance.readyPlayersCount + " - " + requiredToStart);
		yield return new WaitForSeconds(0.1f);
		if (GameManager.Instance.readyPlayersCount < requiredToStart)
		{
			StartCoroutine(waitForPlayersToStart());
			yield break;
		}
		AllPlayersReady = true;
		SetTurn();
	}

	public int GetCurrentPlayerIndex()
	{
		return currentPlayerIndex;
	}

	public void TIPButton()
	{
		if (TIPObject.activeSelf)
		{
			TIPObject.SetActive(value: false);
		}
		else
		{
			TIPObject.SetActive(value: true);
		}
	}

	public void FacebookShare()
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

	public void StopAndFinishGame()
	{
		StopTimers();
		SetFinishGame(PhotonNetwork.player.NickName, me: true);
		ShowGameFinishWindow();
	}

	public void ShareScreenShot()
	{
		string shareScreenShotText = StaticStrings.ShareScreenShotText;
		shareScreenShotText = shareScreenShotText + " https://play.google.com/store/apps/details?id=" + StaticStrings.AndroidPackageName;
		ScreenShotController.GetComponent<NativeShare>().ShareScreenshotWithText(shareScreenShotText);
	}

	public void ShowGameFinishWindow()
	{
		if (FinishWindowActive)
		{
			return;
		}
		AdsManager.Instance.adsScript.ShowAd(AdLocation.GameFinishWindow);
		FinishWindowActive = true;
		List<PlayerObject> list = new List<PlayerObject>();
		for (int i = 0; i < playerObjects.Count; i++)
		{
			PlayerAvatarController component = playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>();
			if (component.Active && !component.finished)
			{
				list.Add(playerObjects[i]);
			}
		}
		GameFinishWindow.GetComponent<GameFinishWindowController>().showWindow(playersFinished, list, firstPlacePrize, secondPlacePrize);
	}

	private void ButtonClick(string id)
	{
		int index = 0;
		for (int i = 0; i < playerObjects.Count; i++)
		{
			if (playerObjects[i].id == id)
			{
				index = i;
				break;
			}
		}
		CurrentPlayerID = id;
		if (playerObjects[index].AvatarObject.GetComponent<PlayerAvatarController>().Active)
		{
			PlayerInfoWindow.GetComponent<PlayerInfoController>().ShowPlayerInfo(playerObjects[index].avatar, playerObjects[index].name, playerObjects[index].data);
		}
	}

	public void AddFriendButtonClick()
	{
		if (!CurrentPlayerID.Contains("_BOT"))
		{
			PlayFabClientAPI.AddFriend(new AddFriendRequest
			{
				FriendPlayFabId = CurrentPlayerID
			}, delegate
			{
				PhotonNetwork.RaiseEvent(177, PhotonNetwork.playerName + ";" + GameManager.Instance.nameMy + ";" + CurrentPlayerID, sendReliable: true, null);
				addedFriendWindow.SetActive(value: true);
				DConsole.Log("Added friend successfully");
			}, delegate(PlayFabError error)
			{
				addedFriendWindow.SetActive(value: true);
				DConsole.Log("Error adding friend: " + error.Error);
			});
		}
		else
		{
			DConsole.Log("Add Friend - It's bot!");
			addedFriendWindow.SetActive(value: true);
		}
	}

	private void Update()
	{
	}

	public void FinishedGame()
	{
		if (GameManager.Instance.currentPlayer.id == PhotonNetwork.player.NickName)
		{
			SetFinishGame(GameManager.Instance.currentPlayer.id, me: true);
		}
		else
		{
			SetFinishGame(GameManager.Instance.currentPlayer.id, me: false);
		}
	}

	private void SetFinishGame(string id, bool me)
	{
		if (me && iFinished)
		{
			return;
		}
		DConsole.Log("SET FINISH");
		ActivePlayersInRoom--;
		int playerPosition = GetPlayerPosition(id);
		playersFinished.Add(playerObjects[playerPosition]);
		PlayerAvatarController component = playerObjects[playerPosition].AvatarObject.GetComponent<PlayerAvatarController>();
		component.Name.GetComponent<Text>().text = "";
		component.Active = false;
		component.finished = true;
		playerObjects[playerPosition].dice.SetActive(value: false);
		int count = playersFinished.Count;
		if (count == 1)
		{
			component.Crown.SetActive(value: true);
		}
		if (me)
		{
			PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
			iFinished = true;
			if (ActivePlayersInRoom >= 0)
			{
				PhotonNetwork.RaiseEvent(178, PhotonNetwork.player.NickName, sendReliable: true, null);
				DConsole.Log("set finish call finish turn");
				SendFinishTurn();
			}
			switch (count)
			{
			case 1:
			{
				WinSound.Play();
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() + firstPlacePrize).ToString());
				dictionary2.Add(MyPlayerData.TotalEarningsKey, (GameManager.Instance.myPlayerData.GetTotalEarnings() + firstPlacePrize).ToString());
				if (GameManager.Instance.type == MyGameType.TwoPlayer)
				{
					dictionary2.Add(MyPlayerData.TwoPlayerWinsKey, (GameManager.Instance.myPlayerData.GetTwoPlayerWins() + 1).ToString());
				}
				else if (GameManager.Instance.type == MyGameType.FourPlayer)
				{
					dictionary2.Add(MyPlayerData.FourPlayerWinsKey, (GameManager.Instance.myPlayerData.GetFourPlayerWins() + 1).ToString());
				}
				GameManager.Instance.myPlayerData.UpdateUserData(dictionary2);
				break;
			}
			case 2:
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() + secondPlacePrize).ToString());
				dictionary.Add(MyPlayerData.TotalEarningsKey, (GameManager.Instance.myPlayerData.GetTotalEarnings() + secondPlacePrize).ToString());
				GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
				break;
			}
			}
		}
		else if (GameManager.Instance.currentPlayer.isBot)
		{
			SendFinishTurn();
		}
		component.setPositionSprite(count);
		CheckPlayersIfShouldFinishGame();
	}

	public int GetPlayerPosition(string id)
	{
		for (int i = 0; i < playerObjects.Count; i++)
		{
			if (playerObjects[i].id.Equals(id))
			{
				return i;
			}
		}
		return -1;
	}

	public void SendFinishTurn()
	{
		if (!FinishWindowActive && ActivePlayersInRoom > 1)
		{
			if (GameManager.Instance.currentPlayer.isBot)
			{
				BotDelay();
				return;
			}
			PhotonNetwork.RaiseEvent(172, myIndex, sendReliable: true, null);
			DConsole.Log("PLAYER BEFORE: " + currentPlayerIndex);
			setCurrentPlayerIndex(myIndex);
			DConsole.Log("PLAYER AFTER: " + currentPlayerIndex + " isbot: " + GameManager.Instance.currentPlayer.isBot.ToString());
			SetTurn();
			GameManager.Instance.miniGame.setOpponentTurn();
		}
	}

	private void Awake()
	{
		PhotonNetwork.OnEventCall += OnEvent;
	}

	private void OnDestroy()
	{
		PhotonNetwork.OnEventCall -= OnEvent;
	}

	private void OnEvent(byte eventcode, object content, int senderid)
	{
		DConsole.Log("received event: " + eventcode);
		switch (eventcode)
		{
		case 172:
			if (playerObjects[(int)content].AvatarObject.GetComponent<PlayerAvatarController>().Active && currentPlayerIndex == (int)content && !FinishWindowActive)
			{
				setCurrentPlayerIndex((int)content);
				SetTurn();
			}
			break;
		case 175:
		{
			string[] array3 = ((string)content).Split(';');
			DConsole.Log("Received message " + array3[0] + " from " + array3[1]);
			for (int j = 0; j < playerObjects.Count; j++)
			{
				if (playerObjects[j].id.Equals(array3[1]))
				{
					playerObjects[j].ChatBubbleText.SetActive(value: true);
					playerObjects[j].ChatbubbleImage.SetActive(value: false);
					playerObjects[j].ChatBubbleText.GetComponent<Text>().text = array3[0];
					playerObjects[j].ChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
				}
			}
			break;
		}
		case 176:
		{
			string[] array2 = ((string)content).Split(';');
			DConsole.Log("Received message " + array2[0] + " from " + array2[1]);
			for (int i = 0; i < playerObjects.Count; i++)
			{
				if (playerObjects[i].id.Equals(array2[1]))
				{
					playerObjects[i].ChatBubbleText.SetActive(value: false);
					playerObjects[i].ChatbubbleImage.SetActive(value: true);
					int num = int.Parse(array2[0]);
					if (num > emojiSprites.Length - 1)
					{
						num = emojiSprites.Length;
					}
					playerObjects[i].ChatbubbleImage.GetComponent<Image>().sprite = emojiSprites[num];
					playerObjects[i].ChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
				}
			}
			break;
		}
		case 177:
			if (PlayerPrefs.GetInt(StaticStrings.FriendsRequestesKey, 0) == 0)
			{
				string[] array = ((string)content).Split(';');
				if (PhotonNetwork.playerName.Equals(array[2]))
				{
					invitiationDialog.GetComponent<PhotonChatListener2>().showInvitationDialog(array[0], array[1], null);
				}
			}
			else
			{
				DConsole.Log("Invitations OFF");
			}
			break;
		case 178:
		{
			string id = (string)content;
			SetFinishGame(id, me: false);
			break;
		}
		}
	}

	private void SetMyTurn()
	{
		GameManager.Instance.isMyTurn = true;
		if (GameManager.Instance.miniGame != null)
		{
			GameManager.Instance.miniGame.setMyTurn();
		}
		StartTimer();
	}

	private void BotTurn()
	{
		oppoTurnSource.Play();
		GameManager.Instance.isMyTurn = false;
		DConsole.Log("Bot Turn");
		StartTimer();
		GameManager.Instance.miniGame.BotTurn(first: true);
	}

	private void SetTurn()
	{
		DConsole.Log("SET TURN CALLED");
		for (int i = 0; i < playerObjects.Count; i++)
		{
			playerObjects[i].dice.GetComponent<GameDiceController>().EnableDiceShadow();
		}
		playerObjects[currentPlayerIndex].dice.GetComponent<GameDiceController>().DisableDiceShadow();
		GameManager.Instance.currentPlayer = playerObjects[currentPlayerIndex];
		if (playerObjects[currentPlayerIndex].id == myId)
		{
			SetMyTurn();
		}
		else if (playerObjects[currentPlayerIndex].isBot)
		{
			BotTurn();
		}
		else
		{
			SetOpponentTurn();
		}
	}

	private void BotDelay()
	{
		if (!FinishWindowActive)
		{
			setCurrentPlayerIndex(currentPlayerIndex);
			SetTurn();
		}
	}

	private void setCurrentPlayerIndex(int current)
	{
		do
		{
			current++;
			currentPlayerIndex = current % playerObjects.Count;
			GameManager.Instance.currentPlayer = playerObjects[currentPlayerIndex];
		}
		while (!playerObjects[currentPlayerIndex].AvatarObject.GetComponent<PlayerAvatarController>().Active);
	}

	private void SetOpponentTurn()
	{
		DConsole.Log("Opponent turn");
		oppoTurnSource.Play();
		GameManager.Instance.isMyTurn = false;
		StartTimer();
	}

	private void StartTimer()
	{
		for (int i = 0; i < playerObjects.Count; i++)
		{
			if (i == currentPlayerIndex)
			{
				playerObjects[currentPlayerIndex].timer.SetActive(value: true);
			}
			else
			{
				playerObjects[i].timer.SetActive(value: false);
			}
		}
	}

	public void StopTimers()
	{
		for (int i = 0; i < playerObjects.Count; i++)
		{
			playerObjects[i].timer.SetActive(value: false);
		}
	}

	public void PauseTimers()
	{
		playerObjects[currentPlayerIndex].timer.GetComponent<UpdatePlayerTimer>().Pause();
	}

	public void restartTimer()
	{
		playerObjects[currentPlayerIndex].timer.GetComponent<UpdatePlayerTimer>().restartTimer();
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		DConsole.Log("Player disconnected: " + otherPlayer.NickName);
		for (int i = 0; i < playerObjects.Count; i++)
		{
			if (playerObjects[i].id.Equals(otherPlayer.NickName))
			{
				setPlayerDisconnected(i);
				break;
			}
		}
		CheckPlayersIfShouldFinishGame();
	}

	public void CheckPlayersIfShouldFinishGame()
	{
		if (!FinishWindowActive)
		{
			if (ActivePlayersInRoom == 1 && !iFinished)
			{
				StopAndFinishGame();
			}
			else if (ActivePlayersInRoom == 0)
			{
				StopAndFinishGame();
			}
			else if (iFinished && ActivePlayersInRoom == 1 && CheckIfOtherPlayerIsBot())
			{
				AddBotToListOfWinners();
				StopAndFinishGame();
			}
			else if (ActivePlayersInRoom > 1 && iFinished)
			{
				TIPButtonObject.SetActive(value: true);
			}
		}
	}

	public void AddBotToListOfWinners()
	{
		for (int i = 0; i < playerObjects.Count; i++)
		{
			if (playerObjects[i].id.Contains("_BOT") && playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().Active)
			{
				playersFinished.Add(playerObjects[i]);
			}
		}
	}

	public bool CheckIfOtherPlayerIsBot()
	{
		for (int i = 0; i < playerObjects.Count; i++)
		{
			if (playerObjects[i].id.Contains("_BOT") && playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().Active)
			{
				playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().finished = true;
				return true;
			}
		}
		return false;
	}

	public void setPlayerDisconnected(int i)
	{
		requiredToStart--;
		if (FinishWindowActive)
		{
			return;
		}
		if (!playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().finished)
		{
			ActivePlayersInRoom--;
		}
		DConsole.Log("Active players: " + ActivePlayersInRoom);
		if (currentPlayerIndex == i && ActivePlayersInRoom > 1)
		{
			setCurrentPlayerIndex(currentPlayerIndex);
			if (AllPlayersReady)
			{
				SetTurn();
			}
		}
		DConsole.Log("za petla");
		playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().PlayerLeftRoom();
		playerObjects[i].dice.SetActive(value: false);
		if (!playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().finished)
		{
			for (int j = 0; j < playerObjects[i].pawns.Length; j++)
			{
				playerObjects[i].pawns[j].GetComponent<LudoPawnController>().GoToInitPosition(callEnd: false);
			}
		}
	}

	public void LeaveGame(bool finishWindow)
	{
		if (!iFinished || finishWindow)
		{
			PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 1) + 1);
			SceneManager.LoadScene("MenuScene");
			PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
			PhotonNetwork.LeaveRoom();
			GameManager.Instance.playfabManager.roomOwner = false;
			GameManager.Instance.roomOwner = false;
			GameManager.Instance.resetAllData();
		}
		else
		{
			ShowGameFinishWindow();
		}
	}

	public void ShowHideChatWindow()
	{
		if (!ChatWindow.activeSelf)
		{
			ChatWindow.SetActive(value: true);
			ChatButton.GetComponent<Text>().text = "X";
		}
		else
		{
			ChatWindow.SetActive(value: false);
			ChatButton.GetComponent<Text>().text = "CHAT";
		}
	}
}
