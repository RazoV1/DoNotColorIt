using Assets._Scripts.Interaction_System.Interfaces;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
	[SerializeField] protected float mass;
	[SerializeField] protected Rigidbody rb;

	public float GetMass() => rb.mass;

	public Rigidbody GetRigidbody() => rb;
}