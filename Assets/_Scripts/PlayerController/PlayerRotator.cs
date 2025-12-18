using Assets._Scripts.Interaction_System.Interfaces;
using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
	[SerializeField] private float rotationForce;
	[SerializeField] private PlayerGrabber grabber;
	[SerializeField] private CameraController cameraController;

	private Transform rotatingTransform;

	private void Update()
	{
		HandleInput();
	}

	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(1))
		{
			HandleMouseButtonPressed();
		}
		if (Input.GetMouseButton(1))
		{
			HandleRotation();
		}
		if (Input.GetMouseButtonUp(1))
		{
			ResetRotatingObject();
		}
	}

	private void HandleRotation()
	{
		if (!grabber.GetIsGrabbing()) return;
		if (rotatingTransform == null)
		{
			Debug.Log("<color=red>Нет объекта для ротации!");
			grabber.StopGrabbing();
			return;
		}
		//Quaternion rotation = cameraController.GetRotationFromMouseInput(false);

		//rotatingTransform.rotation = Quaternion.Inverse(rotation);
		rotatingTransform.Rotate(Vector3.up,-Input.mousePositionDelta.x);

		rotatingTransform.Rotate(Vector3.forward, -Input.mousePositionDelta.y);
	}

	private void ResetRotatingObject()
	{
		if (rotatingTransform == null) return;
		//((IGrabbable)rotatingTransform.GetComponent<InteractableObject>()).SetMaintainDirection(true);
		//rotatingTransform.localEulerAngles = Vector3.zero;
		cameraController.SetShouldRotate(true);
	}

	private void HandleMouseButtonPressed()
	{
		if (!grabber.GetIsGrabbing()) return;
		InteractableObject rotatingObjectClass = grabber.GetGrabbedObject();
		if (!(rotatingObjectClass is IRotatable))
		{
			return;
		}
		((IGrabbable)rotatingObjectClass).SetMaintainDirection(false);
		cameraController.SetShouldRotate(false);
		//rotatingTransform = rotatingObjectClass.transform;
		rotatingTransform = grabber.getGrabPivot();
	}
}

//При удалении надо сбрасывать граббедобжект