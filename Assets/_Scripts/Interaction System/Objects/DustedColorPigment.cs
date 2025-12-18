using UnityEngine;

public class DustedColorPigment : ColorPigment
{
	public override void Start()
	{
		SubscribeToSaveEvent();
	}

	public override void InitializePigment(Color color, float volume)
	{
		base.InitializePigment(color, volume);
	}
}
