using Assets._Scripts.Events;
using System.Collections;
using UnityEngine;

public class TutorialKeyCheck : MonoBehaviour
{
	private TutorialManager tutorialManager;
	private Coroutine keyCheckRoutine;

	private void Awake()
	{
		TutorialEvents.OnTutorialIndexChanged.AddListener(StartCheckingForKey);
	}

	private void StartCheckingForKey(string tutorialId)
	{
		if (keyCheckRoutine != null)
		{
			StopCoroutine(keyCheckRoutine);
		}
		keyCheckRoutine = StartCoroutine(CheckForKey(tutorialId));
	}

	private IEnumerator CheckForKey(string tutorialId)
	{
		tutorialManager = GameManager.Instance.GetTutorial();
		KeyCode key = KeyCode.None;
		Debug.Log(tutorialId);
		switch (tutorialId)
		{
			case "w":
				key = KeyCode.W; break;
			case "space":
				key = KeyCode.Space; break;
			case "ad":
				yield return new WaitForSeconds(4);
				tutorialManager.ProgressTutorial(tutorialId);
				key = KeyCode.None; break;
			case "ctrl":

				key = KeyCode.LeftControl; 
				break;
		}
		if (key != KeyCode.None)
		{
			while (true)
			{
				if (Input.GetKeyDown(key))
				{
					tutorialManager.ProgressTutorial(tutorialId);
					break;
				}

				yield return null;
			}
		}
		yield return null;
	}
}
