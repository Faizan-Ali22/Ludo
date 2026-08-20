using System.Collections;
using System.IO;
using UnityEngine;

public class NativeShare : MonoBehaviour
{
	public string ScreenshotName = "ScreenshotLudo.png";

	public void ShareScreenshotWithText(string text)
	{
		string text2 = Application.persistentDataPath + "/" + ScreenshotName;
		if (File.Exists(text2))
		{
			File.Delete(text2);
		}
		ScreenCapture.CaptureScreenshot(ScreenshotName);
		Debug.Log("Screenshot path: " + text2 + " Text: " + text);
		StartCoroutine(delayedShare(text2, text));
	}

	private IEnumerator delayedShare(string screenShotPath, string text)
	{
		Debug.Log("Delay share");
		while (!File.Exists(screenShotPath))
		{
			yield return new WaitForSeconds(0.05f);
		}
		Share(text, screenShotPath, "");
		yield return null;
	}

	public void Share(string shareText, string imagePath, string url, string subject = "")
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent");
		androidJavaObject.Call<AndroidJavaObject>("setAction", new object[1] { androidJavaClass.GetStatic<string>("ACTION_SEND") });
		if (imagePath != null)
		{
			AndroidJavaObject androidJavaObject2 = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", new object[1] { "file://" + imagePath });
			androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				androidJavaClass.GetStatic<string>("EXTRA_STREAM"),
				androidJavaObject2
			});
			androidJavaObject.Call<AndroidJavaObject>("setType", new object[1] { "image/png" });
		}
		else
		{
			androidJavaObject.Call<AndroidJavaObject>("setType", new object[1] { "text/plain" });
		}
		androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
		{
			androidJavaClass.GetStatic<string>("EXTRA_TEXT"),
			shareText
		});
		AndroidJavaObject androidJavaObject3 = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject androidJavaObject4 = androidJavaClass.CallStatic<AndroidJavaObject>("createChooser", new object[2] { androidJavaObject, subject });
		androidJavaObject3.Call("startActivity", androidJavaObject4);
	}
}
