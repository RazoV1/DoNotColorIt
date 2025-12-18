using UnityEngine;

public class DustDock : MonoBehaviour
{
	[SerializeField] private Infuser infuser;

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<DustedColorPigment>())
		{
			infuser.PutDustInside(other.GetComponent<DustedColorPigment>());
		}
	}
}
