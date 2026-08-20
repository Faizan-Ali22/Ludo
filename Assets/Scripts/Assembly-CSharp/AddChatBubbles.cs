using AssemblyCSharp;
using UnityEngine;

public class AddChatBubbles : MonoBehaviour
{
	public GameObject prefab;

	public GameObject parent;

	private void Start()
	{
		for (int i = 0; i < StaticStrings.chatNames.Length; i++)
		{
			GameObject obj = Object.Instantiate(prefab);
			obj.GetComponent<ChatShopController>().fillData(i);
			obj.transform.parent = parent.transform;
			obj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void Update()
	{
	}
}
