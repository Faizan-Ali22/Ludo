using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayerTimer : MonoBehaviour
{
	private float playerTime;

	public GameObject timerObject;

	private Image timer;

	private bool timeSoundsStarted;

	public AudioSource[] audioSources;

	public GameObject GUIController;

	public bool myTimer;

	public bool paused;

	private void Start()
	{
		timer = base.gameObject.GetComponent<Image>();
	}

	private void OnEnable()
	{
		timer = base.gameObject.GetComponent<Image>();
	}

	public void Pause()
	{
		paused = true;
		audioSources[0].Stop();
	}

	private void Update()
	{
		if (!paused)
		{
			updateClock();
		}
	}

	public void restartTimer()
	{
		paused = false;
		timer.fillAmount = 1f;
	}

	private void OnDisable()
	{
		if (timer != null)
		{
			timer.fillAmount = 1f;
			paused = false;
			audioSources[0].Stop();
		}
	}

	private void updateClock()
	{
		playerTime = GameManager.Instance.playerTime;
		if (GameManager.Instance.offlineMode)
		{
			playerTime = GameManager.Instance.playerTime + (float)GameManager.Instance.cueTime;
		}
		float num = 1f / playerTime * Time.deltaTime;
		timer.fillAmount -= num;
		if (timer.fillAmount < 0.25f && !timeSoundsStarted)
		{
			audioSources[0].Play();
			timeSoundsStarted = true;
		}
		if (timer.fillAmount != 0f)
		{
			return;
		}
		DConsole.Log("TIME 0");
		audioSources[0].Stop();
		GameManager.Instance.stopTimer = true;
		if (!GameManager.Instance.offlineMode)
		{
			if (myTimer)
			{
				DConsole.Log("Timer call finish turn");
				GUIController.GetComponent<GameGUIController>().SendFinishTurn();
			}
		}
		else
		{
			GameManager.Instance.wasFault = true;
			GameManager.Instance.cueController.setTurnOffline(showTurnMessage: true);
		}
	}
}
