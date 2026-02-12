using Assets._Scripts.Interaction_System.Objects;
using Assets._Scripts.Monsters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MonsterWalk : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float pointPickingRange;
	[Header("Weights")]
	[SerializeField] private float monsterWeight;
	[SerializeField] private float wanderingWeight;
	private MonsterSleep sleep;
	private BasicItem basicItem;
	private NavMeshAgent agent;
	private PigmentMonster monster;
	private Vector2 pointInPlane;
	private Rigidbody rb;
	private GameObject baseMesh;
	private Fence home;
	private Animator animator;
	private Dictionary<string, float> stats;
	private bool isWandering = false;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		monster = GetComponent<PigmentMonster>();
		basicItem = GetComponent<BasicItem>();
		rb = GetComponent<Rigidbody>();
		baseMesh = GameObject.Find("ostrov_lp");
		animator = GetComponentInChildren<Animator>();
		sleep = GetComponent<MonsterSleep>();
		TrySetupWanderingTarget();
	}

	private IEnumerator WanderCycle()
	{
		while (true)
		{
			DesideBehaviour();
			while (agent.destination != null && Vector3.Distance(agent.destination, transform.position) > 1f)
			{
				yield return null;
			}
			if (!isWandering)
			{
				yield return sleep.Sleep();
			}
		}
	}

	public void SetHome(Fence fence)
	{
		home = fence;
	}

	private void GoHome()
	{
		agent.SetDestination(ProjectTarget(home.transform.position));
		agent.isStopped = false;
	}

	private void Interrupt()
	{
		if (basicItem.GetIsGrabbed())
		{
			agent.enabled = false;
		}
	}

	private void DesideBehaviour()
	{
		stats = monster.GetMonsterStats();
		float strenght = stats["strenght"];

		float homeWeight = (strenght > 0.2f ? 0 : 1);
		float monsterWeight = this.monsterWeight * strenght * Random.Range(0.8f,1.1f);
		//Добавить реализацию POI сюда
		float wanderWeight = this.wanderingWeight * strenght * Random.Range(0.8f,1.1f);

		float maxWeight = Mathf.Max(homeWeight,monsterWeight, wanderWeight);

		
		if (maxWeight == wanderWeight)
		{
			TrySetupWanderingTarget();
			isWandering = true;
			return;
		}
		else if (maxWeight == homeWeight)
		{
			GoHome();
			isWandering = false;
			return;
		}
		else if (maxWeight == monsterWeight)
		{
			GoToMonster();
			isWandering = true;
			return;
		}
	}

	private void GoToMonster()
	{
		List<PigmentMonster> monsters = FindAllMonsters();
		Vector3 worldMonsterPos = monsters[Random.Range(0,monsters.Count)].transform.position;
		agent.SetDestination(ProjectTarget(worldMonsterPos));
	}

	private List<PigmentMonster> FindAllMonsters() => GameObject.FindObjectsByType<PigmentMonster>(FindObjectsSortMode.InstanceID).Where(x => x != monster).ToList();
	

	private void TrySetupWanderingTarget()
	{
		pointInPlane = (Vector3)(Random.insideUnitCircle * pointPickingRange) + transform.position;
		Debug.Log($"<color=green>Picking point in circle: {pointInPlane}");
		agent.SetDestination(ProjectTarget(pointInPlane));
		Debug.Log(agent.destination == null);
		animator.SetBool("Walking", true);
	}

	private Vector3 ProjectTarget(Vector3 target)
	{
		NavMeshHit hit;
		NavMesh.SamplePosition(pointInPlane, out hit, pointPickingRange * 2, NavMesh.AllAreas);
		return hit.position;
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
			if (isWandering)
			{
				TrySetupWanderingTarget();
			}
		}
	}

}
