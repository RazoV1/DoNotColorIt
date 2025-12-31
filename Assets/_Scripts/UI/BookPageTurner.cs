using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookPageTurner : MonoBehaviour
{
	[SerializeField] private List<Button> pageButtons;
	private int currentButtonIndex = 0;

	private void Update()
	{
		HandleInput();
	}

	private void HandleInput()
	{
		if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
		{

			currentButtonIndex = pageButtons.IndexOf(pageButtons.First(x => !x.interactable));
			currentButtonIndex = Mathf.Clamp(currentButtonIndex+1,0,3);
			PressButton();
		}
		else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			currentButtonIndex = pageButtons.IndexOf(pageButtons.First(x => !x.interactable));
			currentButtonIndex = Mathf.Clamp(currentButtonIndex - 1, 0, 3);
			PressButton();
		}
	}

	private void PressButton()
	{
		pageButtons[currentButtonIndex].onClick.Invoke();
	}
}
