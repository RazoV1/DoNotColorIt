using Assets._Scripts.Game.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets._Scripts.Monsters
{
	public class MonsterAge : MonoBehaviour
	{
		[Header("Settings")]
		[SerializeField] private MonsterAgePhases age;
		[SerializeField] private float timeLived;
		[SerializeField] private PigmentMonster monster;
		[Header("UI")]
		[SerializeField] private TextMeshProUGUI ageField;

		public void CalculateAge(int ticks)
		{
			timeLived += ticks * 2;
			UpdateUI();
		}

		public MonsterAgePhases GetCurrentAge() { return age; }

		public float GetTimeLived() => timeLived;

		private void UpdateUI()
		{

			if (timeLived <= Config.MonsterLifetime / 3f)
			{
				age = MonsterAgePhases.Young;
			}
			else if (timeLived <= Config.MonsterLifetime / (2 / 3f))
			{
				age = MonsterAgePhases.Mature;
			}
			else if (timeLived <= Config.MonsterLifetime + (Config.MonsterLifetime / 3f))
			{
				age = MonsterAgePhases.Old;
			}
			else
			{
				monster.Die();
			}
			ageField.text = LanguageManager.Instance.GetTranslatable($"monster.age.{age.ToString()}");
		}

	    public void SetTimeLived(float time)
		{
			timeLived = time;
		}
	}

	public enum MonsterAgePhases
	{
		Young,
		Mature,
		Old
	}
}
