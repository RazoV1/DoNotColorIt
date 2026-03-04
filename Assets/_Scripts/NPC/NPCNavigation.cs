using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets._Scripts.NPC
{
	public class NPCNavigation : MonoBehaviour
	{
		[Header("Settings")]
		[SerializeField] private float destinationFluct;
		[SerializeField] private Animator animator;

		private NavMeshAgent agent;
		private Coroutine routine;


		public void MoveToPoint(Transform point)
		{
			if (routine != null)
			{
				StopCoroutine(routine);
			}
			routine = StartCoroutine(TraverseToPoint(point.position));
		}

		private IEnumerator TraverseToPoint(Vector3 point)
		{
			agent.SetDestination(point);
			animator.SetBool("Walking",true);
			while (Vector3.Distance(point, transform.position) > destinationFluct)
			{
				yield return null;
			}
			agent.isStopped = true;
			animator.SetBool("Walking", false);
			yield break;
		}
	}
}
