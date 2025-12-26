using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour, ISavable
{
	[SerializeField] private List<GameObject> questLinesByIndex;
	[SerializeField] private List<GameObject> additionalTutorials;
	[SerializeField] private GameObject tutorialIndicator;

	private bool shouldShowTutorial;
	private int tutorialIndex;

	public void SetShouldShowTutorial(bool shouldShowTutorial) { this.shouldShowTutorial = shouldShowTutorial; }
	public bool GetShouldShowTutorial() => shouldShowTutorial;

	public void SetTutorialIndex(int index) { tutorialIndex = index; }
	public int GetTutorialIndex() => tutorialIndex;
	private bool hasPickedUpMonster = false;

	public void Start()
	{
		SubscribeToSaveEvent();
	}

	public void Update()
	{
		HandleInput();
	}

	public void HandleInput()
	{

		if (Input.GetKeyDown(KeyCode.T))
		{
			if (SceneManager.GetActiveScene().buildIndex == 0)
			{
				HideAllTutorials();
				return;
			}
			if (questLinesByIndex[tutorialIndex].activeSelf)
			{
				HideAllTutorials();
				ProgressTutorial(12);
			}
			else
			{
				ShowTutorialByIndex(tutorialIndex);
			}
		}
	}

	public void ShowTutorialByIndex(int index)
	{
		HideAllTutorials();
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			return;
		}
		try
		{
			questLinesByIndex[index].SetActive(true);
		}
		catch
		{
			tutorialIndicator.SetActive(false);
			HideAllTutorials();
		};
	}

	public void HideAllTutorials()
	{
		questLinesByIndex.ForEach(list => list.SetActive(false));
		additionalTutorials.ForEach(list => list.SetActive(false));
	}

	public void ProgressTutorial(int index)
	{
		if (index != tutorialIndex) return;
		tutorialIndex++;
		if (tutorialIndex >= questLinesByIndex.Count)
		{
			tutorialIndicator.SetActive(false);
			HideAllTutorials();
		}
		else
		{
			if (questLinesByIndex[Mathf.Clamp(tutorialIndex - 1, 0, 999)].activeSelf)
			{
				ShowTutorialByIndex(tutorialIndex);
			}
			else
			{
				HideAllTutorials();
			}
		}
		TutorialEvents.OnTutorialIndexChanged.Invoke(tutorialIndex);
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
		SaveEvents.OnLoadEvent.AddListener(SyncDataEmpty);
		TutorialEvents.OnAdditionalTutorialTriggered.AddListener(ShowAdditionalTutorial);
	}

	private void ShowAdditionalTutorial(int id)
	{
		if (tutorialIndex >= questLinesByIndex.Count) return;
		HideAllTutorials();
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			return;
		}
		try
		{
			if (id == 0)
			{
				if (hasPickedUpMonster) return;
				hasPickedUpMonster = true;
			}
			additionalTutorials[id].SetActive(true);
		}
		catch
		{
			tutorialIndicator.SetActive(false);
			HideAllTutorials();
		};

	}

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;

		saveManager.SaveFloat("shouldShowTutorial", shouldShowTutorial ? 1f : 0f);
		saveManager.SaveFloat("tutorialIndex", tutorialIndex);

		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			HideAllTutorials();
		}
	}

	private void SyncDataEmpty() { SyncData(new SavablePrefab()); }

	public void SyncData(SavablePrefab data)
	{
		SaveManager saveManager = SaveManager.Instance;

		shouldShowTutorial = saveManager.GetFloat("shouldShowTutorial") == 1f;
		tutorialIndex = (int)saveManager.GetFloat("tutorialIndex");
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			HideAllTutorials();
		}
		ShowTutorialByIndex(tutorialIndex);
	}
}
