using UnityEngine;

public class ObjectReturner : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float minY;
    [SerializeField] private Transform mirrorPoint;

    [SerializeField] private float upY;

    private void CheckHeight()
    {
        if (transform.position.y < minY)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = new Vector3(-2,upY,8);
        }
    }
	private void Update()
	{
        CheckHeight();
	}
}