using Assets._Scripts.Interaction_System.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MonsterWalk : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float pointPickingRange;
	private BasicItem basicItem;
	private NavMeshAgent agent;
	private PigmentMonster monster;
	private Vector2 pointInPlane;
	private Rigidbody rb;
	private GameObject baseMesh;
	private Fence home;
	private Animator animator;
	private Dictionary<string, float> stats;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		monster = GetComponent<PigmentMonster>();
		basicItem = GetComponent<BasicItem>();
		rb = GetComponent<Rigidbody>();
		baseMesh = GameObject.Find("ostrov_lp");
		animator = GetComponentInChildren<Animator>();
		TrySetupTarget();
	}

	private IEnumerator WanderCycle()
	{
		while (true)
		{
			stats = monster.GetMonsterStats();
		}
	}

	public void SetHome(Fence fence)
	{
		home = fence;
	}

	private void Interrupt()
	{
		if (basicItem.GetIsGrabbed())
		{
			agent.enabled = false;
		    
		}
	}

	private void TrySetupTarget()
	{
		pointInPlane = (Vector3)(Random.insideUnitCircle * pointPickingRange) + transform.position;
		Debug.Log($"<color=green>Picking point in circle: {pointInPlane}");
		NavMeshHit hit;
		NavMesh.SamplePosition(pointInPlane, out hit, pointPickingRange * 2, NavMesh.AllAreas);
		agent.SetDestination(hit.position);
		Debug.Log(agent.destination == null);
		animator.SetBool("Walking", true);
	}

	private void Update()
	{
		if (basicItem.GetIsGrabbed())
		{
			agent.enabled = false;
			rb.isKinematic = false;
			animator.SetBool("Walking", false);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject == baseMesh && !basicItem.GetIsGrabbed())
		{
			agent.enabled = true;
			rb.isKinematic = true;
			TrySetupTarget();
		}
	}

}
