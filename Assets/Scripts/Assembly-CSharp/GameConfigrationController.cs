using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class GameConfigrationController : MonoBehaviour
{
	public GameObject TitleText;

	public GameObject bidText;

	public GameObject MinusButton;

	public GameObject PlusButton;

	public GameObject[] Toggles;

	private int currentBidIndex;

	private MyGameMode[] modes = new MyGameMode[3]
	{
		MyGameMode.Classic,
		MyGameMode.Quick,
		MyGameMode.Master
	};

	public GameObject privateRoomJoin;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		for (int i = 0; i < Toggles.Length; i++)
		{
			int index = i;
			Toggles[i].GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool value)
			{
				ChangeGameMode(value, modes[index]);
			});
		}
		currentBidIndex = 0;
		UpdateBid(changeBidInGM: true);
		Toggles[0].GetComponent<Toggle>().isOn = true;
		GameManager.Instance.mode = MyGameMode.Classic;
		switch (GameManager.Instance.type)
		{
		case MyGameType.TwoPlayer:
			TitleText.GetComponent<Text>().text = "Two Players";
			break;
		case MyGameType.FourPlayer:
			TitleText.GetComponent<Text>().text = "Four Players";
			break;
		case MyGameType.Private:
			TitleText.GetComponent<Text>().text = "Private Room";
			privateRoomJoin.SetActive(value: true);
			break;
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < Toggles.Length; i++)
		{
			Toggles[i].GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		}
		privateRoomJoin.SetActive(value: false);
		currentBidIndex = 0;
		UpdateBid(changeBidInGM: false);
		Toggles[0].GetComponent<Toggle>().isOn = true;
		Toggles[1].GetComponent<Toggle>().isOn = false;
		Toggles[2].GetComponent<Toggle>().isOn = false;
	}

	public void setCreatedProvateRoom()
	{
		GameManager.Instance.JoinedByID = false;
	}

	public void startGame()
	{
		if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.payoutCoins)
		{
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

	private void ChangeGameMode(bool isActive, MyGameMode mode)
	{
		if (isActive)
		{
			GameManager.Instance.mode = mode;
		}
	}

	public void IncreaseBid()
	{
		if (currentBidIndex < StaticStrings.bidValues.Length - 1)
		{
			currentBidIndex++;
			UpdateBid(changeBidInGM: true);
		}
	}

	public void DecreaseBid()
	{
		if (currentBidIndex > 0)
		{
			currentBidIndex--;
			UpdateBid(changeBidInGM: true);
		}
	}

	private void UpdateBid(bool changeBidInGM)
	{
		bidText.GetComponent<Text>().text = StaticStrings.bidValuesStrings[currentBidIndex];
		if (changeBidInGM)
		{
			GameManager.Instance.payoutCoins = StaticStrings.bidValues[currentBidIndex];
		}
		if (currentBidIndex == 0)
		{
			MinusButton.GetComponent<Button>().interactable = false;
		}
		else
		{
			MinusButton.GetComponent<Button>().interactable = true;
		}
		if (currentBidIndex == StaticStrings.bidValues.Length - 1)
		{
			PlusButton.GetComponent<Button>().interactable = false;
		}
		else
		{
			PlusButton.GetComponent<Button>().interactable = true;
		}
	}

	public void HideThisScreen()
	{
		base.gameObject.SetActive(value: false);
	}
}
