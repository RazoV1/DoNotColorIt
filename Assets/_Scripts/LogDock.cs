using UnityEngine;

public class LogDock : MonoBehaviour
{
	[SerializeField] private Infuser infuser;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Log")
		{
			infuser.AddLog();
			Destroy(other.gameObject);
		}
	}
}
