using Assets._Scripts.Audio;
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



		private AudioSource source;
		private Animator animator;
		private PigmentMonster monster;

		private void Start()
		{
			monster = GetComponent<PigmentMonster>();
			source = GetComponent<AudioSource>();	
		}

		public IEnumerator Sleep()
		{
			float strenght = monster.GetStrenght();
			sleepParticles.Play();
			float deltaStrenght;
			//animator.SetBool("Sleeping",true);
			source.PlayOneShot(AudioManager.Instance.MonsterSleep);
			while (strenght != 1)
			{
				deltaStrenght = sleepStrenghtPerSec * Time.deltaTime;
				monster.AddStenght(deltaStrenght);
				//Debug.Log($"<color=green>SLEEPING {strenght}");
				strenght = Mathf.Clamp(deltaStrenght+strenght,0,1f);
				yield return null;
			}
			source.Stop();
			sleepParticles.Stop();
			//animator.SetBool("Sleeping", false);
		}
	}
}
