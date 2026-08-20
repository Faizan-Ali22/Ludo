using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class CheckIfPlayerIsFriend : MonoBehaviour
{
	public GameObject AddFriendButton;

	public GameObject mainObject;

	private void Start()
	{
		GameManager.Instance.smallMenu = mainObject;
		GameManager.Instance.friendButtonMenu = AddFriendButton;
		if (!GameManager.Instance.offlineMode)
		{
			PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
			{
				IncludeFacebookFriends = true
			}, delegate(GetFriendsListResult result)
			{
				foreach (PlayFab.ClientModels.FriendInfo friend in result.Friends)
				{
					if (PhotonNetwork.otherPlayers[0].NickName.Equals(friend.FriendPlayFabId))
					{
						DConsole.Log("Already friends");
						AddFriendButton.SetActive(value: false);
						mainObject.GetComponent<RectTransform>().sizeDelta = new Vector2(mainObject.GetComponent<RectTransform>().sizeDelta.x, 260f);
						break;
					}
				}
			}, OnPlayFabError);
		}
		else
		{
			AddFriendButton.SetActive(value: false);
			mainObject.GetComponent<RectTransform>().sizeDelta = new Vector2(mainObject.GetComponent<RectTransform>().sizeDelta.x, 260f);
		}
	}

	private void OnPlayFabError(PlayFabError error)
	{
		DConsole.Log("Playfab Error: " + error.ErrorMessage);
	}

	private void Update()
	{
	}
}
