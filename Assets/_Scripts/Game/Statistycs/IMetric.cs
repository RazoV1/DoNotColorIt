using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game.Statistycs
{
	public interface IMetric
	{
		public string GetMetricName();

		public int GetMetric();

		public void ResetMetric();

		public void StartCollection();
	}
}
