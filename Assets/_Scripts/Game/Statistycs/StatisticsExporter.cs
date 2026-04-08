using Assets._Scripts.Game.Statistycs;
using Assets._Scripts.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StatisticsExporter : MonoBehaviour
{
	[SerializeField] private List<IMetric> metricsToCollect = new List<IMetric>();

	public void InitializeMetricsToCollectList(List<IMetric> metricsToCollect)
	{
		this.metricsToCollect = metricsToCollect;
	}

	public void ExportData()
	{
		string path = Application.isEditor? Application.dataPath + "/Resources/Statistics" : Directory.GetCurrentDirectory() + "/Statistics";
		InitializeDirectory(path);

		string[] fileNames = Directory.GetFiles(path);
		int fileId = Directory.GetFiles(path).Length;

		string fileName = path + $"/metrics_{fileId}.json";

		StatisticsModel model = CreateModel();
		WriteToJson(model, fileName);
	}

	private void InitializeDirectory(string path)
	{
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
	}

	private StatisticsModel CreateModel()
	{
		StatisticsModel model = new StatisticsModel();
		model.statistics = new Dictionary<string, int>();
		foreach (IMetric metric in metricsToCollect)
		{
			model.statistics.Add(metric.GetMetricName(), metric.GetMetric());
		}
		return model;
	}

	private void WriteToJson(StatisticsModel model, string path)
	{
		string jsonModel = JsonConvert.SerializeObject(model, Formatting.Indented);
		File.WriteAllText(path, jsonModel);
	}
}
