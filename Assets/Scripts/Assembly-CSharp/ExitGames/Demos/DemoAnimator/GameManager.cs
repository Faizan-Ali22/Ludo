using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExitGames.Demos.DemoAnimator
{
	public class GameManager : PunBehaviour
	{
		public static GameManager Instance;

		[Tooltip("The prefab to use for representing the player")]
		public GameObject playerPrefab;

		private GameObject instance;

		private void Start()
		{
			Instance = this;
			if (!PhotonNetwork.connected)
			{
				SceneManager.LoadScene("PunBasics-Launcher");
			}
			else if (playerPrefab == null)
			{
				DConsole.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			}
			else if (PlayerManager.LocalPlayerInstance == null)
			{
				DConsole.Log("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);
				PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
			}
			else
			{
				DConsole.Log("Ignoring scene load for " + SceneManagerHelper.ActiveSceneName);
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
		}

		public override void OnPhotonPlayerConnected(PhotonPlayer other)
		{
			DConsole.Log("OnPhotonPlayerConnected() " + other.NickName);
			if (PhotonNetwork.isMasterClient)
			{
				DConsole.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
				LoadArena();
			}
		}

		public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
		{
			DConsole.Log("OnPhotonPlayerDisconnected() " + other.NickName);
			if (PhotonNetwork.isMasterClient)
			{
				DConsole.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
				LoadArena();
			}
		}

		public override void OnLeftRoom()
		{
			SceneManager.LoadScene("PunBasics-Launcher");
		}

		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

		private void LoadArena()
		{
			if (!PhotonNetwork.isMasterClient)
			{
				DConsole.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
			}
			DConsole.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
			PhotonNetwork.LoadLevel("PunBasics-Room for " + PhotonNetwork.room.PlayerCount);
		}
	}
}
