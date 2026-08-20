using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class StartScriptController : MonoBehaviour
{
	public GameObject splashCanvas;

	public GameObject LoginCanvas;

	public GameObject menuCanvas;

	public GameObject[] go;

	public GameObject fbButton;

	public GameObject fbLoginCoinText;

	public GameObject guestLoginCoinText;

	private void Start()
	{
		fbLoginCoinText.GetComponent<Text>().text = StaticStrings.initCoinsCountFacebook.ToString();
		guestLoginCoinText.GetComponent<Text>().text = StaticStrings.initCoinsCountGuest.ToString();
		DConsole.Log("START SCRIPT");
		if (PlayerPrefs.HasKey("LoggedType"))
		{
			splashCanvas.SetActive(value: true);
		}
		else
		{
			LoginCanvas.SetActive(value: true);
		}
	}

	private void Update()
	{
	}

	public void HideAllElements()
	{
		menuCanvas.SetActive(value: true);
	}
}
