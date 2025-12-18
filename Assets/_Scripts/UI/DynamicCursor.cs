using UnityEngine;

public class DynamicCursor : MonoBehaviour
{
	[SerializeField] private RectTransform canvasRect;

	public void SetMouseVisibility(bool visible)
	{
		Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
	}

	private void FollowMouse()
	{
		Vector2 localPos;
		Vector2 mousePos = Input.mousePosition;

		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePos, Camera.main, out localPos))
		{
			transform.localPosition = localPos;
		}
	}

	public void Start()
	{
		SetMouseVisibility(false);
	}

	public void Update()
	{
		FollowMouse();
	}
}