using UnityEngine;

public class ReconnectWindowController : MonoBehaviour
{
	public GameObject window;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
		GameManager.Instance.reconnectingWindow = window;
	}

	private void Update()
	{
	}

	public void destroy()
	{
		if (base.gameObject != null)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
