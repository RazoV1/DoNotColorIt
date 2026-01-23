using Assets._Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WaterLevel : MonoBehaviour
{
	[SerializeField] private List<float> yByTaskIndex = new List<float>();
	[SerializeField] private List<GameObject> barrierModels = new List<GameObject>();
	[SerializeField] private float waterSpeed;
	private Coroutine waterLevelChangeRoutine;

	private IEnumerator LowerWater(int ind)
	{
		while (transform.position.y > yByTaskIndex[ind])
		{
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, yByTaskIndex[ind], transform.position.z),Time.deltaTime * waterSpeed);
			yield return null;
		}
		try
		{
			StopCoroutine(waterLevelChangeRoutine);
			waterLevelChangeRoutine = null;
		}
		catch { }
	}

	private void Awake()
	{
		GameplayEvents.OnWaterLevelChanged.AddListener(ChangeWaterLevel);
	}

	private void OnDestroy()
	{
		GameplayEvents.OnWaterLevelChanged.RemoveListener(ChangeWaterLevel);
	}

	private void ChangeWaterLevel()
	{
		if (waterLevelChangeRoutine != null)
		{
			StopCoroutine(waterLevelChangeRoutine);
		}
		waterLevelChangeRoutine = StartCoroutine(LowerWater(GameManager.Instance.GetCurrentTaskIndex()));
		barrierModels.ForEach(gameObject => gameObject.SetActive(false));
		barrierModels[GameManager.Instance.GetCurrentTaskIndex()].SetActive(true);
	}
}
