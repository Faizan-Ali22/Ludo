using UnityEngine;
using UnityEngine.UI;

public class SetMyData : MonoBehaviour
{
	public GameObject avatar;

	public GameObject fullName;

	public GameObject matchCanvas;

	public GameObject controlAvatars;

	public GameObject backButton;

	public void MatchPlayer()
	{
		if (GameManager.Instance.avatarMy != null)
		{
			avatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
		}
		controlAvatars.GetComponent<ControlAvatars>().reset();
	}

	public void setBackButton(bool active)
	{
		backButton.SetActive(active);
	}
}
