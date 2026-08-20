using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ChatAppIdCheckerUI : MonoBehaviour
{
	public Text Description;

	public void Update()
	{
		if (ChatSettings.Instance == null || string.IsNullOrEmpty(ChatSettings.Instance.AppId))
		{
			Description.text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the ChatSettings file.";
		}
		else
		{
			Description.text = string.Empty;
		}
	}
}
