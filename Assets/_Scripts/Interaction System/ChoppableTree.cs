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
	private AudioSource audio;

	[SerializeField] private Rigidbody rb;

	private void Start()
	{
		audio = GetComponent<AudioSource>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Axe")
		{
			StrikeWithForce(collision.impulse.magnitude,collision);
		}
	}

	public void StrikeWithForce(float force, Collision collision)
	{
		Debug.Log($"Struck with force: {force}");
		if (force > neededForce)
		{
			chopProgress += force;
			audio.pitch = Random.Range(0.75f,1f);
			audio.PlayOneShot(AudioManager.Instance.WoodHit);
			Instantiate(chopParticle, collision.contacts[0].point,Quaternion.Inverse(collision.transform.rotation));
		}
		if (chopProgress >= neededProgress && rb.constraints != RigidbodyConstraints.None)
		{
			rb.constraints = RigidbodyConstraints.None;
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
