using Assets._Scripts.Audio;
using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Infuser : MonoBehaviour, ISavable
{
	[SerializeField] private DustedColorPigment dustInside;
	[SerializeField] private float neededTemperature;
	[SerializeField] private float currrentTemperature;
	[SerializeField] private int logs;
	[SerializeField] private float smolaVolume;
	[SerializeField] private float logBurnDuration;
	[SerializeField] private float logAdditor;
	[SerializeField] private float smolaAddSpeed;
	private float volume;
	private Color color;
	[Header("Audio")]
	[SerializeField]private AudioSource burnsource;
	[SerializeField] private AudioSource colorSource;
	[Header("UI")]
	[SerializeField] private Image smolaBar;
	[SerializeField] private Image logsBar;
	[SerializeField] private Material outputMaterial;
	[SerializeField] private GameObject outputl;
	[SerializeField] private Bucket bucket;
	[Header("Pivots")]
	[SerializeField] private Transform dustSpitPivot;

	[Header("Prefabs")]
	[SerializeField] private GameObject dustPrefab;
	private float timeBurning;

	public void SetBucket(Bucket bucket) { this.bucket = bucket; }

	private void HandleSound()
	{
		if (logs > 0)
		{
			burnsource.volume = Mathf.Lerp(burnsource.volume,1f,Time.deltaTime);
		}
		else
		{
			burnsource.volume = Mathf.Lerp(burnsource.volume, 0f, Time.deltaTime);
		}
	}
	#region Logs
	public void AddLog()
	{
		logs++;
	}

	public void Start()
	{
		StartCoroutine(Burn());
		SubscribeToSaveEvent();
		outputMaterial = Instantiate(outputMaterial);
		outputl.GetComponent<MeshRenderer>().material = outputMaterial;
	}

	public void Update()
	{
		outputMaterial.color = TryCalculateColor();
		HandleSound();
	}

	private IEnumerator Burn()
	{
		while (true)
		{
			while (logs > 0)
			{
				float timePassed = 0f;
				while (timePassed < logBurnDuration)
				{
					timePassed += Time.deltaTime;
					currrentTemperature = Mathf.Clamp(currrentTemperature + Time.deltaTime * logAdditor, 0, neededTemperature);
					logsBar.fillAmount = currrentTemperature / neededTemperature;
					if (currrentTemperature >= neededTemperature)
					{

						GameManager.Instance.GetTutorial().ProgressTutorial(9);
					}
					yield return null;
				}
				logs--;
			}
			while (logs == 0)
			{
				currrentTemperature = currrentTemperature = Mathf.Clamp(currrentTemperature - Time.deltaTime, 0, neededTemperature);
				logsBar.fillAmount = currrentTemperature / neededTemperature;
				yield return null;
			}
		}
	}
	#endregion

	#region Smola
	public float ChangeSmole(float turnAngle)
	{
		try
		{
			float prev = smolaVolume;
			smolaVolume = Mathf.Clamp(smolaVolume + turnAngle, 0f, volume);
			smolaBar.fillAmount = smolaVolume / volume;
			return smolaVolume - prev;
		}
		catch
		{
			smolaVolume = 0f;
			smolaBar.fillAmount = 0f;
			return 0;
		}
	}

	public float GetSmolaVolume() => smolaVolume;
	#endregion
	#region Dust
	public void PutDustInside(DustedColorPigment dust)
	{
		if (dustInside == null)
		{
			GameManager.Instance.GetTutorial().ProgressTutorial(8);
			dustInside = dust;
			volume = dust.GetVolume();
			color = dust.GetColor();
			dust.gameObject.SetActive(false);
		}
	}

	public void SpitDust()
	{
		try
		{
			dustInside.transform.position = dustSpitPivot.position;
			dustInside.gameObject.SetActive(true);
			dustInside = null;
		}
		catch
		{
			Debug.Log("No Dust Inside");
		}
	}
	#endregion

	public Color TryCalculateColor()
	{
		if (dustInside == null)
		{
			return Color.black;
		}
		float r = color.r;
		float g = color.g;
		float b = color.b;

		return new Color(Mathf.Clamp(r-(smolaVolume/volume),0f,1f), Mathf.Clamp(g - (smolaVolume / volume), 0f, 1f), Mathf.Clamp(b - (smolaVolume / volume), 0f, 1f));
	}

	public void TryCook()
	{

		Debug.Log("<color=yellow>TryCook");
		if (currrentTemperature == neededTemperature && bucket != null && dustInside != null)
		{
			bucket.Fill(TryCalculateColor());
			colorSource.PlayOneShot(AudioManager.Instance.ColorCooked);
			dustInside = null;
			smolaVolume = 0f;

			GameManager.Instance.GetTutorial().ProgressTutorial(10);
			Debug.Log("<color=green>Cook");
		}
	}

	public void SubscribeToSaveEvent()
	{
		SaveEvents.OnSaveEvent.AddListener(SaveData);
		SaveEvents.OnLoadEvent.AddListener(SyncDataEmpty);
	}

	private void OnDestroy()
	{
		SaveEvents.OnSaveEvent.RemoveListener(SaveData);
		SaveEvents.OnLoadEvent.RemoveListener(SyncDataEmpty);
	}

	public void SaveData()
	{
		SaveManager saveManager = SaveManager.Instance;

        if (dustInside != null)
        {
			dustInside.SaveData();
        }
        SpitDust();

        //saveManager.SaveFloat("infuserR",color.r);
		//saveManager.SaveFloat("infuserG",color.g);
		//saveManager.SaveFloat("infuserB",color.b);

		saveManager.SaveFloat("infuserSmola",smolaVolume);
	}

	private void SyncDataEmpty() { SyncData(new SavablePrefab()); }

	public void SyncData(SavablePrefab data)
	{
		SaveManager saveManager = SaveManager.Instance;

		//float infuserR = saveManager.GetFloat("infuserR");
		//float infuserG = saveManager.GetFloat("infuserG");
		//float infuserB = saveManager.GetFloat("infuserB");

		float infuserSmoal = saveManager.GetFloat("infuserSmola");

		//Color newColor = new Color(infuserR,infuserG, infuserB);

		this.smolaVolume = infuserSmoal;
		//color = newColor;
	}
}