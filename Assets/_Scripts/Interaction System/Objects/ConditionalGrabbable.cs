using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Interaction_System.Objects
{
	internal class ConditionalGrabbable : BasicItem
	{
		private bool canBeGrabbed = false;

		public bool GetCanBeGrabbed() => canBeGrabbed;

		public void SetCanBeGrabbed(bool canBeGrabbed) {  this.canBeGrabbed = canBeGrabbed; }
	}
}
