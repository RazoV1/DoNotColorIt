using Assets._Scripts.Interaction_System.Objects;
using UnityEngine;

public class FillBucketlever : MonoBehaviour
{
	[SerializeField] private Infuser infuser;
	[SerializeField] private BasicItem lever;

	[SerializeField] private float stiffness;

	[SerializeField] private float startAngle;
	[SerializeField] private float activationAngle;
	[SerializeField] private Rigidbody rb;

	[SerializeField] private Transform rotationParent;
	[SerializeField] private Transform activationParent;

	private bool isReturning = false;
	float rotationalDelta;
	

	

	void Update()
	{
		if (Quaternion.Angle(transform.localRotation, activationParent.localRotation) < 5f)
		{
		   Activate();
		}
		if (isReturning)
		{
			Debug.Log($"<color=yellow>Return: {transform.localEulerAngles.x}");
			transform.localRotation = Quaternion.RotateTowards(transform.localRotation, rotationParent.localRotation, Time.deltaTime * stiffness);
			if (Quaternion.Angle(transform.localRotation, rotationParent.localRotation) < 0.2f)
			{
				transform.localRotation = rotationParent.localRotation;
				isReturning = false;
				Debug.Log("<color=green>READY");
			}
		}
	}

	protected virtual void Activate()
	{
		infuser.TryCook();
		lever.SetIsGrabbed(false);
		isReturning = true;
		rb.angularVelocity = new Vector3(0f, 0f, 0f);
	}
}
