using UnityEngine;

public class RainFollow : MonoBehaviour
{
	[SerializeField] private Transform toFollow;
	[SerializeField] private Vector3 offset;

	private Vector3 previousPos = Vector3.zero;
	private Vector3 delta;

	void Update()
	{
		delta = toFollow.position - previousPos;
		previousPos = toFollow.position;
		transform.position = toFollow.position + offset + delta * 50f;
	}
}
