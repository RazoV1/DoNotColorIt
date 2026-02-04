using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

namespace Assets._Scripts.Events
{
	public class TutorialEvents
	{
		public static UnityEvent<string> OnTutorialIndexChanged = new UnityEvent<string>();

		public static UnityEvent<int> OnAdditionalTutorialTriggered = new UnityEvent<int>();
	}
}