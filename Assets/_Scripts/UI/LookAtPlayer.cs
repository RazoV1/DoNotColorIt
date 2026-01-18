using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
	private Transform camera;

	private void Start()
	{
		camera = Camera.main.transform;
	}

	private void Update()
	{
		transform.LookAt(camera);
	}
}
