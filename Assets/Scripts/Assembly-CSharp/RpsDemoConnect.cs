using Photon;
using UnityEngine;
using UnityEngine.UI;

public class RpsDemoConnect : PunBehaviour
{
	public InputField InputField;

	public string UserId;

	private string previousRoomPlayerPrefKey = "PUN:Demo:RPS:PreviousRoom";

	public string previousRoom;

	private const string MainSceneName = "DemoRPS-Scene";

	private const string NickNamePlayerPrefsKey = "NickName";

	private void Start()
	{
		InputField.text = (PlayerPrefs.HasKey("NickName") ? PlayerPrefs.GetString("NickName") : "");
	}

	public void ApplyUserIdAndConnect()
	{
		string text = "DemoNick";
		if (InputField != null && !string.IsNullOrEmpty(InputField.text))
		{
			text = InputField.text;
			PlayerPrefs.SetString("NickName", text);
		}
		if (PhotonNetwork.AuthValues == null)
		{
			PhotonNetwork.AuthValues = new AuthenticationValues();
		}
		PhotonNetwork.AuthValues.UserId = text;
		DConsole.Log("Nickname: " + text + " userID: " + UserId, this);
		PhotonNetwork.playerName = text;
		PhotonNetwork.ConnectUsingSettings("0.5");
		PhotonHandler.StopFallbackSendAckThread();
	}

	public override void OnConnectedToMaster()
	{
		UserId = PhotonNetwork.player.UserId;
		if (PlayerPrefs.HasKey(previousRoomPlayerPrefKey))
		{
			DConsole.Log("getting previous room from prefs: ");
			previousRoom = PlayerPrefs.GetString(previousRoomPlayerPrefKey);
			PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey);
		}
		if (!string.IsNullOrEmpty(previousRoom))
		{
			DConsole.Log("ReJoining previous room: " + previousRoom);
			PhotonNetwork.ReJoinRoom(previousRoom);
			previousRoom = null;
		}
		else
		{
			PhotonNetwork.JoinRandomRoom();
		}
	}

	public override void OnJoinedLobby()
	{
		OnConnectedToMaster();
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		DConsole.Log("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			MaxPlayers = 2,
			PlayerTtl = 20000
		}, null);
	}

	public override void OnJoinedRoom()
	{
		DConsole.Log("Joined room: " + PhotonNetwork.room.Name);
		previousRoom = PhotonNetwork.room.Name;
		PlayerPrefs.SetString(previousRoomPlayerPrefKey, previousRoom);
	}

	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		DConsole.Log("OnPhotonJoinRoomFailed");
		previousRoom = null;
		PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey);
	}

	public override void OnConnectionFail(DisconnectCause cause)
	{
		DConsole.Log(string.Concat("Disconnected due to: ", cause, ". this.previousRoom: ", previousRoom));
	}

	public override void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
	{
		DConsole.Log("OnPhotonPlayerActivityChanged() for " + otherPlayer.NickName + " IsInactive: " + otherPlayer.IsInactive);
	}
}
