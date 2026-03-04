using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets._Scripts.Events
{
	public class GameplayEvents
	{
		public static UnityEvent<int> ChangeDimensionsEvent = new UnityEvent<int>();

		public static UnityEvent<bool> OnPauseToggled = new UnityEvent<bool>();

		public static UnityEvent OnWaterLevelChanged = new UnityEvent();

		public static UnityEvent OnMount = new UnityEvent();

		public static UnityEvent OnTaskIndexChanged = new UnityEvent();

		public static UnityEvent<int> OnNpcTick = new UnityEvent<int>();
	}
}
