using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectedTabluButtonClicked : MonoBehaviour
{
	public int tableNumber;

	public int fee;

	private void Start()
	{
		DConsole.Log("start");
		base.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
		base.gameObject.GetComponent<Button>().onClick.AddListener(startGame);
	}

	private void Update()
	{
	}

	public void startGame()
	{
		GameManager.Instance.GameScene = "GameScene";
		GameManager.Instance.requiredPlayers = tableNumber;
		DConsole.Log("Fee: " + fee + "  Coins: " + GameManager.Instance.myPlayerData.GetCoins());
		if (GameManager.Instance.myPlayerData.GetCoins() >= fee)
		{
			if (GameManager.Instance.inviteFriendActivated)
			{
				GameManager.Instance.tableNumber = tableNumber;
				GameManager.Instance.payoutCoins = fee;
				GameManager.Instance.initMenuScript.backToMenuFromTableSelect();
				GameManager.Instance.playfabManager.challengeFriend(GameManager.Instance.challengedFriendID, fee + ";" + tableNumber);
			}
			else if (GameManager.Instance.offlineMode)
			{
				GameManager.Instance.payoutCoins = fee;
				if (!GameManager.Instance.gameSceneStarted)
				{
					SceneManager.LoadScene(GameManager.Instance.GameScene);
					GameManager.Instance.gameSceneStarted = true;
				}
			}
			else
			{
				GameManager.Instance.tableNumber = tableNumber;
				GameManager.Instance.payoutCoins = fee;
				GameManager.Instance.facebookManager.startRandomGame();
			}
		}
		else
		{
			GameManager.Instance.dialog.SetActive(value: true);
		}
	}
}
