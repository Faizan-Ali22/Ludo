using AssemblyCSharp;
using Photon;
using UnityEngine;
using UnityEngine.UI;

public class PhotonChatListener : PunBehaviour
{
	private Animator animator;

	public Text text;

	private string roomName;

	public string type;

	public GameObject okButton;

	public GameObject rejectButton;

	public GameObject acceptButton;

	public GameObject matchPlayersCanvas;

	public GameObject friendsCanvas;

	public GameObject menuCanvas;

	public GameObject gameTitle;

	public GameObject payoutCoinsText;

	private bool leftRoom;

	private bool Joined;

	private void Start()
	{
		GameManager.Instance.invitationDialog = base.gameObject;
		animator = GetComponent<Animator>();
	}

	public void showInvitationDialog(int type, string name, string id, string room, int tableNumber)
	{
		if (PlayerPrefs.GetInt(StaticStrings.PrivateRoomKey, 0) == 0)
		{
			leftRoom = false;
			Joined = false;
			payoutCoinsText.GetComponent<Text>().text = string.Concat(GameManager.Instance.payoutCoins);
			rejectButton.SetActive(value: true);
			acceptButton.SetActive(value: true);
			okButton.SetActive(value: false);
			this.type = "invited";
			roomName = room;
			text.text = name + " invite you to private room.";
			animator.Play("InvitationDialogShow");
		}
		else
		{
			DConsole.Log("Invitations OFF");
		}
	}

	public override void OnConnectedToMaster()
	{
		if (!Joined && leftRoom)
		{
			JoinRoom("accepted");
			Joined = true;
		}
	}

	public void JoinRoom(string a)
	{
		if (!a.Equals("accepted"))
		{
			return;
		}
		DConsole.Log("Trying to join room: " + roomName);
		if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.payoutCoins)
		{
			PhotonNetwork.JoinRoom(roomName);
			if (GameManager.Instance.type != MyGameType.Private)
			{
				GameManager.Instance.facebookManager.startRandomGame();
			}
			else if (GameManager.Instance.JoinedByID)
			{
				DConsole.Log("Joined by id!");
				GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
			}
			else
			{
				DConsole.Log("Joined and created");
				GameManager.Instance.playfabManager.CreatePrivateRoom();
				GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
			}
		}
		else
		{
			GameManager.Instance.dialog.SetActive(value: true);
		}
	}

	public void hideDialog(string a)
	{
		GameManager.Instance.type = MyGameType.Private;
		GameManager.Instance.JoinedByID = true;
		if (PhotonNetwork.inRoom)
		{
			leftRoom = true;
			PhotonNetwork.LeaveRoom();
		}
		else
		{
			JoinRoom(a);
		}
		animator.Play("InvitationDialogHide");
	}
}
