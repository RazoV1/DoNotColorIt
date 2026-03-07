
using Assets._Scripts.Interaction_System.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSnapper : MonoBehaviour
{
	[SerializeField] private List<string> snappableTags = new List<string>();
	[SerializeField] private Transform snapPlace;
	[SerializeField] private BasicItem snappedItem;
	[SerializeField] private PlayerGrabber grabber;

	private Coroutine waitorRoutine;

	private void Start()
	{
		if (snappedItem != null)
		{
			waitorRoutine = StartCoroutine(Waitor());
		}
	}

	private IEnumerator Waitor()
	{
		while (!snappedItem.GetIsGrabbed())
		{
			snappedItem.transform.localPosition = Vector3.zero;
			snappedItem.transform.localRotation = Quaternion.identity;
			yield return null;
		}
		Debug.Log("<color=blue>GrabbedItem");
		snappedItem.GetComponent<Rigidbody>().isKinematic = false;
		snappedItem.transform.parent = null;
		snappedItem = null;
		yield return new WaitForSeconds(2f);
		waitorRoutine = null;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (snappableTags.Contains(collision.collider.tag) && snapPlace.childCount == 0 && waitorRoutine == null)
		{
			if (collision.collider.GetComponent<BasicItem>().GetIsGrabbed())
			{
				grabber.StopGrabbing();
			}
			collision.rigidbody.isKinematic = true;
			
			snappedItem = collision.collider.GetComponent<BasicItem>();
			collision.transform.parent = snapPlace;
			collision.transform.position = snapPlace.position;
			collision.transform.rotation = snapPlace.transform.rotation;
			waitorRoutine = StartCoroutine(Waitor());
		}
	}

    private void OnTriggerEnter(Collider collision)
    {
        if (snappableTags.Contains(collision.gameObject.tag) && snapPlace.childCount == 0 && waitorRoutine == null)
        {
            if (collision.gameObject.GetComponent<BasicItem>().GetIsGrabbed())
            {
                grabber.StopGrabbing();
            }
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            snappedItem = collision.gameObject.GetComponent<BasicItem>();
            collision.transform.parent = snapPlace;
            collision.transform.position = snapPlace.position;
            collision.transform.rotation = snapPlace.transform.rotation;
            waitorRoutine = StartCoroutine(Waitor());
        }
    }
}
