using UnityEngine;
using UnityEngine.UI;

public class CueController : MonoBehaviour
{
	[HideInInspector]
	public bool isServer;

	public GameObject youWonMessage;

	private bool canShowControllers = true;

	public GameObject prizeText;

	public GameObject audioController;

	public GameObject invitiationDialog;

	public GameObject chatButton;

	public GameControllerScript gameControllerScript;

	private void Start()
	{
		gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
		if (GameManager.Instance.offlineMode)
		{
			chatButton.SetActive(value: false);
		}
		if (!GameManager.Instance.offlineMode)
		{
			GameManager.Instance.playfabManager.addCoinsRequest(-GameManager.Instance.payoutCoins);
		}
		GameManager.Instance.audioSources = audioController.GetComponents<AudioSource>();
		GameManager.Instance.iWon = false;
		GameManager.Instance.iLost = false;
		GameManager.Instance.iDraw = false;
		setPrizeText();
		GameManager.Instance.cueController = this;
		isServer = false;
		if (GameManager.Instance.roomOwner)
		{
			isServer = true;
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			PhotonNetwork.RaiseEvent(151, 1, sendReliable: true, null);
			PhotonNetwork.SendOutgoingCommands();
			DConsole.Log("Application pause");
		}
		else
		{
			PhotonNetwork.RaiseEvent(152, 1, sendReliable: true, null);
			PhotonNetwork.SendOutgoingCommands();
			DConsole.Log("Application resume");
		}
	}

	private void setPrizeText()
	{
		int num = GameManager.Instance.payoutCoins * 2;
		if (num >= 1000)
		{
			if (num >= 1000000)
			{
				if ((float)num % 1000000f == 0f)
				{
					prizeText.GetComponent<Text>().text = ((float)num / 1000000f).ToString("0") + "M";
				}
				else
				{
					prizeText.GetComponent<Text>().text = ((float)num / 1000000f).ToString("0.0") + "M";
				}
			}
			else if ((float)num % 1000f == 0f)
			{
				prizeText.GetComponent<Text>().text = ((float)num / 1000f).ToString("0") + "k";
			}
			else
			{
				prizeText.GetComponent<Text>().text = ((float)num / 1000f).ToString("0.0") + "k";
			}
		}
		else
		{
			prizeText.GetComponent<Text>().text = string.Concat(num);
		}
		if (GameManager.Instance.offlineMode)
		{
			prizeText.GetComponent<Text>().text = "Practice";
		}
	}

	private void Awake()
	{
		PhotonNetwork.OnEventCall += OnEvent;
	}

	public void removeOnEventCall()
	{
		PhotonNetwork.OnEventCall -= OnEvent;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
	}

	private void OnDestroy()
	{
		PhotonNetwork.OnEventCall -= OnEvent;
	}

	private void OnEvent(byte eventcode, object content, int senderid)
	{
	}

	public void setOpponentTurn()
	{
		isServer = false;
		gameControllerScript.resetTimers(2, showMessageBool: true);
		GameManager.Instance.miniGame.setOpponentTurn();
	}

	public void setMyTurn()
	{
		GameManager.Instance.myTurnDone = false;
		isServer = true;
		gameControllerScript.resetTimers(1, showMessageBool: true);
		GameManager.Instance.miniGame.setMyTurn();
	}

	public void checkShot()
	{
		if (GameManager.Instance.iWon)
		{
			IWon();
		}
		else if (GameManager.Instance.iLost)
		{
			ILost();
		}
	}

	public void IWon()
	{
		GameManager.Instance.iWon = true;
		HideAllControllers();
		GameManager.Instance.audioSources[3].Play();
		youWonMessage.SetActive(value: true);
		youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(19, null, sendReliable: true, null);
		}
	}

	public void Draw()
	{
		GameManager.Instance.iDraw = true;
		HideAllControllers();
		GameManager.Instance.audioSources[3].Play();
		youWonMessage.SetActive(value: true);
		youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(21, null, sendReliable: true, null);
		}
	}

	public void ILost()
	{
		GameManager.Instance.iWon = false;
		HideAllControllers();
		GameManager.Instance.audioSources[3].Play();
		youWonMessage.SetActive(value: true);
		youWonMessage.GetComponent<YouWinMessageChangeSprite>().changeSprite();
		youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
		if (!GameManager.Instance.offlineMode)
		{
			PhotonNetwork.RaiseEvent(20, null, sendReliable: true, null);
		}
	}

	public void setTurnOffline(bool showTurnMessage)
	{
	}

	private void ShowAllControllers()
	{
		if (canShowControllers)
		{
			DConsole.Log("Showing controllers");
		}
	}

	public void HideAllControllers()
	{
	}

	public void stopTimer()
	{
		GameManager.Instance.stopTimer = true;
	}
}
