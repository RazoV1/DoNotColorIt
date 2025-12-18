using UnityEngine;

public class SmolaCollector : MonoBehaviour
{
	[SerializeField] private Transform smolaPivot;

	[SerializeField] private float yOffset = 0.25f;
	[SerializeField] private float yPerVolume=0.05f;

	public void Start()
	{
		smolaPivot.Translate(0, PocketTicker.Instance.GetSmola()*yPerVolume, 0);
	}

	public void Move(float delta)
	{
		try
		{
			smolaPivot.Translate(0, delta * yPerVolume, 0);
		}
		catch{ }
	}
}
