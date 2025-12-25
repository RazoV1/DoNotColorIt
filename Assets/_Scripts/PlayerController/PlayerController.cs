using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{
	[SerializeField] private float walkingSpeed;
	[SerializeField] private float acceleration;
	[SerializeField] private CameraController cameraController;
	[SerializeField] private bool shouldSave;
	[SerializeField] private LayerMask layerMask;

	public void SetCanWalk(bool isWalking) { this.canWalk = isWalking; }

	private Rigidbody rb;

	private Transform cameraPivotTransform;

	private bool canWalk = true;
	private float currentSpeed;

	private void LateUpdate()
	{
		if (!canWalk) return;
		HandleMovementByInput();
	}


	private void Start()
	{
		SubscribeToSaveEvent();
		rb = GetComponent<Rigidbody>();
		SyncData(new SavablePrefab());
		cameraPivotTransform = cameraController.transform.parent;
		rb.inertiaTensor = Vector3.zero;
	}

	private void HandleMovementByInput()
	{
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		float verticalInput = Input.GetAxisRaw("Vertical");

		Vector2 direction = new Vector2(horizontalInput, verticalInput).normalized;
		Vector3 forward = cameraPivotTransform.forward * direction.y;
		Vector3 right = cameraPivotTransform.right * direction.x;
		Vector3 movement = (forward + right) * walkingSpeed;
		movement.y = 0;
		transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
		RaycastHit[] hit = Physics.RaycastAll(transform.position + movement / 4f, Vector3.down, 2f, ~layerMask);
		if (hit.Where(x => x.collider.gameObject.tag == "Ground").ToList().Count == 0 || hit.Where(x => x.collider.gameObject.tag == "Water").ToList().Count >= 1)
		{
			rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
			return;
		}

		//rb.linearVelocity = new Vector3(movement.x,rb.linearVelocity.y,movement.z);
		rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
	}

	private void OnDestroy()
	{
		SaveEvents.OnSaveEvent.RemoveListener(SaveData);
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
	}

	public void SaveData()
	{
		if (!shouldSave) return;
		SaveManager saveManager = SaveManager.Instance;

		saveManager.SaveFloat("playerX",transform.position.x);
		saveManager.SaveFloat("playerY", transform.position.y);
		saveManager.SaveFloat("playerZ", transform.position.z);
	}

	public void SyncData(SavablePrefab data)
	{
		if (!shouldSave) return;
		SaveManager saveManager = SaveManager.Instance;

		float x = saveManager.GetFloat("playerX");
		float y = saveManager.GetFloat("playerY");
		float z = saveManager.GetFloat("playerZ");

		if (x == y && x == z && z == 0)
		{
			return;
		}

		rb.MovePosition(new Vector3(x,y,z));
	}
}
