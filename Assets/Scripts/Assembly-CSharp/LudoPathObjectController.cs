using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LudoPathObjectController : MonoBehaviour
{
	public List<GameObject> pawns = new List<GameObject>();

	public bool isProtectedPlace;

	private void Start()
	{
		GetComponent<Image>().enabled = false;
	}

	public void AddPawn(GameObject pawn)
	{
		pawns.Add(pawn);
	}

	public void RemovePawn(GameObject pawn)
	{
		pawns.Remove(pawn);
	}

	private void Update()
	{
	}
}
