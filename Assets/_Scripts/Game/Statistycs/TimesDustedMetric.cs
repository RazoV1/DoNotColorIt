using Assets._Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game.Statistycs
{
	public class TimesDustedMetric : MonoBehaviour, IMetric
	{
		private int timesDusted = 0;
		private string metricName = "timesDusted";

		private void IncreaseMetric() { timesDusted++; }

		public int GetMetric() => timesDusted;

		public string GetMetricName() => metricName;

		public void ResetMetric() { timesDusted = 0; }

		public void StartCollection() { Debug.Log("<color=yellow>TimesDustedMetric получил сообщение о начале сбора!"); }

		private void OnDestroy()
		{
			GameplayEvents.OnDusted.RemoveListener(IncreaseMetric);
		}
		private void Awake()
		{
			GameplayEvents.OnDusted.AddListener(IncreaseMetric);
		}

	}
}
