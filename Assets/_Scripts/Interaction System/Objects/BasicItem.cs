using Assets._Scripts.Audio;
using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.Interaction_System.Interfaces;
using Assets._Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using static UnityEngine.UI.GridLayoutGroup;

namespace Assets._Scripts.Interaction_System.Objects
{
	public class BasicItem : InteractableObject, IGrabbable, IRotatable, ISavable
	{
		[SerializeField] private bool shouldBeSaved = false;
		[SerializeField] private string prefabName;

		[SerializeField] private bool shouldUseGravity;
		protected bool isGrabbed = false;
		protected bool shouldMaintainDirection = false;

		[SerializeField] protected bool shouldPlayAudio = false;
		[SerializeField] private AudioSource source;
		[SerializeField] private AudioClip sound;

		public bool GetIsGrabbed() => isGrabbed;

		public string GetPrefabName() => prefabName;	

		public float GetRotationResistance()
		{
			throw new NotImplementedException();
		}

		public float GetRotationSpeed()
		{
			throw new NotImplementedException();
		}

		public bool SetIsGrabbed(bool isGrabbed, Transform target = null)
		{
			this.isGrabbed = isGrabbed;
			//rb.freezeRotation = isGrabbed;
			if (rb.gameObject.tag == "Kapot" && isGrabbed)
			{
				source.PlayOneShot(AudioManager.Instance.KapotMovement);
			}
			else if (sound != null)
			{
				source.pitch = Random.Range(0.95f, 1.15f);
				source.PlayOneShot(sound);
			}
			else if (shouldPlayAudio)
			{
				source.pitch = UnityEngine.Random.Range(0.9f,1.1f);
				source.Play();
			}
			return isGrabbed;
		}

		public virtual void SetMaintainDirection(bool shouldMaintainDirection)
		{
			this.shouldMaintainDirection = shouldMaintainDirection;
		}

		public virtual void MaintainDirection()
		{
			if (!isGrabbed || !shouldMaintainDirection) return;
		}

		private void Update()
		{
			MaintainDirection();
		}

		protected virtual void OnCollisionEnter()
		{
			if (rb.gameObject.tag == "Kapot")
			{
				source.PlayOneShot(AudioManager.Instance.KapotHit);
			}
			else if (rb.gameObject.tag == "Pounder")
			{
				source.PlayOneShot(AudioManager.Instance.PounderHit);
			}
			
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Portal")
			{
				if (transform.tag == "Seed")
				{
					PocketTicker.Instance.AddSeed();
					Destroy(gameObject);
				}
				if (transform.tag == "Log")
				{
					PocketTicker.Instance.AddLog();
					GameManager.Instance.GetTutorial().ProgressTutorial(2);
					Destroy(gameObject);
				}
				if (transform.tag == "Smola")
				{
					PocketTicker.Instance.AddSmola();
					GameManager.Instance.GetTutorial().ProgressTutorial(3);
					Destroy(gameObject);
				}
				if (transform.tag == "Bucket")
				{
					if (!GetComponent<Bucket>().IsFilled()) { return; }
					PocketTicker.Instance.PutBucket(GetComponent<Bucket>().GetColor());
					Destroy(gameObject);
				}
				if (transform.tag == "Egg")
				{
					PocketTicker.Instance.AddEgg(name);
					Destroy(gameObject);
				}
			}
		}

		public void Start()
		{
			SubscribeToSaveEvent();
		}

		public void SubscribeToSaveEvent()
		{
			if (!shouldBeSaved) { return; }
			SaveEvents.OnSaveEvent.AddListener(SaveData);
		}

		public void OnDestroy()
		{
			if (!shouldBeSaved) { return; }
			SaveEvents.OnSaveEvent.RemoveListener(SaveData);
		}

		public virtual void SaveData()
		{
			if (!shouldBeSaved) { return; }
			SavablePrefab pref = new SavablePrefab
			{
				prefabName = $"{prefabName}",
				dimension = SceneManager.GetActiveScene().buildIndex,
				worldPosition =Mapper.VectorToFloatData(transform.position), //new List<float> { transform.position.x, transform.position.y, transform.position.z },
				quaternionRotation =Mapper.QuaternionToFloatData(transform.rotation) //new List<float> { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w }
			};
			SaveManager.Instance.SavePrefab(pref);
		}

		public virtual void SyncData(SavablePrefab data)
		{
		}
	}
}