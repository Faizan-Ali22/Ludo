using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindowController : MonoBehaviour
{
	public GameObject Sounds;

	public GameObject Vibrations;

	public GameObject Notifications;

	public GameObject FriendsRequests;

	public GameObject PrivateRoomRequests;

	private void Start()
	{
		if (PlayerPrefs.GetInt(StaticStrings.SoundsKey, 0) == 1)
		{
			Sounds.GetComponent<Toggle>().isOn = false;
		}
		if (PlayerPrefs.GetInt(StaticStrings.NotificationsKey, 0) == 1)
		{
			Notifications.GetComponent<Toggle>().isOn = false;
		}
		if (PlayerPrefs.GetInt(StaticStrings.VibrationsKey, 0) == 1)
		{
			Vibrations.GetComponent<Toggle>().isOn = false;
		}
		if (PlayerPrefs.GetInt(StaticStrings.FriendsRequestesKey, 0) == 1)
		{
			FriendsRequests.GetComponent<Toggle>().isOn = false;
		}
		if (PlayerPrefs.GetInt(StaticStrings.PrivateRoomKey, 0) == 1)
		{
			PrivateRoomRequests.GetComponent<Toggle>().isOn = false;
		}
		Sounds.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		Notifications.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		Vibrations.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		FriendsRequests.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		PrivateRoomRequests.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		Sounds.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool value)
		{
			PlayerPrefs.SetInt(StaticStrings.SoundsKey, (!value) ? 1 : 0);
			if (value)
			{
				AudioListener.volume = 1f;
			}
			else
			{
				AudioListener.volume = 0f;
			}
		});
		Notifications.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool value)
		{
			PlayerPrefs.SetInt(StaticStrings.NotificationsKey, (!value) ? 1 : 0);
			if (!value)
			{
				DConsole.Log("Clear notifications!");
				LocalNotification.CancelNotification(1);
			}
		});
		Vibrations.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool value)
		{
			PlayerPrefs.SetInt(StaticStrings.VibrationsKey, (!value) ? 1 : 0);
		});
		FriendsRequests.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool value)
		{
			PlayerPrefs.SetInt(StaticStrings.FriendsRequestesKey, (!value) ? 1 : 0);
		});
		PrivateRoomRequests.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool value)
		{
			PlayerPrefs.SetInt(StaticStrings.PrivateRoomKey, (!value) ? 1 : 0);
		});
	}
}
