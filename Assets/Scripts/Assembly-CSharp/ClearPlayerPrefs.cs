using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
	public void clear()
	{
		PlayerPrefs.DeleteAll();
	}
}
