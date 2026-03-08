using UnityEngine;

public class NestChecker : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Egg"))
        {
            other.gameObject.GetComponent<MonsterEgg>().StartTickingInFence();
        }
    }
}
