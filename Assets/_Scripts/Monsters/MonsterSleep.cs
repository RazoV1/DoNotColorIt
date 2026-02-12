using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Monsters
{
	public class MonsterSleep : MonoBehaviour
	{
		[Header("Settings")]
		[SerializeField] private float sleepStrenghtPerSec;
		[Header("VFX")]
		[SerializeField] private ParticleSystem sleepParticles;

		private Animator animator;
		private PigmentMonster monster;

		private void Start()
		{
			monster = GetComponent<PigmentMonster>();
		}

		public IEnumerator Sleep()
		{
			float strenght = monster.GetStrenght();
			sleepParticles.Play();
			//animator.SetBool("Sleeping",true);
			while (strenght != 1)
			{
				float deltaStrenght = sleepStrenghtPerSec * Time.deltaTime;
				monster.AddStenght(deltaStrenght);
				//Debug.Log($"<color=green>SLEEPING {strenght}");
				strenght = Mathf.Clamp(deltaStrenght+strenght,0,1f);
				yield return null;
			}
			sleepParticles.Stop();
			//animator.SetBool("Sleeping", false);
		}
	}
}
