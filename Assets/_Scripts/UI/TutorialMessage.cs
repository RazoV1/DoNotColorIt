using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets._Scripts.UI
{
	public class TutorialMessage : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI output;
		private Animator animator;

		public void SetText(string textKey)
		{
			output.text = LanguageManager.Instance.GetTranslatable(textKey);
		}

		public void Hide()
		{
			animator.SetTrigger("Hide");
		}

		public void Show()
		{
			animator.SetTrigger("Show");
		}

		public void Kill()
		{
			animator.SetTrigger("Hide");
		}
	}
}
