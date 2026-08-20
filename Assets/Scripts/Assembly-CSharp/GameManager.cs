using System.Collections.Generic;
using Photon.Chat;
using UnityEngine;

public class GameManager
{
	public int readyPlayersCount = 1;

	public int menuLoadCount;

	public List<int> botDiceValues = new List<int>();

	public List<float> botDelays = new List<float>();

	public bool needToKillOpponentToEnterHome;

	public List<PlayerObject> playerObjects;

	public PlayerObject currentPlayer;

	public Sprite facebookAvatar;

	public MyPlayerData myPlayerData = new MyPlayerData();

	public string privateRoomID;

	public string[] scenes = new string[4] { "GameScene", "CheckersScene", "TheMillScene", "SoccerScene" };

	public string[] gamesNames = new string[4] { "GOMOKU", "CHECKERS", "THE MILL", "SOCCER" };

	public string GameScene = "SoccerScene";

	private static GameManager instance;

	public List<Sprite> opponentsAvatars = new List<Sprite> { null, null, null };

	public List<string> opponentsNames = new List<string> { null, null, null };

	public List<string> opponentsIDs = new List<string> { null, null, null };

	public GameObject myAvatarGameObject;

	public GameObject myNameGameObject;

	public int requiredPlayers = 4;

	public int firstPlayerInGame;

	public int readyPlayers;

	public int currentPlayersCount;

	public bool offlineMode;

	public AdsController adsController;

	public int myPlayerIndex;

	public float playerTime = 20f;

	public bool readyToAnimateCoins;

	public bool showTargetLines;

	public bool callPocketBlack;

	public bool callPocketAll;

	public bool LinkFbAccount;

	public bool inviteFriendActivated;

	public InitMenuScript initMenuScript;

	public string challengedFriendID;

	public GameObject tablesCanvas;

	public bool stopTimer;

	public bool ownSolids;

	public bool playersHaveTypes;

	public bool firstBallTouched;

	public bool wasFault;

	public bool validPot;

	public int validPotsCount;

	public string faultMessage = "";

	public FacebookFriendsMenu facebookFriendsMenu;

	public GameObject matchPlayerObject;

	public GameObject backButtonMatchPlayers;

	public GameObject MatchPlayersCanvas;

	public GameObject reconnectingWindow;

	public GameControllerScript gameControllerScript;

	public FacebookManager facebookManager;

	public GameObject whiteBall;

	public bool testValue;

	public bool hasCueInHand;

	public GameObject FacebookLinkButton;

	public int shotPower;

	public bool ballsStriked;

	public List<string> ballTouchBeforeStrike = new List<string>();

	public GameObject ballHand;

	public bool iWon;

	public bool iLost;

	public bool iDraw;

	public bool calledPocket;

	public int solidPoted;

	public int stripedPoted;

	public bool noTypesPotedStriped;

	public bool noTypesPotedSolid;

	public GameObject usingCueText;

	public int ballTouchedBand;

	public bool receivedInitPositions;

	public Vector3[] initPositions;

	public GameObject[] balls;

	public bool logged;

	public List<string> friendsIDForStatus = new List<string>();

	public string nameMy;

	public Sprite avatarMy;

	public string avatarMyUrl;

	public GameObject dialog;

	public string nameOpponent;

	public Sprite avatarOpponent;

	public string opponentPlayFabID;

	public int offlinePlayerTurn = 1;

	public bool offlinePlayer1OwnSolid = true;

	public string facebookIDMy;

	public bool playerDisconnected;

	public GameObject invitationDialog;

	public ChatClient chatClient;

	public int coinsCount;

	public bool roomOwner;

	public float linesLength = 5f;

	public int avatarMoveSpeed = 15;

	public bool opponentDisconnected;

	public CueController cueController;

	public GameObject friendButtonMenu;

	public GameObject smallMenu;

	public PlayFabManager playfabManager;

	public float messageTime;

	public int tableNumber;

	public AudioSource[] audioSources;

	public int calledPocketID;

	public GameObject coinsTextMenu;

	public GameObject coinsTextShop;

	public int cueIndex;

	public int cuePower;

	public int cueAim;

	public int cueTime;

	public IAPController IAPControl;

	public GameObject cueObject;

	public List<string[]> friendsStatuses = new List<string[]>();

	public int opponentCueIndex;

	public int opponentCueTime;

	public ControlAvatars controlAvatars;

	public InterstitialAdsControllerScript interstitialAds;

	public AdMobObjectController adsScript;

	public ConnectionLostController connectionLost;

	public bool opponentActive = true;

	public IMiniGame miniGame;

	public bool myTurnDone;

	public string invitationID = "";

	public MyGameMode mode;

	public MyGameType type;

	public bool isMyTurn;

	public bool diceShot;

	public string[] PlayersIDs;

	public bool gameSceneStarted;

	public int payoutCoins = 15000000;

	public bool JoinedByID;

	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameManager();
			}
			return instance;
		}
	}

	public void resetAllData()
	{
		readyPlayersCount = 1;
		gameSceneStarted = false;
		opponentsIDs = new List<string> { null, null, null };
		opponentsAvatars = new List<Sprite> { null, null, null };
		opponentsNames = new List<string> { null, null, null };
		currentPlayersCount = 0;
		myTurnDone = false;
		opponentActive = true;
		readyToAnimateCoins = false;
		opponentDisconnected = false;
		offlinePlayerTurn = 1;
		offlinePlayer1OwnSolid = true;
		offlineMode = false;
		solidPoted = 0;
		stripedPoted = 0;
		messageTime = 0f;
		stopTimer = false;
		ownSolids = false;
		playersHaveTypes = false;
		firstBallTouched = false;
		wasFault = false;
		validPot = false;
		validPotsCount = 0;
		faultMessage = "";
		hasCueInHand = false;
		ballsStriked = false;
		ballTouchBeforeStrike = new List<string>();
		PlayersIDs = null;
		ballTouchedBand = 0;
		receivedInitPositions = false;
	}

	private GameManager()
	{
	}

	public void resetTurnVariables()
	{
		stopTimer = false;
	}
}
