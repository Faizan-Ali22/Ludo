using UnityEngine;

public class ConnectionLostController : MonoBehaviour
{
	public GameObject canvas;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
		GameManager.Instance.connectionLost = this;
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			showDialog();
		}
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

	public void showDialog()
	{
		canvas.SetActive(value: true);
	}

	public void closeApp()
	{
		Application.Quit();
	}
}
