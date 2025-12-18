using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class LanguageManager : MonoBehaviour,ISavable
{
	public static LanguageManager Instance;
	private Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();
	private string path = Application.isEditor ? Application.dataPath + "/Resources/Lang" : Directory.GetCurrentDirectory() + "/Lang";

	[SerializeField] private string currentLanguage = "Русский.json";

	public string GetPath() => path;

	public List<string> GetLanguages() => languages.Keys.ToList();

	public string GetCurrentLanguage() => currentLanguage;

	private void SetupDirectory()
	{
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
	}

	private void Awake()
	{
		InitializeInstance();
		SubscribeToSaveEvent();
		SetupDirectory();
		Debug.Log("Слушаем UI");
		GameSetupEvents.OnUiGenerated.AddListener(ReadAllLangFiles);
		GameplayEvents.ChangeDimensionsEvent.AddListener(ApplyLanguage);
		DontDestroyOnLoad(this);
	}

	public void ChangeLanguage(string newLang)
	{
		RevertAllButtons();
		Debug.Log($"<color=green>Изменили язык на{newLang}");
		currentLanguage = newLang;
		ApplyLanguage();
	}

	private void RevertAllButtons()
	{
		List<TextMeshProUGUI> allLinesOnScene = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().ToList();
		Dictionary<string, string> languagePack = languages[currentLanguage];
		Debug.Log($"<color=yellow>Ревертаем ключи от языка {currentLanguage}");
		foreach (TextMeshProUGUI line in allLinesOnScene)
		{
			try
			{
				string key = languagePack.FirstOrDefault(x => x.Value == line.text).Key;
				if (key != null)
				{
					Debug.Log($"{key}");
					line.text = key;
				}
			}
			catch
			{
				Debug.Log($"<color=red>Не нашли строку с ключом{line.text!}");
			}
		}
	}

	private void InitializeInstance()
	{
		if (LanguageManager.Instance == null)
		{
			LanguageManager.Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void ApplyLanguage(int placeholder = 0)
	{
		List<TextMeshProUGUI> allLinesOnScene = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().ToList();
		Dictionary<string, string> languagePack = languages[currentLanguage];
		foreach (TextMeshProUGUI line in allLinesOnScene)
		{
			try
			{
				Debug.Log($"<color=green>Заменили ключь {line.text} на {languagePack[line.text]}");
				line.text = languagePack[line.text];
			}
			catch
			{
				//Debug.Log($"<color=red>Не нашли строку с ключом {line.text}!");
			}
		}
		GameSetupEvents.OnTranslationLoaded.Invoke();
	}

	private void ReadAllLangFiles()
	{
		
		List<string> languagePackNames = new List<string>();

		foreach (var n in Directory.GetFiles(path))
		{
			if (n.Contains(".meta")) continue;
			Debug.Log($"Нашли языковой пакет для языка {n.Split('/')[n.Split('/').Length - 1]}");
			languagePackNames.Add(n.Split('/')[n.Split('/').Length - 1]);
		}
		if (languagePackNames.Count == 0)
		{
			Debug.Log("<color=yellow>Не нашли ни одного языкового пакета! Генерируем шаблон...");
			CreateLanguageTemplate();
			foreach (var n in Directory.GetFiles(path))
			{
				if (n.Contains(".meta")) continue;
				Debug.Log($"Нашли языковой пакет для языка {n.Split('/')[n.Split('/').Length - 1]}");
				languagePackNames.Add(n.Split('/')[n.Split('/').Length - 1]);
			}
		}
		foreach (string languagePackName in languagePackNames)
		{
			try
			{
				Debug.Log($"Ищем пакет по пути {path + '/' + languagePackName.Split('\\')[1]}");
				LanguagePackModel newModel = JsonConvert.DeserializeObject<LanguagePackModel>(File.ReadAllText(path + '/' + languagePackName.Split('\\')[1]));
				languages.Add(languagePackName.Split('\\')[1], newModel.Lines);
				Debug.Log($"Добавили пакет с именем {languagePackName.Split('\\')[1]}");
			}
			catch (Exception e)
			{
				Debug.Log($"<color=red>ОШИБКА: </color><color=yellow>{e}!");
			}
		}
		ApplyLanguage();
	}

	public string GetTranslatable(string key)
	{
		try
		{
			string askedString = languages[currentLanguage][key];
			return askedString;
		}
		catch
		{
			Debug.Log($"<color=red>Не нашли переводимую строку с ключом</color><color=yellow> {key}!");
			return key;
		}
	}

	public void CreateLanguageTemplate()
	{
		Dictionary<string, string> templateLines = new Dictionary<string, string>
		{
			{ "ui.pause.continue", "Продолжить" },
			{ "ui.pause.exit", "Выход" },
			{ "ui.game.switchplanes", "Сменить Измерение" },
			{"ui.menu.start","Начать Игру" },
			{"ui.menu.exit","Выход Из Игры" },
			{"ui.menu.settings","Настройки Языка" },
			{"ui.settings.back","Назад" },
			{"ui.unit.actionpoints","Очки Действия" },
			{"ui.unit.health", "Здоровье" },
			{"ui.unit.morale","Мораль" },
			{"ui.hint.endturn","Окончить Ход" }
		};
		Dictionary<string, string> englishLines = new Dictionary<string, string>
		{
			{ "ui.pause.continue", "Continue" },
			{ "ui.pause.exit", "Exit" },
			{"ui.menu.start","Start Game" },
			{"ui.menu.exit","Leave To Desktop" },
			{"ui.menu.settings","Language Settings" },
			{"ui.settings.back","Back" },
			{"ui.unit.actionpoints","Action Points" },
			{"ui.unit.health", "Health" },
			{"ui.unit.morale","Morale" },
			{"ui.hint.endturn","End Turn" }
		};

		LanguagePackModel template = new LanguagePackModel()
		{
			Lines = templateLines
		};
		LanguagePackModel english = new LanguagePackModel()
		{
			Lines = englishLines
		};
		File.WriteAllText(path + "/Русский.json", JsonConvert.SerializeObject(template, Formatting.Indented));
		File.WriteAllText(path + "/English.json", JsonConvert.SerializeObject(english, Formatting.Indented));
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSettingsSaveEvent.AddListener(SaveData);
		SaveEvents.OnSettingsLoadEvent.AddListener(SyncDataEmpty);
	}

	private void SyncDataEmpty() { SyncData(new SavablePrefab()); }

	public void SaveData()
	{
		SaveManager.Instance.SaveStringOption("language",currentLanguage);
	}

	public void SyncData(SavablePrefab data)
	{
		currentLanguage = SaveManager.Instance.GetStringOption("language") == "" ? currentLanguage : SaveManager.Instance.GetStringOption("language");
	}
}
