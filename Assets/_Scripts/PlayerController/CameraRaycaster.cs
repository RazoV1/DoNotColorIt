using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.PlayerController
{
	public class CameraRaycaster : MonoBehaviour
	{
		[Header("Ray Settings")]
		[SerializeField] private float maxDistance;
		private RaycastHit hit;
		private bool hasHitSomething;

		private Camera camera;

		private void Awake()
		{
			camera = Camera.main;
		}

		private void CastRay()
		{
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);

			hasHitSomething = Physics.Raycast(ray, out hit) && Vector3.Distance(hit.point, ray.origin) <= maxDistance;
		}

		public bool HasHitSomething() => hasHitSomething;

		public RaycastHit GetHit() => hit;

		private void Update()
		{
			CastRay();
		}
	}
}
