using System;
using UnityEngine;

public class LocalNotification
{
	private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";

	public static int SendNotification(TimeSpan delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		return SendNotification(new System.Random().Next(), (int)delay.TotalSeconds * 1000, title, message, bgColor, sound, vibrate, lights, bigIcon);
	}

	public static int SendNotification(int id, TimeSpan delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		return SendNotification(id, (int)delay.TotalSeconds * 1000, title, message, bgColor, sound, vibrate, lights, bigIcon);
	}

	public static int SendNotification(int id, long delayMs, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		return 0;
	}

	public static int SendRepeatingNotification(TimeSpan delay, TimeSpan timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		return SendRepeatingNotification(new System.Random().Next(), (int)delay.TotalSeconds * 1000, (int)timeout.TotalSeconds, title, message, bgColor, sound, vibrate, lights, bigIcon);
	}

	public static int SendRepeatingNotification(int id, TimeSpan delay, TimeSpan timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		return SendRepeatingNotification(id, (int)delay.TotalSeconds * 1000, (int)timeout.TotalSeconds, title, message, bgColor, sound, vibrate, lights, bigIcon);
	}

	public static int SendRepeatingNotification(int id, long delayMs, long timeoutMs, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		return 0;
	}

	public static void CancelNotification(int id)
	{
	}

	public static void ClearNotifications()
	{
		new AndroidJavaClass(fullClassName)?.CallStatic("ClearShowingNotifications");
	}
}
