using Assets._Scripts.Interaction_System.Objects;
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
		[SerializeField] private List<ConditionalGrabbable> grabbables;
		[SerializeField] private List<DefaultGrabbablePosForName> defaultPos;
		[SerializeField] private List<string> grabbablesToFindByName;

		private Transform cameraPivot;
		private CameraController cameraController;
		private PlayerController playerController;

		private bool isLocked = false;
		private Coroutine routine;

		public bool GetIsLocked() => isLocked;

		[Serializable]
		public struct DefaultGrabbablePosForName
		{
			public string prefabName;
			public Transform point;
		}

		public void SetLocked(bool isLocked)
		{
			this.isLocked = isLocked;
			FindCameraComponents();
			playerController.SetCanWalk(!isLocked);
			cameraController.SetShouldRotate(!isLocked);
			Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = isLocked;
			SetGrabbables(isLocked);
			cameraController.SetFollowBody(isLocked ? cameraPoint : null);

			if (isLocked)
			{
				try
				{
					grabbables.ForEach(grabbable => grabbable.transform.position = defaultPos.First(x => x.prefabName == grabbable.GetPrefabName()).point.position);
				}
				catch (Exception e)
				{
					Debug.Log("<color=red>Ошибка FixedInteraction! " + e);
				}
			}
			//if (routine != null)  StopCoroutine(routine);
			//if (isLocked)
			//{
			//	routine = StartCoroutine(MoveCameraToPoint(cameraPoint));
			//}
		}

		private void SetGrabbables(bool shouldBeGrabbed)
		{
			if (grabbables.Count == 0) return;
			grabbables.ForEach(x => x.SetCanBeGrabbed(shouldBeGrabbed));
		}

		private void FindGrabbablesByName()
		{
			if (grabbablesToFindByName.Count == 0) return;
			grabbables = FindObjectsByType<ConditionalGrabbable>(FindObjectsSortMode.InstanceID).ToList().Where(x => grabbablesToFindByName.Contains(x.GetPrefabName())).ToList();
		}

		private void FindCameraComponents()
		{
			cameraController = FindObjectOfType<CameraController>();
			cameraPivot = cameraController.transform.parent;

			playerController = FindObjectOfType<PlayerController>();
		}

		private void Start()
		{
			FindGrabbablesByName();
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
