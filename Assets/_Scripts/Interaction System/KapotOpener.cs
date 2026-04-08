using Assets._Scripts.PlayerController;
using UnityEngine;

public class KapotOpener : MonoBehaviour
{
	[SerializeField] private CameraRaycaster cameraCaster;
	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		CheckInput();
	}

	private void CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			CheckCast();
		}
	}

	private void CheckCast()
	{
		if (cameraCaster.HasHitSomething())
		{
			RaycastHit hit = cameraCaster.GetHit();
			if (hit.collider.gameObject == gameObject)
			{
				animator.SetBool("IsOpen", !animator.GetBool("IsOpen"));
				GameManager.Instance.GetTutorial().ProgressTutorial("openTrunk");
			}
		}
	}
}