using System;
using System.Collections;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
	[SerializeField] private GameObject prefab;

	[SerializeField] private SpawnMode mode;

	private IEnumerator SpawnObjects()
	{
		int amount = 0;
		switch (mode)
		{
			case SpawnMode.Logs:
				amount = PocketTicker.Instance.GetLogs();
				PocketTicker.Instance.ClearLogs();
				SaveManager.Instance.SaveFloat("logs",0);
				Debug.Log($"Will spawn {amount} logs");
				break;
			case SpawnMode.Seeds:
				amount = PocketTicker.Instance.GetSeeds();
				PocketTicker.Instance.ClearSeeds();
				SaveManager.Instance.SaveFloat("seeds", 0);
				Debug.Log($"Will spawn {amount} seeds");
				break;
		}
		for (int i = 0; i < amount; i++)
		{
			Instantiate(prefab, transform.position, Quaternion.identity);
			yield return new WaitForSeconds(0.3f);
		}
	}

	public void Awake()
	{
		StartCoroutine(SpawnObjects());
	}

	[Serializable]
	private enum SpawnMode
	{
		Logs,
		Seeds
	}
}
