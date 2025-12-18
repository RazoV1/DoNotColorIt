using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
	[SerializeField] private PigmentMonster monsterInside;
	[SerializeField] private List<Fence> neighboursForCalculations = new List<Fence>();


	public PigmentMonster GetMonsterInside() => monsterInside;

	private void OnTriggerEnter(Collider other)
	{
		PigmentMonster monster = other.GetComponent<PigmentMonster>();
		if (monster != null && monsterInside == null)
		{
			monsterInside = monster;
			monster.SetInTheFence(true);
			monster.SetNeighbour(neighboursForCalculations[0].GetMonsterInside());
		}
	}

	private void OnTriggerExit(Collider other)
	{
		PigmentMonster monster = other.GetComponent<PigmentMonster>();
		if (monster != null && monsterInside == monster)
		{
			monsterInside = null;
			monster.SetInTheFence(false);
			monster.SetNeighbour(null);
		}
	}
}
