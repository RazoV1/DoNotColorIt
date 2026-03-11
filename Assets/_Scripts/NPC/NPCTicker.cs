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

		private bool shouldTick = true;

		private void Start()
		{
			tickerRoutine = StartCoroutine(Ticker());
		}

		public void SetShouldTick(bool shouldTick) { this.shouldTick = shouldTick; }

		private IEnumerator Ticker()
		{
			int tick = 0;
			while (true)
			{
				yield return new WaitForSeconds(tickTime);
				while (!shouldTick)
				{
					yield return null;
				}
				tick = tick == 60 ? 0 : tick + 1;
				Debug.Log($"Invoking NPC tick {tick}");
				GameplayEvents.OnNpcTick.Invoke(tick);
			}
		}
	}
}