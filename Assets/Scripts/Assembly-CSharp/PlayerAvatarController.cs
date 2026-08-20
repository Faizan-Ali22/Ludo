using UnityEngine;
using UnityEngine.UI;

public class PlayerAvatarController : MonoBehaviour
{
	public GameObject Name;

	public GameObject Avatar;

	public GameObject Timer;

	public GameObject leftRoomObject;

	public GameObject MainObject;

	public GameObject Crown;

	public GameObject Position;

	public Sprite[] PositionSprites;

	[HideInInspector]
	public bool Active = true;

	[HideInInspector]
	public bool finished;

	public AudioSource PlayerLeftRoomAudio;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void PlayerLeftRoom()
	{
		if (!finished)
		{
			PlayerLeftRoomAudio.Play();
			Active = false;
			Name.GetComponent<Text>().text = "";
			MainObject.transform.localScale = new Vector2(0.8f, 0.8f);
			leftRoomObject.SetActive(value: true);
		}
	}

	public void PlayerFinishedGame()
	{
	}

	public void setPositionSprite(int index)
	{
		Position.SetActive(value: true);
		Position.GetComponent<Image>().sprite = PositionSprites[index - 1];
	}
}
