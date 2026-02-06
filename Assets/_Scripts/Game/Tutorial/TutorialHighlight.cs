using Assets._Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Game.Tutorial
{
	public class TutorialHighlight : MonoBehaviour
	{
		[SerializeField] private List<string> order;
		[SerializeField] private GameObject highlight;
		[SerializeField] private Transform player;
		[SerializeField] private bool shouldRotate = true;
		private void Awake()
		{
			TutorialEvents.OnTutorialIndexChanged.AddListener(ShowIfOrder);
		}

		private void Start()
		{
			//ShowIfOrder(GameManager.Instance.GetTutorial().GetTutorialIndex());
		}

		private void OnDestroy()
		{
			TutorialEvents.OnTutorialIndexChanged.RemoveListener(ShowIfOrder);
		}

		public void ShowIfOrder(string currentIndex)
		{
			highlight.SetActive(order.Contains(currentIndex));
		}

		private void Update()
		{
			if (!shouldRotate) { return; }
			transform.LookAt(player);
		}
	}
}
