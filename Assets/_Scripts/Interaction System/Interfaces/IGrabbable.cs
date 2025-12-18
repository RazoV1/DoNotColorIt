using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Interaction_System.Interfaces
{
	public interface IGrabbable
	{
		public bool SetIsGrabbed(bool isGrabbed, Transform target = null);

		public bool GetIsGrabbed();

		public void SetMaintainDirection(bool shouldMaintainDirection);

		/// <summary>
		/// Для внутреннего использования. Смотреть на игрока
		/// </summary>
		public void MaintainDirection();
	}
}
