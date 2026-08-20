using System.Collections;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour
{
	private Image imageClock1;

	private Image imageClock2;

	private Animator messageBubble;

	private Text messageBubbleText;

	private int currentImage = 1;

	public float playerTime;

	public float hideBubbleAfter = 3f;

	private float messageTime;

	private AudioSource[] audioSources;

	private bool timeSoundsStarted;

	private float waitingOpponentTime;

	private void Start()
	{
		GameManager.Instance.gameControllerScript = this;
		audioSources = GetComponents<AudioSource>();
		playerTime = GameManager.Instance.playerTime;
		imageClock1 = GameObject.Find("AvatarClock1").GetComponent<Image>();
		imageClock2 = GameObject.Find("AvatarClock2").GetComponent<Image>();
		messageBubble = GameObject.Find("MessageBubble").GetComponent<Animator>();
		messageBubbleText = GameObject.Find("BubbleText").GetComponent<Text>();
		if (GameManager.Instance.offlineMode)
		{
			GameObject.Find("Name1").GetComponent<Text>().text = StaticStrings.offlineModePlayer1Name;
			GameObject.Find("Name2").GetComponent<Text>().text = StaticStrings.offlineModePlayer2Name;
			GameObject.Find("Avatar2").GetComponent<Image>().color = Color.red;
		}
		else
		{
			GameObject.Find("Name1").GetComponent<Text>().text = GameManager.Instance.nameMy;
			if (GameManager.Instance.avatarMy != null)
			{
				GameObject.Find("Avatar1").GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
			}
			GameObject.Find("Name2").GetComponent<Text>().text = GameManager.Instance.nameOpponent;
			if (GameManager.Instance.avatarOpponent != null)
			{
				GameObject.Find("Avatar2").GetComponent<Image>().sprite = GameManager.Instance.avatarOpponent;
			}
		}
		playerTime *= Time.timeScale;
		if (GameManager.Instance.roomOwner)
		{
			showMessage(StaticStrings.youAreBreaking);
		}
		else
		{
			showMessage(GameManager.Instance.nameOpponent + " " + StaticStrings.opponentIsBreaking);
		}
		if (!GameManager.Instance.roomOwner)
		{
			currentImage = 2;
		}
	}

	private void Update()
	{
		if (!GameManager.Instance.stopTimer)
		{
			updateClock();
		}
	}

	private void updateClock()
	{
		float num;
		if (currentImage == 1)
		{
			playerTime = GameManager.Instance.playerTime;
			if (GameManager.Instance.offlineMode)
			{
				playerTime = GameManager.Instance.playerTime + (float)GameManager.Instance.cueTime;
			}
			num = 1f / playerTime * Time.deltaTime;
			imageClock1.fillAmount -= num;
			if (imageClock1.fillAmount < 0.25f && !timeSoundsStarted)
			{
				audioSources[0].Play();
				timeSoundsStarted = true;
			}
			if (imageClock1.fillAmount == 0f)
			{
				audioSources[0].Stop();
				GameManager.Instance.stopTimer = true;
				if (GameManager.Instance.offlineMode)
				{
					GameManager.Instance.wasFault = true;
					GameManager.Instance.cueController.setTurnOffline(showTurnMessage: true);
				}
				showMessage("You " + StaticStrings.runOutOfTime);
				if (!GameManager.Instance.offlineMode)
				{
					GameManager.Instance.cueController.setOpponentTurn();
				}
			}
			return;
		}
		playerTime = GameManager.Instance.playerTime;
		if (GameManager.Instance.offlineMode)
		{
			playerTime = GameManager.Instance.playerTime + (float)GameManager.Instance.opponentCueTime;
		}
		num = 1f / playerTime * Time.deltaTime;
		imageClock2.fillAmount -= num;
		if (GameManager.Instance.offlineMode && imageClock2.fillAmount < 0.25f && !timeSoundsStarted)
		{
			audioSources[0].Play();
			timeSoundsStarted = true;
		}
		if (imageClock2.fillAmount == 0f)
		{
			GameManager.Instance.stopTimer = true;
			if (GameManager.Instance.offlineMode)
			{
				showMessage("You " + StaticStrings.runOutOfTime);
			}
			else
			{
				showMessage(GameManager.Instance.nameOpponent + " " + StaticStrings.runOutOfTime);
			}
			if (GameManager.Instance.offlineMode)
			{
				GameManager.Instance.wasFault = true;
				GameManager.Instance.cueController.setTurnOffline(showTurnMessage: true);
			}
		}
	}

	public void showMessage(string message)
	{
		float num = Time.time - messageTime;
		DConsole.Log("Time diff: " + num);
		if (num > hideBubbleAfter + 1f)
		{
			messageBubbleText.text = message;
			messageBubble.Play("ShowBubble");
			if (!message.Contains(StaticStrings.waitingForOpponent))
			{
				Invoke("hideBubble", hideBubbleAfter);
			}
			else
			{
				waitingOpponentTime = StaticStrings.photonDisconnectTimeout;
				StartCoroutine(updateMessageBubbleText());
			}
			messageTime = Time.time;
		}
		else
		{
			DConsole.Log("Show message with delay");
			StartCoroutine(showMessageWithDelay(message, (hideBubbleAfter + 1f - num) / 1f));
		}
	}

	public void hideBubble()
	{
		messageBubble.Play("HideBubble");
	}

	private IEnumerator showMessageWithDelay(string message, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		messageBubbleText.text = message;
		messageBubble.Play("ShowBubble");
		if (!message.Contains(StaticStrings.waitingForOpponent))
		{
			Invoke("hideBubble", hideBubbleAfter);
		}
		else
		{
			waitingOpponentTime = StaticStrings.photonDisconnectTimeout;
			StartCoroutine(updateMessageBubbleText());
		}
		messageTime = Time.time;
	}

	public IEnumerator updateMessageBubbleText()
	{
		yield return new WaitForSeconds(2f);
		waitingOpponentTime -= 1f;
		if (!GameManager.Instance.opponentDisconnected && !messageBubbleText.text.Contains("disconnected from room"))
		{
			messageBubbleText.text = StaticStrings.waitingForOpponent + " " + waitingOpponentTime;
		}
		if (waitingOpponentTime > 0f && !GameManager.Instance.opponentActive && !GameManager.Instance.opponentDisconnected)
		{
			StartCoroutine(updateMessageBubbleText());
		}
	}

	public void stopSound()
	{
		audioSources[0].Stop();
	}

	public void resetTimers(int currentTimer, bool showMessageBool)
	{
		stopSound();
		timeSoundsStarted = false;
		imageClock1.fillAmount = 1f;
		imageClock2.fillAmount = 1f;
		currentImage = currentTimer;
		if (GameManager.Instance.offlineMode)
		{
			if (showMessageBool)
			{
				if (currentTimer == 2)
				{
					showMessage(StaticStrings.offlineModePlayer2Name + " turn");
				}
				else
				{
					showMessage(StaticStrings.offlineModePlayer1Name + " turn");
				}
			}
		}
		else if (currentTimer == 1 && showMessageBool)
		{
			showMessage("It's your turn");
		}
		GameManager.Instance.stopTimer = false;
	}
}
