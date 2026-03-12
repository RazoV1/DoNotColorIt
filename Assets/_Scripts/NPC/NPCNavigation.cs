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

		private void Awake()
		{
			animator = GetComponent<Animator>();
			agent = GetComponent<NavMeshAgent>();
		}

        private void Update()
        {
            animator.SetBool("Walking", !agent.isStopped && agent.remainingDistance > agent.stoppingDistance);
        }

        public IEnumerator TraverseToPoint(Transform point)
		{

			agent.isStopped = false;
			agent.SetDestination(point.position);
			animator.SetBool("Walking",true);
			while (Vector3.Distance(point.position, transform.position) > destinationFluct)
			{
				yield return null;
			}
			
			agent.isStopped = true;
			transform.rotation  =point.rotation;
			animator.SetBool("Walking", false);
			yield break;
		}
	}
}
