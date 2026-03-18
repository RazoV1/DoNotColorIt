using UnityEngine;

public class ObjectsReruenerFromDown : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Sponge"))
        {
            other.gameObject.transform.position = new Vector3(other.gameObject.transform.position.x, 3f, other.gameObject.transform.position.z);
        }
    }
}
