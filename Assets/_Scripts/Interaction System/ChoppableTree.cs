using Assets._Scripts.Audio;
using System.Collections;
using UnityEngine;

public class ChoppableTree : MonoBehaviour
{
	[SerializeField] private int logsInside;
	[SerializeField] private float neededForce;
	[SerializeField] private float chopProgress;
	[SerializeField] private float neededProgress;
	[SerializeField] private GameObject logsPrefab;
	[SerializeField] private GameObject chopParticle;
	[SerializeField] private ParticleSystem leavesParticle;
	[SerializeField] private float shakeDuration;
	private Vector3 startPosition;
	private Coroutine shakeRoutine;
	private AudioSource audio;

	[SerializeField] private Rigidbody rb;

	private void Start()
	{
		audio = GetComponent<AudioSource>();
		startPosition = transform.position;
	}

	private IEnumerator Shake()
	{
		float time = 0;
		while (time < shakeDuration)
		{
			time += Time.deltaTime;
			transform.position = startPosition + Random.insideUnitSphere/15f;
			yield return null;
		}
		transform.position = startPosition;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Axe")
		{
			StrikeWithForce(collision.impulse.magnitude, collision);
		}
	}

	public void StrikeWithForce(float force, Collision collision)
	{
		Debug.Log($"Struck with force: {force}");
		if (force > neededForce)
		{
			chopProgress += force;
			audio.pitch = Random.Range(0.75f, 1f);
			audio.PlayOneShot(AudioManager.Instance.WoodHit);
			Instantiate(chopParticle, collision.contacts[0].point, Quaternion.Inverse(collision.transform.rotation));

		    if (shakeRoutine != null)
			{
				StopCoroutine(shakeRoutine);
			}
			shakeRoutine = StartCoroutine(Shake());

			leavesParticle.Play();
		}
		if (chopProgress >= neededProgress && rb.constraints != RigidbodyConstraints.FreezeRotationY)
		{
			rb.constraints = RigidbodyConstraints.None;
			rb.constraints = RigidbodyConstraints.FreezeRotationY;
			rb.isKinematic = false;
			audio.PlayOneShot(AudioManager.Instance.TreeFall2);
			StartCoroutine(FallApart());
		}
	}

	private IEnumerator FallApart()
	{
		yield return new WaitForSeconds(10);
		Instantiate(logsPrefab, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}
