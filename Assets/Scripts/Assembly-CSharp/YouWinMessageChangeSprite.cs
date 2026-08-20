using AssemblyCSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YouWinMessageChangeSprite : MonoBehaviour
{
	public Sprite other;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void changeSprite()
	{
		GetComponent<Image>().sprite = other;
	}

	public void loadWinnerScene()
	{
		if (GameManager.Instance.offlineMode)
		{
			GameManager.Instance.playfabManager.roomOwner = false;
			GameManager.Instance.roomOwner = false;
			GameManager.Instance.resetAllData();
			SceneManager.LoadScene("MenuScene");
			PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong;
		}
		else
		{
			SceneManager.LoadScene("WinnerScene");
		}
	}
}
