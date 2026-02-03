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
	[SerializeField] private float neededProgress;

	private void CalculateParticleColors()
	{
		particleSystem.startColor = GetMiddleColor();
	}

	public void Stomp(float force)
	{
		force = Mathf.Abs(force);
		Debug.Log($"Stomp with force {force}!");
		if (force >= forceThreashhold)
		{
			CalculateParticleColors();
			particleSystem.Play();
			progress = Mathf.Clamp(progress + stompByForceCoef, 0, neededProgress);
		}
		if (progress == neededProgress)
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

		//GameManager.Instance.GetTutorial().ProgressTutorial(7);
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

            // 1. Смягчение влияния объема
            float softenedVolume = Mathf.Pow(volume, 0.6f);

            // Учет черного пигмента
            if (pigmentColor.r < 0.01f && pigmentColor.g < 0.01f && pigmentColor.b < 0.01f)
            {
                totalBlackVolume += softenedVolume;
            }

            outputR += pigmentColor.r * softenedVolume;
            outputG += pigmentColor.g * softenedVolume;
            outputB += pigmentColor.b * softenedVolume;

            combinedVolume += softenedVolume;
        }

        if (combinedVolume <= 0f)
            return Color.black;
        float avgR = outputR / combinedVolume;
        float avgG = outputG / combinedVolume;
        float avgB = outputB / combinedVolume;

        float maxAvgChannel = Mathf.Max(avgR, avgG, avgB);
        if (maxAvgChannel > 0f)
        {
            float softNormalizeFactor = Mathf.Lerp(1f, 1f / maxAvgChannel, 0.4f);
            avgR *= softNormalizeFactor;
            avgG *= softNormalizeFactor;
            avgB *= softNormalizeFactor;
        }
        float blackVolumeRatio = 1f - (totalBlackVolume / combinedVolume);
        float blackInfluence = Mathf.Pow(blackVolumeRatio, 0.7f);

        avgR *= blackInfluence;
        avgG *= blackInfluence;
        avgB *= blackInfluence;

        return new Color(
            Mathf.Clamp01(avgR),
            Mathf.Clamp01(avgG),
            Mathf.Clamp01(avgB)
        );
    }


    public void OnCollisionEnter(Collision collision)
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
			pigment.SetListenForPound(true,this);
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

			pigment.SetListenForPound(false);
			pigment.transform.parent = null;
			progress = 0;
			pigment.GetRigidbody().constraints = RigidbodyConstraints.None;
		}
	}
}