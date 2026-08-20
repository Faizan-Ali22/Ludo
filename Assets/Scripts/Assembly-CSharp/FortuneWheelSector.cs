using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FortuneWheelSector
{
	[Tooltip("Text object where value will be placed (not required)")]
	public GameObject ValueTextObject;

	[Tooltip("Value of reward")]
	public string RewardValue = "100";

	[Tooltip("Chance that this sector will be randomly selected")]
	[Range(0f, 100f)]
	public int Probability = 100;

	[Tooltip("Method that will be invoked if this sector will be randomly selected")]
	public UnityEvent RewardCallback;
}
