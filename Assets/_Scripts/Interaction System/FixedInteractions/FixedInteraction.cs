using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Interaction_System.FixedInteractions
{
	public class FixedInteraction : MonoBehaviour
	{
		[SerializeField] private Transform cameraPoint;
		[SerializeField] private float transitionTime;

		private Transform cameraPivot;
		private CameraController cameraController;
	    private PlayerController playerController;

		private bool isLocked = false;
		private Coroutine routine;

		public bool GetIsLocked() => isLocked;

		public void SetLocked(bool isLocked)
		{
			this.isLocked = isLocked;
			FindCameraComponents();
			playerController.SetCanWalk(!isLocked);
			cameraController.SetShouldRotate(!isLocked);
			Cursor.lockState = isLocked ?  CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = isLocked;
			cameraController.SetFollowBody(isLocked ? cameraPoint : null);

			//if (routine != null)  StopCoroutine(routine);
			//if (isLocked)
			//{
			//	routine = StartCoroutine(MoveCameraToPoint(cameraPoint));
			//}
		}

		private void FindCameraComponents()
		{
			cameraController = FindObjectOfType<CameraController>();
			cameraPivot = cameraController.transform.parent;

			playerController = FindObjectOfType<PlayerController>();
		}

		//private IEnumerator MoveCameraToPoint(Transform point)
		//{
		//	float timePassed = 0;

		//	while (timePassed < transitionTime)
		//	{
		//		cameraPivot.position = Vector3.Lerp(cameraPivot.position,point.position,timePassed/transitionTime);
		//		cameraPivot.rotation = Quaternion.Lerp(cameraPivot.rotation, point.rotation, timePassed / transitionTime);
		//		timePassed += Time.deltaTime;
		//		yield return null;
		//	}
		//}
	}
}
