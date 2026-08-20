namespace AssemblyCSharp
{
	public static class StaticStrings
	{
		public static bool isDebug = true;

		public static string AndroidPackageName = "com.ludo.chupamobile";

		public static string ITunesAppID = "11111111111";

		public static string notificationTitle = "Ludo Masters";

		public static string notificationMessage = "Get your FREE fortune spin!";

		public static float WaitTimeUntilStartWithBots = 5f;

		public static string PlayFabTitleID = "B05D6";

		public static string PhotonAppID = "3b67bffc-08b6-48fb-98ba-176d40e7da1f";

		public static string PhotonChatID = "bf38375c-b6b6-4993-b1a6-3f0057aec48c";

		public static string adMobAndroidID = "ca-app-pub-4150875028712336/";

		public static string adMobiOSID = "ca-app-pub-4150875028712336/";

		public static string facebookShareLinkTitle = "I'm playing Ludo Masters!. Available on Android and iOS.";

		public static string SharePrivateLinkMessage = "Join me in Ludo Masters. My PRIVATE ROOM CODE is:";

		public static string SharePrivateLinkMessage2 = "Download Ludo Masters from:";

		public static string ShareScreenShotText = "I finished game in Ludo Masters. It's my score :-) Join me and download Ludo Masters:";

		public static int initCoinsCountGuest = 5000;

		public static int initCoinsCountFacebook = 20000;

		public static int CoinsForLinkToFacebook = 15000;

		public static int rewardForVideoAd = 250;

		public static string facebookInviteMessage = "Come play this great game!";

		public static int rewardCoinsForFriendInvite = 250;

		public static int rewardCoinsForShareViaFacebook = 50;

		public static string addCoinsHackString = "Cheat:AddCoins";

		public static bool hideCoinsTabInShop = false;

		public static string runOutOfTime = "ran out of time";

		public static string waitingForOpponent = "Waiting for your opponent";

		public static string youAreBreaking = "You start, good luck";

		public static string opponentIsBreaking = "is starting";

		public static string IWantPlayAgain = "I want to play again!";

		public static string cantPlayRightNow = "Can't play right now";

		public static string offlineModePlayer1Name = "Player 1";

		public static string offlineModePlayer2Name = "Player 2";

		public static float photonDisconnectTimeout = 0.2f;

		public static float photonDisconnectTimeoutLong = 300f;

		public static int[] bidValues = new int[8] { 500, 2000, 10000, 50000, 250000, 1000000, 2000000, 5000000 };

		public static string[] bidValuesStrings = new string[8] { "500", "2000", "10k", "50k", "250k", "1M", "2M", "5M" };

		public static bool isFourPlayerModeEnabled = true;

		public static string SoundsKey = "EnableSounds";

		public static string VibrationsKey = "EnableVibrations";

		public static string NotificationsKey = "EnableNotifications";

		public static string FriendsRequestesKey = "EnableFriendsRequestes";

		public static string PrivateRoomKey = "EnablePrivateRoomRequestes";

		public static string PrefsPlayerRemovedAds = "UserRemovedAds";

		public static string[] chatMessages = new string[18]
		{
			"Please don't kill", "Play Fast", "I will eat you", "You are good", "Well played", "Today is your day", "Hehehe", "Unlucky", "Thanks", "Yeah",
			"Remove Blockade", "Good Game", "Oops", "Today is my day", "All the best", "Hi", "Hello", "Nice move"
		};

		public static int[] chatPrices = new int[6] { 1000, 5000, 10000, 50000, 100000, 250000 };

		public static int[] emojisPrices = new int[5] { 1000, 5000, 10000, 50000, 100000 };

		public static string[] chatNames = new string[6] { "Motivate", "Emoticons", "Cheers", "Gags", "Laughing", "Talking" };

		public static string[][] chatMessagesExtended = new string[6][]
		{
			new string[6] { "Never give up", "You can do it", "I know you have it in you!", "You play like a pro!", "You can win now!", "You're great!" },
			new string[6] { ":)", ":(", ":o", ";D", ":P", ":|" },
			new string[6] { "Keep it going", "Go opponents!", "Fabulastic", "You're awesome", "Best shot ever", "That was amazing" },
			new string[6] { "OMG", "LOL", "ROFL", "O'RLY?!", "CYA", "YOLO" },
			new string[6] { "Hahaha!!!", "Ho ho ho!!!", "Mwhahahaa", "Jejeje", "Booooo!", "Muuuuuuuhhh!" },
			new string[6] { "Yes", "No", "I don't know", "Maybe", "Definitely", "Of course" }
		};
	}
}
