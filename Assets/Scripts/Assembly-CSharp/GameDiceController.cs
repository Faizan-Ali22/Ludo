using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class GameDiceController : MonoBehaviour
{
	public Sprite[] diceValueSprites;

	public GameObject arrowObject;

	public GameObject diceValueObject;

	public GameObject diceAnim;

	public bool isMyDice;

	public GameObject LudoController;

	public LudoGameController controller;

	public int player = 1;

	private Button button;

	public GameObject notInteractable;

	private int steps;

	private void Start()
	{
		button = GetComponent<Button>();
		controller = LudoController.GetComponent<LudoGameController>();
		button.interactable = false;
	}

	public void SetDiceValue()
	{
		DConsole.Log("Set dice value called");
		diceValueObject.GetComponent<Image>().sprite = diceValueSprites[steps - 1];
		diceValueObject.SetActive(value: true);
		diceAnim.SetActive(value: false);
		controller.gUIController.restartTimer();
		if (isMyDice)
		{
			controller.HighlightPawnsToMove(player, steps);
		}
		if (GameManager.Instance.currentPlayer.isBot)
		{
			controller.HighlightPawnsToMove(player, steps);
		}
	}

	private void Update()
	{
	}

	public void EnableShot()
	{
		if (GameManager.Instance.currentPlayer.isBot)
		{
			GameManager.Instance.miniGame.BotTurn(first: false);
			notInteractable.SetActive(value: false);
			return;
		}
		if (PlayerPrefs.GetInt(StaticStrings.VibrationsKey, 0) == 0)
		{
			DConsole.Log("Vibrate");
			Handheld.Vibrate();
		}
		else
		{
			DConsole.Log("Vibrations OFF");
		}
		controller.gUIController.myTurnSource.Play();
		notInteractable.SetActive(value: false);
		button.interactable = true;
		arrowObject.SetActive(value: true);
	}

	public void DisableShot()
	{
		notInteractable.SetActive(value: true);
		button.interactable = false;
		arrowObject.SetActive(value: false);
	}

	public void EnableDiceShadow()
	{
		notInteractable.SetActive(value: true);
	}

	public void DisableDiceShadow()
	{
		notInteractable.SetActive(value: false);
	}

	public void RollDice()
	{
		if (isMyDice)
		{
			controller.nextShotPossible = false;
			controller.gUIController.PauseTimers();
			button.interactable = false;
			DConsole.Log("Roll Dice");
			arrowObject.SetActive(value: false);
			steps = Random.Range(1, 7);
			RollDiceStart(steps);
			string eventContent = steps + ";" + controller.gUIController.GetCurrentPlayerIndex();
			PhotonNetwork.RaiseEvent(50, eventContent, sendReliable: true, null);
			DConsole.Log("Value: " + steps);
		}
	}

	public void RollDiceBot(int value)
	{
		controller.nextShotPossible = false;
		controller.gUIController.PauseTimers();
		DConsole.Log("Roll Dice bot");
		steps = value;
		RollDiceStart(steps);
	}

	public void RollDiceStart(int steps)
	{
		GetComponent<AudioSource>().Play();
		this.steps = steps;
		diceValueObject.SetActive(value: false);
		diceAnim.SetActive(value: true);
		diceAnim.GetComponent<Animator>().Play("RollDiceAnimation");
	}
}
