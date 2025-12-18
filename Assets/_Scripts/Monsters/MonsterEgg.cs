using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterEgg : MonoBehaviour, ISavable
{
	[SerializeField] private GameObject monsterInside;
	[SerializeField] private float timeToHatch;

	public void SaveData()
	{
		SavablePrefab pref = new SavablePrefab
		{
			prefabName = $"monsterEgg{monsterInside.name}",
			dimension = SceneManager.GetActiveScene().buildIndex,
			worldPosition = Mapper.VectorToFloatData(transform.position), //new List<float> {transform.position.x,transform.position.y,transform.position.z},
			quaternionRotation = Mapper.QuaternionToFloatData(transform.rotation),//new List<float> { transform.rotation.x,transform.rotation.y,transform.rotation.z, transform.rotation.w },
			floatData = new Dictionary<string, float> { { "timeToHatch",timeToHatch} }
		};
		SaveManager.Instance.SavePrefab(pref);
	}

	private IEnumerator TickHatch()
	{
	    while (timeToHatch > 0)
		{
			yield return new WaitForSeconds(2);
			timeToHatch -= PocketTicker.Instance.GetTicksForCalculations();
		}
		Instantiate(monsterInside,transform.position,Quaternion.identity);
		
		Destroy(gameObject);
	}

	private void Start()
	{
		SubscribeToSaveEvent();
		if (SceneManager.GetActiveScene().buildIndex == 2)
		{
			StartCoroutine(TickHatch());
		}
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