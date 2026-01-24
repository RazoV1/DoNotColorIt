using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PocketTicker : MonoBehaviour, ISavable
{
	public static PocketTicker Instance;

	[SerializeField] private int ticks;

	[SerializeField] private float smolaVolume;
	[SerializeField] private int logs;
	[SerializeField] private int seed;

	[SerializeField] private GameObject logPrefab;
	[SerializeField] private GameObject seedPrefab;

	[SerializeField] private Color bucket;
	private bool hasBucket = false;
	[SerializeField] private GameObject bucketPrefab;

	public void AddEgg(string eggName)
	{
		SaveManager.Instance.SavePrefab(new SavablePrefab
		{
			prefabName = eggName,
			dimension = 2,
			floatData = new System.Collections.Generic.Dictionary<string, float> { { "timeToHatch", 150f } },
			worldPosition = Mapper.VectorToFloatData(-3.212f, 0.5f, 2.5f),
			quaternionRotation = Mapper.QuaternionToFloatData(0f, 0f, 0f, 1f)
		});
	}

	public void AddSeed()
	{
		seed++;
	}

	public void AddLog()
	{
		logs++;
		if (logs >= 3)
		{
			GameManager.Instance.GetTutorial().ProgressTutorial(2);
		}
	}

	public void AddSmola()
	{
		smolaVolume++;
		if (smolaVolume >= 2f)
		{
			GameManager.Instance.GetTutorial().ProgressTutorial(3);
		}
	}

	public void PutBucket(Color color)
	{
		bucket = color;
		hasBucket = true;
	}

	public void ClearLogs()
	{
		logs = 0;
		Debug.Log(logs);
	}

	public void ClearSeeds()
	{
		seed = 0;
	}

	public int GetSeeds() => seed;

	public int GetLogs() => logs;

	public float GetSmola() => smolaVolume;

	public float ChangeSmola(float angle)
	{
		float newSmola = Mathf.Clamp(smolaVolume + angle, 0, 1000000000f);
		float delta = newSmola - smolaVolume;
		//Debug.Log(delta);    
		if (delta != Mathf.NegativeInfinity && delta != Mathf.Infinity)
		{
			smolaVolume += delta;
			smolaVolume = Mathf.Clamp(smolaVolume, 0, 1000);
			return delta;
		}
		return 0f;
	}

	private bool shouldCountTicks = false;
	private Coroutine ticker;

	public void Awake()
	{
		if (PocketTicker.Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			GameplayEvents.ChangeDimensionsEvent.AddListener(OnDimensionChange);
			SubscribeToSaveEvent();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void OnDimensionChange(int index)
	{
		if (index == 1)
		{
			ShouldCountTicks(true);
			if (hasBucket)
			{
				GameObject lowPolyBucket = Instantiate(bucketPrefab, transform.position, Quaternion.identity);
				SaveManager saveManager = SaveManager.Instance;
				Vector3 playerPos = new Vector3(saveManager.GetFloat("playerX"), saveManager.GetFloat("playerY") + 2f, saveManager.GetFloat("playerZ"));
				lowPolyBucket.GetComponent<Rigidbody>().MovePosition(playerPos);
				lowPolyBucket.GetComponent<Bucket>().Fill(bucket);
				bucket = Color.black;
				hasBucket = false;
			}
		}
	}

	public void ShouldCountTicks(bool shouldCountTicks)
	{
		this.shouldCountTicks = shouldCountTicks;
		if (shouldCountTicks)
		{
			//ticker = StartCoroutine(OutOfPocketTicker());
		}
		else
		{
			ResetTicker();
		}
	}

	private IEnumerator OutOfPocketTicker()
	{
		while (shouldCountTicks)
		{
			yield return new WaitForSeconds(2);
			ticks++;
		}
	}

	public int GetTicksForCalculations()
	{
		//if (ticks > 1)
		//{
		//	int toReturn = ticks;
		//	ticks = 1;
		//	return toReturn;
		//}
		return ticks;
	}

	public void ResetTicker()
	{
		try
		{
			StopCoroutine(ticker);
			ticker = null;
		}
		catch { }
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
		SaveEvents.OnLoadEvent.AddListener(SyncDataEmpty);
	}

	private void SyncDataEmpty()
	{
		SyncData(new SavablePrefab());
	} //♿♿♿

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;

		saveManager.SaveFloat("smola", smolaVolume);
		saveManager.SaveFloat("logs", logs);
		saveManager.SaveFloat("seeds", seed);
	}

	public void SyncData(SavablePrefab data)
	{
		SaveManager saveManager = SaveManager.Instance;
		smolaVolume = saveManager.GetFloat("smola");
		if (SceneManager.GetActiveScene().buildIndex == 2)
		{
			return;
		}
		seed = (int)saveManager.GetFloat("seeds");
		logs = (int)saveManager.GetFloat("logs");
	}
}
