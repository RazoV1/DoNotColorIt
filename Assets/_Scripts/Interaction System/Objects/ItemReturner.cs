using UnityEngine;

public class ItemReturner : MonoBehaviour
{
	[SerializeField] private Transform returnPoint;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.tag == "TP_")
		{
			transform.position = returnPoint.position;
		}
	}
}