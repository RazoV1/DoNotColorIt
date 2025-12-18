using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets._Scripts.Events
{
	public class TutorialEvents
	{
		public static UnityEvent<int> OnTutorialIndexChanged = new UnityEvent<int>();
	}
}