using Assets._Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Interaction_System.FixedInteractions
{
	public class FixedInteractionTrigger : MonoBehaviour
	{
		[SerializeField] private MouseHints hintName;
		[SerializeField] private string dropCondition;

		private FixedInteraction interactionMainScript;
		private bool hasEntered = false;

		private void OnTriggerEnter(Collider other) { if (other.tag == "Player") hasEntered = true; }
		private void OnTriggerExit(Collider other) { if (other.tag == "Player") hasEntered = false; }

		private void LateUpdate()
		{
			HandleInput();
		}


		private void Awake()
		{
			interactionMainScript = GetComponent<FixedInteraction>();
			switch (dropCondition)
			{
				case "Mortar":
					GameplayEvents.OnDusted.AddListener(Drop);
					break;
				case "Infuser":
					GameplayEvents.OnInfused.AddListener(Drop);
					break;
			}
		}

		private void OnDestroy()
		{
			switch (dropCondition)
			{
				case "Mortar":
					GameplayEvents.OnDusted.RemoveListener(Drop);
					break;
				case "Infuser":
					GameplayEvents.OnInfused.RemoveListener(Drop);
					break;
			}
		}

		private void HandleInput()
		{
			if (!hasEntered) return;
			if (!interactionMainScript.GetIsLocked())
			{
				GameManager.Instance.GetCursorHint().ShowHint(hintName);
			}
			Debug.Log("hinr");
			if (Input.GetKeyDown(KeyCode.E))
			{
				GameManager.Instance.GetTutorial().ProgressTutorial("infuserPress");
				interactionMainScript.SetLocked(!interactionMainScript.GetIsLocked());
			}
		}

		private void Drop()
		{
			interactionMainScript.SetLocked(false);
		}
	}
}
