using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseScene : MonoBehaviour
{
	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void LoadSettingsScene()
	{
		SceneManager.LoadScene("SettingsScene");
	}
}
