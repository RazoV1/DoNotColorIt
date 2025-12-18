using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets._Scripts.Game.SaveSystem
{
	public interface ISavable
	{
		public void SubscribeToSaveEvent();

		public void SaveData();

		public void SyncData(SavablePrefab data);
	}
}
