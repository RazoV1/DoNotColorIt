
using System.Collections;
using UnityEngine;

public class TextAnimations : MonoBehaviour
{
	private Coroutine moveTextRoutine;
	private Coroutine moveImageRoutine;

	public void StartMoveText(Vector2 pos, Transform text,float cameraFollowSpeed)
	{
		if (moveTextRoutine != null)
		{
			StopCoroutine(moveTextRoutine);
		}
		moveTextRoutine = StartCoroutine(MoveText(pos, text, cameraFollowSpeed));
	}

	private IEnumerator MoveText(Vector2 pos, Transform text, float cameraFollowSpeed)
	{
		while (Vector2.Distance(pos, text.localPosition) > 0.2f)
		{
			text.localPosition = Vector2.Lerp(text.localPosition, pos, Time.deltaTime * cameraFollowSpeed);
			yield return null;
		}
		text.localPosition = pos;
	}

	public void StartMoveImage(Vector2 pos, Transform text, float cameraFollowSpeed)
	{
		if (moveImageRoutine != null)
		{
			StopCoroutine(moveImageRoutine);
		}
		moveImageRoutine = StartCoroutine(MoveText(pos, text, cameraFollowSpeed));
	}
}
