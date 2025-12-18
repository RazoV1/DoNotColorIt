using System.Collections;
using UnityEngine;

public class GrowAGarden : MonoBehaviour
{
    [SerializeField] private float growthSpeed;
    [SerializeField] private GameObject vegetablePrefab;
    [SerializeField] private GameObject growingVegetablePrefab;
    [SerializeField] private Transform seedPivot;
    private Coroutine growthRoutine;

	private void OnCollisionEnter(Collision collision)
	{
        if ( collision.collider.tag != "Seed")
        {
            return;
        }
        if (growthRoutine == null)
        {
            Destroy(collision.gameObject);
            growthRoutine = StartCoroutine(Grow());
        }
    }

    private IEnumerator Grow()
    {
        GameObject growingItem = Instantiate(growingVegetablePrefab,seedPivot.position,Quaternion.identity);
        yield return new WaitForSeconds(growthSpeed);
        Destroy(growingItem);
        GameObject vegetable = Instantiate(vegetablePrefab,seedPivot.position,Quaternion.identity);
        ClearRoutine();
    }

    private void ClearRoutine()
    {
        growthRoutine = null;
    }
}
