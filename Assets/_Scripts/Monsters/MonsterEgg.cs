using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Monsters;
using Assets._Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MonsterEgg : MonoBehaviour, ISavable
{
	[SerializeField] private GameObject monsterInside;
	[SerializeField] private float timeToHatch;
	[SerializeField] private GameObject monsterNameGui;
	[SerializeField] private TextMeshProUGUI inputFieldText;

	private Coroutine ticker;

	public void SaveData()
	{
		SavablePrefab pref = new SavablePrefab
		{
			prefabName = $"monsterEgg{monsterInside.name}",
			dimension = SceneManager.GetActiveScene().buildIndex,
			worldPosition = Mapper.VectorToFloatData(transform.position), //new List<float> {transform.position.x,transform.position.y,transform.position.z},
			quaternionRotation = Mapper.QuaternionToFloatData(transform.rotation),//new List<float> { transform.rotation.x,transform.rotation.y,transform.rotation.z, transform.rotation.w },
			floatData = new Dictionary<string, float> { { "timeToHatch", timeToHatch } }
		};
		SaveManager.Instance.SavePrefab(pref);
	}

	public void SetMonsterInside(GameObject monsterInside)
	{
		this.monsterInside = monsterInside;
	}

	public void StartTickingInFence()
	{
		if (ticker != null)
		{
			return;
		}
	    
		ticker = StartCoroutine(TickHatch());
	}

	public void StopTicking()
	{
		StopCoroutine(ticker);
		ticker = null;
	}

	private IEnumerator TickHatch()
	{
		while (timeToHatch > 0)
		{
			yield return new WaitForSeconds(2);
			timeToHatch -= PocketTicker.Instance.GetTicksForCalculations();
		}
	}

	public void SetNameFromInput()
	{
		SpawnMonster(inputFieldText.text);
	}

	public void SetRandomName()
	{
		SpawnMonster("");
	}

	public void SpawnMonster(string name)
	{
		GameObject newMonster = Instantiate(monsterInside, transform.position, Quaternion.identity);
		newMonster.GetComponent<PigmentMonster>().Initialize(name);
		if (name != "")
		{
			newMonster.GetComponentInChildren<MonsterNameTag>().SetNameTag(name);
		}
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		Destroy(gameObject);
	}

	public void TryOpeningHatchMenu()
	{
		if (timeToHatch <= 0f)
		{
			monsterNameGui.SetActive(true);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void Start()
	{
		SubscribeToSaveEvent();
		//if (SceneManager.GetActiveScene().buildIndex == 2)
		//{
		//	StartCoroutine(TickHatch());
		//}
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
	}

	public void OnDestroy()
	{
		SaveEvents.OnSaveEvent.RemoveListener(SaveData);
	}

	public void SyncData(SavablePrefab data)
	{
		timeToHatch = data.floatData["timeToHatch"];
	}
}