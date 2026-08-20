using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class FortuneWheelManager : MonoBehaviour
{
	public GameObject FreeTurnIndicator;

	[HideInInspector]
	public Text timeToFreeText;

	public GameObject TimeToFreeTurnIndicator;

	[Header("Game Objects for some elements")]
	public Button PaidTurnButton;

	public Button FreeTurnButton;

	public GameObject Circle;

	public Text DeltaCoinsText;

	public Text CurrentCoinsText;

	public GameObject NextTurnTimerWrapper;

	public Text NextFreeTurnTimerText;

	[Header("How much currency one paid turn costs")]
	public int TurnCost = 300;

	private bool _isStarted;

	[Header("Params for each sector")]
	public FortuneWheelSector[] Sectors;

	private float _finalAngle;

	private float _startAngle;

	private float _currentLerpRotationTime;

	private int _currentCoinsAmount = 1000;

	private int _previousCoinsAmount;

	[Header("Time Between Two Free Turns")]
	public int TimerMaxHours;

	[Range(0f, 59f)]
	public int TimerMaxMinutes;

	[Range(0f, 59f)]
	public int TimerMaxSeconds = 10;

	private int _timerRemainingHours;

	private int _timerRemainingMinutes;

	private int _timerRemainingSeconds;

	private DateTime _nextFreeTurnTime;

	private const string LAST_FREE_TURN_TIME_NAME = "LastFreeTurnTimeTicks";

	[Header("Can players turn the wheel for currency?")]
	public bool IsPaidTurnEnabled = true;

	[Header("Can players turn the wheel for FREE from time to time?")]
	public bool IsFreeTurnEnabled = true;

	private bool _isFreeTurnAvailable;

	private FortuneWheelSector _finalSector;

	private void Start()
	{
		timeToFreeText = TimeToFreeTurnIndicator.GetComponent<Text>();
	}

	private void Awake()
	{
		DConsole.Log("Fortune wheel awake");
		PlayerPrefs.SetString("LastFreeTurnTimeTicks", GameManager.Instance.myPlayerData.GetLastFortuneTime());
		_previousCoinsAmount = _currentCoinsAmount;
		CurrentCoinsText.text = _currentCoinsAmount.ToString();
		FortuneWheelSector[] sectors = Sectors;
		foreach (FortuneWheelSector fortuneWheelSector in sectors)
		{
			if (fortuneWheelSector.ValueTextObject != null)
			{
				fortuneWheelSector.ValueTextObject.GetComponent<Text>().text = fortuneWheelSector.RewardValue.ToString();
			}
		}
		if (IsFreeTurnEnabled)
		{
			SetNextFreeTime();
			if (!PlayerPrefs.HasKey("LastFreeTurnTimeTicks"))
			{
				PlayerPrefs.SetString("LastFreeTurnTimeTicks", DateTime.Now.Ticks.ToString());
			}
		}
		else
		{
			NextTurnTimerWrapper.gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		DeltaCoinsText.gameObject.SetActive(value: false);
	}

	private void TurnWheelForFree()
	{
		TurnWheel(isFree: true);
	}

	private void TurnWheelForCoins()
	{
		TurnWheel(isFree: false);
	}

	private void TurnWheel(bool isFree)
	{
		DConsole.Log("turn wheel");
		_currentLerpRotationTime = 0f;
		int[] array = new int[Sectors.Length];
		for (int i = 1; i <= Sectors.Length; i++)
		{
			array[i - 1] = 360 / Sectors.Length * i;
		}
		double num = UnityEngine.Random.Range(1, Sectors.Sum((FortuneWheelSector sector) => sector.Probability));
		int num2 = 0;
		int num3 = array[0];
		_finalSector = Sectors[0];
		for (int num4 = 0; num4 < Sectors.Length; num4++)
		{
			num2 += Sectors[num4].Probability;
			if (num <= (double)num2)
			{
				num3 = array[num4];
				_finalSector = Sectors[num4];
				break;
			}
		}
		int num5 = 5;
		_finalAngle = num5 * 360 + num3;
		_isStarted = true;
		_previousCoinsAmount = _currentCoinsAmount;
		if (!isFree)
		{
			_currentCoinsAmount -= TurnCost;
			DeltaCoinsText.text = $"-{TurnCost}";
			DeltaCoinsText.gameObject.SetActive(value: true);
			StartCoroutine(HideCoinsDelta());
			StartCoroutine(UpdateCoinsAmount());
		}
		else
		{
			PlayerPrefs.SetString("LastFreeTurnTimeTicks", DateTime.Now.Ticks.ToString());
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add(MyPlayerData.FortuneWheelLastFreeKey, DateTime.Now.Ticks.ToString());
			GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
			SetNextFreeTime();
		}
	}

	public void TurnWheelButtonClick()
	{
		if (_isFreeTurnAvailable)
		{
			TurnWheelForFree();
		}
		else if (IsPaidTurnEnabled && GameManager.Instance.myPlayerData.GetCoins() >= TurnCost)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() - TurnCost).ToString());
			GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
			TurnWheelForCoins();
		}
	}

	public void SetNextFreeTime()
	{
		DConsole.Log("Next free turn");
		_timerRemainingHours = TimerMaxHours;
		_timerRemainingMinutes = TimerMaxMinutes;
		_timerRemainingSeconds = TimerMaxSeconds;
		_nextFreeTurnTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString("LastFreeTurnTimeTicks", DateTime.Now.Ticks.ToString()))).AddHours(TimerMaxHours).AddMinutes(TimerMaxMinutes).AddSeconds(TimerMaxSeconds);
		_isFreeTurnAvailable = false;
		int num = TimerMaxHours * 3600000 + TimerMaxMinutes * 60000 + TimerMaxSeconds * 1000;
		LocalNotification.CancelNotification(1);
		if (PlayerPrefs.GetInt(StaticStrings.NotificationsKey, 0) == 0)
		{
			DConsole.Log("Start notification");
			LocalNotification.SendNotification(1, num, StaticStrings.notificationTitle, StaticStrings.notificationMessage, new Color32(byte.MaxValue, 68, 68, byte.MaxValue), sound: true, vibrate: true, lights: true, "app_icon");
		}
		else
		{
			DConsole.Log("Notification disabled");
		}
	}

	private void ShowTurnButtons()
	{
		if (_isFreeTurnAvailable)
		{
			ShowFreeTurnButton();
			EnableFreeTurnButton();
			return;
		}
		if (!IsPaidTurnEnabled)
		{
			ShowFreeTurnButton();
			DisableFreeTurnButton();
			return;
		}
		ShowPaidTurnButton();
		if (_isStarted || GameManager.Instance.myPlayerData.GetCoins() < TurnCost)
		{
			DisablePaidTurnButton();
		}
		else
		{
			EnablePaidTurnButton();
		}
	}

	private void Update()
	{
		ShowTurnButtons();
		if (IsFreeTurnEnabled)
		{
			UpdateFreeTurnTimer();
		}
		if (_isStarted)
		{
			float num = 4f;
			_currentLerpRotationTime += Time.deltaTime;
			if (_currentLerpRotationTime > num || Circle.transform.eulerAngles.z == _finalAngle)
			{
				_currentLerpRotationTime = num;
				_isStarted = false;
				_startAngle = _finalAngle % 360f;
				_finalSector.RewardCallback.Invoke();
				StartCoroutine(HideCoinsDelta());
			}
			else
			{
				float num2 = _currentLerpRotationTime / num;
				num2 = num2 * num2 * num2 * (num2 * (6f * num2 - 15f) + 10f);
				float z = Mathf.Lerp(_startAngle, _finalAngle, num2);
				Circle.transform.eulerAngles = new Vector3(0f, 0f, z);
			}
		}
	}

	public void RewardCoins(int awardCoins)
	{
		_currentCoinsAmount += awardCoins;
		DeltaCoinsText.text = $"+{awardCoins}";
		DeltaCoinsText.gameObject.SetActive(value: true);
		StartCoroutine(UpdateCoinsAmount());
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() + awardCoins).ToString());
		GameManager.Instance.myPlayerData.UpdateUserData(dictionary);
	}

	private IEnumerator HideCoinsDelta()
	{
		yield return new WaitForSeconds(1f);
		DeltaCoinsText.gameObject.SetActive(value: false);
	}

	private IEnumerator UpdateCoinsAmount()
	{
		float elapsedTime = 0f;
		while (elapsedTime < 0.5f)
		{
			CurrentCoinsText.text = Mathf.Floor(Mathf.Lerp(_previousCoinsAmount, _currentCoinsAmount, elapsedTime / 0.5f)).ToString();
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		_previousCoinsAmount = _currentCoinsAmount;
		CurrentCoinsText.text = _currentCoinsAmount.ToString();
	}

	private void UpdateFreeTurnTimer()
	{
		if (!_isFreeTurnAvailable)
		{
			_timerRemainingHours = (_nextFreeTurnTime - DateTime.Now).Hours;
			_timerRemainingMinutes = (_nextFreeTurnTime - DateTime.Now).Minutes;
			_timerRemainingSeconds = (_nextFreeTurnTime - DateTime.Now).Seconds;
			if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
			{
				FreeTurnIndicator.SetActive(value: true);
				TimeToFreeTurnIndicator.SetActive(value: false);
				NextFreeTurnTimerText.text = "Ready!";
				_isFreeTurnAvailable = true;
			}
			else
			{
				FreeTurnIndicator.SetActive(value: false);
				TimeToFreeTurnIndicator.SetActive(value: true);
				NextFreeTurnTimerText.text = $"{_timerRemainingHours:00}:{_timerRemainingMinutes:00}:{_timerRemainingSeconds:00}";
				timeToFreeText.text = $"{_timerRemainingHours:00}:{_timerRemainingMinutes:00}:{_timerRemainingSeconds:00}";
				_isFreeTurnAvailable = false;
			}
		}
	}

	private void EnableButton(Button button)
	{
		button.interactable = true;
		button.GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
	}

	private void DisableButton(Button button)
	{
		button.interactable = false;
		button.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.5f);
	}

	private void EnableFreeTurnButton()
	{
		EnableButton(FreeTurnButton);
	}

	private void DisableFreeTurnButton()
	{
		DisableButton(FreeTurnButton);
	}

	private void EnablePaidTurnButton()
	{
		EnableButton(PaidTurnButton);
	}

	private void DisablePaidTurnButton()
	{
		DisableButton(PaidTurnButton);
	}

	private void ShowFreeTurnButton()
	{
		FreeTurnButton.gameObject.SetActive(value: true);
	}

	private void ShowPaidTurnButton()
	{
		PaidTurnButton.gameObject.SetActive(value: true);
		FreeTurnButton.gameObject.SetActive(value: false);
	}

	public void ResetTimer()
	{
		PlayerPrefs.DeleteKey("LastFreeTurnTimeTicks");
	}
}
