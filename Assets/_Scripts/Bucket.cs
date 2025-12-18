using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bucket : MonoBehaviour, ISavable
{
	[SerializeField] private Material materialPrefab;

	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private Color color;

	public Color GetColor() => color;

	private void Start()
	{
		SubscribeToSaveEvent();
	}

	public bool IsFilled() => meshRenderer.gameObject.activeSelf;

	public void Fill(Color color)
	{
		this.color = color;
		Material newMat = Instantiate(materialPrefab);
		meshRenderer.gameObject.SetActive(true);
		newMat.color = color;
		meshRenderer.material = newMat;
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
	}

	public void SaveData()
	{
		if (!IsFilled()) return;

		SaveManager saveManager = SaveManager.Instance;

		SavablePrefab save = new SavablePrefab
		{
			prefabName = "filledBucket",
			floatData = Mapper.ColorToFloatData(color),
			dimension = SceneManager.GetActiveScene().buildIndex,
			worldPosition = Mapper.VectorToFloatData(transform.position),
			quaternionRotation = Mapper.QuaternionToFloatData(transform.rotation)
		};

		saveManager.SavePrefab(save);
	}

	public void SyncData(SavablePrefab data)
	{
		color = Mapper.FloatDataToColor(data.floatData);
		Fill(color);
	}
}