using Assets._Scripts.Interaction_System.Objects;
using Assets._Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorPigment : BasicItem
{
	[Header("Настройки пигмента:")]
	[SerializeField] protected Color color;
	[SerializeField] protected float volume;
	[SerializeField] protected float baseVibrancy;
	[SerializeField] protected MeshRenderer meshRenderer;
	private float scaleFactor = 0.1f;
	protected Material pigmentMaterial;

	public Color GetColor() => color;
	public float GetVolume() => volume;

	public virtual void Start()
	{
		//InitializePigment(color,volume);
		SubscribeToSaveEvent();
	}

	public virtual void InitializePigment(Color color, float volume)
	{
		pigmentMaterial = Instantiate(meshRenderer.material);
		this.color = color;
		this.volume = volume;
		transform.localScale = new Vector3(MathF.Pow(volume,0.3f)*scaleFactor, MathF.Pow(volume, 0.3f)*scaleFactor, MathF.Pow(volume, 0.3f) * scaleFactor);
		pigmentMaterial.color = color;
		meshRenderer.material = pigmentMaterial;
	}

	private void OnDestroy()
	{
		Destroy(pigmentMaterial);
	}

	public override void SaveData()
	{
		SavablePrefab pref = new SavablePrefab
		{
			prefabName = $"pigment",
			dimension = SceneManager.GetActiveScene().buildIndex,
			worldPosition = Mapper.VectorToFloatData(transform.position),// new List<float> { transform.position.x, transform.position.y, transform.position.z },
			quaternionRotation =Mapper.QuaternionToFloatData(transform.rotation), //new List<float> { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w },
			floatData = new Dictionary<string, float>
			{
				{ "colorR",color.r},
				{"colorG", color.g},
				{"colorB",color.b },
				{"volume",volume }
			}
		};
		SaveManager.Instance.SavePrefab(pref);
	}

	public override void SyncData(SavablePrefab data)
	{
		base.SyncData(data);
		try
		{
			Color savedColor = new Color
				(data.floatData["colorR"],
				data.floatData["colorG"],
				data.floatData["colorB"]);
			float savedVolume = data.floatData["volume"];

			InitializePigment(savedColor, savedVolume);
		}
		catch
		{
			Debug.Log("<color=red>Error during file reading! Skipping prefab. . .");
			Destroy(gameObject);
		}
	}
}