using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishWindowController : MonoBehaviour
{
	public GameObject Window;

	public GameObject[] AvatarsMain;

	public GameObject[] AvatarsImage;

	public GameObject[] Names;

	public GameObject[] Backgrounds;

	public GameObject[] PrizeMainObjects;

	public GameObject[] prizeText;

	public GameObject[] placeIndicators;

	private void Start()
	{
		for (int i = 0; i < AvatarsMain.Length; i++)
		{
			AvatarsMain[i].SetActive(value: false);
		}
	}

	public void showWindow(List<PlayerObject> playersFinished, List<PlayerObject> otherPlayers, int firstPlacePrize, int secondPlacePrize)
	{
		if (secondPlacePrize == 0)
		{
			PrizeMainObjects[1].SetActive(value: false);
		}
		prizeText[0].GetComponent<Text>().text = firstPlacePrize.ToString();
		prizeText[1].GetComponent<Text>().text = secondPlacePrize.ToString();
		Window.SetActive(value: true);
		for (int i = 0; i < playersFinished.Count; i++)
		{
			AvatarsMain[i].SetActive(value: true);
			AvatarsImage[i].GetComponent<Image>().sprite = playersFinished[i].avatar;
			Names[i].GetComponent<Text>().text = playersFinished[i].name;
			if (playersFinished[i].id.Equals(PhotonNetwork.player.NickName))
			{
				Backgrounds[i].SetActive(value: true);
			}
		}
		int num = 0;
		for (int j = playersFinished.Count; j < playersFinished.Count + otherPlayers.Count; j++)
		{
			if (j == 1)
			{
				PrizeMainObjects[1].SetActive(value: false);
			}
			AvatarsMain[j].SetActive(value: true);
			AvatarsImage[j].GetComponent<Image>().sprite = otherPlayers[num].avatar;
			Names[j].GetComponent<Text>().text = otherPlayers[num].name;
			if (otherPlayers[num].id.Equals(PhotonNetwork.player.NickName))
			{
				Backgrounds[j].SetActive(value: true);
			}
			if (otherPlayers.Count > 1)
			{
				placeIndicators[j].SetActive(value: false);
			}
			num++;
		}
	}

	private void Update()
	{
	}
}
