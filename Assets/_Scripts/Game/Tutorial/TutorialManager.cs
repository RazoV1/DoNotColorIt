using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour, ISavable
{
	[SerializeField] private List<QuestLine> questLinesByIndex;
	[SerializeField] private List<GameObject> additionalTutorials;
	[SerializeField] private GameObject tutorialIndicator;
	[SerializeField] private TextMeshProUGUI tutorialOutput;
	[SerializeField] private Animator taskanim;

	private List<int> shownMonsterIndexex = new List<int>();

	private bool shouldShowTutorial;
	private int tutorialIndex;

	public void SetShouldShowTutorial(bool shouldShowTutorial) { this.shouldShowTutorial = shouldShowTutorial; }
	public bool GetShouldShowTutorial() => shouldShowTutorial;

	public void SetTutorialIndex(int index) { tutorialIndex = index; }
	public int GetTutorialIndex() => tutorialIndex;
	private bool hasPickedUpMonster = false;

	[Serializable]
	public struct QuestLine
	{
		public string tutorialName;
		/// <summary>
		/// ÒÓÒ ÏÅĞÅÄÀ¨Ì ÑÏÈÑÎÊ ÎÁÚÅÊÒÎÂ ÄËß ÏÎÊÀÇÀ, ÂÊËŞ×Àß ÏËÀØÊÓ Ñ ÒÅÊÑÒÎÌ ÒÓÒÎĞÈÀËÀ
		/// </summary>
		public List<GameObject> toShow;
	}

	public void Start()
	{
		SubscribeToSaveEvent();
	}

	public void Update()
	{
		HandleInput();
	}

	private void SetVisibleForLine(QuestLine line, bool visibility)
	{
		line.toShow.ForEach(x => x.SetActive(visibility));
	}

	private bool IsVisibleLine(QuestLine line) => line.toShow.All(x => x.activeSelf);

	public void HandleInput()
	{

		//if (Input.GetKeyDown(KeyCode.T))
		//{
		//	try
		//	{
		//		if (SceneManager.GetActiveScene().buildIndex == 0)
		//		{
		//			HideAllTutorials();
		//			return;
		//		}
		//		if (IsVisibleLine(questLinesByIndex[0]))
		//		{
		//			HideAllTutorials();
		//			ProgressTutorial("last");
		//		}
		//		else
		//		{
		//			ShowTutorialByIndex(tutorialIndex);
		//		}
		//	}
		//	catch { HideAllTutorials(); }
		//}
	}

	public void ShowTutorialByIndex(int index)
	{
		HideAllTutorials();
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			return;
		}
		if (index >= questLinesByIndex.Count - 1)
		{
			tutorialIndicator.SetActive(false);
			HideAllTutorials();
			return;
		}
		try
		{
			SetVisibleForLine(questLinesByIndex[index], true);
			Reload();
			TutorialEvents.OnTutorialIndexChanged.Invoke(questLinesByIndex[index].tutorialName);
		}
		catch
		{
			tutorialIndicator.SetActive(false);
			HideAllTutorials();
		};
	}

	public void HideAllTutorials()
	{
		questLinesByIndex.ForEach(list => SetVisibleForLine(list, false));
		//foreach (var additionalTutorial in additionalTutorials)
		//{
		//	int id = additionalTutorials.IndexOf(additionalTutorial);
		//	if (id < 4)
		//	{
		//		additionalTutorial.SetActive(shownMonsterIndexex.Contains(id));
		//		continue;
		//	}
		//	additionalTutorial.SetActive(false);
		//}
	}

	public void HideAllAddTutorials()
	{
		foreach (var additionalTutorial in additionalTutorials)
		{
			int id = additionalTutorials.IndexOf(additionalTutorial);
			if (id < 4)
			{
				additionalTutorial.SetActive(shownMonsterIndexex.Contains(id));
				continue;
			}
			additionalTutorial.SetActive(false);
		}
	}

	public void ProgressTutorial(string lineName)
	{
		try
		{
			if (lineName != questLinesByIndex[tutorialIndex].tutorialName) return;
		}
		catch
		{
			return;
		}
		tutorialIndex++;
		taskanim.SetTrigger("IsNewTask");
		Debug.Log(tutorialIndex);
		if (tutorialIndex >= questLinesByIndex.Count - 1 || (questLinesByIndex[tutorialIndex].tutorialName == "talkLeo" && tutorialIndex >= questLinesByIndex.Count - 1))
		{
			tutorialIndicator.SetActive(false);
			HideAllTutorials();
		}
		else
		{
			if (IsVisibleLine(questLinesByIndex[Mathf.Clamp(tutorialIndex - 1, 0, 999)]))
			{
				ShowTutorialByIndex(tutorialIndex);

			}
			else
			{
				HideAllTutorials();
			}
		}
		if (tutorialIndex < questLinesByIndex.Count)
			TutorialEvents.OnTutorialIndexChanged.Invoke(questLinesByIndex[tutorialIndex].tutorialName);
		taskanim.SetTrigger("IsNewTask");
		Debug.Log("IsNewTask");
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
		SaveEvents.OnLoadEvent.AddListener(SyncDataEmpty);
		GameSetupEvents.OnTranslationLoaded.AddListener(Reload);
		TutorialEvents.OnAdditionalTutorialTriggered.AddListener(ShowAdditionalTutorial);
		CommandEvents.OnTutorialSkip.AddListener(SkipTutorial);
	}

	private void Reload()
	{
		try
		{
			string currentTutorialString = LanguageManager.Instance.GetTranslatable("tutorial.output." + questLinesByIndex[tutorialIndex].tutorialName);
			tutorialOutput.text = currentTutorialString;
		}
		catch
		{
			Debug.Log("<color=red>Îøèáêà âî âğåìÿ çàãğóçêè ïåğåâîäà òóòîğèàëà!");
		}
	}

	private void SkipTutorial(int placeholder)
	{
		HideAllTutorials();
		tutorialIndex = questLinesByIndex.Count - 1;
		ProgressTutorial("talkLeo");
	}

	private void ShowAdditionalTutorial(int id)
	{
		HideAllAddTutorials();
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			return;
		}
		try
		{
			additionalTutorials[id].SetActive(true);
			if (id < 4 && !shownMonsterIndexex.Contains(id))
			{
				shownMonsterIndexex.Add(id);
			}
		}
		catch
		{
			tutorialIndicator.SetActive(false);
			HideAllAddTutorials();
		};

	}

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;

		saveManager.SaveFloat("shouldShowTutorial", shouldShowTutorial ? 1f : 0f);
		saveManager.SaveFloat("tutorialIndex", tutorialIndex);

		string monsterCahce = "";
		for (int i = 0; i < shownMonsterIndexex.Count; i++)
		{
			monsterCahce += $" {shownMonsterIndexex[i]}";
		}
		saveManager.SaveString("shownMonsterIndexex", monsterCahce);

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

		try
		{
			string str = saveManager.GetString("shownMonsterIndexex");
			Debug.Log(str);
			List<string> monsterStr = str.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
			foreach (var ds in monsterStr)
			{
				Debug.Log(ds);
			}
			List<int> monsterCache = monsterStr.Select(x => int.Parse(x)).ToList();
			Debug.Log(monsterCache[0]);
			shownMonsterIndexex = monsterCache;
			HideAllAddTutorials();
		}
		catch (Exception e)
		{
			shownMonsterIndexex = new List<int>();
			Debug.Log($"<color=red>{e}");
		}
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			HideAllTutorials();
			HideAllAddTutorials();
		}
		ShowTutorialByIndex(tutorialIndex);
	}
}
