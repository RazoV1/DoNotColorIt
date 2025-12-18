using System;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;

namespace Assets._Scripts.Weather
{
	internal class WeatherManager : MonoBehaviour
	{
		public static WeatherManager Instance;

		[SerializeField] private ParticleSystem rain;
		[SerializeField] private AudioSource rainSource;

		[SerializeField] private float minRainTime;
		[SerializeField] private float maxRainTime;

		private Coroutine rainFade;

		private WeatherType currentWeather;

		public void Awake()
		{
			if (WeatherManager.Instance == null)
			{
				Instance = this;
				StartCoroutine(WeatherCycle());
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public void SetRainVisibility(bool isVisible)
		{
			rain.gameObject.SetActive(isVisible);
		}

		private float GetRandomInterval() => Random.Range(minRainTime, maxRainTime);

		private IEnumerator WeatherCycle()
		{
			while (true)
			{
				yield return new WaitForSeconds(GetRandomInterval());
				rain.Play();
				StartRainFading(0.18f);
				currentWeather = WeatherType.Rain;
				yield return new WaitForSeconds(GetRandomInterval());
				StartRainFading(0f);
				rain.Stop();
				currentWeather = WeatherType.Sunny;
			}
		}
		private void StartRainFading(float volume)
		{
			if (rainFade != null)
			{
				StopCoroutine(rainFade);
			}
			rainFade = StartCoroutine(RainFade(volume));
		}

		private IEnumerator RainFade(float volume)
		{
			while (Mathf.Abs(rainSource.volume - volume) > 0.01f)
			{
				rainSource.volume = Mathf.Lerp(rainSource.volume, volume, Time.deltaTime * 0.5f);
				yield return null;
			}

			StopCoroutine(rainFade);
		}

		public WeatherType GetCurrentWeather() => currentWeather;
	}



	[Serializable]
	public enum WeatherType
	{
		Rain,
		Sunny
	}
}
