using UnityEngine;

public class AddEmojiBubbles : MonoBehaviour
{
	public GameObject prefab;

	public GameObject parent;

	private void Start()
	{
		int packsCount = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().packsCount;
		for (int i = 0; i < packsCount - 1; i++)
		{
			GameObject obj = Object.Instantiate(prefab);
			obj.GetComponent<EmojiShopController>().fillData(i);
			obj.transform.parent = parent.transform;
			obj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void Update()
	{
	}
}
