using Assets._Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DustedColorPigment : ColorPigment
{
	public override void Start()
	{
		SubscribeToSaveEvent();
	}

	public override void InitializePigment(Color color, float volume)
	{
		base.InitializePigment(color, volume);
	}

	public override void SaveData()
	{
		SavablePrefab pref = new SavablePrefab
		{
			prefabName = $"dust",
			dimension = SceneManager.GetActiveScene().buildIndex,
			worldPosition = Mapper.VectorToFloatData(transform.position),// new List<float> { transform.position.x, transform.position.y, transform.position.z },
			quaternionRotation = Mapper.QuaternionToFloatData(transform.rotation), //new List<float> { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w },
			floatData = new Dictionary<string, float>
			{
				{"colorR",color.r},
				{"colorG", color.g},
				{"colorB",color.b},
				{"volume",volume}
			}
		};
		SaveManager.Instance.SavePrefab(pref);
	}
}
