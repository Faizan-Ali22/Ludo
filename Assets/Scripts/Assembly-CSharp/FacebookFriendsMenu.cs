using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FacebookFriendsMenu : MonoBehaviour
{
	public GameObject list;

	public GameObject friendPrefab;

	public GameObject friendPrefab2;

	public GameObject friendsMenu;

	public GameObject mainMenu;

	public InputField filterInputField;

	public GameObject confirmDialog;

	public GameObject confirmDialogText;

	public GameObject confirmDialogButton;

	private List<GameObject> friendsObjects = new List<GameObject>();

	private Sprite[] playersAvatars;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void updateName(int i, string text, string id)
	{
		DConsole.Log(i + " -- " + friendsObjects.Count);
		if (friendsObjects != null && friendsObjects.Count > 0 && i <= friendsObjects.Count - 1 && friendsObjects[i] != null)
		{
			friendsObjects[i].SetActive(value: true);
			friendsObjects[i].transform.Find("FriendName").GetComponent<Text>().text = text;
		}
	}

	public void addPlayFabFriends(List<string> playfabIDs, List<string> playfabFBName, List<string> playfabFBID)
	{
		playersAvatars = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars;
		friendsObjects = new List<GameObject>();
		friendsMenu.gameObject.SetActive(value: true);
		for (int i = 0; i < playfabIDs.Count; i++)
		{
			GameObject friend = Object.Instantiate(friendPrefab2, Vector3.zero, Quaternion.identity);
			string text = playfabFBName[i];
			if (playfabFBName[i].Length > 13)
			{
				text = playfabFBName[i].Substring(0, 12) + "...";
			}
			friend.transform.Find("FriendName").GetComponent<Text>().text = text;
			string friendName = playfabFBName[i];
			string friendID = playfabIDs[i];
			friend.GetComponent<PlayFabFriendScript>().playfabID = friendID;
			DConsole.Log("ADD LISTENER");
			friend.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.RemoveAllListeners();
			friend.transform.Find("DeleteFriend").GetComponent<Button>().onClick.RemoveAllListeners();
			friend.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.AddListener(delegate
			{
				ChallengeFriend(friendID);
			});
			friend.transform.Find("DeleteFriend").GetComponent<Button>().onClick.AddListener(delegate
			{
				RemoveFriend(friendID, friendName, friend);
			});
			getFriendImageUrl(friendID, friend.transform.Find("Avatar/FriendAvatar").GetComponent<Image>(), friend.transform.Find("Avatar/FriendAvatar").gameObject);
			friend.transform.parent = list.transform;
			friend.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			friendsObjects.Add(friend);
			if (playfabFBName[i].Length < 1)
			{
				friendsObjects[i].SetActive(value: false);
			}
		}
	}

	public void updateFriendStatus(int status, string id)
	{
		foreach (GameObject friendsObject in friendsObjects)
		{
			if (friendsObject.GetComponent<PlayFabFriendScript>().playfabID.Equals(id))
			{
				switch (status)
				{
				case 2:
					friendsObject.GetComponent<PlayFabFriendScript>().statusIndicatorText.GetComponent<Text>().text = "Online";
					friendsObject.GetComponent<PlayFabFriendScript>().statusIndicator.GetComponent<Image>().color = Color.green;
					break;
				case 0:
					friendsObject.GetComponent<PlayFabFriendScript>().statusIndicatorText.GetComponent<Text>().text = "Offline";
					friendsObject.GetComponent<PlayFabFriendScript>().statusIndicator.GetComponent<Image>().color = Color.red;
					break;
				}
			}
		}
	}

	public void getFriendImageUrl(string id, Image image, GameObject imobject)
	{
		PlayFabClientAPI.GetUserData(new GetUserDataRequest
		{
			PlayFabId = id
		}, delegate(GetUserDataResult result)
		{
			Dictionary<string, UserDataRecord> data = result.Data;
			imobject.SetActive(value: true);
			if (data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
			{
				if (data.ContainsKey("PlayerAvatarUrl"))
				{
					filterInputField.GetComponent<MonoBehaviour>().StartCoroutine(loadImage(data["PlayerAvatarUrl"].Value, image));
				}
			}
			else
			{
				if (playersAvatars == null)
				{
					DConsole.Log("NULLLLL");
				}
				image.sprite = playersAvatars[int.Parse(data[MyPlayerData.AvatarIndexKey].Value)];
			}
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Data updated error " + error.ErrorMessage);
		});
	}

	public void showFriends()
	{
	}

	public void showFriends(List<string> friendsNames, List<string> friendsIDs, List<string> friendsAvatars)
	{
		friendsMenu.gameObject.SetActive(value: true);
		if (friendsNames == null)
		{
			return;
		}
		for (int i = 0; i < friendsNames.Count; i++)
		{
			GameObject gameObject = Object.Instantiate(friendPrefab, Vector3.zero, Quaternion.identity);
			string text = friendsNames[i];
			if (friendsNames[i].Length > 13)
			{
				text = friendsNames[i].Substring(0, 12) + "...";
			}
			gameObject.transform.Find("FriendName").GetComponent<Text>().text = text;
			string friendID = friendsIDs[i];
			gameObject.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.RemoveAllListeners();
			gameObject.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.AddListener(delegate
			{
				InviteFriend(friendID);
			});
			gameObject.GetComponent<MonoBehaviour>().StartCoroutine(loadImage(friendsAvatars[i], gameObject.transform.Find("Avatar/FriendAvatar").GetComponent<Image>()));
			gameObject.transform.parent = list.transform;
			gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			friendsObjects.Add(gameObject);
			DConsole.Log("KUPA");
			for (int num = 0; num < GameManager.Instance.friendsStatuses.Count; num++)
			{
				string[] array = GameManager.Instance.friendsStatuses[num];
				DConsole.Log(friendID + "  " + array[0]);
				if (array[0].Equals(friendID))
				{
					DConsole.Log("Found FRIEND");
					if (array[1].Equals(string.Concat(2)))
					{
						GameManager.Instance.facebookFriendsMenu.updateFriendStatus(2, friendID);
					}
					break;
				}
			}
		}
	}

	public void AddFacebookFriend(string friendsNames, string friendsIDs, string friendsAvatars)
	{
		if (friendsNames != null)
		{
			GameObject gameObject = Object.Instantiate(friendPrefab, Vector3.zero, Quaternion.identity);
			gameObject.transform.parent = list.transform;
			string text = friendsNames;
			if (friendsNames.Length > 13)
			{
				text = friendsNames.Substring(0, 12) + "...";
			}
			gameObject.transform.Find("FriendName").GetComponent<Text>().text = text;
			string friendID = friendsIDs;
			gameObject.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.RemoveAllListeners();
			gameObject.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.AddListener(delegate
			{
				InviteFriend(friendID);
			});
			gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			friendsObjects.Add(gameObject);
			gameObject.GetComponent<MonoBehaviour>().StartCoroutine(loadImage(friendsAvatars, gameObject.transform.Find("Avatar/FriendAvatar").GetComponent<Image>()));
		}
	}

	public void RemoveFriend(string id, string name, GameObject friend)
	{
		DConsole.Log("click");
		confirmDialog.SetActive(value: true);
		confirmDialogText.GetComponent<Text>().text = "Remove " + friend.transform.Find("FriendName").GetComponent<Text>().text + " from your friends?";
		string friendID = id;
		confirmDialogButton.GetComponent<Button>().onClick.RemoveAllListeners();
		confirmDialogButton.GetComponent<Button>().onClick.AddListener(delegate
		{
			removeFriendRequest(friendID, friend);
		});
	}

	public void removeFriendRequest(string id, GameObject friend)
	{
		DConsole.Log("REMOVE CLICK");
		PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
		{
			FriendPlayFabId = id
		}, delegate
		{
			DConsole.Log("Removed friend successfully");
			friend.SetActive(value: false);
		}, delegate(PlayFabError error)
		{
			DConsole.Log("Error removing friend: " + error.Error);
		});
	}

	public void hideFriends()
	{
		filterInputField.text = "";
		foreach (GameObject friendsObject in friendsObjects)
		{
			Object.Destroy(friendsObject);
		}
		friendsMenu.gameObject.SetActive(value: false);
	}

	public void FilterFriends()
	{
		string text = filterInputField.text;
		for (int i = 0; i < friendsObjects.Count; i++)
		{
			if (friendsObjects[i].transform.Find("FriendName").GetComponent<Text>().text.Length > 0)
			{
				friendsObjects[i].SetActive(value: true);
			}
			if (!friendsObjects[i].transform.Find("FriendName").GetComponent<Text>().text.ToLower().Contains(text.ToLower()))
			{
				friendsObjects[i].SetActive(value: false);
			}
		}
	}

	public void InviteFriend(string i)
	{
		DConsole.Log(i ?? "");
		List<string> list = new List<string>();
		list.Add(i);
		FB.AppRequest(StaticStrings.facebookInviteMessage, list, null, null, null, null, null, delegate(IAppRequestResult result)
		{
			DConsole.Log("RESULT: Cancelled - " + result.Cancelled);
			if (!result.Cancelled && (result.Error == null || (result.Error != null && result.Error.Equals(""))))
			{
				GameManager.Instance.playfabManager.addCoinsRequest(StaticStrings.rewardCoinsForFriendInvite);
			}
			DConsole.Log("REQUEST RESULT: " + result.RawResult);
		});
	}

	public void ChallengeFriend(string id)
	{
		DConsole.Log("Challenge friend: " + id);
		GameManager.Instance.facebookFriendsMenu.hideFriends();
		GameManager.Instance.playfabManager.challengeFriend(id, GameManager.Instance.payoutCoins + ";" + GameManager.Instance.privateRoomID);
	}

	public void loadImageFBID(string userID, Image image)
	{
		FB.API("/" + userID + "/picture?type=square&height=92&width=92", HttpMethod.GET, delegate(IGraphResult result)
		{
			if (result.Texture != null)
			{
				image.sprite = Sprite.Create(result.Texture, new Rect(0f, 0f, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32f);
			}
		});
	}

	public IEnumerator loadImage(string url, Image image)
	{
		UnityWebRequest www = UnityWebRequest.Get(url);
		yield return www.SendWebRequest();
		Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
		image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 32f);
	}
}
