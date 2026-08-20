using UnityEngine;

public class OnPickedUpScript : MonoBehaviour
{
	public void OnPickedUp(PickupItem item)
	{
		if (item.PickupIsMine)
		{
			DConsole.Log("I picked up something. That's a score!");
			PhotonNetwork.player.AddScore(1);
		}
		else
		{
			DConsole.Log("Someone else picked up something. Lucky!");
		}
	}
}
