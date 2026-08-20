using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PhotonChatListener2 : MonoBehaviour
{
	private Animator animator;

	public Text text;

	public string type;

	public GameObject okButton;

	public GameObject rejectButton;

	public GameObject acceptButton;

	public GameObject matchPlayersCanvas;

	public GameObject friendsCanvas;

	public GameObject menuCanvas;

	public GameObject gameTitle;

	public GameObject addedFriendWindow;

	private string friendID;

	private void Start()
	{
		GameManager.Instance.invitationDialog = base.gameObject;
		animator = GetComponent<Animator>();
	}

	public void showInvitationDialog(string name, string id, string room)
	{
		friendID = name;
		rejectButton.SetActive(value: true);
		acceptButton.SetActive(value: true);
		type = "invited";
		text.text = id + " want to add you to Friends";
		animator.Play("AddFriendAnimation");
	}

	public void accept()
	{
		PlayFabClientAPI.AddFriend(new AddFriendRequest
		{
			FriendPlayFabId = friendID
		}, delegate
		{
			addedFriendWindow.SetActive(value: true);
			DConsole.Log("Added friend successfully");
		}, delegate(PlayFabError error)
		{
			addedFriendWindow.SetActive(value: true);
			DConsole.Log("Error adding friend: " + error.Error);
		});
		animator.Play("InvitationDialogHide");
	}

	public void hideDialog(string a)
	{
		animator.Play("InvitationDialogHide");
	}
}
