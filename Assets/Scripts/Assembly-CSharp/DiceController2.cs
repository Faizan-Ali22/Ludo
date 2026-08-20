using UnityEngine;

public class DiceController2 : MonoBehaviour
{
	public GameObject mainDice;

	private void Start()
	{
	}

	public void FinishAnim()
	{
		mainDice.GetComponent<GameDiceController>().SetDiceValue();
	}

	private void Update()
	{
	}
}
