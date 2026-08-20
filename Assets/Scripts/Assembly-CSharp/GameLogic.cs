using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public static int playerWhoIsIt;

	private static PhotonView ScenePhotonView;

	public void Start()
	{
		ScenePhotonView = GetComponent<PhotonView>();
	}

	public void OnJoinedRoom()
	{
		if (PhotonNetwork.playerList.Length == 1)
		{
			playerWhoIsIt = PhotonNetwork.player.ID;
		}
		DConsole.Log("playerWhoIsIt: " + playerWhoIsIt);
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		DConsole.Log("OnPhotonPlayerConnected: " + player);
		if (PhotonNetwork.isMasterClient)
		{
			TagPlayer(playerWhoIsIt);
		}
	}

	public static void TagPlayer(int playerID)
	{
		DConsole.Log("TagPlayer: " + playerID);
		ScenePhotonView.RPC("TaggedPlayer", PhotonTargets.All, playerID);
	}

	[PunRPC]
	public void TaggedPlayer(int playerID)
	{
		playerWhoIsIt = playerID;
		DConsole.Log("TaggedPlayer: " + playerID);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		DConsole.Log("OnPhotonPlayerDisconnected: " + player);
		if (PhotonNetwork.isMasterClient && player.ID == playerWhoIsIt)
		{
			TagPlayer(PhotonNetwork.player.ID);
		}
	}

	public void OnMasterClientSwitched()
	{
		DConsole.Log("OnMasterClientSwitched");
	}
}
