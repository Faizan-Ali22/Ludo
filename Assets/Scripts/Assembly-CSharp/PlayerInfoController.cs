using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoController : MonoBehaviour
{
	public GameObject window;

	public GameObject avatar;

	public GameObject playername;

	public Sprite avatarSprite;

	public GameObject TotalEarningsValue;

	public GameObject CurrentMoneyValue;

	public GameObject GamesWonValue;

	public GameObject WinRateValue;

	public GameObject TwoPlayerWinsValue;

	public GameObject FourPlayerWinsValue;

	public GameObject FourPlayerWinsText;

	public GameObject GamesPlayedValue;

	public Sprite defaultAvatar;

	public GameObject addFriendButton;

	public GameObject editProfileButton;

	public GameObject EditButton;

	private void Start()
	{
		if (!StaticStrings.isFourPlayerModeEnabled)
		{
			FourPlayerWinsValue.SetActive(value: false);
			FourPlayerWinsText.SetActive(value: false);
		}
		defaultAvatar = avatar.GetComponent<Image>().sprite;
	}

	public void ShowPlayerInfo(int index)
	{
		window.SetActive(value: true);
		if (index == 0)
		{
			FillData(GameManager.Instance.avatarMy, GameManager.Instance.nameMy, GameManager.Instance.myPlayerData);
			addFriendButton.SetActive(value: false);
			editProfileButton.SetActive(value: true);
		}
		else
		{
			addFriendButton.SetActive(value: true);
			editProfileButton.SetActive(value: false);
			DConsole.Log("Player info " + index);
			FillData(GameManager.Instance.playerObjects[index].avatar, GameManager.Instance.playerObjects[index].name, GameManager.Instance.playerObjects[index].data);
		}
	}

	public void ShowPlayerInfo(Sprite avatarSprite, string name, MyPlayerData data)
	{
		editProfileButton.SetActive(value: false);
		addFriendButton.SetActive(value: true);
		window.SetActive(value: true);
		FillData(avatarSprite, name, data);
	}

	public void FillData(Sprite avatarSprite, string name, MyPlayerData data)
	{
		if (avatarSprite == null)
		{
			avatar.GetComponent<Image>().sprite = defaultAvatar;
		}
		else
		{
			avatar.GetComponent<Image>().sprite = avatarSprite;
		}
		playername.GetComponent<Text>().text = name;
		TotalEarningsValue.GetComponent<Text>().text = data.GetTotalEarnings().ToString();
		GamesPlayedValue.GetComponent<Text>().text = data.GetPlayedGamesCount().ToString();
		CurrentMoneyValue.GetComponent<Text>().text = data.GetCoins().ToString();
		GamesWonValue.GetComponent<Text>().text = (data.GetTwoPlayerWins() + data.GetFourPlayerWins()).ToString();
		float num = data.GetTwoPlayerWins() + data.GetFourPlayerWins();
		DConsole.Log("WON: " + num);
		DConsole.Log("played: " + data.GetPlayedGamesCount());
		if (data.GetPlayedGamesCount() != 0 && num != 0f)
		{
			WinRateValue.GetComponent<Text>().text = Mathf.RoundToInt(num / (float)data.GetPlayedGamesCount() * 100f) + "%";
		}
		else
		{
			WinRateValue.GetComponent<Text>().text = "0%";
		}
		TwoPlayerWinsValue.GetComponent<Text>().text = data.GetTwoPlayerWins().ToString();
		FourPlayerWinsValue.GetComponent<Text>().text = data.GetFourPlayerWins().ToString();
	}
}
