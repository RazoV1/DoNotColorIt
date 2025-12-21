using UnityEngine;

public class ObjectReturner : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float minY;
    [SerializeField] private Transform mirrorPoint;
    [SerializeField] private ParticleSystem smokeParticle;
    [SerializeField] private ParticleSystem impactParticle;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip impactSfx;

    private bool queuedSfx;

    [SerializeField] private float upY;

    private void CheckHeight()
    {
        if (transform.position.y < minY)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = new Vector3(-2,upY,8);
            smokeParticle.Play();
            queuedSfx = true;
        }
    }

	private void OnCollisionEnter(Collision collision)
	{
        if (!queuedSfx) return;

        //impactParticle.Play();
        smokeParticle.Stop();

        source.pitch = Random.Range(0.95f, 1.15f);
		source.PlayOneShot(impactSfx);
        queuedSfx = false;
	}

	private void Update()
	{
        CheckHeight();
	}
}