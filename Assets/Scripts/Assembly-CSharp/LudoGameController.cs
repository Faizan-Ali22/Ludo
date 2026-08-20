using System;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class LudoGameController : PunBehaviour, IMiniGame
{
	public GameObject[] dice;

	public GameObject GameGui;

	public GameGUIController gUIController;

	public GameObject[] Pawns1;

	public GameObject[] Pawns2;

	public GameObject[] Pawns3;

	public GameObject[] Pawns4;

	public GameObject gameBoard;

	public GameObject gameBoardScaler;

	[HideInInspector]
	public int steps = 5;

	public bool nextShotPossible;

	private int SixStepsCount;

	public int finishedPawns;

	private int botCounter;

	private List<GameObject> botPawns;

	public void HighlightPawnsToMove(int player, int steps)
	{
		botPawns = new List<GameObject>();
		gUIController.restartTimer();
		GameObject[] pawns = GameManager.Instance.currentPlayer.pawns;
		this.steps = steps;
		if (steps == 6)
		{
			nextShotPossible = true;
			SixStepsCount++;
			if (SixStepsCount == 3)
			{
				nextShotPossible = false;
				if (GameGui != null)
				{
					Invoke("sendFinishTurnWithDelay", 1f);
				}
				return;
			}
		}
		else
		{
			SixStepsCount = 0;
			nextShotPossible = false;
		}
		bool flag = false;
		int num = 0;
		GameObject gameObject = null;
		for (int i = 0; i < pawns.Length; i++)
		{
			if (pawns[i].GetComponent<LudoPawnController>().CheckIfCanMove(steps))
			{
				gameObject = pawns[i];
				flag = true;
				num++;
				botPawns.Add(pawns[i]);
			}
		}
		if (num == 1)
		{
			if (GameManager.Instance.currentPlayer.isBot)
			{
				StartCoroutine(movePawn(gameObject, delay: false));
			}
			else
			{
				gameObject.GetComponent<LudoPawnController>().MakeMove();
			}
		}
		else if (num == 2 && gameObject.GetComponent<LudoPawnController>().pawnInJoint != null)
		{
			if (GameManager.Instance.currentPlayer.isBot)
			{
				if (!gameObject.GetComponent<LudoPawnController>().mainInJoint)
				{
					StartCoroutine(movePawn(gameObject, delay: false));
					DConsole.Log("AAA");
				}
				else
				{
					StartCoroutine(movePawn(gameObject.GetComponent<LudoPawnController>().pawnInJoint, delay: false));
					DConsole.Log("BBB");
				}
			}
			else if (!gameObject.GetComponent<LudoPawnController>().mainInJoint)
			{
				gameObject.GetComponent<LudoPawnController>().MakeMove();
			}
			else
			{
				gameObject.GetComponent<LudoPawnController>().pawnInJoint.GetComponent<LudoPawnController>().MakeMove();
			}
		}
		else if (num > 0 && GameManager.Instance.currentPlayer.isBot)
		{
			int index = 0;
			int num2 = int.MinValue;
			for (int j = 0; j < botPawns.Count; j++)
			{
				int moveScore = botPawns[j].GetComponent<LudoPawnController>().GetMoveScore(steps);
				if (moveScore > num2)
				{
					num2 = moveScore;
					index = j;
				}
			}
			StartCoroutine(movePawn(botPawns[index], delay: true));
		}
		if (!flag && GameGui != null)
		{
			DConsole.Log("game controller call finish turn");
			gUIController.PauseTimers();
			Invoke("sendFinishTurnWithDelay", 1f);
		}
	}

	private IEnumerator MovePawnWithDelay(GameObject lastPawn)
	{
		yield return new WaitForSeconds(1f);
		lastPawn.GetComponent<LudoPawnController>().MakeMove();
	}

	public void sendFinishTurnWithDelay()
	{
		gUIController.SendFinishTurn();
	}

	public void Unhighlight()
	{
		for (int i = 0; i < Pawns1.Length; i++)
		{
			Pawns1[i].GetComponent<LudoPawnController>().Highlight(active: false);
		}
		for (int j = 0; j < Pawns2.Length; j++)
		{
			Pawns2[j].GetComponent<LudoPawnController>().Highlight(active: false);
		}
		for (int k = 0; k < Pawns3.Length; k++)
		{
			Pawns3[k].GetComponent<LudoPawnController>().Highlight(active: false);
		}
		for (int l = 0; l < Pawns4.Length; l++)
		{
			Pawns4[l].GetComponent<LudoPawnController>().Highlight(active: false);
		}
	}

	void IMiniGame.BotTurn(bool first)
	{
		if (first)
		{
			SixStepsCount = 0;
		}
		Invoke("RollDiceWithDelay", GameManager.Instance.botDelays[(botCounter + 1) % GameManager.Instance.botDelays.Count]);
		botCounter++;
	}

	public IEnumerator movePawn(GameObject pawn, bool delay)
	{
		if (delay)
		{
			yield return new WaitForSeconds(GameManager.Instance.botDelays[(botCounter + 1) % GameManager.Instance.botDelays.Count]);
			botCounter++;
		}
		pawn.GetComponent<LudoPawnController>().MakeMovePC();
	}

	public void RollDiceWithDelay()
	{
		GameManager.Instance.currentPlayer.dice.GetComponent<GameDiceController>().RollDiceBot(GameManager.Instance.botDiceValues[(botCounter + 1) % GameManager.Instance.botDelays.Count]);
	}

	void IMiniGame.CheckShot()
	{
		throw new NotImplementedException();
	}

	void IMiniGame.setMyTurn()
	{
		SixStepsCount = 0;
		GameManager.Instance.diceShot = false;
		dice[0].GetComponent<GameDiceController>().EnableShot();
	}

	void IMiniGame.setOpponentTurn()
	{
		SixStepsCount = 0;
		GameManager.Instance.diceShot = false;
		dice[0].GetComponent<GameDiceController>().DisableShot();
		Unhighlight();
	}

	private void Awake()
	{
		GameManager.Instance.miniGame = this;
		PhotonNetwork.OnEventCall += OnEvent;
	}

	private void Start()
	{
		float x = gameBoardScaler.GetComponent<RectTransform>().rect.size.x;
		float x2 = gameBoard.GetComponent<RectTransform>().rect.size.x;
		gameBoard.GetComponent<RectTransform>().localScale = new Vector2(x / x2, x / x2);
		gUIController = GameGui.GetComponent<GameGUIController>();
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		PhotonNetwork.OnEventCall -= OnEvent;
	}

	private void OnEvent(byte eventcode, object content, int senderid)
	{
		DConsole.Log("Received event Ludo: " + eventcode);
		switch (eventcode)
		{
		case 50:
		{
			gUIController.PauseTimers();
			string[] array3 = ((string)content).Split(';');
			steps = int.Parse(array3[0]);
			int index3 = int.Parse(array3[1]);
			GameManager.Instance.playerObjects[index3].dice.GetComponent<GameDiceController>().RollDiceStart(steps);
			break;
		}
		case 51:
		{
			string[] array2 = ((string)content).Split(';');
			int num2 = int.Parse(array2[0]);
			int index2 = int.Parse(array2[1]);
			steps = int.Parse(array2[2]);
			GameManager.Instance.playerObjects[index2].pawns[num2].GetComponent<LudoPawnController>().MakeMovePC();
			break;
		}
		case 52:
		{
			string[] array = ((string)content).Split(';');
			int num = int.Parse(array[1]);
			int index = int.Parse(array[0]);
			GameManager.Instance.playerObjects[index].pawns[num].GetComponent<LudoPawnController>().GoToInitPosition(callEnd: false);
			break;
		}
		}
	}
}
