using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour, ISavable
{
	[SerializeField] private GameObject taskObject;
	[SerializeField] private GameObject bookCanvas;
	[SerializeField] private Image taskImage;
	[SerializeField] private bool shouldShowTask = false;

	[SerializeField] private List<ColorTask> orderedTasks;

	public bool GetShowTask() => shouldShowTask;

	private void Awake()
	{
		SubscribeToSaveEvent();
	}

	public void ToggleBook()
	{
		bookCanvas.SetActive(!bookCanvas.activeSelf);

		GameManager.Instance.GetTutorial().ProgressTutorial(6);
		Time.timeScale = bookCanvas.activeSelf ? 0.0001f : 1f;
		Cursor.lockState = bookCanvas.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = bookCanvas.activeSelf;
		GameplayEvents.OnPauseToggled.Invoke(bookCanvas.activeSelf);
	}

	public void TakeTask(string npcName)
	{
		shouldShowTask = true;
		UpdateTask(npcName);
	}

	public void RemoveTask()
	{
		shouldShowTask = false;
		UpdateTask("");
	}

	public void UpdateTask(string npcName)
	{
		int index = GameManager.Instance.GetCurrentTaskIndex();
		GameplayEvents.OnWaterLevelChanged.Invoke();
		
		try
		{
			var objescts = Resources.FindObjectsOfTypeAll<NPCWaiter>();
			if (shouldShowTask)
			{
				Sprite taskImagery = objescts.Where(x => x.GetName()==npcName).ToList()[0].GetImage();
			    
				taskImage.sprite = taskImagery;
			}
		}
		catch { }
		taskObject.SetActive(shouldShowTask);
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
		SaveEvents.OnLoadEvent.AddListener(SyncDataEmpty);
	}

	private void SyncDataEmpty() { SyncData(new SavablePrefab()); }

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;
		saveManager.SaveFloat("shouldShowTask", shouldShowTask ? 1f : 0f);
	}

	public void SyncData(SavablePrefab data)
	{
		SaveManager saveManager = SaveManager.Instance;
		shouldShowTask = saveManager.GetFloat("shouldShowTask") == 1f;
	}
}