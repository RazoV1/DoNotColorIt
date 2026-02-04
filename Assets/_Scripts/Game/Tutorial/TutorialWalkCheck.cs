using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game.Tutorial
{
	public class TutorialWalkCheck : MonoBehaviour
	{
		public void Start()
		{
			if (GameManager.Instance.GetTutorial().GetTutorialIndex() > 5)
			{
				StartCoroutine(KostylWalk());
			}
		}

		private IEnumerator KostylWalk()
		{
			yield return new WaitForSeconds(5);
			GameManager.Instance.GetTutorial().ProgressTutorial("smalWalk");
		}
	}
}
