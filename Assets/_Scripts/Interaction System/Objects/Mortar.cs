using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class Mortar : MonoBehaviour
{
	[SerializeField] private List<ColorPigment> pigmentInside;
	[SerializeField] private GameObject dustPrefab;
	[SerializeField] private Transform componentsPivot;
	[SerializeField] private float forceThreashhold;
	[SerializeField] private ParticleSystem particleSystem;

	private DustedColorPigment output;
	[SerializeField] private float stompByForceCoef;

	[SerializeField] private float progress = 0f;

	private void CalculateParticleColors()
	{
		particleSystem.startColor = GetMiddleColor();
	}

	public void Stomp(float force)
	{
		Debug.Log($"Stomp with force {force}!");
		if (force >= forceThreashhold)
		{
			CalculateParticleColors();
			particleSystem.Play();
			progress = Mathf.Clamp(progress + stompByForceCoef, 0, 1f);
		}
		if (progress == 1f)
		{
			Transmutate();
		}
	}

	private void Transmutate()
	{
		Color outputColor = GetMiddleColor();
		float volume = pigmentInside.Sum(x => x.GetVolume());
		foreach (ColorPigment p in pigmentInside)
		{
			Destroy(p.gameObject);
		}

		GameManager.Instance.GetTutorial().ProgressTutorial(7);
		pigmentInside.Clear();
		GameObject dust = Instantiate(dustPrefab, componentsPivot.position, Quaternion.identity);
		dust.GetComponent<DustedColorPigment>().InitializePigment(outputColor, volume);
		progress = 0;
	}

	private Color GetMiddleColor()
	{
		float outputR = 0f;
		float outputG = 0f;
		float outputB = 0f;
		float combinedVolume = 0f;
		float totalBlackVolume = 0f;

		foreach (ColorPigment pigment in pigmentInside)
		{
			Color pigmentColor = pigment.GetColor();
			float volume = pigment.GetVolume();

			if (pigmentColor.r < 0.01f && pigmentColor.g < 0.01f && pigmentColor.b < 0.01f)
			{
				totalBlackVolume += volume;
			}

			outputR += pigmentColor.r * volume;
			outputG += pigmentColor.g * volume;
			outputB += pigmentColor.b * volume;

			combinedVolume += volume;
		}

		float avgR = outputR / combinedVolume;
		float avgG = outputG / combinedVolume;
		float avgB = outputB / combinedVolume;

		float maxAvgChannel = Mathf.Max(avgR, avgG, avgB);

		float factor = 1.0f / maxAvgChannel;

		outputR = avgR * factor;
		outputG = avgG * factor;
		outputB = avgB * factor;

		float blackVolumeRatio = 1 - (totalBlackVolume / combinedVolume);

		outputR *= blackVolumeRatio;
		outputG *= blackVolumeRatio;
		outputB *= blackVolumeRatio;

		return new Color(outputR, outputG, outputB);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Pounder" && pigmentInside.Count >= 1)
		{
			Stomp(collision.impulse.y);
			return;
		}

		ColorPigment pigment = collision.collider.GetComponent<ColorPigment>();
		if (pigment != null && progress == 0)
		{
			pigmentInside.Add(pigment);
			pigment.transform.parent = transform;
			pigment.GetRigidbody().constraints = RigidbodyConstraints.FreezePositionX;
			pigment.GetRigidbody().constraints = RigidbodyConstraints.FreezePositionZ;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		ColorPigment pigment = collision.collider.GetComponent<ColorPigment>();
		if (pigment == null)
		{
			return;
		}
		if (pigmentInside.Contains(pigment))
		{
			pigmentInside.Remove(pigment);
			pigment.transform.parent = null;
			progress = 0;
			pigment.GetRigidbody().constraints = RigidbodyConstraints.None;
		}
	}
}