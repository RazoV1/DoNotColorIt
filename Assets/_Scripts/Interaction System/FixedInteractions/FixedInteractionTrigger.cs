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
		}

		private void HandleInput()
		{
			if (!hasEntered) return;
		    GameManager.Instance.GetCursorHint().ShowHint(MouseHints.ToggleMode);
			Debug.Log("hinr");
			if (Input.GetKeyDown(KeyCode.E))
			{
				interactionMainScript.SetLocked(!interactionMainScript.GetIsLocked());
			}
		}
	}
}
