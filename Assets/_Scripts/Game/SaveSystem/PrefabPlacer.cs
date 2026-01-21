
using Assets._Scripts.Game.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabPlacer : MonoBehaviour
{
	[Serializable]
	public struct SpawnablePrefab
	{
		public string Name;
		public GameObject Prefab;
	}

	[SerializeField] private int dimensionIndex;
	[SerializeField] private List<SpawnablePrefab> prefabList = new List<SpawnablePrefab>();

	public void Start()
	{
		SaveManager saveManager = SaveManager.Instance;

		if (!saveManager.IsSavePresent()) return;

		List<SavablePrefab> prefabsToSpawn = saveManager.GetPrefabsToSpawn().Where(prefab => prefab.dimension == dimensionIndex).ToList();
		if (prefabsToSpawn.Count == 0) { return; }
		foreach (var prefab in prefabsToSpawn)
		{
			try
			{
				Debug.Log($"<color=yellow>Trying to spawn </color><color=green>{prefab.prefabName}</color>...");
				GameObject spawnedPrefab = Instantiate(prefabList.First(x => x.Name == prefab.prefabName).Prefab,
					new Vector3(prefab.worldPosition["x"], prefab.worldPosition["y"], prefab.worldPosition["z"]),
					new Quaternion(prefab.quaternionRotation["x"], prefab.quaternionRotation["y"], prefab.quaternionRotation["z"], prefab.quaternionRotation["w"]));
				spawnedPrefab.GetComponent<ISavable>().SyncData(prefab);
			}
			catch (Exception ex)
			{
				Debug.Log("<color=red>ERROR DURING PREFAB PLACEMENT!");
				Debug.Log(ex);
			}
		}
	}
}
