using UnityEngine;

public class BucketDock : MonoBehaviour
{
    [SerializeField] private Infuser infuser;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Bucket")
		{
			infuser.SetBucket(other.GetComponent<Bucket>());
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Bucket")
		{
			infuser.SetBucket(null);
		}
	}
}
