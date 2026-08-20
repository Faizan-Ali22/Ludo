using UnityEngine;
using UnityEngine.UI;

public class EnterPrivateCodeDialogController : MonoBehaviour
{
	public GameObject inputField;

	public GameObject confirmationText;

	public GameObject joinButton;

	private Button join;

	private InputField field;

	public GameObject GameConfiguration;

	public GameObject failedDialog;

	private void OnEnable()
	{
		if (field != null)
		{
			field.text = "";
		}
		if (confirmationText != null)
		{
			confirmationText.SetActive(value: false);
		}
		if (join != null)
		{
			join.interactable = false;
		}
	}

	private void Start()
	{
		field = inputField.GetComponent<InputField>();
		join = joinButton.GetComponent<Button>();
		join.interactable = false;
	}

	private void Update()
	{
	}

	public void onValueChanged()
	{
		if (field.text.Length < 8)
		{
			confirmationText.SetActive(value: true);
			join.interactable = false;
		}
		else
		{
			confirmationText.SetActive(value: false);
			join.interactable = true;
		}
	}

	public void JoinByRoomID()
	{
		GameManager.Instance.JoinedByID = true;
		GameManager.Instance.payoutCoins = 0;
		string text = field.text;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		DConsole.Log("Rooms count: " + roomList.Length);
		if (roomList.Length == 0)
		{
			DConsole.Log("no rooms!");
			failedDialog.SetActive(value: true);
			return;
		}
		bool flag = false;
		for (int i = 0; i < roomList.Length; i++)
		{
			if (!roomList[i].Name.Equals(text))
			{
				continue;
			}
			flag = true;
			if (roomList[i].CustomProperties.ContainsKey("pc"))
			{
				GameManager.Instance.payoutCoins = int.Parse(roomList[i].CustomProperties["pc"].ToString());
				if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.payoutCoins)
				{
					PhotonNetwork.JoinRoom(text);
				}
				GameConfiguration.GetComponent<GameConfigrationController>().startGame();
			}
			else
			{
				GameManager.Instance.payoutCoins = int.MaxValue;
				GameConfiguration.GetComponent<GameConfigrationController>().startGame();
			}
		}
		if (!flag)
		{
			failedDialog.SetActive(value: true);
		}
	}
}
