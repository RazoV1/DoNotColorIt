using Assets._Scripts.Delivery;
using Assets._Scripts.Events;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	[Header("General")]
	[Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
	[Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)][SerializeField] private float yRotationLimit = 88f;
	[SerializeField] private Transform playerBody;
	[SerializeField] private Vector3 bodyOffset;
	[SerializeField] private float followSpeed;
	[Header("Wobble")]
	[SerializeField] private float horizontalStrength;
	[SerializeField] private float horizontalWobbleLimit;
	[SerializeField] private float wobbleTransitionSpeed;
	private Transform pivot;
	Vector3 rotation = Vector3.zero;
	const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
	const string yAxis = "Mouse Y";
	private bool shouldRotateCamera = true;
	private bool isLockedByPause = false;

	private Transform followedBody;

	Vector3 preservedAngles = Vector3.zero;

	public float GetSensitivity() => sensitivity;

	private void Start()
	{
		pivot = transform.parent;
		followedBody = playerBody;
		Cursor.lockState = CursorLockMode.Locked;
		GameplayEvents.OnPauseToggled.AddListener(SetIsLockedByPause);
	}
	
	private void SetIsLockedByPause(bool isLockedByPause) { this.isLockedByPause = isLockedByPause;}

	private void OnDestroy()
	{
		GameplayEvents.OnPauseToggled.RemoveListener(SetIsLockedByPause);
	}

	public void MountFiat(MetlaController fiat)
	{
		shouldRotateCamera = false;
		fiat.SetIsMounted(true, this);
		playerBody.GetComponent<PlayerController>().SetCanWalk(false);
		playerBody.transform.parent = fiat.transform;
		playerBody.gameObject.SetActive(false);
		playerBody.GetComponent<Rigidbody>().isKinematic = true;
		followedBody = fiat.GetPivot();
		transform.rotation = followedBody.transform.rotation;
		playerBody.GetComponent<Collider>().enabled = false;
		//pivot.parent = followedBody;
	}

	public void UnmountFiat(MetlaController fiat)
	{
		playerBody.gameObject.SetActive(true);
		shouldRotateCamera = true;
		playerBody.GetComponent<PlayerController>().SetCanWalk(true);
		playerBody.transform.parent = null;
		playerBody.GetComponent<Rigidbody>().isKinematic = false;
		followedBody = playerBody;
		playerBody.GetComponent<Collider>().enabled = true;
	}

	public bool GetShouldRotate() => shouldRotateCamera;

	void Update()
	{
		FollowPlayerBody();
		if (isLockedByPause) return;
		HandleCameraRotationByMouse();
	}

	private void FollowPlayerBody()
	{
		pivot.position = Vector3.Lerp(pivot.position,followedBody.position + bodyOffset,Time.deltaTime * followSpeed * (playerBody == followedBody ? 1f :1));
		if (playerBody != followedBody && !isLockedByPause)
		{
		   transform.rotation = Quaternion.Lerp(transform.rotation, followedBody.rotation,Time.deltaTime * followSpeed);
		}
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


	public Quaternion GetRotationFromMouseInput(bool isLimited)
	{
		rotation.x += Input.GetAxis(xAxis) * sensitivity;
		rotation.y += Input.GetAxis(yAxis) * sensitivity;

		if (isLimited)
		{
			rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		}

		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

		return xQuat * yQuat;
	}

	private void HandleWobbleByMouse()
	{
		float mouseDelta = Input.GetAxis(xAxis);
		float wobble = mouseDelta * horizontalStrength;
		wobble = Mathf.Clamp(wobble, -horizontalWobbleLimit, horizontalWobbleLimit);
		transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.localEulerAngles.z, wobble, Time.deltaTime * wobbleTransitionSpeed));
	}

	private void HandleCameraRotationByMouse()
	{
		if (!shouldRotateCamera) return;
		pivot.localRotation = Quaternion.Lerp(pivot.localRotation, GetRotationFromMouseInput(true),Time.deltaTime * 50f);
		HandleWobbleByMouse();
	}
}
