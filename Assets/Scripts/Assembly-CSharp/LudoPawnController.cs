using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LudoPawnController : MonoBehaviour
{
	public AudioSource killedPawnSound;

	public AudioSource inHomeSound;

	public GameObject pawnTop;

	public GameObject pawnTopMultiple;

	public GameObject dice;

	public GameObject pawnInJoint;

	public bool mainInJoint;

	public GameObject highlight;

	public bool isOnBoard;

	private LudoGameController ludoController;

	public RectTransform[] path;

	private int currentPosition = -1;

	private float singlePathSpeed = 0.13f;

	private float MoveToStartPositionSpeed = 0.25f;

	private RectTransform rect;

	private Vector3 initScale;

	public bool isMinePawn;

	public int index;

	public bool myTurn;

	[HideInInspector]
	private int playerIndex;

	public AudioSource[] sound;

	public Vector2 initPosition;

	private bool canMakeJoint;

	private int currentAudioSource;

	private void Start()
	{
		ludoController = GameObject.Find("GameSpecific").GetComponent<LudoGameController>();
		rect = GetComponent<RectTransform>();
		initScale = rect.localScale;
		initPosition = rect.anchoredPosition;
		GetComponent<Button>().interactable = false;
		if (GameManager.Instance.mode == MyGameMode.Master)
		{
			canMakeJoint = true;
		}
	}

	public void setPlayerIndex(int index)
	{
		playerIndex = index;
	}

	public void Highlight(bool active)
	{
		if (GameManager.Instance.currentPlayer.isBot)
		{
			GetComponent<Button>().interactable = false;
			highlight.SetActive(value: false);
		}
		else if (active)
		{
			GetComponent<Button>().interactable = true;
			highlight.SetActive(value: true);
		}
		else
		{
			GetComponent<Button>().interactable = false;
			highlight.SetActive(value: false);
		}
	}

	public int GetMoveScore(int steps)
	{
		if (steps == 6 && !isOnBoard)
		{
			return 300;
		}
		if (isOnBoard)
		{
			if (GameManager.Instance.mode == MyGameMode.Quick && GameManager.Instance.currentPlayer.canEnterHome)
			{
				return 500;
			}
			if (pawnInJoint != null)
			{
				steps /= 2;
			}
			if (currentPosition + steps == path.Length - 1)
			{
				return 1000;
			}
			if (!path[currentPosition].GetComponent<LudoPathObjectController>().isProtectedPlace && path[currentPosition + steps].GetComponent<LudoPathObjectController>().isProtectedPlace)
			{
				return 400;
			}
			LudoPathObjectController component = path[currentPosition + steps].GetComponent<LudoPathObjectController>();
			if (component.pawns.Count > 0)
			{
				for (int i = 0; i < component.pawns.Count; i++)
				{
					if (component.pawns[i].GetComponent<LudoPawnController>().playerIndex == playerIndex)
					{
						return 700;
					}
				}
			}
			if (component.pawns.Count > 0)
			{
				for (int j = 0; j < component.pawns.Count; j++)
				{
					if (component.pawns[j].GetComponent<LudoPawnController>().playerIndex != playerIndex)
					{
						return 500;
					}
				}
			}
			if (path[currentPosition].GetComponent<LudoPathObjectController>().isProtectedPlace)
			{
				return -100;
			}
		}
		return 0;
	}

	public bool CheckIfCanMove(int steps)
	{
		if (steps == 6 && !isOnBoard)
		{
			Highlight(active: true);
			return true;
		}
		if (isOnBoard)
		{
			if (pawnInJoint != null)
			{
				if (steps % 2 != 0)
				{
					return false;
				}
				steps /= 2;
			}
			if (currentPosition + steps < path.Length)
			{
				LudoPathObjectController component = path[currentPosition + steps].GetComponent<LudoPathObjectController>();
				DConsole.Log("pawns count on destination: " + component.pawns.Count);
				if (component.pawns.Count == 2 && component.pawns[0].GetComponent<LudoPawnController>().pawnInJoint != null)
				{
					DConsole.Log("im inside");
					if (pawnInJoint != null)
					{
						DConsole.Log("return true");
						if (component.pawns[0].GetComponent<LudoPawnController>().playerIndex != playerIndex)
						{
							Highlight(active: true);
							return true;
						}
						return false;
					}
					return false;
				}
			}
			for (int i = 1; i < steps + 1; i++)
			{
				if (currentPosition + i >= path.Length)
				{
					continue;
				}
				DConsole.Log("check count: " + path[currentPosition + i].GetComponent<LudoPathObjectController>().pawns.Count);
				if (path[currentPosition + i].GetComponent<LudoPathObjectController>().pawns.Count > 1)
				{
					DConsole.Log("more than 1");
					if (path[currentPosition + i].GetComponent<LudoPathObjectController>().pawns[0].GetComponent<LudoPawnController>().pawnInJoint != null)
					{
						DConsole.Log("blockade");
						return false;
					}
				}
			}
			if (currentPosition == path.Length - 1 || currentPosition + steps > path.Length - 1)
			{
				return false;
			}
			if (currentPosition + steps > path.Length - 1 - 6 && GameManager.Instance.needToKillOpponentToEnterHome && !GameManager.Instance.playerObjects[playerIndex].canEnterHome)
			{
				return false;
			}
			Highlight(active: true);
			return true;
		}
		return false;
	}

	public void GoToStartPosition()
	{
		rect.SetAsLastSibling();
		currentPosition = 0;
		StartCoroutine(MoveDelayed(0, initPosition, path[currentPosition].anchoredPosition, MoveToStartPositionSpeed, last: true, playSound: true));
		if (pawnInJoint != null)
		{
			pawnInJoint.GetComponent<LudoPawnController>().pawnInJoint = null;
			pawnInJoint.GetComponent<LudoPawnController>().GoToStartPosition();
			pawnInJoint = null;
		}
	}

	public void GoToInitPosition(bool callEnd)
	{
		killedPawnSound.Play();
		rect.SetAsLastSibling();
		isOnBoard = false;
		currentPosition = -1;
		pawnTop.SetActive(value: true);
		pawnTopMultiple.SetActive(value: false);
		StartCoroutine(MoveDelayed(0, rect.anchoredPosition, initPosition, MoveToStartPositionSpeed, last: true, playSound: false));
		if (pawnInJoint != null)
		{
			pawnInJoint.GetComponent<LudoPawnController>().pawnInJoint = null;
			pawnInJoint.GetComponent<LudoPawnController>().GoToInitPosition(callEnd: true);
			pawnInJoint = null;
		}
	}

	public void MoveBySteps(int steps)
	{
		LudoPathObjectController component = path[currentPosition].GetComponent<LudoPathObjectController>();
		component.RemovePawn(base.gameObject);
		RepositionPawns(component.pawns.Count, currentPosition);
		rect.SetAsLastSibling();
		for (int i = 0; i < steps; i++)
		{
			bool last = false;
			if (i == steps - 1)
			{
				last = true;
			}
			currentPosition++;
			StartCoroutine(MoveDelayed(i, path[currentPosition - 1].anchoredPosition, path[currentPosition].anchoredPosition, singlePathSpeed, last, playSound: true));
		}
	}

	public void MakeMove()
	{
		DConsole.Log("Make move button");
		string eventContent = index + ";" + ludoController.gUIController.GetCurrentPlayerIndex() + ";" + ludoController.steps;
		PhotonNetwork.RaiseEvent(51, eventContent, sendReliable: true, null);
		if (pawnInJoint != null)
		{
			ludoController.steps /= 2;
		}
		GameManager.Instance.diceShot = true;
		myTurn = true;
		ludoController.gUIController.PauseTimers();
		ludoController.Unhighlight();
		if (!isOnBoard)
		{
			GoToStartPosition();
		}
		else
		{
			if (pawnInJoint != null)
			{
				pawnInJoint.GetComponent<LudoPawnController>().MoveBySteps(ludoController.steps);
			}
			MoveBySteps(ludoController.steps);
		}
		isOnBoard = true;
	}

	public void MakeMovePC()
	{
		if (pawnInJoint != null)
		{
			ludoController.steps /= 2;
		}
		myTurn = false;
		ludoController.gUIController.PauseTimers();
		if (!isOnBoard)
		{
			GoToStartPosition();
		}
		else
		{
			if (pawnInJoint != null)
			{
				pawnInJoint.GetComponent<LudoPawnController>().MoveBySteps(ludoController.steps);
			}
			MoveBySteps(ludoController.steps);
		}
		isOnBoard = true;
	}

	private IEnumerator MoveDelayed(int delay, Vector2 from, Vector2 to, float time, bool last, bool playSound)
	{
		rect.localScale = new Vector3(initScale.x * 1.2f, initScale.y * 1.2f, initScale.z);
		yield return new WaitForSeconds((float)delay * singlePathSpeed);
		if (playSound)
		{
			sound[currentAudioSource % sound.Length].Play();
			currentAudioSource++;
		}
		if (last)
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("from", from, "to", to, "time", time, "easetype", iTween.EaseType.linear, "onupdate", "UpdatePosition", "oncomplete", "MoveFinished"));
		}
		else
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("from", from, "to", to, "time", time, "easetype", iTween.EaseType.linear, "onupdate", "UpdatePosition"));
		}
	}

	private void resetScale()
	{
		rect.localScale = initScale;
	}

	private void MoveFinished()
	{
		resetScale();
		if (currentPosition < 0)
		{
			return;
		}
		bool flag = true;
		LudoPathObjectController component = path[currentPosition].GetComponent<LudoPathObjectController>();
		component.AddPawn(base.gameObject);
		if (!(pawnInJoint == null) && (!(pawnInJoint != null) || !mainInJoint))
		{
			return;
		}
		DConsole.Log("Main in joint");
		int count = component.pawns.Count;
		DConsole.Log("Pawns count: " + count);
		if (!component.isProtectedPlace)
		{
			if (count > 1)
			{
				for (int num = count - 2; num >= 0; num--)
				{
					if (component.pawns[num].GetComponent<LudoPawnController>().playerIndex != playerIndex)
					{
						int num2 = component.pawns[num].GetComponent<LudoPawnController>().playerIndex;
						int num3 = 0;
						for (int i = 0; i < count; i++)
						{
							if (component.pawns[i].GetComponent<LudoPawnController>().playerIndex == num2)
							{
								num3++;
							}
						}
						if (num3 == 1 || canMakeJoint)
						{
							ludoController.nextShotPossible = true;
							GameManager.Instance.playerObjects[playerIndex].canEnterHome = true;
							GameManager.Instance.playerObjects[playerIndex].homeLockObjects.SetActive(value: false);
							component.pawns[num].GetComponent<LudoPawnController>().GoToInitPosition(callEnd: false);
							component.RemovePawn(component.pawns[num]);
						}
					}
					else if (canMakeJoint && pawnInJoint == null)
					{
						DConsole.Log("Joint");
						pawnInJoint = component.pawns[num];
						mainInJoint = true;
						component.pawns[num].GetComponent<LudoPawnController>().mainInJoint = false;
						component.pawns[num].GetComponent<LudoPawnController>().pawnInJoint = base.gameObject;
						pawnTop.SetActive(value: false);
						pawnTopMultiple.SetActive(value: true);
						component.pawns[num].GetComponent<LudoPawnController>().pawnTop.SetActive(value: false);
						component.pawns[num].GetComponent<LudoPawnController>().pawnTopMultiple.SetActive(value: true);
					}
				}
			}
		}
		else if (pawnInJoint != null)
		{
			flag = false;
			pawnTop.SetActive(value: true);
			pawnTopMultiple.SetActive(value: false);
			pawnInJoint.GetComponent<LudoPawnController>().pawnTop.SetActive(value: true);
			pawnInJoint.GetComponent<LudoPawnController>().pawnTopMultiple.SetActive(value: false);
			pawnInJoint.GetComponent<LudoPawnController>().pawnInJoint = null;
			pawnInJoint = null;
		}
		count = component.pawns.Count;
		if (pawnInJoint == null)
		{
			RepositionPawns(count, currentPosition);
		}
		if (currentPosition == path.Length - 1)
		{
			inHomeSound.Play();
		}
		if ((myTurn || GameManager.Instance.currentPlayer.isBot) && currentPosition == path.Length - 1)
		{
			DConsole.Log("FINISHSSSS");
			GameManager.Instance.currentPlayer.finishedPawns++;
			if (GameManager.Instance.mode == MyGameMode.Quick)
			{
				if (GameManager.Instance.currentPlayer.finishedPawns == 1)
				{
					ludoController.gUIController.FinishedGame();
					return;
				}
			}
			else if (GameManager.Instance.currentPlayer.finishedPawns == 4)
			{
				ludoController.gUIController.FinishedGame();
				return;
			}
			ludoController.nextShotPossible = true;
		}
		if (((myTurn && GameManager.Instance.diceShot) || GameManager.Instance.currentPlayer.isBot) && flag)
		{
			if (ludoController.nextShotPossible)
			{
				GameManager.Instance.currentPlayer.dice.GetComponent<GameDiceController>().EnableShot();
				ludoController.gUIController.restartTimer();
			}
			else
			{
				DConsole.Log("move finished call finish turn");
				StartCoroutine(CheckTurnDelay());
			}
		}
		else
		{
			ludoController.gUIController.restartTimer();
		}
	}

	private IEnumerator CheckTurnDelay()
	{
		yield return new WaitForSeconds(1f);
		ludoController.gUIController.SendFinishTurn();
	}

	private void RepositionPawns(int otherCount, int currentPosition)
	{
		LudoPathObjectController component = path[currentPosition].GetComponent<LudoPathObjectController>();
		float num = 0.8f;
		float num2 = 20f / (float)otherCount;
		float num3 = 0f;
		num3 = (0f - num2) / 2f * (float)otherCount + num2 / 2f;
		num = 1f - 0.05f * (float)otherCount + 0.05f;
		List<int> list = new List<int>();
		for (int i = 0; i < otherCount; i++)
		{
			if (component.pawns[i].GetComponent<LudoPawnController>().playerIndex == GameManager.Instance.myPlayerIndex)
			{
				list.Add(i);
			}
			else
			{
				list.Insert(0, i);
			}
		}
		for (int j = 0; j < otherCount; j++)
		{
			component.pawns[list[j]].GetComponent<RectTransform>().anchoredPosition = new Vector2(path[currentPosition].GetComponent<RectTransform>().anchoredPosition.x + num3 + (float)j * num2, path[currentPosition].GetComponent<RectTransform>().anchoredPosition.y);
			component.pawns[list[j]].GetComponent<RectTransform>().localScale = new Vector2(initScale.x * num, initScale.y * num);
			component.pawns[list[j]].GetComponent<RectTransform>().SetAsLastSibling();
		}
	}

	private void UpdatePosition(Vector2 pos)
	{
		rect.anchoredPosition = pos;
	}

	private void Update()
	{
	}
}
