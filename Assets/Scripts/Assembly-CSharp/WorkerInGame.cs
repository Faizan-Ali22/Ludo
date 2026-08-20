using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkerInGame : Photon.MonoBehaviour
{
	public Transform playerPrefab;

	public void Awake()
	{
		if (!PhotonNetwork.connected)
		{
			SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
		}
		else
		{
			PhotonNetwork.Instantiate(playerPrefab.name, base.transform.position, Quaternion.identity, 0);
		}
	}

	public void OnGUI()
	{
		if (GUILayout.Button("Return to Lobby"))
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		DConsole.Log("OnMasterClientSwitched: " + player);
		InRoomChat component = GetComponent<InRoomChat>();
		if (component != null)
		{
			string newLine = ((!player.IsLocal) ? (player.NickName + " is Master Client now.") : "You are Master Client now.");
			component.AddLine(newLine);
		}
	}

	public void OnLeftRoom()
	{
		DConsole.Log("OnLeftRoom (local)");
		SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
	}

	public void OnDisconnectedFromPhoton()
	{
		DConsole.Log("OnDisconnectedFromPhoton");
		SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		DConsole.Log("OnPhotonInstantiate " + info.sender);
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		DConsole.Log("OnPhotonPlayerConnected: " + player);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		DConsole.Log("OnPlayerDisconneced: " + player);
	}

	public void OnFailedToConnectToPhoton()
	{
		DConsole.Log("OnFailedToConnectToPhoton");
		SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
	}
}
