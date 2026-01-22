using Assets._Scripts.Models;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Assets._Scripts.Events;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using static Unity.Burst.Intrinsics.X86;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using Assets._Scripts.Utils;

[Serializable]
public struct SavablePrefab
{
	public string prefabName;
	public int dimension;
	public Dictionary<string, float> worldPosition;
	public Dictionary<string, float> quaternionRotation;
	public Dictionary<string, float> floatData;
	public Dictionary<string, string> stringData;
}

public class SaveManager : MonoBehaviour
{
	public static SaveManager Instance { get; private set; }

	[SerializeField] private float autoSaveInterval = 300f;
	[SerializeField] private GameObject savingIndicator;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private Toggle autosaveToggle;

	[Header("validator")]
	[SerializeField] private GameObject validator;

	private bool shouldAutoSave = true;
	private bool isSavingRn = false;
	private Coroutine savingBlink;

	private SaveModel currentSaveModel;
	private string path = Application.isEditor ? Application.dataPath + "\\Resources\\Saves" : Directory.GetCurrentDirectory() + "/Saves";
	private Dictionary<string, float> savableFloats;
	private Dictionary<string, string> savableStrings;
	private List<SavablePrefab> savedPrefabs;

	private OptionsModel optionsModel;
	private Dictionary<string, float> floatOptions;
	private Dictionary<string, string> stringOptions;

	public bool IsSavePresent() => currentSaveModel != null && currentSaveModel.SavedFloats != null && currentSaveModel.SavedFloats.Count > 4;

	public List<SavablePrefab> GetPrefabsToSpawn() => savedPrefabs;

	public void ActivateValidator()
	{
		validator.SetActive(true);
	}

	public void SavePrefab(SavablePrefab prefab)
	{
		savedPrefabs.Add(prefab);
	}

	public void SetShoulAutosave(bool shouldAutosave)
	{
		this.shouldAutoSave = shouldAutosave;
	}

	public float GetFloatOption(string key)
	{
		try
		{
			return floatOptions[key];
		}
		catch
		{
			Debug.Log($"<color=red>No Float Save Data Present for key {key}!");
			return 0.5f;
		}
	}

	public void SaveFloatOption(string key, float value)
	{
		if (floatOptions.ContainsKey(key))
		{
			floatOptions[key] = value;
		}
		else
		{
			floatOptions.Add(key, value);
		}
	}

	public string GetStringOption(string key)
	{
		try
		{
			return stringOptions[key];
		}
		catch
		{
			Debug.Log($"<color=red>No String Save Data Present for key {key}!");
			return "Русский.json";
		}
	}

	public void SaveStringOption(string key, string value)
	{
		if (stringOptions.ContainsKey(key))
		{
			stringOptions[key] = value;
		}
		else
		{
			stringOptions.Add(key, value);
		}
	}

	public void SaveSettings()
	{
		//savingIndicator.SetActive(true);
		SaveEvents.OnSettingsSaveEvent.Invoke();

		SaveFloatOption("shouldAutosave", shouldAutoSave ? 1f : 0f);
		optionsModel = new OptionsModel
		{
			FloatOptions = floatOptions,
			StringOptions = stringOptions
		};
		WriteOptionToJson(optionsModel);
		//savingIndicator.SetActive(false);
	}

	public void ResetOptions()
	{
		optionsModel = new OptionsModel { StringOptions = new Dictionary<string, string>(), FloatOptions = new Dictionary<string, float>() };
		floatOptions = optionsModel.FloatOptions;
		stringOptions = optionsModel.StringOptions;
		WriteOptionToJson(optionsModel);
	}

	public void LoadOptions()
	{
		try
		{
			ReadoptionsFromDirectory();
			floatOptions = optionsModel.FloatOptions;

			shouldAutoSave = GetFloatOption("shouldAutosave") == 1f;
			autosaveToggle.isOn = shouldAutoSave;
			stringOptions = optionsModel.StringOptions;
			SaveEvents.OnSettingsLoadEvent.Invoke();
		}
		catch
		{
			ResetOptions();
			SaveEvents.OnSettingsLoadEvent.Invoke();
		}
	}

