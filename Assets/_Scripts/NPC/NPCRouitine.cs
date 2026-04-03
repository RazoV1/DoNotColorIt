using System.Collections.Generic;
using System;
using UnityEngine;
using Assets._Scripts.Events;
using System.Linq;
using System.Collections;
using Assets._Scripts.NPC;
using Assets._Scripts.Game.SaveSystem;

public class NPCRouitine : MonoBehaviour, ISavable
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

	[Header("Savable")]
	private string npcName; //Used to give a name to currentActionIndex in the save file.
	private int currentActionIndex = 0;

	private void Awake()
	{
		GameplayEvents.OnNpcTick.AddListener(DecideAction);

		npcName = GetComponent<NPCWaiter>().GetName();
		navigation = GetComponent<NPCNavigation>();
		animator = GetComponent<Animator>();
	}

	private void OnDestroy()
	{
		GameplayEvents.OnNpcTick.RemoveListener(DecideAction);
		SaveEvents.OnSaveEvent.RemoveListener(SaveData);
		SaveEvents.OnLoadEvent.RemoveListener(SyncDataPlaceholder);
		
	}

	private void DecideAction(int time)
	{
		List<NpcAction> actionOnTime = availableActions.Where(x => x.time <= time && x.progressionIndexContdition <= GameManager.Instance.GetCurrentTaskIndex()).ToList();
		if (actionOnTime.Count == 0)
		{
			//Debug.Log("Fallback action!");
			Act(fallbackAction);
			return;
		}
		
		Act(actionOnTime[0]);
	}

	private void Act(NpcAction action)
	{
		try
		{
			//Debug.Log(action.point.name);
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
			}
			coroutine = StartCoroutine(ActionRoutine(action));
		}
		catch
		{

		}
	}

	private IEnumerator ActionRoutine(NpcAction action)
	{
		yield return navigation.StartCoroutine(navigation.TraverseToPoint(action.point));
		if (action.animationTrigger != "" && !animator.GetBool("Walking"))
		{
			animator.SetTrigger(action.animationTrigger);
		}
		else
		{
			//Debug.Log("FallbackAnimation");
		}
	}
	#region Save System
	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
		SaveEvents.OnLoadEvent.AddListener(SyncDataPlaceholder);
	}

	private void SyncDataPlaceholder() { SyncData(new SavablePrefab()); }

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;

		saveManager.SaveFloat($"{npcName}currentAction",currentActionIndex);
	}

	public void SyncData(SavablePrefab data)
	{
		SaveManager saveManager = SaveManager.Instance;

		try
		{
			currentActionIndex = (int)saveManager.GetFloat($"{npcName}currentAction");
			Act(availableActions[currentActionIndex]);
		}
		catch
		{
			Debug.Log($"<color=yellow>Error during sync of npc with name {npcName}! K.Y.S.");
		}
	}
	#endregion
}