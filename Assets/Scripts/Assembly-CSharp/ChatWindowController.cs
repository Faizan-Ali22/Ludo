using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindowController : MonoBehaviour
{
	public GameObject gridView;

	public GameObject horizontalEmojiView;

	public GameObject ChatMessageButtonPrefab;

	public GameObject ChatEmojiButtonPrefab;

	public GameObject ChatButton;

	public GameObject chatWindow;

	public GameObject myChatBubble;

	public GameObject myChatBubbleText;

	public GameObject myChatBubbleImage;

	[HideInInspector]
	public Sprite[] emojiSprites;

	private int emojiPerPack;

	private int packsCount = 6;

	private void Start()
	{
		emojiSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emoji;
		emojiPerPack = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emojiPerPack;
		packsCount = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().packsCount;
		for (int i = 0; i < StaticStrings.chatMessages.Length; i++)
		{
			GameObject obj = Object.Instantiate(ChatMessageButtonPrefab);
			obj.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.chatMessages[i];
			obj.transform.SetParent(gridView.transform, worldPositionStays: false);
			obj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			string index = StaticStrings.chatMessages[i];
			obj.GetComponent<Button>().onClick.RemoveAllListeners();
			obj.GetComponent<Button>().onClick.AddListener(delegate
			{
				SendMessageEvent(index);
			});
		}
		for (int num = 0; num < packsCount; num++)
		{
			if (num != 0 && (GameManager.Instance.myPlayerData.GetEmoji() == null || !GameManager.Instance.myPlayerData.GetEmoji().Contains("'" + (num - 1) + "'")))
			{
				continue;
			}
			for (int num2 = 0; num2 < emojiPerPack; num2++)
			{
				GameObject obj2 = Object.Instantiate(ChatEmojiButtonPrefab);
				obj2.transform.GetComponent<Image>().sprite = emojiSprites[num * emojiPerPack + num2];
				obj2.transform.parent = horizontalEmojiView.transform;
				obj2.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
				int index2 = num * emojiPerPack + num2;
				obj2.GetComponent<Button>().onClick.RemoveAllListeners();
				obj2.GetComponent<Button>().onClick.AddListener(delegate
				{
					SendMessageEventEmoji(index2);
				});
			}
		}
		for (int num3 = 0; num3 < StaticStrings.chatMessagesExtended.Length; num3++)
		{
			if (GameManager.Instance.myPlayerData.GetChats() == null || !GameManager.Instance.myPlayerData.GetChats().Contains("'" + num3 + "'"))
			{
				continue;
			}
			for (int num4 = 0; num4 < StaticStrings.chatMessagesExtended[num3].Length; num4++)
			{
				GameObject obj3 = Object.Instantiate(ChatMessageButtonPrefab);
				obj3.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.chatMessagesExtended[num3][num4];
				obj3.transform.parent = gridView.transform;
				obj3.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
				string index3 = StaticStrings.chatMessagesExtended[num3][num4];
				obj3.GetComponent<Button>().onClick.RemoveAllListeners();
				obj3.GetComponent<Button>().onClick.AddListener(delegate
				{
					SendMessageEvent(index3);
				});
			}
		}
	}

	public void SendMessageEvent(string index)
	{
		DConsole.Log("Button Clicked " + index);
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(175, index + ";" + PhotonNetwork.playerName, sendReliable: true, null);
		}
		chatWindow.SetActive(value: false);
		ChatButton.GetComponent<Text>().text = "CHAT";
		myChatBubbleImage.SetActive(value: false);
		myChatBubbleText.SetActive(value: true);
		myChatBubbleText.GetComponent<Text>().text = index;
		myChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
	}

	public void SendMessageEventEmoji(int index)
	{
		DConsole.Log("Button Clicked " + index);
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(176, index + ";" + PhotonNetwork.playerName, sendReliable: true, null);
		}
		chatWindow.SetActive(value: false);
		ChatButton.GetComponent<Text>().text = "CHAT";
		myChatBubbleImage.SetActive(value: true);
		myChatBubbleText.SetActive(value: false);
		myChatBubbleImage.GetComponent<Image>().sprite = emojiSprites[index];
		myChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
	}

	private void Update()
	{
	}
}
