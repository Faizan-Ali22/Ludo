using AssemblyCSharp;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabAddFriend : MonoBehaviour
{
	public GameObject menuObject;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void AddFriend()
	{
		menuObject.GetComponent<Animator>().Play("hideMenuAnimation");
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(192, 1, sendReliable: true, null);
			PlayFabClientAPI.AddFriend(new AddFriendRequest
			{
				FriendPlayFabId = PhotonNetwork.otherPlayers[0].NickName
			}, delegate
			{
				DConsole.Log("Added friend successfully");
				GameManager.Instance.friendButtonMenu.SetActive(value: false);
				GameManager.Instance.smallMenu.GetComponent<RectTransform>().sizeDelta = new Vector2(GameManager.Instance.smallMenu.GetComponent<RectTransform>().sizeDelta.x, 260f);
			}, delegate(PlayFabError error)
			{
				DConsole.Log("Error adding friend: " + error.Error);
			});
		}
	}

	public void showMenu()
	{
		menuObject.GetComponent<Animator>().Play("ShowMenuAnimation");
	}

	public void hideMenu()
	{
		menuObject.GetComponent<Animator>().Play("hideMenuAnimation");
	}

	public void LeaveGame()
	{
		SceneManager.LoadScene("MenuScene");
		PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
		DConsole.Log("Timeout 3");
		PhotonNetwork.LeaveRoom();
		GameManager.Instance.playfabManager.roomOwner = false;
		GameManager.Instance.roomOwner = false;
		GameManager.Instance.resetAllData();
	}
}
