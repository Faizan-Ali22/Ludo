using UnityEngine;
using UnityEngine.UI;

public class ChangeStoreTabsColor : MonoBehaviour
{
	public GameObject[] tabs;

	private Color normalColor;

	private Color otherColor = new Color(0f, 0.2f, 0.47058824f);

	private void Start()
	{
		normalColor = tabs[2].GetComponent<Image>().color;
		SetSelectectedTab(2);
	}

	public void SetSelectectedTab(int index)
	{
		for (int i = 0; i < tabs.Length; i++)
		{
			if (i != index)
			{
				tabs[i].GetComponent<Image>().color = otherColor;
			}
			else
			{
				tabs[i].GetComponent<Image>().color = normalColor;
			}
		}
	}

	private void Update()
	{
	}
}
