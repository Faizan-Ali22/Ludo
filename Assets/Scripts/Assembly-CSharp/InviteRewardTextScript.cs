using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class InviteRewardTextScript : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Text>().text = "Earn " + StaticStrings.rewardCoinsForFriendInvite + " coins";
	}

	private void Update()
	{
	}
}
