using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets._Scripts.Events
{
	public class CommandEvents
	{
	    private static Dictionary<string,UnityEvent<int>> CommandEventsRegistry = new Dictionary<string, UnityEvent<int>>();

		public static void Register()
		{
			CommandEventsRegistry.Add("/skipTutorial", OnTutorialSkip);
			CommandEventsRegistry.Add("/giveLogs", OnLogGive);
			CommandEventsRegistry.Add("/giveSmola",OnSmolaGive);
			CommandEventsRegistry.Add("/help", OnHelp);
		}

		public static bool TryInvokeEvent(string eventId,int data)
		{
			if (CommandEventsRegistry.ContainsKey(eventId))
			{
				CommandEventsRegistry[eventId].Invoke(data);
				return true;
			}
			return false;
		}
		public static UnityEvent<int> OnHelp = new UnityEvent<int>();

		public static UnityEvent<int> OnTutorialSkip = new UnityEvent<int>();

		public static UnityEvent<int> OnLogGive = new UnityEvent<int>();

		public static UnityEvent<int> OnSmolaGive = new UnityEvent<int>();
	}
}
