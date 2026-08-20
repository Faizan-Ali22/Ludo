using UnityEngine;

public class GameInitScript : MonoBehaviour
{
	private void Start()
	{
		if (GameManager.Instance.roomOwner)
		{
			PhotonNetwork.RaiseEvent(198, null, sendReliable: true, null);
		}
	}

	private void Update()
	{
	}
}
