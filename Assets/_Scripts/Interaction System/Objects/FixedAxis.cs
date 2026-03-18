using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Interaction_System.Objects
{
	public class FixedAxis : ConditionalGrabbable
	{
		[SerializeField] private Vector3 rotationAxis;
		[SerializeField] public float rotationRadius;

		public Vector3 GetRotationAxis()
		{
			return rotationAxis;
		}

		public Vector3 GetRotationCenter() => rb.worldCenterOfMass;
		public float GetRotationRadius() => rotationRadius;
	}
}
