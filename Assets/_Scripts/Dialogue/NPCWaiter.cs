using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ColorTask
{
	public int id;
	public Color color;
	public Sprite image;
	public string npcName;
}

public class NPCWaiter : MonoBehaviour, ISavable
{
	[SerializeField] private string npcName;
	[SerializeField] private Animator npcAnimator;
	[SerializeField] private List<ColorTask> tasks;
	[SerializeField] private List<GameObject> houseToPaint;

	private bool isPlayerInTrigger = false;
	private bool gaveTask = false;
	private bool isCompleted = false;
	private bool wasIntroduced = false;

	private CameraController cameraController;
	private PlayerController playerController;

	private Bucket bucketInTrigger;

	public bool GetWasIntroduced() => wasIntroduced;
	public void SetWasIntroduced() { wasIntroduced = true; }

	public string GetName() => npcName;

	public List<ColorTask> GetColorTasks() => tasks;

	public void SetGaveTask(bool gaveTask)
	{
		this.gaveTask = gaveTask;
	}
	public bool GetGaveTask() => gaveTask;

	public void SetCompleted(bool isCompleted)
	{
		this.isCompleted = isCompleted;
	}

	public bool GetIsCompleted() => isCompleted;

	public bool ShouldShowOnLoad() => gaveTask && !isCompleted;

	public Sprite GetImage() => tasks[0].image;

	public void PaintHouse()
	{
		houseToPaint.ForEach(house =>
		{
			try
			{
				var children = house.GetComponentsInChildren<Transform>(includeInactive: true);
				foreach (var child in children)
				{
					child.gameObject.layer = 3;
				}
			}
			catch
			{

			}
		});
	}

	public void Awake()
	{
		SubscribeToSaveEvent();
		GameplayEvents.OnMount.AddListener(DropPlayer);
		playerController = GameObject.FindFirstObjectByType<PlayerController>();
		cameraController = GameObject.FindFirstObjectByType<CameraController>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
		{
			if (other.tag == "Bucket")
			{
				bucketInTrigger = other.GetComponent<Bucket>();
			}
			return;
		}
		int currentTaskIndex = GameManager.Instance.GetCurrentTaskIndex();

		//if (tasks.Where(x => x.id == currentTaskIndex).ToList().Count > 0)
		//{
		npcAnimator.SetTrigger("Wave");
		isPlayerInTrigger = true;
		//}
		//else
		//{
		//	npcAnimator.SetTrigger("Idk");
		//}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Bucket")
		{
			bucketInTrigger = null;
		}
		if (other.tag == "Player")
		{
			DropPlayer();
		}
	}

	private void DropPlayer()
	{
		isPlayerInTrigger = false;
		GameManager.Instance.DisruptDialogue();
	}

	private void HandleInput()
	{
		if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
		{
			GameManager.Instance.StartDialogueForCurrentIndex(this, tasks[0].color, bucketInTrigger);
			cameraController.SetShouldRotate(false);
			playerController.SetCanWalk(false);
		}
	}

	public void Update()
	{
		HandleInput();
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
		SaveEvents.OnLoadEvent.AddListener(SyncDataEmpty);
	}

	private void SyncDataEmpty() { SyncData(new SavablePrefab()); }

	public void OnDestroy()
	{
		SaveEvents.OnSaveEvent.RemoveListener(SaveData);
		SaveEvents.OnLoadEvent.RemoveListener(SyncDataEmpty);
		GameplayEvents.OnMount.RemoveListener(DropPlayer);
	}

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;
		saveManager.SaveFloat(npcName, gaveTask ? 1 : 0);
		saveManager.SaveFloat(npcName + "Completed", isCompleted ? 1 : 0);
		saveManager.SaveFloat(npcName + "Introduced", wasIntroduced ? 1 : 0);
	}

	public void SyncData(SavablePrefab data)
	{
		gaveTask = SaveManager.Instance.GetFloat(npcName) == 1f;
		isCompleted = SaveManager.Instance.GetFloat(npcName + "Completed") == 1f;
		wasIntroduced = SaveManager.Instance.GetFloat(npcName + "Introduced") == 1f;
		if (isCompleted)
		{
			PaintHouse();
		}
	}
}