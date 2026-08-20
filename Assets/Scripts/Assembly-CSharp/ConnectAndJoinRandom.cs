using Photon;

public class ConnectAndJoinRandom : MonoBehaviour
{
	public bool AutoConnect = true;

	public byte Version = 1;

	private bool ConnectInUpdate = true;

	public virtual void Start()
	{
		PhotonNetwork.autoJoinLobby = false;
	}

	public virtual void Update()
	{
		if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
		{
			DConsole.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
			ConnectInUpdate = false;
			PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
		}
	}

	public virtual void OnConnectedToMaster()
	{
		DConsole.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
	}

	public virtual void OnJoinedLobby()
	{
		DConsole.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		DConsole.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			MaxPlayers = 4
		}, null);
	}

	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		DConsole.LogError("Cause: " + cause);
	}

	public void OnJoinedRoom()
	{
		DConsole.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
	}
}
