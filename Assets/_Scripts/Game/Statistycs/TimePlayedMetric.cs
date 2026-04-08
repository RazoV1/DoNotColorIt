using Assets._Scripts.Game.Statistycs;
using System.Collections;
using UnityEngine;

public class TimePlayedMetric : MonoBehaviour, IMetric
{
	private int timePlayed = 0;
	private string metricName = "timePlayed";

	private Coroutine timeTicker;

	public int GetMetric() => timePlayed;

	public string GetMetricName() => metricName;

	public void ResetMetric() 
	{ 
		StopCoroutine(timeTicker);
		timePlayed = 0; 
	}

	public void StartCollection() 
	{ 
		Debug.Log("<color=yellow>TimesPlayedMetric получил сообщение о начале сбора!");
		timeTicker = StartCoroutine(TickTimePlayed());
	}

	private IEnumerator TickTimePlayed()
	{
		while (true)
		{
			yield return new WaitForSeconds(60);
			timePlayed++;
		}
	}
}
