using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

namespace Assets._Scripts.Interaction_System.Objects
{
	public class ConditionalGrabbable : BasicItem
	{
		[Header("ConditionalGrabbable")]
		[SerializeField] private int tutorialLock = 0;

		private bool canBeGrabbed = false;

		public bool GetCanBeGrabbed() => canBeGrabbed && GameManager.Instance.GetTutorial().GetTutorialIndex() >= tutorialLock;

		public void SetCanBeGrabbed(bool canBeGrabbed) {  this.canBeGrabbed = canBeGrabbed; }
	}
}
