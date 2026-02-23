using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Scripts.Interaction_System.Objects
{
	public class PivotedItem : BasicItem
	{
		[Header("Pivoted Settings")]
		[SerializeField] private Transform preferredAnglePivot;
		[SerializeField] private float transitionTime;

		private Coroutine routine;

		public Transform GetPivot() => preferredAnglePivot;

		public override bool SetIsGrabbed(bool isGrabbed, Transform target = null)
		{
			if (isGrabbed)
			{
				//StartAlligning();
			}
			else
			{
				//StopCoroutine(routine);
				//routine = null;
			}
			Debug.Log($"Going To Base: {isGrabbed}");
			return base.SetIsGrabbed(isGrabbed, target);
		}

		private void StartAlligning()
		{
			if (routine != null)
			{
			   StopCoroutine(routine);
			}
			routine = StartCoroutine(AllignToPreferredRotation());
		}

		private IEnumerator AllignToPreferredRotation()
		{
			float timePassed = 0;
			while (timePassed < transitionTime)
			{
			    timePassed += Time.deltaTime;
				transform.rotation = Quaternion.Lerp(transform.rotation, preferredAnglePivot.rotation, Time.deltaTime * transitionTime * 10);
				yield return null;
			}
		}
	}
}
