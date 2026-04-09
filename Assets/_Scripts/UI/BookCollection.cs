using Assets._Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.UI
{
	public class BookCollection : MonoBehaviour
	{
		[SerializeField] private List<pictureToNpc> pictures;

		[Serializable]
		public struct pictureToNpc
		{
			public string name;
			public GameObject picture;
		}

		private void Awake()
		{
			ResetPics();
			GameplayEvents.OnTaskIndexChanged.AddListener(GetNPCToShow);
			SaveEvents.OnLoadEvent.AddListener(GetNPCToShow);
		}

		private void GetNPCToShow()
		{
			ResetPics();
			foreach (pictureToNpc picToNpc in pictures)
			{
				if (SaveManager.Instance.GetFloat(picToNpc.name) == 1)
				{
					picToNpc.picture.SetActive(true);
				}
			}
		}

		private void ResetPics()
		{
			pictures.ForEach(x => x.picture.SetActive(false));
		}
	}
}