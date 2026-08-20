using UnityEngine;

public class DialogGUIController : MonoBehaviour
{
	public static DialogGUIController instance;

	public GameObject Other;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			Other.GetComponent<AdMobObjectController>().Init();
		}
		else if (instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
