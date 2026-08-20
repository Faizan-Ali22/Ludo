using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileController : MonoBehaviour
{
	public GameObject changeName;

	public GameObject gridView;

	public GameObject buttonPrefab;

	private string avatarIndex;

	public GameObject PlayerNameMain;

	public GameObject PlayerAvatarMain;

	private StaticGameVariablesController staticController;

	private List<GameObject> buttons = new List<GameObject>();

	private void Start()
	{
		avatarIndex = GameManager.Instance.myPlayerData.GetAvatarIndex();
		staticController = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>();
		changeName.GetComponent<InputField>().text = GameManager.Instance.nameMy;
		if (GameManager.Instance.facebookAvatar != null)
		{
			GameObject gameObject = Object.Instantiate(buttonPrefab);
			gameObject.GetComponent<ProfilePictureController>().picture.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
			gameObject.transform.SetParent(gridView.transform, worldPositionStays: false);
			GameObject border = gameObject.GetComponent<ProfilePictureController>().frame;
			if (GameManager.Instance.myPlayerData.GetAvatarIndex().Equals("fb"))
			{
				border.GetComponent<Image>().color = Color.green;
			}
			string index = "fb";
			gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				ClickButton(index, border);
			});
			buttons.Add(border);
		}
		for (int num = 0; num < staticController.avatars.Length; num++)
		{
			GameObject gameObject2 = Object.Instantiate(buttonPrefab);
			gameObject2.GetComponent<ProfilePictureController>().picture.GetComponent<Image>().sprite = staticController.avatars[num];
			gameObject2.transform.SetParent(gridView.transform, worldPositionStays: false);
			GameObject border2 = gameObject2.GetComponent<ProfilePictureController>().frame;
			if (GameManager.Instance.myPlayerData.GetAvatarIndex().Equals(string.Concat(num)))
			{
				border2.GetComponent<Image>().color = Color.green;
			}
			string index2 = string.Concat(num);
			gameObject2.GetComponent<Button>().onClick.RemoveAllListeners();
			gameObject2.GetComponent<Button>().onClick.AddListener(delegate
			{
				ClickButton(index2, border2);
			});
			buttons.Add(border2);
		}
	}

	public void ClickButton(string avatarIndex, GameObject border)
	{
		this.avatarIndex = avatarIndex;
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].GetComponent<Image>().color = Color.white;
		}
		border.GetComponent<Image>().color = Color.green;
	}

	public void Save()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(MyPlayerData.AvatarIndexKey, avatarIndex);
		dictionary.Add(MyPlayerData.PlayerName, changeName.GetComponent<InputField>().text);
		GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
		PlayerNameMain.GetComponent<Text>().text = changeName.GetComponent<InputField>().text;
		GameManager.Instance.nameMy = changeName.GetComponent<InputField>().text;
		if (avatarIndex.Equals("fb"))
		{
			PlayerAvatarMain.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
			GameManager.Instance.avatarMy = GameManager.Instance.facebookAvatar;
		}
		else
		{
			PlayerAvatarMain.GetComponent<Image>().sprite = staticController.avatars[int.Parse(avatarIndex)];
			GameManager.Instance.avatarMy = staticController.avatars[int.Parse(avatarIndex)];
		}
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
	}
}
