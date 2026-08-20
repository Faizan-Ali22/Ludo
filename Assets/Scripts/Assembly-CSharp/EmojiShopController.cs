using System.Globalization;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class EmojiShopController : MonoBehaviour
{
	public GameObject priceText;

	public GameObject chatName;

	public GameObject button;

	public GameObject buttonText;

	private int price;

	private int index;

	public GameObject[] bubbles;

	public GameObject parent;

	public GameObject emojiPrefab;

	private Sprite[] emojiSprites;

	private int emojiPerPack;

	private void Start()
	{
	}

	public void fillData(int i)
	{
		emojiSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emoji;
		emojiPerPack = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emojiPerPack;
		index = i;
		int num = (price = StaticStrings.emojisPrices[i]);
		priceText.GetComponent<Text>().text = num.ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
		for (int j = 0; j < emojiPerPack; j++)
		{
			GameObject obj = Object.Instantiate(emojiPrefab);
			obj.transform.SetParent(parent.transform, worldPositionStays: false);
			obj.GetComponent<Image>().sprite = emojiSprites[(i + 1) * emojiPerPack + j];
		}
		if (GameManager.Instance.myPlayerData.GetEmoji() != null && GameManager.Instance.myPlayerData.GetEmoji().Length > 0 && GameManager.Instance.myPlayerData.GetEmoji().Contains("'" + i + "'"))
		{
			button.GetComponent<Button>().interactable = false;
			buttonText.GetComponent<Text>().text = "Owned";
		}
	}

	private void Update()
	{
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
