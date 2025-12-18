using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets._Scripts.Events
{
	public class GameSetupEvents
	{
		public static UnityEvent OnUiGenerated = new UnityEvent();

		public static UnityEvent OnTranslationLoaded = new UnityEvent();
	}
}
