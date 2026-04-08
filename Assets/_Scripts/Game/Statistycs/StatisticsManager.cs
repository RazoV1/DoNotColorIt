using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game.Statistycs
{
	public class StatisticsManager : MonoBehaviour
	{
		[SerializeField] private List<IMetric> metrics = new List<IMetric>();
		private StatisticsExporter statisticsExporter;

		private void Awake()
		{
			metrics = GetComponents<IMetric>().ToList();
			Debug.Log(metrics[0].GetMetricName());
			statisticsExporter = GetComponent<StatisticsExporter>();
			statisticsExporter.InitializeMetricsToCollectList(metrics);
			Debug.Log(metrics.Count);
		}

		public void StartSession()
		{
		    foreach (IMetric metric in metrics)
			{
				metric.StartCollection();
			}
		}

		public void EndSession()
		{
			statisticsExporter.ExportData();
			foreach (IMetric metric in metrics)
			{
				metric.ResetMetric();
			}
		}
	}
}
