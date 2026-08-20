using UnityEngine;

public class WindowTransitionController : MonoBehaviour
{
	private Animator animator;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		animator.Play("ShowScreen");
	}

	public void HideScreen()
	{
		animator.Play("HideScreen");
	}

	public void DisableGameObject()
	{
		base.gameObject.SetActive(value: false);
	}
}
