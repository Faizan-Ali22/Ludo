using UnityEngine;

public class NotificationTest : MonoBehaviour
{
	private void Awake()
	{
		LocalNotification.ClearNotifications();
	}

	public void OneTime()
	{
		LocalNotification.SendNotification(1, 5000L, "Title", "Long message text", new Color32(byte.MaxValue, 68, 68, byte.MaxValue));
	}

	public void OneTimeBigIcon()
	{
		LocalNotification.SendNotification(1, 5000L, "Title", "Long message text with big icon", new Color32(byte.MaxValue, 68, 68, byte.MaxValue), sound: true, vibrate: true, lights: true, "app_icon");
	}

	public void Repeating()
	{
		LocalNotification.SendRepeatingNotification(1, 5000L, 5000L, "Title", "Long message text", new Color32(byte.MaxValue, 68, 68, byte.MaxValue));
	}

	public void Stop()
	{
		LocalNotification.CancelNotification(1);
	}
}
