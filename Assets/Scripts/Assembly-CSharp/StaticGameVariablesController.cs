using UnityEngine;

public class StaticGameVariablesController : MonoBehaviour
{
	public Sprite[] avatars;

	public Sprite[] emoji;

	public int emojiPerPack = 12;

	public int packsCount = 6;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	public void destroy()
	{
		if (base.gameObject != null)
		{
			Object.DestroyImmediate(base.gameObject);
		}
	}
}
