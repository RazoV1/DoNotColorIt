using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game
{
	public class JsonAccessor
	{
		public static DialogueModel GetDialogueInfo(string eventFolder, string dialogueName)
		{
			string path = LanguageManager.Instance.GetPath() + $"/{LanguageManager.Instance.GetCurrentLanguage().Split('.')[0]}/{eventFolder}/{dialogueName}.json";
			string rawJsonString = File.ReadAllText(path);
			DialogueModel model = JsonConvert.DeserializeObject<DialogueModel>(rawJsonString);
			if (model != null)
			{
				return model;
			}
			return null;
		}
	}
}
