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
	[SerializeField] private float strengthExpenditure = 0.016f;
	[Header("Weights")]
	[SerializeField] private float monsterWeight;
	[SerializeField] private float wanderingWeight;
	private MonsterSleep sleep;
	private BasicItem basicItem;
	private NavMeshAgent agent;
	private PigmentMonster monster;
	private Vector3 pointInPlane;
	private Rigidbody rb;
	private GameObject baseMesh;
	[SerializeField]private Transform home;
	private Animator animator;
	private Dictionary<string, float> stats;
	private bool isWandering = false;

	private Coroutine tryWanderRoutine;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		monster = GetComponent<PigmentMonster>();
		basicItem = GetComponent<BasicItem>();
		rb = GetComponent<Rigidbody>();
		baseMesh = GameObject.Find("ostrov_lp");
		animator = GetComponentInChildren<Animator>();
		sleep = GetComponent<MonsterSleep>();
		StartCoroutine(WanderCycle());
		//TrySetupWanderingTarget();
	}

	private IEnumerator WanderCycle()
	{
		yield return new WaitForSeconds(1f);
		while (true)
		{
			animator.SetBool("Walking", false);

			agent.isStopped = true;
			DesideBehaviour();
			yield return new WaitForSeconds(4f);
			while (agent.destination != null && Vector3.Distance(agent.destination, transform.position) > 1f)
			{
				monster.ChangeStrength(-Time.deltaTime * strengthExpenditure);
				yield return null;
			}
			if (!isWandering)
			{
				animator.SetBool("Walking", false);
				StartCoroutine(sleep.Sleep());
				yield return new WaitForSeconds(10);
			}
		}
	}

	public void SetHome(Transform fence)
	{
		if (home != null)
		{
			return;
		}
		home = fence;
	}

	private void GoHome()
	{
		agent.SetDestination(ProjectTarget(home.position));

		animator.SetBool("Walking", true);
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
		//float monsterWeight = this.monsterWeight * strenght * Random.Range(0.8f,1.1f);
		//Добавить реализацию POI сюда
		float wanderWeight = this.wanderingWeight * strenght * Random.Range(0.8f,1.1f);
		float monsterWeight = 0f; //Временно

		float maxWeight = Mathf.Max(homeWeight,monsterWeight, wanderWeight);

		
		if (maxWeight == wanderWeight)
		{
			Debug.Log("Гуляю");
			if (tryWanderRoutine != null) { StopCoroutine(tryWanderRoutine); }
			tryWanderRoutine = StartCoroutine(TrySetupWanderingTarget());
			isWandering = true;
			return;
		}
		else if (maxWeight == homeWeight)
		{
			Debug.Log("По домам");
			try
			{
				GoHome();
			}
			catch
			{
				return;
			}
			isWandering = false;
			return;
		}
		else if (maxWeight == monsterWeight)
		{
			Debug.Log("На забив");
			GoToMonster();
			isWandering = true;
			return;
		}
	}

	private void GoToMonster()
	{
		List<PigmentMonster> monsters = FindAllMonsters();
		if (monsters.Count == 0) { return; }
		Vector3 worldMonsterPos = monsters[Random.Range(0,monsters.Count)].transform.position;
		agent.SetDestination(ProjectTarget(worldMonsterPos));
	}

	private List<PigmentMonster> FindAllMonsters() => GameObject.FindObjectsByType<PigmentMonster>(FindObjectsSortMode.InstanceID).Where(x => x != monster).ToList();
	

	private IEnumerator TrySetupWanderingTarget()
	{
		yield return new WaitForSeconds(3);
		pointInPlane = (Vector3)(Random.insideUnitSphere * pointPickingRange) + transform.position;
		Debug.Log($"<color=green>Picking point in circle: {pointInPlane}");
		agent.SetDestination(ProjectTarget(pointInPlane));
		Debug.Log(agent.destination == null);
		animator.SetBool("Walking", true);
		agent.isStopped = false;
	}

	private Vector3 ProjectTarget(Vector3 target)
	{
		NavMeshHit hit;
		//NavMesh.SamplePosition(pointInPlane, out hit, pointPickingRange, NavMesh.AllAreas);
		//return hit.position;
		return new Vector3(target.x,transform.position.y,target.z);
	}

	private void Update()
	{
		if (basicItem.GetIsGrabbed())
		{
			agent.enabled = false;
			rb.isKinematic = false;
			rb.interpolation = RigidbodyInterpolation.Interpolate;
			animator.SetBool("Walking", false);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if ((collision.collider.gameObject == baseMesh || collision.collider.tag == "Ground") && !basicItem.GetIsGrabbed())
		{
			agent.enabled = true;
			rb.isKinematic = true;
			rb.interpolation = RigidbodyInterpolation.None;
			if (isWandering)
			{
				if (tryWanderRoutine != null) { StopCoroutine(tryWanderRoutine); }
				tryWanderRoutine = StartCoroutine(TrySetupWanderingTarget());
			}
		}
	}

}
