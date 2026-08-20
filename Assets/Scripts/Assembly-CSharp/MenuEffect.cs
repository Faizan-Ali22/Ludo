using UnityEngine;
using UnityEngine.UI;

public class MenuEffect : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Image>().enabled = false;
		Invoke("start", Random.Range(0f, 1f));
	}

	private void start()
	{
		GetComponent<Image>().enabled = true;
		GetComponent<Animator>().speed = Random.Range(0.1f, 0.3f);
		GetComponent<Animator>().Play("Menu_Effect");
	}
}
