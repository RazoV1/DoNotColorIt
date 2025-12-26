using Assets._Scripts.Events;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
	[SerializeField] private PigmentMonster monsterInside;
	[SerializeField] private List<Fence> neighboursForCalculations = new List<Fence>();


	public PigmentMonster GetMonsterInside() => monsterInside;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Egg")
		{
			TutorialEvents.OnAdditionalTutorialTriggered.Invoke(4);
		}
		PigmentMonster monster = other.GetComponent<PigmentMonster>();
		if (monster != null && monsterInside == null)
		{
			monsterInside = monster;
			monster.SetInTheFence(true);
			monster.SetNeighbour(neighboursForCalculations[0].GetMonsterInside());
			if (monsterInside.GetIdealColor() != neighboursForCalculations[0].GetMonsterInside().GetIdealColor())
			{
				TutorialEvents.OnAdditionalTutorialTriggered.Invoke(3);
			}
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
