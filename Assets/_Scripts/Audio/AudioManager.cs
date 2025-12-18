using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Scripts.Audio
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance;

		[SerializeField] private AudioMixer mixer;

		public AudioClip bgOut;
		public AudioClip CarDoor;
		public AudioClip CarFlight;
		public AudioClip InfuserFire;
		public AudioClip ActionSound;
		public AudioClip bgIn;
		public AudioClip WoodHit;
		public AudioClip ColorCooking;
		public AudioClip BookOpen;
		public AudioClip PounderHit;
		public AudioClip UiInteraction;
		public AudioClip ColorCooked;
		public AudioClip ValveSpin;
		public AudioClip KapotMovement;
		public AudioClip KapotHit;
		public AudioClip TreeFall1;
		public AudioClip TreeFall2;
		public AudioClip TreeFall3;

		//private float masterVolume = 0.5f;
		//private float musicVolume = 0.5f;
		//private float soundEffectsVolume = 0.5f;

		public void Awake()
		{
			InitializeInstance();
			//SubscribeToSaveEvent();
		}

		private void InitializeInstance()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public void SetMasterVolume(float value)
		{
			//masterVolume = value;
			SetMixerVolume("masterVolume", value);
		}

		public void SetMusicVolume(float value)
		{
			//musicVolume = value;
			SetMixerVolume("musicVolume", value);
		}

		public void SetSoundEffectVolume(float value)
		{
			//soundEffectsVolume = value;
			SetMixerVolume("soundEffectVolume", value);
		}

		private void SetMixerVolume(string name, float value)
		{
			mixer.SetFloat(name, value < 0.01f ? -80 : Mathf.Log10(value) * 20);
		}

		//public void SubscribeToSaveEvent()
		//{
		//	SaveEvents.OnSettingsSaveEvent.AddListener(SaveData);
		//}

		//public void SaveData()
		//{
		//	SaveManager saveManager = SaveManager.Instance;

		//	saveManager.SaveFloat("masterVolume", masterVolume);
		//	saveManager.SaveFloat("musicVolume", musicVolume);
		//	saveManager.SaveFloat("soundEffectsVolume", soundEffectsVolume);
		//}

		//public void SyncData(SavablePrefab data)
		//{
		//	SaveManager saveManager = SaveManager.Instance;

		//	float masterVolume = saveManager.GetFloat("masterVolume");
		//	float musicVolume = saveManager.GetFloat("musicVolume");
		//	float soundEffectsVolume = saveManager.GetFloat("soundEffectsVolume");

		//	this.masterVolume = saveManager.IsFloatPresent("masterVolume") ? masterVolume : this.masterVolume;
		//	this.musicVolume = saveManager.IsFloatPresent("musicVolume") ? musicVolume : this.musicVolume;
		//	this.soundEffectsVolume = saveManager.IsFloatPresent("soundEffectsVolume") ? soundEffectsVolume : this.soundEffectsVolume ;
		//}
	}
}
