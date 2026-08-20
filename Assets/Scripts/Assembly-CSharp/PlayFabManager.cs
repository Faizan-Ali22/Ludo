using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AssemblyCSharp;
using ExitGames.Client.Photon;
using Facebook.Unity;
using Photon;
using Photon.Chat;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabManager : PunBehaviour, IChatClientListener
{
	private Sprite[] avatarSprites;

	public string PlayFabId;

	public string authToken;

	public bool multiGame = true;

	public bool roomOwner;

	private FacebookManager fbManager;

	public GameObject fbButton;

	private FacebookFriendsMenu facebookFriendsMenu;

	public ChatClient chatClient;

	private bool alreadyGotFriends;

	public GameObject menuCanvas;

	public GameObject MatchPlayersCanvas;

	public GameObject splashCanvas;

	public bool opponentReady;

	public bool imReady;

	public GameObject playerAvatar;

	public GameObject playerName;

	public GameObject backButtonMatchPlayers;

	public GameObject loginEmail;

	public GameObject loginPassword;

	public GameObject loginInvalidEmailorPassword;

	public GameObject loginCanvas;

	public GameObject regiterEmail;

	public GameObject registerPassword;

	public GameObject registerNickname;

	public GameObject registerInvalidInput;

	public GameObject registerCanvas;

	public GameObject resetPasswordEmail;

	public GameObject resetPasswordInformationText;

	public bool isInLobby;

	public bool isInMaster;

	private string roomname = "";

	private void Awake()
	{
		DConsole.Log("Playfab awake");
		PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
		PhotonNetwork.PhotonServerSettings.PreferredRegion = CloudRegionCode.eu;
		PhotonNetwork.PhotonServerSettings.Protocol = ConnectionProtocol.Udp;
		DConsole.Log("PORT: " + PhotonNetwork.PhotonServerSettings.ServerPort);
		PlayFabSettings.TitleId = StaticStrings.PlayFabTitleID;
		PhotonNetwork.OnEventCall += OnEvent;
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void OnDestroy()
	{
		PhotonNetwork.OnEventCall -= OnEvent;
	}

	public void destroy()
	{
		if (base.gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void Start()
	{
		DConsole.Log("Playfab start");
		PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
		GameManager.Instance.playfabManager = this;
		fbManager = GameObject.Find("FacebookManager").GetComponent<FacebookManager>();
		facebookFriendsMenu = GameManager.Instance.facebookFriendsMenu;
		avatarSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars;
	}

	private void Update()
	{
		if (chatClient != null)
		{
			chatClient.Service();
		}
	}

	private void OnEvent(byte eventcode, object content, int senderid)
	{
		DConsole.Log("Received event: " + (int)eventcode + " Sender ID: " + senderid);
		switch (eventcode)
		{
		case 171:
			LoadGameScene();
			return;
		case 173:
			if (senderid != PhotonNetwork.player.ID)
			{
				LoadBots();
				return;
			}
			break;
		}
		switch (eventcode)
		{
		case 174:
			LoadGameScene();
			break;
		case 179:
			GameManager.Instance.readyPlayersCount++;
			break;
		}
	}

	public void LoadGameWithDelay()
	{
		LoadGameScene();
	}

	public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		if (GameManager.Instance.controlAvatars != null && GameManager.Instance.type == MyGameType.Private)
		{
			PhotonNetwork.LeaveRoom();
			GameManager.Instance.controlAvatars.ShowJoinFailed("Room closed");
		}
		else if (newMasterClient.NickName == PhotonNetwork.player.NickName)
		{
			DConsole.Log("Im new master client");
			WaitForNewPlayer();
		}
	}

	public void StartGame()
	{
		PhotonNetwork.room.IsOpen = false;
		PhotonNetwork.room.IsVisible = false;
		CancelInvoke("StartGameWithBots");
		Invoke("startGameScene", 3f);
	}

	private IEnumerator waitAndStartGame()
	{
		while (GameManager.Instance.readyPlayers < GameManager.Instance.requiredPlayers - 1 || !imReady)
		{
			yield return 0;
		}
		startGameScene();
		GameManager.Instance.readyPlayers = 0;
		opponentReady = false;
		imReady = false;
	}

	public void startGameScene()
	{
		if (GameManager.Instance.currentPlayersCount >= GameManager.Instance.requiredPlayers || GameManager.Instance.type == MyGameType.Private)
		{
			LoadGameScene();
			if (GameManager.Instance.type == MyGameType.Private)
			{
				PhotonNetwork.RaiseEvent(171, null, sendReliable: true, null);
			}
			else
			{
				PhotonNetwork.RaiseEvent(174, null, sendReliable: true, null);
			}
		}
		else if (PhotonNetwork.isMasterClient)
		{
			WaitForNewPlayer();
		}
	}

	public void LoadGameScene()
	{
		GameManager.Instance.GameScene = "GameScene";
		if (!GameManager.Instance.gameSceneStarted)
		{
			SceneManager.LoadScene(GameManager.Instance.GameScene);
			GameManager.Instance.gameSceneStarted = true;
		}
	}

	public void WaitForNewPlayer()
	{
		if (PhotonNetwork.isMasterClient && GameManager.Instance.type != MyGameType.Private)
		{
			DConsole.Log("START INVOKE");
			CancelInvoke("StartGameWithBots");
			Invoke("StartGameWithBots", StaticStrings.WaitTimeUntilStartWithBots);
		}
	}

	public void StartGameWithBots()
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (PhotonNetwork.room.PlayerCount < GameManager.Instance.requiredPlayers)
			{
				DConsole.Log("Master Client");
				LoadBots();
			}
		}
		else
		{
			DConsole.Log("Not Master client");
		}
	}

	public void LoadBots()
	{
		DConsole.Log("Close room - add bots");
		PhotonNetwork.room.IsOpen = false;
		PhotonNetwork.room.IsVisible = false;
		if (PhotonNetwork.isMasterClient)
		{
			Invoke("AddBots", 3f);
		}
		else
		{
			AddBots();
		}
	}

	public void AddBots()
	{
		DConsole.Log("Add Bots with delay");
		if (PhotonNetwork.room.PlayerCount >= GameManager.Instance.requiredPlayers)
		{
			return;
		}
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.RaiseEvent(173, null, sendReliable: true, null);
		}
		for (int i = 0; i < GameManager.Instance.requiredPlayers - 1; i++)
		{
			if (GameManager.Instance.opponentsIDs[i] == null)
			{
				StartCoroutine(AddBot(i));
			}
		}
	}

	public IEnumerator AddBot(int i)
	{
		yield return new WaitForSeconds((float)i + UnityEngine.Random.Range(0f, 0.9f));
		GameManager.Instance.opponentsAvatars[i] = avatarSprites[UnityEngine.Random.Range(0, avatarSprites.Length - 1)];
		GameManager.Instance.opponentsIDs[i] = "_BOT" + i;
		GameManager.Instance.opponentsNames[i] = "Guest" + UnityEngine.Random.Range(100000, 999999);
		DConsole.Log("Name: " + GameManager.Instance.opponentsNames[i]);
		GameManager.Instance.controlAvatars.PlayerJoined(i, "_BOT" + i);
	}

	public void resetPassword()
	{
		resetPasswordInformationText.SetActive(value: false);
		PlayFabClientAPI.SendAccountRecoveryEmail(new SendAccountRecoveryEmailRequest
		{
			TitleId = PlayFabSettings.TitleId,
			Email = resetPasswordEmail.GetComponent<Text>().text
		}, delegate
		{
			resetPasswordInformationText.SetActive(value: true);
			resetPasswordInformationText.GetComponent<Text>().text = "Email sent to your address. Check your inbox";
		}, delegate
		{
			resetPasswordInformationText.SetActive(value: true);
			resetPasswordInformationText.GetComponent<Text>().text = "Account with specified email doesn't exist";
		});
	}

	public void setInitNewAccountData(bool fb)
	{
		Dictionary<string, string> data = MyPlayerData.InitialUserData(fb);
		GameManager.Instance.myPlayerData.UpdateUserData(data);
	}

	public void updateBoughtChats(int index)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(MyPlayerData.ChatsKey, GameManager.Instance.myPlayerData.GetChats() + ";'" + index + "'");
		GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
	}

	public void UpdateBoughtEmojis(int index)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(MyPlayerData.EmojiKey, GameManager.Instance.myPlayerData.GetEmoji() + ";'" + index + "'");
		GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
	}

	public void addCoinsRequest(int count)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(MyPlayerData.CoinsKey, string.Concat(GameManager.Instance.myPlayerData.GetCoins() + count));
		GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
	}

	public void getPlayerDataRequest()
	{
		DConsole.Log("Get player data request!!");
		PlayFabClientAPI.GetUserData(new GetUserDataRequest
		{
			PlayFabId = GameManager.Instance.playfabManager.PlayFabId
		}, delegate(GetUserDataResult result)
		{
			Dictionary<string, UserDataRecord> data = result.Data;
			GameManager.Instance.myPlayerData = new MyPlayerData(data, myData: true);
			DConsole.Log("Get player data request finish!!");
			StartCoroutine(loadSceneMenu());
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Data updated error " + error.ErrorMessage);
		});
	}

	private IEnumerator loadSceneMenu()
	{
		yield return new WaitForSeconds(0.1f);
		if (isInMaster && isInLobby)
		{
			SceneManager.LoadScene("MenuScene");
		}
		else
		{
			StartCoroutine(loadSceneMenu());
		}
	}

	public void RegisterNewAccountWithID()
	{
		string email = regiterEmail.GetComponent<Text>().text;
		string password = registerPassword.GetComponent<Text>().text;
		string nickname = registerNickname.GetComponent<Text>().text;
		registerInvalidInput.SetActive(value: false);
		if (Regex.IsMatch(email, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$") && password.Length >= 6 && nickname.Length > 0)
		{
			PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
			{
				TitleId = PlayFabSettings.TitleId,
				Email = email,
				Password = password,
				RequireBothUsernameAndEmail = false
			}, delegate(RegisterPlayFabUserResult result)
			{
				PlayFabId = result.PlayFabId;
				DConsole.Log("Got PlayFabID: " + PlayFabId);
				registerCanvas.SetActive(value: false);
				PlayerPrefs.SetString("email_account", email);
				PlayerPrefs.SetString("password", password);
				PlayerPrefs.SetString("LoggedType", "EmailAccount");
				PlayerPrefs.Save();
				GameManager.Instance.nameMy = nickname;
				setInitNewAccountData(fb: false);
				PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
				{
					DisplayName = GameManager.Instance.playfabManager.PlayFabId
				}, delegate
				{
					DConsole.Log("Title Display name updated successfully");
				}, delegate(PlayFabError error)
				{
					DConsole.Log("Title Display name updated error: " + error.Error);
				});
				Dictionary<string, string> data = new Dictionary<string, string>
				{
					{ "LoggedType", "EmailAccount" },
					{
						"PlayerName",
						GameManager.Instance.nameMy
					}
				};
				GameManager.Instance.myPlayerData.UpdateUserData(data);
				fbManager.showLoadingCanvas();
				GetPhotonToken();
			}, delegate(PlayFabError error)
			{
				registerInvalidInput.SetActive(value: true);
				registerInvalidInput.GetComponent<Text>().text = error.ErrorMessage;
				DConsole.Log("Error registering new account with email: " + error.ErrorMessage + "\n" + error.ErrorDetails);
			});
		}
		else
		{
			registerInvalidInput.SetActive(value: true);
			registerInvalidInput.GetComponent<Text>().text = "Invalid input specified";
		}
	}

	public void LinkFacebookAccount()
	{
		PlayFabClientAPI.LinkFacebookAccount(new LinkFacebookAccountRequest
		{
			AccessToken = AccessToken.CurrentAccessToken.TokenString,
			ForceLink = true
		}, delegate
		{
			Dictionary<string, string> data = new Dictionary<string, string>
			{
				{ "LoggedType", "Facebook" },
				{
					"FacebookID",
					AccessToken.CurrentAccessToken.UserId
				},
				{
					"PlayerAvatarUrl",
					GameManager.Instance.avatarMyUrl
				},
				{
					MyPlayerData.PlayerName,
					GameManager.Instance.nameMy
				},
				{
					MyPlayerData.AvatarIndexKey,
					"fb"
				},
				{
					MyPlayerData.CoinsKey,
					(GameManager.Instance.myPlayerData.GetCoins() + StaticStrings.CoinsForLinkToFacebook).ToString()
				}
			};
			GameManager.Instance.myAvatarGameObject.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
			GameManager.Instance.myNameGameObject.GetComponent<Text>().text = GameManager.Instance.nameMy;
			GameManager.Instance.myPlayerData.UpdateUserData(data);
			GameManager.Instance.FacebookLinkButton.SetActive(value: false);
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Error linking facebook account: " + error.ErrorMessage + "\n" + error.ErrorDetails);
			GameManager.Instance.connectionLost.showDialog();
		});
	}

	public void LoginWithFacebook()
	{
		PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest
		{
			TitleId = PlayFabSettings.TitleId,
			CreateAccount = true,
			AccessToken = AccessToken.CurrentAccessToken.TokenString
		}, delegate(PlayFab.ClientModels.LoginResult result)
		{
			PlayFabId = result.PlayFabId;
			DConsole.Log("Got PlayFabID: " + PlayFabId);
			if (result.NewlyCreated)
			{
				DConsole.Log("(new account)");
				setInitNewAccountData(fb: true);
				Dictionary<string, string> data = new Dictionary<string, string> { 
				{
					MyPlayerData.AvatarIndexKey,
					"fb"
				} };
				GameManager.Instance.myPlayerData.UpdateUserData(data);
			}
			else
			{
				CheckIfFirstTitleLogin(PlayFabId, fb: true);
				DConsole.Log("(existing account)");
			}
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = GameManager.Instance.playfabManager.PlayFabId
			}, delegate
			{
				DConsole.Log("Title Display name updated successfully");
			}, delegate(PlayFabError error)
			{
				DConsole.Log("Title Display name updated error: " + error.Error);
			});
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "LoggedType", "Facebook" },
				{
					"FacebookID",
					AccessToken.CurrentAccessToken.UserId
				}
			};
			if (result.NewlyCreated)
			{
				dictionary.Add("PlayerName", GameManager.Instance.nameMy);
			}
			else
			{
				PlayFabClientAPI.GetUserData(new GetUserDataRequest
				{
					PlayFabId = result.PlayFabId
				}, delegate(GetUserDataResult getUserDataResult)
				{
					Dictionary<string, UserDataRecord> data2 = getUserDataResult.Data;
					if (data2.ContainsKey("PlayerName"))
					{
						GameManager.Instance.nameMy = data2["PlayerName"].Value;
					}
					else
					{
						Dictionary<string, string> data3 = new Dictionary<string, string>
						{
							{
								"PlayerName",
								GameManager.Instance.nameMy
							},
							{
								MyPlayerData.AvatarIndexKey,
								"fb"
							}
						};
						GameManager.Instance.myPlayerData.UpdateUserData(data3);
					}
				}, delegate(PlayFabError error)
				{
					DConsole.Log("Data updated error " + error.ErrorMessage);
				});
			}
			dictionary.Add("PlayerAvatarUrl", GameManager.Instance.avatarMyUrl);
			GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
			GetPhotonToken();
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Error logging in player with custom ID: " + error.ErrorMessage + "\n" + error.ErrorDetails);
			GameManager.Instance.connectionLost.showDialog();
		});
	}

	public void CheckIfFirstTitleLogin(string id, bool fb)
	{
		PlayFabClientAPI.GetUserData(new GetUserDataRequest
		{
			PlayFabId = id
		}, delegate(GetUserDataResult result)
		{
			if (!result.Data.ContainsKey(MyPlayerData.TitleFirstLoginKey))
			{
				DConsole.Log("First login for this title. Set initial data");
				setInitNewAccountData(fb);
			}
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Data updated error " + error.ErrorMessage);
		});
	}

	private string androidUnique()
	{
		AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getContentResolver", Array.Empty<object>());
		return new AndroidJavaClass("android.provider.Settings$Secure").CallStatic<string>("getString", new object[2] { androidJavaObject, "android_id" });
	}

	public void LoginWithEmailAccount()
	{
		loginInvalidEmailorPassword.SetActive(value: false);
		string email = "";
		string password = "";
		if (PlayerPrefs.HasKey("email_account"))
		{
			email = PlayerPrefs.GetString("email_account");
			password = PlayerPrefs.GetString("password");
		}
		else
		{
			email = loginEmail.GetComponent<Text>().text;
			password = loginPassword.GetComponent<Text>().text;
		}
		PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
		{
			TitleId = PlayFabSettings.TitleId,
			Email = email,
			Password = password
		}, delegate(PlayFab.ClientModels.LoginResult result)
		{
			PlayFabId = result.PlayFabId;
			DConsole.Log("Got PlayFabID: " + PlayFabId);
			loginCanvas.SetActive(value: false);
			PlayerPrefs.SetString("email_account", email);
			PlayerPrefs.SetString("password", password);
			PlayerPrefs.SetString("LoggedType", "EmailAccount");
			PlayerPrefs.Save();
			if (result.NewlyCreated)
			{
				DConsole.Log("(new account)");
				setInitNewAccountData(fb: false);
			}
			else
			{
				CheckIfFirstTitleLogin(PlayFabId, fb: false);
				DConsole.Log("(existing account)");
			}
			PlayFabClientAPI.GetUserData(new GetUserDataRequest
			{
				PlayFabId = result.PlayFabId
			}, delegate(GetUserDataResult getUserDataResult)
			{
				Dictionary<string, UserDataRecord> data = getUserDataResult.Data;
				if (data.ContainsKey("PlayerName"))
				{
					GameManager.Instance.nameMy = data["PlayerName"].Value;
				}
				else
				{
					Dictionary<string, string> data2 = new Dictionary<string, string> { 
					{
						"PlayerName",
						GameManager.Instance.nameMy
					} };
					GameManager.Instance.myPlayerData.UpdateUserData(data2);
				}
				GameManager.Instance.nameMy = data["PlayerName"].Value;
			}, delegate(PlayFabError error)
			{
				DConsole.Log("Data updated error " + error.ErrorMessage);
			});
			fbManager.showLoadingCanvas();
			GetPhotonToken();
		}, delegate(PlayFabError error)
		{
			loginInvalidEmailorPassword.SetActive(value: true);
			DConsole.Log("Error logging in player with custom ID: " + error.ErrorMessage);
		});
	}

	public void Login()
	{
		string text = "";
		if (PlayerPrefs.HasKey("unique_identifier"))
		{
			text = PlayerPrefs.GetString("unique_identifier");
		}
		else
		{
			text = Guid.NewGuid().ToString();
			PlayerPrefs.SetString("unique_identifier", text);
		}
		DConsole.Log("UNIQUE IDENTIFIER: " + text);
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
		{
			TitleId = PlayFabSettings.TitleId,
			CreateAccount = true,
			CustomId = text
		}, delegate(PlayFab.ClientModels.LoginResult result)
		{
			PlayFabId = result.PlayFabId;
			DConsole.Log("Got PlayFabID: " + PlayFabId);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (result.NewlyCreated)
			{
				DConsole.Log("(new account)");
				setInitNewAccountData(fb: false);
				string playFabId = result.PlayFabId;
				playFabId = "Guest";
				for (int i = 0; i < 6; i++)
				{
					playFabId += UnityEngine.Random.Range(0, 9);
				}
				dictionary.Add("PlayerName", playFabId);
			}
			else
			{
				CheckIfFirstTitleLogin(PlayFabId, fb: false);
				DConsole.Log("(existing account)");
			}
			dictionary.Add("LoggedType", "Guest");
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = GameManager.Instance.playfabManager.PlayFabId
			}, delegate
			{
				DConsole.Log("Title Display name updated successfully");
			}, delegate(PlayFabError error)
			{
				DConsole.Log("Title Display name updated error: " + error.Error);
			});
			GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
			GameManager.Instance.nameMy = base.name;
			PlayerPrefs.SetString("LoggedType", "Guest");
			PlayerPrefs.Save();
			fbManager.showLoadingCanvas();
			GetPhotonToken();
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Error logging in player with custom ID:");
			DConsole.Log(error.ErrorMessage);
			GameManager.Instance.connectionLost.showDialog();
		});
	}

	public void GetPlayfabFriends()
	{
		if (alreadyGotFriends)
		{
			DConsole.Log("show firneds FFFF");
			if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
			{
				fbManager.getFacebookInvitableFriends();
			}
			else
			{
				facebookFriendsMenu.showFriends(null, null, null);
			}
			return;
		}
		DConsole.Log("IND");
		PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
		{
			IncludeFacebookFriends = true
		}, delegate(GetFriendsListResult result)
		{
			DConsole.Log("Friends list Playfab: " + result.Friends.Count);
			List<PlayFab.ClientModels.FriendInfo> friends = result.Friends;
			List<string> list = new List<string>();
			List<string> playfabFriendsName = new List<string>();
			List<string> playfabFBID = new List<string>();
			chatClient.RemoveFriends(GameManager.Instance.friendsIDForStatus.ToArray());
			List<string> list2 = new List<string>();
			int num = 0;
			foreach (PlayFab.ClientModels.FriendInfo friend in friends)
			{
				list.Add(friend.FriendPlayFabId);
				DConsole.Log("Title: " + friend.TitleDisplayName);
				GetUserDataRequest request = new GetUserDataRequest
				{
					PlayFabId = friend.TitleDisplayName
				};
				int ind2 = num;
				PlayFabClientAPI.GetUserData(request, delegate(GetUserDataResult getUserDataResult)
				{
					Dictionary<string, UserDataRecord> data = getUserDataResult.Data;
					playfabFriendsName[ind2] = data["PlayerName"].Value;
					DConsole.Log("Added " + data["PlayerName"].Value);
					GameManager.Instance.facebookFriendsMenu.updateName(ind2, data["PlayerName"].Value, friend.TitleDisplayName);
				}, delegate(PlayFabError error)
				{
					DConsole.Log("Data updated error " + error.ErrorMessage);
				});
				playfabFriendsName.Add("");
				list2.Add(friend.FriendPlayFabId);
				num++;
			}
			GameManager.Instance.friendsIDForStatus = list2;
			chatClient.AddFriends(list2.ToArray());
			GameManager.Instance.facebookFriendsMenu.addPlayFabFriends(list, playfabFriendsName, playfabFBID);
			if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
			{
				fbManager.getFacebookInvitableFriends();
			}
			else
			{
				GameManager.Instance.facebookFriendsMenu.showFriends(null, null, null);
			}
		}, OnPlayFabError);
	}

	private void OnPlayFabError(PlayFabError error)
	{
		DConsole.Log("Playfab Error: " + error.ErrorMessage);
	}

	private void GetPhotonToken()
	{
		PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest
		{
			PhotonApplicationId = StaticStrings.PhotonAppID.Trim()
		}, OnPhotonAuthenticationSuccess, OnPlayFabError);
	}

	private void OnPhotonAuthenticationSuccess(GetPhotonAuthenticationTokenResult result)
	{
		string photonCustomAuthenticationToken = result.PhotonCustomAuthenticationToken;
		DConsole.Log($"Yay, logged in session token: {photonCustomAuthenticationToken}");
		PhotonNetwork.AuthValues = new AuthenticationValues();
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
		PhotonNetwork.AuthValues.AddAuthParameter("username", PlayFabId);
		PhotonNetwork.AuthValues.AddAuthParameter("Token", result.PhotonCustomAuthenticationToken);
		PhotonNetwork.AuthValues.UserId = PlayFabId;
		PhotonNetwork.ConnectUsingSettings("1.4");
		PhotonNetwork.playerName = PlayFabId;
		authToken = result.PhotonCustomAuthenticationToken;
		getPlayerDataRequest();
		connectToChat();
	}

	public void connectToChat()
	{
		chatClient = new ChatClient(this);
		GameManager.Instance.chatClient = chatClient;
		Photon.Chat.AuthenticationValues authenticationValues = new Photon.Chat.AuthenticationValues();
		authenticationValues.UserId = PlayFabId;
		authenticationValues.AuthType = Photon.Chat.CustomAuthenticationType.Custom;
		authenticationValues.AddAuthParameter("username", PlayFabId);
		authenticationValues.AddAuthParameter("Token", authToken);
		chatClient.Connect(StaticStrings.PhotonChatID, "1.4", authenticationValues);
	}

	public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		DConsole.Log("Custom properties changed: " + DateTime.Now.ToString());
	}

	public void OnConnected()
	{
		DConsole.Log("Photon Chat connected!!!");
		chatClient.Subscribe(new string[1] { "invitationsChannel" });
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		GameManager.Instance.opponentDisconnected = true;
		GameManager.Instance.invitationID = "";
		if (GameManager.Instance.controlAvatars != null)
		{
			DConsole.Log("PLAYER DISCONNECTED " + player.NickName);
			if (PhotonNetwork.room.PlayerCount > 1)
			{
				GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = true;
			}
			else
			{
				GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = false;
			}
			int index = GameManager.Instance.opponentsIDs.IndexOf(player.NickName);
			GameManager.Instance.controlAvatars.PlayerDisconnected(index);
		}
	}

	public void showMenu()
	{
		menuCanvas.gameObject.SetActive(value: true);
		playerName.GetComponent<Text>().text = GameManager.Instance.nameMy;
		if (GameManager.Instance.avatarMy != null)
		{
			playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
		}
		splashCanvas.SetActive(value: false);
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		DConsole.Log("Subscribed to CHAT - set online status!");
		chatClient.SetOnlineStatus(2);
	}

	public void challengeFriend(string id, string message)
	{
		chatClient.SendPrivateMessage(id, "INVITE_TO_PLAY_PRIVATE;" + GameManager.Instance.nameMy + ";" + message);
		GameManager.Instance.invitationID = id;
		DConsole.Log("Send invitation to: " + id);
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		if (!sender.Equals(PlayFabId) && message.ToString().Contains("INVITE_TO_PLAY_PRIVATE"))
		{
			GameManager.Instance.invitationID = sender;
			string[] array = message.ToString().Split(';');
			string text = array[1];
			string text2 = array[2];
			string room = array[3];
			GameManager.Instance.payoutCoins = int.Parse(text2);
			GameManager.Instance.invitationDialog.GetComponent<PhotonChatListener>().showInvitationDialog(0, text, text2, room, 0);
		}
		if (GameManager.Instance.invitationID.Length != 0 && GameManager.Instance.invitationID.Equals(sender))
		{
			GameManager.Instance.invitationID = "";
		}
	}

	public void join()
	{
		PhotonNetwork.JoinRoom(roomname);
	}

	public void DebugReturn(DebugLevel level, string message)
	{
	}

	public void OnChatStateChange(ChatState state)
	{
	}

	public override void OnDisconnectedFromPhoton()
	{
		DConsole.Log("Disconnected from photon");
		switchUser();
	}

	public void DisconnecteFromPhoton()
	{
		PhotonNetwork.Disconnect();
	}

	public void switchUser()
	{
		GameManager.Instance.playfabManager.destroy();
		GameManager.Instance.facebookManager.destroy();
		GameManager.Instance.connectionLost.destroy();
		GameManager.Instance.avatarMy = null;
		GameManager.Instance.logged = false;
		GameManager.Instance.resetAllData();
		SceneManager.LoadScene("LoginSplash");
	}

	public void OnDisconnected()
	{
		DConsole.Log("Chat disconnected - Reconnect");
		connectToChat();
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
	}

	public void OnUnsubscribed(string[] channels)
	{
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		DConsole.Log("STATUS UPDATE CHAT!");
		DConsole.Log("Status change for: " + user + " to: " + status);
		bool flag = false;
		for (int i = 0; i < GameManager.Instance.friendsStatuses.Count; i++)
		{
			if (GameManager.Instance.friendsStatuses[i][0].Equals(user))
			{
				GameManager.Instance.friendsStatuses[i][1] = string.Concat(status);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			GameManager.Instance.friendsStatuses.Add(new string[2]
			{
				user,
				string.Concat(status)
			});
		}
		if (GameManager.Instance.facebookFriendsMenu != null)
		{
			GameManager.Instance.facebookFriendsMenu.updateFriendStatus(status, user);
		}
	}

	public override void OnConnectedToMaster()
	{
		isInMaster = true;
		DConsole.Log("Connected to master");
		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		DConsole.Log("Joined lobby");
		isInLobby = true;
	}

	public void JoinRoomAndStartGame()
	{
		ExitGames.Client.Photon.Hashtable roomOptions = new ExitGames.Client.Photon.Hashtable { 
		{
			"m",
			GameManager.Instance.mode.ToString() + GameManager.Instance.type.ToString() + GameManager.Instance.payoutCoins
		} };
		StartCoroutine(TryToJoinRandomRoom(roomOptions));
	}

	public IEnumerator TryToJoinRandomRoom(ExitGames.Client.Photon.Hashtable roomOptions)
	{
		while (!isInLobby || !isInMaster)
		{
			yield return new WaitForSeconds(0.05f);
		}
		PhotonNetwork.JoinRandomRoom(roomOptions, 0);
	}

	public void OnPhotonRandomJoinFailed()
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.CustomRoomPropertiesForLobby = new string[2] { "m", "v" };
		string value = generateBotMoves();
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
		{
			{
				"m",
				GameManager.Instance.mode.ToString() + GameManager.Instance.type.ToString() + GameManager.Instance.payoutCoins
			},
			{ "bt", value },
			{
				"fp",
				UnityEngine.Random.Range(0, GameManager.Instance.requiredPlayers)
			}
		};
		DConsole.Log("Create Room: " + GameManager.Instance.mode.ToString() + GameManager.Instance.type.ToString() + GameManager.Instance.payoutCoins);
		roomOptions.MaxPlayers = (byte)GameManager.Instance.requiredPlayers;
		StartCoroutine(TryToCreateGameAfterFailedToJoinRandom(roomOptions));
	}

	public string generateBotMoves()
	{
		string text = "";
		int num = 100;
		for (int i = 0; i < num; i++)
		{
			text += UnityEngine.Random.Range(1, 7);
			if (i < num - 1)
			{
				text += ",";
			}
		}
		text += ";";
		float num2 = GameManager.Instance.playerTime / 10f;
		if (num2 < 1.5f)
		{
			num2 = 1.5f;
		}
		for (int j = 0; j < num; j++)
		{
			text += UnityEngine.Random.Range(num2, GameManager.Instance.playerTime / 8f);
			if (j < num - 1)
			{
				text += ",";
			}
		}
		return text;
	}

	public void extractBotMoves(string data)
	{
		GameManager.Instance.botDiceValues = new List<int>();
		GameManager.Instance.botDelays = new List<float>();
		string[] array = data.Split(';');
		string[] array2 = array[0].Split(',');
		for (int i = 0; i < array2.Length; i++)
		{
			GameManager.Instance.botDiceValues.Add(int.Parse(array2[i]));
		}
		string[] array3 = array[1].Split(',');
		for (int j = 0; j < array3.Length; j++)
		{
			GameManager.Instance.botDelays.Add(float.Parse(array3[j]));
		}
	}

	public override void OnLeftLobby()
	{
		isInLobby = false;
		isInMaster = false;
	}

	public IEnumerator TryToCreateGameAfterFailedToJoinRandom(RoomOptions roomOptions)
	{
		while (!isInLobby || !isInMaster)
		{
			yield return new WaitForSeconds(0.05f);
		}
		PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
	}

	public override void OnJoinedRoom()
	{
		DConsole.Log("OnJoinedRoom");
		if (PhotonNetwork.room.CustomProperties.ContainsKey("bt"))
		{
			extractBotMoves(PhotonNetwork.room.CustomProperties["bt"].ToString());
		}
		if (PhotonNetwork.room.CustomProperties.ContainsKey("fp"))
		{
			GameManager.Instance.firstPlayerInGame = int.Parse(PhotonNetwork.room.CustomProperties["fp"].ToString());
		}
		else
		{
			GameManager.Instance.firstPlayerInGame = 0;
		}
		GameManager.Instance.avatarOpponent = null;
		DConsole.Log("Players in room " + PhotonNetwork.room.PlayerCount);
		GameManager.Instance.currentPlayersCount = 1;
		GameManager.Instance.controlAvatars.setCancelButton();
		if (PhotonNetwork.room.PlayerCount == 1)
		{
			GameManager.Instance.roomOwner = true;
			WaitForNewPlayer();
		}
		else if (PhotonNetwork.room.PlayerCount >= GameManager.Instance.requiredPlayers)
		{
			PhotonNetwork.room.IsOpen = false;
			PhotonNetwork.room.IsVisible = false;
		}
		if (roomOwner)
		{
			return;
		}
		GameManager.Instance.backButtonMatchPlayers.SetActive(value: false);
		for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
		{
			int num = i;
			int index = GetFirstFreeSlot();
			GameManager.Instance.opponentsIDs[index] = PhotonNetwork.otherPlayers[num].NickName;
			GetUserDataRequest request = new GetUserDataRequest
			{
				PlayFabId = PhotonNetwork.otherPlayers[num].NickName
			};
			string otherID = PhotonNetwork.otherPlayers[num].NickName;
			PlayFabClientAPI.GetUserData(request, delegate(GetUserDataResult result)
			{
				Dictionary<string, UserDataRecord> data = result.Data;
				if (data.ContainsKey("LoggedType"))
				{
					if (data["LoggedType"].Value.Equals("Facebook"))
					{
						bool fbAvatar = true;
						int avatarIndex = 0;
						if (!data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
						{
							fbAvatar = false;
							avatarIndex = int.Parse(data[MyPlayerData.AvatarIndexKey].Value.ToString());
						}
						getOpponentData(data, index, fbAvatar, avatarIndex, otherID);
					}
					else if (data.ContainsKey("PlayerName"))
					{
						GameManager.Instance.opponentsNames[index] = data["PlayerName"].Value;
						bool fbAvatar2 = true;
						int avatarIndex2 = 0;
						if (!data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
						{
							fbAvatar2 = false;
							avatarIndex2 = int.Parse(data[MyPlayerData.AvatarIndexKey].Value.ToString());
						}
						getOpponentData(data, index, fbAvatar2, avatarIndex2, otherID);
					}
					else
					{
						DConsole.Log("ERROR");
					}
				}
				else
				{
					DConsole.Log("ERROR");
				}
			}, delegate(PlayFabError error)
			{
				DConsole.Log("Get user data error: " + error.ErrorMessage);
			});
		}
	}

	public void CreatePrivateRoom()
	{
		GameManager.Instance.JoinedByID = false;
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 4;
		string text = "";
		for (int i = 0; i < 8; i++)
		{
			text += UnityEngine.Random.Range(0, 10);
		}
		roomOptions.CustomRoomPropertiesForLobby = new string[1] { "pc" };
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { 
		{
			"pc",
			GameManager.Instance.payoutCoins
		} };
		DConsole.Log("Private room name: " + text);
		PhotonNetwork.CreateRoom(text, roomOptions, TypedLobby.Default);
	}

	public override void OnCreatedRoom()
	{
		DConsole.Log("OnCreatedRoom");
		roomOwner = true;
		GameManager.Instance.roomOwner = true;
		GameManager.Instance.currentPlayersCount = 1;
		GameManager.Instance.controlAvatars.updateRoomID(PhotonNetwork.room.Name);
	}

	public override void OnLeftRoom()
	{
		DConsole.Log("OnLeftRoom called");
		roomOwner = false;
		GameManager.Instance.roomOwner = false;
		GameManager.Instance.resetAllData();
	}

	public int GetFirstFreeSlot()
	{
		int result = 0;
		for (int i = 0; i < GameManager.Instance.opponentsIDs.Count; i++)
		{
			if (GameManager.Instance.opponentsIDs[i] == null)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
	{
		DConsole.Log("Failed to create room");
		CreatePrivateRoom();
	}

	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		DConsole.Log("Failed to join room");
		if (GameManager.Instance.type == MyGameType.Private)
		{
			if (GameManager.Instance.controlAvatars != null)
			{
				GameManager.Instance.controlAvatars.ShowJoinFailed(codeAndMsg[1].ToString());
			}
		}
		else
		{
			GameManager.Instance.facebookManager.startRandomGame();
		}
	}

	private void GetPlayerDataRequest(string playerID)
	{
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		CancelInvoke("StartGameWithBots");
		DConsole.Log("New player joined " + newPlayer.NickName);
		DConsole.Log("Players Count: " + GameManager.Instance.currentPlayersCount);
		if (PhotonNetwork.room.PlayerCount >= GameManager.Instance.requiredPlayers)
		{
			PhotonNetwork.room.IsOpen = false;
			PhotonNetwork.room.IsVisible = false;
		}
		if (PhotonNetwork.room.PlayerCount > 1)
		{
			GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = true;
		}
		else
		{
			GameManager.Instance.controlAvatars.startButtonPrivate.GetComponent<Button>().interactable = true;
		}
		int index = GetFirstFreeSlot();
		GameManager.Instance.opponentsIDs[index] = newPlayer.NickName;
		PlayFabClientAPI.GetUserData(new GetUserDataRequest
		{
			PlayFabId = newPlayer.NickName
		}, delegate(GetUserDataResult result)
		{
			Dictionary<string, UserDataRecord> data = result.Data;
			if (data.ContainsKey("LoggedType"))
			{
				if (data["LoggedType"].Value.Equals("Facebook"))
				{
					bool fbAvatar = true;
					int avatarIndex = 0;
					if (!data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
					{
						fbAvatar = false;
						avatarIndex = int.Parse(data[MyPlayerData.AvatarIndexKey].Value.ToString());
					}
					getOpponentData(data, index, fbAvatar, avatarIndex, newPlayer.NickName);
				}
				else if (data.ContainsKey("PlayerName"))
				{
					GameManager.Instance.opponentsNames[index] = data["PlayerName"].Value;
					bool fbAvatar2 = true;
					int avatarIndex2 = 0;
					if (!data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
					{
						fbAvatar2 = false;
						avatarIndex2 = int.Parse(data[MyPlayerData.AvatarIndexKey].Value.ToString());
					}
					getOpponentData(data, index, fbAvatar2, avatarIndex2, newPlayer.NickName);
				}
				else
				{
					DConsole.Log("ERROR");
				}
			}
			else
			{
				DConsole.Log("ERROR");
			}
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Get user data error: " + error.ErrorMessage);
		});
	}

	private void getOpponentData(Dictionary<string, UserDataRecord> data, int index, bool fbAvatar, int avatarIndex, string id)
	{
		if (data.ContainsKey("PlayerName"))
		{
			GameManager.Instance.opponentsNames[index] = data["PlayerName"].Value;
		}
		else
		{
			GameManager.Instance.opponentsNames[index] = "Guest857643";
		}
		if (data.ContainsKey("PlayerAvatarUrl") && fbAvatar)
		{
			StartCoroutine(loadImageOpponent(data["PlayerAvatarUrl"].Value, index, id));
			return;
		}
		DConsole.Log("GET OPPONENT DATA: " + avatarIndex);
		GameManager.Instance.opponentsAvatars[index] = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars[avatarIndex];
		GameManager.Instance.controlAvatars.PlayerJoined(index, id);
	}

	public IEnumerator loadImageOpponent(string url, int index, string id)
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		yield return www.SendWebRequest();
		Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
		GameManager.Instance.opponentsAvatars[index] = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 32f);
		GameManager.Instance.controlAvatars.PlayerJoined(index, id);
	}

	public void OnUserSubscribed(string channel, string user)
	{
		throw new NotImplementedException();
	}

	public void OnUserUnsubscribed(string channel, string user)
	{
		throw new NotImplementedException();
	}
}
