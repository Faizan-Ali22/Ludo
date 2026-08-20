using System;
using System.Collections.Generic;
using AssemblyCSharp;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class MyPlayerData
{
	public static string TitleFirstLoginKey = "TitleFirstLogin";

	public static string TotalEarningsKey = "TotalEarnings";

	public static string GamesPlayedKey = "GamesPlayed";

	public static string TwoPlayerWinsKey = "TwoPlayerWins";

	public static string FourPlayerWinsKey = "FourPlayerWins";

	public static string PlayerName = "PlayerName";

	public static string CoinsKey = "Coins";

	public static string ChatsKey = "Chats";

	public static string EmojiKey = "Emoji";

	public static string AvatarIndexKey = "AvatarIndex";

	public static string FortuneWheelLastFreeKey = "FortuneWheelLastFreeTime";

	public Dictionary<string, UserDataRecord> data;

	public int GetCoins()
	{
		if (data != null && data.ContainsKey(CoinsKey))
		{
			return int.Parse(data[CoinsKey].Value);
		}
		return 0;
	}

	public int GetTotalEarnings()
	{
		return int.Parse(data[TotalEarningsKey].Value);
	}

	public int GetTwoPlayerWins()
	{
		return int.Parse(data[TwoPlayerWinsKey].Value);
	}

	public int GetFourPlayerWins()
	{
		return int.Parse(data[FourPlayerWinsKey].Value);
	}

	public int GetPlayedGamesCount()
	{
		if (data != null)
		{
			return int.Parse(data[GamesPlayedKey].Value);
		}
		return -1;
	}

	public string GetAvatarIndex()
	{
		return data[AvatarIndexKey].Value;
	}

	public string GetChats()
	{
		return data[ChatsKey].Value;
	}

	public string GetEmoji()
	{
		if (data.ContainsKey(EmojiKey))
		{
			return data[EmojiKey].Value;
		}
		return "error";
	}

	public string GetPlayerName()
	{
		if (data.ContainsKey(PlayerName))
		{
			return data[PlayerName].Value;
		}
		return "Error";
	}

	public string GetLastFortuneTime()
	{
		if (data.ContainsKey(FortuneWheelLastFreeKey))
		{
			return data[FortuneWheelLastFreeKey].Value;
		}
		string text = DateTime.Now.Ticks.ToString();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(FortuneWheelLastFreeKey, text);
		UpdateUserData(dictionary);
		return text;
	}

	public MyPlayerData()
	{
	}

	public MyPlayerData(Dictionary<string, UserDataRecord> data, bool myData)
	{
		this.data = data;
		if (myData)
		{
			if (GetAvatarIndex().Equals("fb"))
			{
				GameManager.Instance.avatarMy = GameManager.Instance.facebookAvatar;
			}
			else
			{
				GameManager.Instance.avatarMy = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars[int.Parse(GetAvatarIndex())];
			}
			GameManager.Instance.nameMy = GetPlayerName();
		}
		DConsole.Log("MY DATA LOADED");
	}

	public void UpdateUserData(Dictionary<string, string> data)
	{
		if (this.data != null)
		{
			foreach (KeyValuePair<string, string> datum in data)
			{
				DConsole.Log("SAVE: " + datum.Key);
				if (this.data.ContainsKey(datum.Key))
				{
					DConsole.Log("AA");
					this.data[datum.Key].Value = datum.Value;
				}
			}
		}
		PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
		{
			Data = data,
			Permission = UserDataPermission.Public
		}, delegate
		{
			DConsole.Log("Data updated successfull ");
		}, delegate(PlayFabError error1)
		{
			DConsole.Log("Data updated error " + error1.ErrorMessage);
		});
	}

	public static Dictionary<string, string> InitialUserData(bool fb)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(TotalEarningsKey, "0");
		dictionary.Add(ChatsKey, "");
		dictionary.Add(EmojiKey, "");
		if (fb)
		{
			dictionary.Add(CoinsKey, StaticStrings.initCoinsCountFacebook.ToString());
			dictionary.Add(AvatarIndexKey, "fb");
		}
		else
		{
			dictionary.Add(CoinsKey, StaticStrings.initCoinsCountGuest.ToString());
			dictionary.Add(AvatarIndexKey, "0");
		}
		dictionary.Add(GamesPlayedKey, "0");
		dictionary.Add(TwoPlayerWinsKey, "0");
		dictionary.Add(FourPlayerWinsKey, "0");
		dictionary.Add(TitleFirstLoginKey, "1");
		dictionary.Add(FortuneWheelLastFreeKey, DateTime.Now.Ticks.ToString());
		return dictionary;
	}
}
