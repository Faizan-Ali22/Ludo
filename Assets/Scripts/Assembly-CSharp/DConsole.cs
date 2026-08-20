using System;
using UnityEngine;
using UnityEngine.UI;

public class DConsole : MonoBehaviour
{
	[SerializeField]
	private GameObject _panel;

	[SerializeField]
	private Text _text;

	[SerializeField]
	private Button _buttonClearConsole;

	[SerializeField]
	private Button _buttonClearPrefs;

	private bool _isShow;

	private static bool isExceptionHandlingSetup;

	public static DConsole Instance;

	public Text console
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
		}
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		_panel.SetActive(value: false);
		_text.text = "";
		_buttonClearConsole.onClick.RemoveAllListeners();
		_buttonClearConsole.onClick.AddListener(delegate
		{
			ClearConsole();
		});
		_buttonClearPrefs.onClick.RemoveAllListeners();
		_buttonClearPrefs.onClick.AddListener(delegate
		{
			PlayerPrefs.DeleteAll();
		});
		SetupExceptionHandling();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!_isShow)
			{
				_panel.SetActive(value: true);
				_isShow = true;
			}
			else
			{
				_panel.SetActive(value: false);
				_isShow = false;
			}
		}
	}

	private void ClearConsole()
	{
		_text.text = "";
	}

	public static void SetupExceptionHandling()
	{
		if (!isExceptionHandlingSetup)
		{
			isExceptionHandlingSetup = true;
			Application.logMessageReceived += HandleException;
		}
	}

	private static void HandleException(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			LogError(condition + "\n" + stackTrace);
		}
	}

	public static void Log(string msg)
	{
		Text text = Instance.console;
		text.text = text.text + msg + "\n";
		Debug.Log(msg);
	}

	public static void Log(string msg, UnityEngine.Object context)
	{
		Text text = Instance.console;
		text.text = text.text + msg + "\n";
		Debug.Log(msg, context);
	}

	public static void LogFormat(string msg, params object[] args)
	{
		Text text = Instance.console;
		text.text = text.text + string.Format(msg, args) + "\n";
		Debug.LogFormat(msg, args);
	}

	public static void LogError(string msg)
	{
		Text text = Instance.console;
		text.text = text.text + "<color=#ff0000>" + msg + "</color>\n";
		Debug.LogError(msg);
	}

	public static void LogError(string msg, UnityEngine.Object context)
	{
		Text text = Instance.console;
		text.text = text.text + "<color=#ff0000>" + msg + "</color>\n";
		Debug.LogError(msg, context);
	}

	public static void LogException(Exception e)
	{
		Text text = Instance.console;
		text.text = text.text + "<color=#ff0000>" + e.Message + "</color>\n";
		Debug.LogException(e);
	}

	public static void LogWarning(string msg)
	{
		Text text = Instance.console;
		text.text = text.text + "<color=orange>" + msg + "</color>\n";
		Debug.LogWarning(msg);
	}

	public static void LogWarning(string msg, UnityEngine.Object context)
	{
		Text text = Instance.console;
		text.text = text.text + "<color=orange>" + msg + "</color>\n";
		Debug.LogWarning(msg, context);
	}
}
