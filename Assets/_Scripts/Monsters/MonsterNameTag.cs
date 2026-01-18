using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Scripts.Monsters
{
	public class MonsterNameTag : MonoBehaviour
	{
		[SerializeField] private string nameTag;
		[SerializeField] private TextMeshProUGUI nameTextField;

		public void SetNameTag(string nameTag)
		{
			this.nameTag = nameTag;
			nameTextField.text = nameTag;
		}

		public string GetNameTag() => nameTag;

		public void SetRandomNameTag()
		{
			List<string> nameTags = LanguageManager.Instance.GetPresetNametags();
			string randomName = nameTags[Random.Range(0, nameTags.Count - 1)];
			SetNameTag(randomName);
		}
	}
}
