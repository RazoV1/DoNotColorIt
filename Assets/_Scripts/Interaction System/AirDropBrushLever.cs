using UnityEngine;

public class AirDropBrushLever : FillBucketlever
{
	[SerializeField] private Transform brush;
	[SerializeField] private Transform spawnPoint;

	private void Start()
	{
		brush = GameObject.Find("Brush(Clone)").transform;
	}

	protected override void Activate()
	{
		brush.transform.position = spawnPoint.position;
		brush.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
	}
}
