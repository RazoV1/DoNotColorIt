using UnityEngine;

public class UpperWorldReturner : MonoBehaviour
{
    [SerializeField] private Transform player;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Player")
		{
			player.transform.Translate(Vector3.up * 2);
		}
	}
}
