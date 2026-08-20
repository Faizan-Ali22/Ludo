using UnityEngine;

public class OnClickRightDestroy : MonoBehaviour
{
	public void OnPressRight()
	{
		DConsole.Log("RightClick Destroy");
		PhotonNetwork.Destroy(base.gameObject);
	}
}
