using Assets._Scripts.Audio;
using UnityEngine;

public class SmolaAccessor : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private int direction;
	[SerializeField] private float speed;
	[SerializeField] private Infuser infuser;

	[SerializeField] private ParticleSystem smolaFlow;
	[SerializeField] private ParticleSystem smolaDrawback;
	[SerializeField] private SmolaCollector collector;
	[SerializeField] private GameObject indicator;
	[SerializeField] private Material shader;
	private AudioSource source;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
		source.resource = AudioManager.Instance.ValveSpin;
		shader = indicator.GetComponent<MeshRenderer>().material;
		source.loop = true;
	}

	public void Update()
	{
		float delta = 0;
		if (rb.angularVelocity.z * speed * -1 < 0)
		{
			delta = PocketTicker.Instance.ChangeSmola(rb.angularVelocity.z * speed * -1, infuser);
			//Debug.Log(delta);
			GameManager.Instance.GetTutorial().ProgressTutorial("valveSpin");
			infuser.ChangeSmole(delta * -1);
		}
		else
		{
			delta = infuser.ChangeSmole(rb.angularVelocity.z * speed);
			PocketTicker.Instance.ChangeSmola(delta*-1,infuser);
		}
		//collector.Move(delta);
		//infuser.ChangeSmole(delta * direction);

		//shader.SetFloat("_FillPercentage", PocketTicker.Instance.GetSmola() / 50f);
		shader.SetFloat("_FillPercentage", 1f);
		if (Mathf.Abs(delta) > 0.1f && !source.isPlaying)
		{
			source.Play();
			source.loop = true;
		}
		else
		{
			source.loop = false;
		}
	}
}
