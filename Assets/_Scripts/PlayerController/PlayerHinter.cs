using Assets._Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Scripts.PlayerController
{
	public class PlayerHinter : MonoBehaviour
	{
		[SerializeField] private List<HintByTag> hints;
		[SerializeField] private GameObject pointer;
		private List<string> hintableTags;
		private float hintDistance;
		private CursorHint hintScript;

		private PlayerGrabber playerGrabber;

		private bool isInFixedInteractionsMode = false;
		private bool shouldHint = true;

		private GameManager gameManager;

		[Header("MonsterStats")]
		[SerializeField] private GameObject monsterStats;
		[SerializeField] private List<TextMeshProUGUI> monsterStatsField;
		[SerializeField] private List<Slider> monstersStats;

		[Serializable]
		public struct HintByTag
		{
			public string tag;
			public MouseHints hintType;
			public int tutorialIndexLock;
			public bool isConditional;
		}

		public void SetShouldHint(bool shouldHint)
		{
			this.shouldHint = shouldHint;
			Debug.Log($"<color=yellow>{this.shouldHint}");
			CastHint();
		}

		private void Awake()
		{
			monsterStats = monsterStats == null ? new GameObject("MOnsterStatsPlaceholder") : monsterStats; //SON 💔💔💔
			hintableTags = hints.Select(hint => hint.tag).ToList();
		    
			gameManager = GameManager.Instance;
			hintScript = gameManager.GetCursorHint();

			playerGrabber = GetComponent<PlayerGrabber>();

			hintDistance = playerGrabber.GetGrabDistance();

			GameplayEvents.OnEnteredFixedInteractionMode.AddListener(SetFixedInteractionsBool);
		}

		private void OnDestroy()
		{
			GameplayEvents.OnEnteredFixedInteractionMode.RemoveListener(SetFixedInteractionsBool);
		}

		private void Update()
		{
			CastHint();
		}

		private void SetFixedInteractionsBool(bool value)
		{
			isInFixedInteractionsMode = value;
			pointer.SetActive(!value);
		}

		private void CastHint()
		{
			monsterStats.SetActive(false);
			if (!shouldHint || playerGrabber.GetIsGrabbing())
			{
				hintScript.ShowHint(MouseHints.None);
				return;
			}

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit) && Vector3.Distance(hit.point, ray.origin) <= hintDistance && IsObjectOnTheList(hit))
			{
				HandleMonsterStatsUpdate(hit);
				HintByTag hintContainer = GetHintByTag(hit.collider.tag);

				if (!TryProcessHintContainer(hintContainer)) hintScript.ShowHint(MouseHints.None);

				return;
			}
			hintScript.ShowHint(MouseHints.None);
		}

		private bool TryProcessHintContainer(HintByTag hint)
		{
			if (hint.isConditional && !isInFixedInteractionsMode) return false;
			if (hint.tutorialIndexLock != 0 && hint.tutorialIndexLock > gameManager.GetTutorial().GetTutorialIndex()) return false;

			hintScript.ShowHint(hint.hintType);
			return true;
		}

		private HintByTag GetHintByTag(string tag)
		{
			return hints.First(hint => hint.tag == tag);
		}

		private bool IsObjectOnTheList(RaycastHit hit)
		{
			string colliderTag = hit.collider.tag;
			return hintableTags.Contains(colliderTag);
		}

		private void HandleMonsterStatsUpdate(RaycastHit hit)
		{
			PigmentMonster monsterObj = hit.collider.GetComponent<PigmentMonster>();
			if (monsterObj != null)
			{
				monsterStats.SetActive(true);
				Dictionary<string, float> stats = monsterObj.GetMonsterStats();
				List<string> list = stats.Keys.ToList();
				for (int i = 0; i < monsterStatsField.Count; i++)
				{
					monsterStatsField[i].text = LanguageManager.Instance.GetTranslatable($"ui.monster_stats.{list[i]}") + $"{Mathf.Clamp((int)(stats[list[i]] * 100), 0, 100f)}%";
					monstersStats[i].value = Mathf.Clamp((int)(stats[list[i]] * 100), 0, 100f);
				}
			}
		}
	}
}
