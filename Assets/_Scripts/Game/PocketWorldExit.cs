using UnityEngine;

public class PocketWorldExit : MonoBehaviour
{
    [SerializeField] private PlayerGrabber grabber;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Bucket")
		{
		    PocketTicker.Instance.PutBucket(collision.collider.GetComponent<Bucket>().GetColor());
			Destroy(collision.collider.gameObject);
		}else if (collision.collider.tag == "Player")
		{
			GameManager.Instance.ChangeDimensions(1);
			Debug.Log("Dimension Change!");
		}
	}
}