using Assets._Scripts.Events;
using System.Collections;
using UnityEngine;

namespace Assets._Scripts.NPC
{
	public class NPCTicker : MonoBehaviour
	{
		[Header("Settings")]
		[SerializeField] private int tickTime;
		[Header("Misc")]
		[SerializeField] private int time;
		private Coroutine tickerRoutine;

		private void Start()
		{
			tickerRoutine = StartCoroutine(Ticker());
		}

		private IEnumerator Ticker()
		{
			while (true)
			{
				yield return new WaitForSeconds(tickTime);
				tickTime = tickTime == 24 ? 0 : tickTime + 1;
				GameplayEvents.OnNpcTick.Invoke(tickTime);
			}
		}
	}
}