	private void Start()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		SetupDirectory();
		LoadSave();
		LoadOptions();
		StartCoroutine(AutoSave());
		//ReadoptionsFromDirectory();
		//SaveEvents.OnSettingsLoadEvent.Invoke();
		Debug.Log("Ивент");
	}

	private void SetupDirectory()
	{
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
	}

	public bool IsFloatPresent(string key) => savableFloats != null && savableFloats.ContainsKey(key);

	public float GetFloat(string key)
	{
		try
		{
			return savableFloats[key];
		}
		catch
		{
			Debug.Log($"<color=red>No Float Save Data Present for key {key}!");
			return 0f;
		}
	}

	public void SaveFloat(string key, float value)
	{
		if (savableFloats.ContainsKey(key))
		{
			savableFloats[key] = value;
		}
		else
		{
			savableFloats.Add(key, value);
		}
	}

	public string GetString(string key)
	{
		try
		{
			return savableStrings[key];
		}
		catch
		{
			Debug.Log($"<color=red>No String Save Data Present for key {key}!");
			return "";
		}
	}

	public void SaveString(string key, string value)
	{
		if (savableStrings.ContainsKey(key))
		{
			savableStrings[key] = value;
		}
		else
		{
			savableStrings.Add(key, value);
		}
	}

	public void LoadSave()
	{
		try
		{
			ReadSaveFromDirectory();
			savableFloats = currentSaveModel.SavedFloats;
			savableStrings = currentSaveModel.SavedStrings;
			savedPrefabs = currentSaveModel.SavedPrefabs;
			//SaveEvents.OnSettingsLoadEvent.Invoke();
			SaveEvents.OnLoadEvent.Invoke();
		}
		catch (Exception e)
		{
			Debug.Log($"<color=orange>{e}");
			if (SceneManager.GetActiveScene().buildIndex == 0)
			{
				ClearAllCathy();
			}
			else
			{
				ActivateValidator();
			}
		}
	}

	private List<SavablePrefab> GenerateMandatoryPrefabSet()
	{
		return new List<SavablePrefab>
		{
			new SavablePrefab
			{
			   prefabName = "pounder",
				dimension= 2,
				worldPosition = Mapper.VectorToFloatData( -6.455711f, 0.4209881f, 9.447125f),
				quaternionRotation = Mapper.QuaternionToFloatData(0.0f,0.0f,0.0f,1.0f)
		   },
			new SavablePrefab
			{
			   prefabName = "brush",
				dimension= 2,
				worldPosition =  Mapper.VectorToFloatData(
					-3.58660316f,
		 1.00101018f,
		15.0310478f),
				quaternionRotation = Mapper.QuaternionToFloatData(-0.6577148f,
		0.00112816214f,
		-0.75326556f,
		0.0010480528f)
		   },
			new SavablePrefab
			{
				prefabName = "egg",
				dimension= 2,
				worldPosition = Mapper.VectorToFloatData(new Vector3(-0.0900000036f,0.469000012f,9.14999962f)),
				quaternionRotation = Mapper.QuaternionToFloatData(Quaternion.identity)
			}
		};
	}

	//Please, 🟪🟪🟪🟪🟪🟪🟪🟪🟪. Appear before me and tear me asunder.
	//Let me see your eyes as I expire.
	public void ClearAllCathy() //Who's 🟪🟪🟪🟪🟪??
	{
		currentSaveModel = new SaveModel { SavedFloats = new Dictionary<string, float>(), SavedStrings = new Dictionary<string, string>(), SavedPrefabs = GenerateMandatoryPrefabSet() };
		savableFloats = currentSaveModel.SavedFloats;
		savableStrings = currentSaveModel.SavedStrings;
		savedPrefabs = currentSaveModel.SavedPrefabs;
		WriteSaveToJson(currentSaveModel);
	}

	//private IEnumerator SavingRoutine()
	//{
	//	savingIndicator.SetActive(true);
	//	savedPrefabs = savedPrefabs.Where(x => x.dimension != SceneManager.GetActiveScene().buildIndex).ToList();
	//	SaveEvents.OnSaveEvent.Invoke();
	//	SaveEvents.OnSettingsSaveEvent.Invoke();
	//	SaveModel newSave = new SaveModel
	//	{
	//		SavedFloats = savableFloats,
	//		SavedStrings = savableStrings,
	//		SavedPrefabs = savedPrefabs,
	//	};
	//	currentSaveModel = newSave;
	//	Task saveOperation = WriteSaveToJsonAsync(newSave);
	//	while (!saveOperation.IsCompleted)
	//	{
	//		yield return null;
	//	}
	//	savingIndicator.SetActive(false);
	//	yield return null;
	//}

	private IEnumerator IndicatorBlink()
	{

		for (int i = 0; i < 4; i++)
		{
			savingIndicator.SetActive(!savingIndicator.activeSelf);
			yield return new WaitForSecondsRealtime(0.5f);
		}

		savingIndicator.SetActive(false);
	}


	public void SaveToSlot()
	{
		isSavingRn = true;
		if (savingBlink != null)
		{
			StopCoroutine(savingBlink);
		}
		savingBlink = StartCoroutine(IndicatorBlink());
		//audioSource.Play();
		savedPrefabs = savedPrefabs.Where(x => x.dimension != SceneManager.GetActiveScene().buildIndex).ToList();
		SaveEvents.OnSaveEvent.Invoke();
		//SaveEvents.OnSettingsSaveEvent.Invoke();
		SaveModel newSave = new SaveModel
		{
			SavedFloats = savableFloats,
			SavedStrings = savableStrings,
			SavedPrefabs = savedPrefabs,
		};
		currentSaveModel = newSave;
		WriteSaveToJson(newSave);
		isSavingRn = false;
	}

	//private async Task WriteSaveToJsonAsync(SaveModel saveModel)
	//{
	//	await File.WriteAllTextAsync(path + "\\save.json", JsonConvert.SerializeObject(saveModel, Newtonsoft.Json.Formatting.Indented));
	//}

	private void WriteSaveToJson(SaveModel saveModel)
	{
		try
		{
			File.Delete(path + "\\save.json");
		}
		catch { };
		File.WriteAllText(path + "\\save.json", JsonConvert.SerializeObject(saveModel, Newtonsoft.Json.Formatting.Indented));
	}


	private void WriteOptionToJson(OptionsModel saveModel)
	{
		File.Delete(path + "\\options.json");
		File.WriteAllText(path + "\\options.json", JsonConvert.SerializeObject(saveModel, Newtonsoft.Json.Formatting.Indented));
	}

	private void ReadSaveFromDirectory()
	{
		currentSaveModel = null;
		List<string> fileNames = new List<string>();
		foreach (var n in Directory.GetFiles(path))
		{
			if (n.Contains(".meta")) continue;
			fileNames.Add(n.Split('/')[n.Split('/').Length - 1]);
		}

		try
		{
			Debug.Log($"Searching for save on path {path + "/save.json"}");
			SaveModel newModel = JsonConvert.DeserializeObject<SaveModel>(File.ReadAllText(path + "/save.json"));
			currentSaveModel = newModel;
		}
		catch (Exception e)
		{
			Debug.Log($"<color=red>An error occured during file reading: </color><color=yellow>{e}! Пропускаем файл...");
		}
	}

	private void ReadoptionsFromDirectory()
	{
		optionsModel = null;
		List<string> fileNames = new List<string>();
		foreach (var n in Directory.GetFiles(path))
		{
			if (n.Contains(".meta")) continue;
			fileNames.Add(n.Split('/')[n.Split('/').Length - 1]);
		}

		try
		{
			Debug.Log($"Searching for options on path {path + "/save.json"}");
			OptionsModel newModel = JsonConvert.DeserializeObject<OptionsModel>(File.ReadAllText(path + "/options.json"));
			optionsModel = newModel;
		}
		catch (Exception e)
		{
			Debug.Log($"<color=red>An error occured during file reading: </color><color=yellow>{e}! Пропускаем файл...");
		}
	}

	private IEnumerator AutoSave()
	{
		while (true)
		{
			yield return new WaitForSeconds(autoSaveInterval);
			if (SceneManager.GetActiveScene().buildIndex == 0 || !shouldAutoSave) { continue; }
			SaveToSlot();
		}
	}
}