using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.PlayerController
{
	public class HintActivator : MonoBehaviour
	{
		private bool shouldProvideHint;
		public bool ShouldProvideHint() => shouldProvideHint;

		public void SetShouldProvideHint(bool shouldProvideHint) { this.shouldProvideHint = shouldProvideHint; }
	}
}
