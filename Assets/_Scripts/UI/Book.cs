using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour, ISavable
{
	[SerializeField] private GameObject taskObject;
	[SerializeField] private GameObject bookCanvas;
	[SerializeField] private Image taskImage;
	[SerializeField] private TextMeshProUGUI taskHeader;
	[SerializeField] private TextMeshProUGUI taskText;
	[SerializeField] private bool shouldShowTask = false;

	[SerializeField] private List<ColorTask> orderedTasks;

	public bool GetShowTask() => shouldShowTask;

	public bool IsBookOpen() => bookCanvas.activeSelf;

	private void Awake()
	{
		SubscribeToSaveEvent();
	}

	public void CloseBook()
	{
		bookCanvas.SetActive(false);

		//GameManager.Instance.GetTutorial().ProgressTutorial(6);
		Time.timeScale = bookCanvas.activeSelf ? 0.0001f : 1f;
		Cursor.lockState = bookCanvas.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = bookCanvas.activeSelf;
		GameplayEvents.OnPauseToggled.Invoke(bookCanvas.activeSelf);
	}

	public void ToggleBook()
	{
		bookCanvas.SetActive(!bookCanvas.activeSelf);

		//GameManager.Instance.GetTutorial().ProgressTutorial(6);
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
		taskHeader.text = "";
		taskText.text = "";
		LanguageManager languageManager = LanguageManager.Instance;
		try
		{
			if (shouldShowTask)
			{
				Sprite taskImagery = orderedTasks.Where(x => x.npcName==npcName).ToList()[0].image;
			    
				taskImage.sprite = taskImagery;
				taskHeader.text = $"{languageManager.GetTranslatable("book.task.header")}: <mark>{languageManager.GetTranslatable("npc."+npcName+".name")}";
				taskText.text = $"{languageManager.GetTranslatable("book.task.asked")}: {languageManager.GetTranslatable("npc."+npcName+".task")}";
			}
		}
		catch (Exception e){ Debug.Log(e); }
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