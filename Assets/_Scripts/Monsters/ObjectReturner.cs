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
            transform.position = new Vector3(-2,upY,8)- new Vector3(rb.linearVelocity.x * upY /10f,0,rb.linearVelocity.z * upY /10f);
        }
    }
	private void Update()
	{
        CheckHeight();
	}
}