using UnityEngine;

public class AirController : MonoBehaviour
{
	[Header("General")]
	[Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
	[Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)][SerializeField] private float yRotationLimit = 88f;
	[SerializeField] private Transform playerBody;
	[SerializeField] private Vector3 bodyOffset;
	[Header("Wobble")]
	[SerializeField] private float horizontalStrength;
	[SerializeField] private float horizontalWobbleLimit;
	[SerializeField] private float wobbleTransitionSpeed;
	private Transform pivot;
	Vector3 rotation = Vector3.zero;
	const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
	const string yAxis = "Mouse Y";
	private bool shouldRotateCamera = true;

	Vector3 preservedAngles = Vector3.zero;

	public float GetSensitivity() => sensitivity;

	private void Start()
	{
		pivot = transform.parent;
	}

	public void SetShouldRotate(bool shouldRotateCamera)
	{
		this.shouldRotateCamera = shouldRotateCamera;
		if (!shouldRotateCamera)
		{
			preservedAngles = rotation;
		}
		else
		{
			rotation = preservedAngles;
		}
	}

	public Vector2 GetRotationFromMouseInput(bool isLimited)
	{
		rotation.x += Input.GetAxis(xAxis) * sensitivity;
		rotation.y += Input.GetAxis(yAxis) * sensitivity;

		if (isLimited)
		{
			rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		}
		return rotation;
	}
}
