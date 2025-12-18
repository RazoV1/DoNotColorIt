using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Interaction_System.Interfaces
{
	public interface IRotatable
	{
		public float GetRotationSpeed();

		public float GetRotationResistance();
	}
}
