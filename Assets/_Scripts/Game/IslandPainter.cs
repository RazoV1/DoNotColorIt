using Assets._Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IslandPainter : MonoBehaviour
{
	[SerializeField] private List<IslandGroup> islandGroups = new List<IslandGroup>();

	[Serializable]
	public struct IslandGroup
	{
		public List<GameObject> group;
	}

	private void Start()
	{
		GameplayEvents.OnTaskIndexChanged.AddListener(PaintGroups);
		PaintGroups();
	}

	public void PaintGroups()
	{
		int trueId = Mathf.FloorToInt(GameManager.Instance.GetCurrentTaskIndex() / 2f);

		if (trueId < 1) return;
		foreach (IslandGroup group in islandGroups.GetRange(0, trueId-1))
		{
			group.group.ForEach(house =>
			{
				try
				{
					var children = house.GetComponentsInChildren<Transform>(includeInactive: true);
					foreach (var child in children)
					{
						child.gameObject.layer = 3;
					}
				}
				catch
				{

				}
			});
		}
	}
}
