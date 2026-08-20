using System.Globalization;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class ChatShopController : MonoBehaviour
{
	public GameObject priceText;

	public GameObject chatName;

	public GameObject button;

	public GameObject buttonText;

	private int price;

	private int index;

	public GameObject[] bubbles;

	private void Start()
	{
	}

	public void fillData(int i)
	{
		index = i;
		string[] array = StaticStrings.chatMessagesExtended[i];
		int num = StaticStrings.chatPrices[i];
		string text = StaticStrings.chatNames[i];
		price = num;
		priceText.GetComponent<Text>().text = num.ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
		chatName.GetComponent<Text>().text = text;
		for (int j = 0; j < array.Length; j++)
		{
			bubbles[j].transform.GetChild(0).GetComponent<Text>().text = array[j];
			bubbles[j].SetActive(value: true);
		}
		for (int num2 = 5; num2 >= array.Length; num2--)
		{
			bubbles[num2].SetActive(value: false);
		}
		if (GameManager.Instance.myPlayerData.GetChats() != null && GameManager.Instance.myPlayerData.GetChats().Length > 0 && GameManager.Instance.myPlayerData.GetChats().Contains("'" + i + "'"))
		{
			button.GetComponent<Button>().interactable = false;
			buttonText.GetComponent<Text>().text = "Owned";
		}
	}

	private void Update()
	{
	}

	public void buyChat()
	{
		if (GameManager.Instance.myPlayerData.GetCoins() >= price)
		{
			GameManager.Instance.playfabManager.addCoinsRequest(-price);
			GameManager.Instance.playfabManager.updateBoughtChats(index);
			button.GetComponent<Button>().interactable = false;
			buttonText.GetComponent<Text>().text = "Owned";
		}
		else
		{
			GameManager.Instance.dialog.SetActive(value: true);
		}
	}

	public void buyEmoji()
	{
		if (GameManager.Instance.myPlayerData.GetCoins() >= price)
		{
			GameManager.Instance.playfabManager.addCoinsRequest(-price);
			GameManager.Instance.playfabManager.UpdateBoughtEmojis(index);
			button.GetComponent<Button>().interactable = false;
			buttonText.GetComponent<Text>().text = "Owned";
		}
		else
		{
			GameManager.Instance.dialog.SetActive(value: true);
		}
	}
}
