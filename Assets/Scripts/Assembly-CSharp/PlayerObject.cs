using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayerObject
{
	public GameObject dice;

	public GameObject[] pawns;

	public GameObject homeLockObjects;

	public bool canEnterHome;

	public string name;

	public Sprite avatar;

	public string id;

	public GameObject timer;

	public bool isActive = true;

	public GameObject AvatarObject;

	public GameObject ChatBubble;

	public GameObject ChatBubbleText;

	public GameObject ChatbubbleImage;

	public MyPlayerData data;

	public bool isBot;

	public int finishedPawns;

	public PlayerObject(string name, string id, Sprite avatar)
	{
		this.name = name;
		this.id = id;
		this.avatar = avatar;
		if (!id.Contains("_BOT"))
		{
			isBot = false;
			getPlayerDataRequest(this.id);
			return;
		}
		isBot = true;
		data = new MyPlayerData();
		data.data = new Dictionary<string, UserDataRecord>();
		UserDataRecord value = new UserDataRecord
		{
			Value = Random.Range(500, 1000).ToString()
		};
		data.data.Add(MyPlayerData.GamesPlayedKey, value);
		UserDataRecord value2 = new UserDataRecord
		{
			Value = Random.Range(1, 250).ToString()
		};
		data.data.Add(MyPlayerData.TwoPlayerWinsKey, value2);
		UserDataRecord value3 = new UserDataRecord
		{
			Value = Random.Range(1, 250).ToString()
		};
		data.data.Add(MyPlayerData.FourPlayerWinsKey, value3);
		UserDataRecord value4 = new UserDataRecord
		{
			Value = (Random.Range(10000, 50000) * 100).ToString()
		};
		data.data.Add(MyPlayerData.TotalEarningsKey, value4);
		UserDataRecord value5 = new UserDataRecord
		{
			Value = (Random.Range(1, 10000) * 100).ToString()
		};
		data.data.Add(MyPlayerData.CoinsKey, value5);
	}

	public void getPlayerDataRequest(string id)
	{
		DConsole.Log("Get player data request: " + id);
		PlayFabClientAPI.GetUserData(new GetUserDataRequest
		{
			PlayFabId = id
		}, delegate(GetUserDataResult result)
		{
			Dictionary<string, UserDataRecord> dictionary = result.Data;
			data = new MyPlayerData(dictionary, myData: false);
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Data updated error " + error.ErrorMessage);
		});
	}
}
