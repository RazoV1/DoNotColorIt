using System.Collections.Generic;
using System;
using UnityEngine;
using Assets._Scripts.Events;
using System.Linq;
using System.Collections;
using Assets._Scripts.NPC;

public class NPCRouitine : MonoBehaviour
{
	[Serializable]
	public struct NpcAction
	{
		public float time;
		public Transform point;
		public string animationTrigger;
		public int progressionIndexContdition;
	}
	[SerializeField] private List<NpcAction> availableActions;
	[SerializeField] private NpcAction fallbackAction;

	private Animator animator;
	private NPCNavigation navigation;
	private Coroutine coroutine;

	private void Awake()
	{
		GameplayEvents.OnNpcTick.AddListener(DecideAction);
		Debug.Log("Added npc tick listener");

		navigation = GetComponent<NPCNavigation>();
		animator = GetComponent<Animator>();
	}

	private void OnDestroy()
	{
		GameplayEvents.OnNpcTick.RemoveListener(DecideAction);
		Debug.Log("Removed npc tick listener");
	}

	private void DecideAction(int time)
	{
		List<NpcAction> actionOnTime = availableActions.Where(x => x.time == time && x.progressionIndexContdition <= GameManager.Instance.GetCurrentTaskIndex()).ToList();
		if (actionOnTime.Count == 0)
		{
			Act(fallbackAction);
			return;
		}
		Act(actionOnTime[0]);
	}

	private void Act(NpcAction action)
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}
		coroutine = StartCoroutine(ActionRoutine(action));
	}

	private IEnumerator ActionRoutine(NpcAction action)
	{
		yield return navigation.StartCoroutine(navigation.TraverseToPoint(action.point.position));
		if (action.animationTrigger != "")
		{
			animator.SetTrigger(action.animationTrigger);
		}
	}
}