using UnityEngine;
using UnityEngine.UI;

public class EscScreenCloser : MonoBehaviour
{
	[SerializeField] private Button closeButton;
	private void Update()
	{
		CheckInput();
	}

	private void CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			closeButton.onClick.Invoke();
		}
	}
}