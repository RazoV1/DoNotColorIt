using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets._Scripts.Events
{
	public class SaveEvents
	{
		public static UnityEvent OnSaveEvent = new UnityEvent();

		public static UnityEvent OnSettingsSaveEvent = new UnityEvent();

		public static UnityEvent OnLoadEvent = new UnityEvent();

		public static UnityEvent OnSettingsLoadEvent = new UnityEvent();
	}
}
