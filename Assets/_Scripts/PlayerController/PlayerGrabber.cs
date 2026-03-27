using Assets._Scripts.Interaction_System.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Assets._Scripts.Delivery;
using Assets._Scripts.Interaction_System.Objects;
using TMPro;
using System.Linq;
using Assets._Scripts.Events;

public class PlayerGrabber : MonoBehaviour
{
	[Header("Grab stats")]
	[SerializeField] private float grabStrenght;
	[SerializeField] private float grabpStability;
	[SerializeField] private float fixedAxisStability;
	[SerializeField] private float maxGrabDistance;
	[SerializeField] private float minGrabDistance;

	[Header("PigmentStats")]
	[SerializeField] private GameObject pigmentStats;
	[SerializeField] private List<TextMeshProUGUI> pigmentFields;
	[Header("ItemStats")]
	[SerializeField] private GameObject itemStats;
	[SerializeField] private TextMeshProUGUI itemStat;
	[SerializeField] private bool shouldShowItemStat;
	[Header("Misc")]
	[SerializeField] private CameraController cameraController;
	[SerializeField] private Transform grabPivot;
	private Vector3 startingGrabPivotPoint;
	[SerializeField] private Transform fixedRotatorPivot;
	[SerializeField] private List<string> interactableTags = new List<string>();
	[SerializeField] private Rigidbody playerRb;
	private int preservedLayer;
	private bool isTalking = false;
	private Transform jointTransform;
	private Transform cameraPivotTransform;
	private PigmentMonster monsterObj;

	private bool isGrabbing = false;
	[SerializeField] private InteractableObject grabbedObject;

	public bool GetIsGrabbing() => isGrabbing;

	public Transform getGrabPivot() => grabPivot;

	public InteractableObject GetGrabbedObject() => grabbedObject;

	public void SetIsTalking(bool isTalking) { this.isTalking = isTalking; }

	private void Start()
	{
		cameraPivotTransform = cameraController.transform;
		startingGrabPivotPoint = grabPivot.transform.localPosition;
	}
	private bool CheckNPC(RaycastHit hit)
	{
		if (hit.collider.tag != "Npc")
		{
			foreach (NPCWaiter n in FindObjectsOfType<NPCWaiter>())
			{
				n.isPlayerInTrigger = false;
			}
			return false;
		}
		if (isTalking) return false;

		NPCWaiter npc = hit.collider.GetComponentInParent<NPCWaiter>();

		if (npc != null) npc.isPlayerInTrigger = true;


		return true;
	}

	private void CastNpc()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit) && Vector3.Distance(hit.point, ray.origin) <= maxGrabDistance)
		{
			CheckNPC(hit);
		}
	}

	/// <summary>
	/// Son 😭😭😭
	/// </summary>
	private void TryGrab()
	{
		if (isGrabbing)
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		//Debug.Log("Cast!");
		//if (Physics.Raycast(cameraPivotTransform.position, cameraPivotTransform.forward * maxGrabDistance, out hit))
		if (Physics.Raycast(ray, out hit))
		{
			//Debug.Log("Hit!");
			ConditionalGrabbable conditional = hit.collider.GetComponent<ConditionalGrabbable>();
			//CheckNPC(hit);
			if (conditional != null)
			{
				if (!conditional.GetCanBeGrabbed()) return;
			}

			if (!interactableTags.Contains(hit.collider.tag) || Vector3.Distance(cameraPivotTransform.position, hit.point) > maxGrabDistance)
			{
				return;
			}
			//Debug.Log("Target!");
			InteractableObject target = hit.collider.GetComponent<InteractableObject>();
			if (target is FixedAxis)
			{
				grabbedObject = target;
				isGrabbing = true;
				((IGrabbable)grabbedObject).SetIsGrabbed(true, cameraPivotTransform);
				((IGrabbable)grabbedObject).SetMaintainDirection(true);
				jointTransform = AttachJoint(target.GetRigidbody(), hit.point, fixedRotatorPivot, true);
			}
			else if (target is IGrabbable)
			{
				if (hit.collider.tag == "Egg")
				{
					hit.collider.GetComponent<MonsterEgg>().TryOpeningHatchMenu();
					GameManager.Instance.GetTutorial().ProgressTutorial("eggPress");
				}
				else if (hit.collider.tag == "Kapot")
				{
					GameManager.Instance.GetTutorial().ProgressTutorial("openTrunk");
				}
				else if (hit.collider.name.ToLower().Contains("axe"))
				{
					GameManager.Instance.GetTutorial().ProgressTutorial("axePick");
				}
				else if (hit.collider.tag == "Sponge")
				{
					GameManager.Instance.GetTutorial().ProgressTutorial("brushPick");
				}
				grabbedObject = target;
				isGrabbing = true;
				((IGrabbable)grabbedObject).SetIsGrabbed(true, cameraPivotTransform);
				((IGrabbable)grabbedObject).SetMaintainDirection(true);
				preservedLayer = grabbedObject.gameObject.layer;
				grabbedObject.gameObject.layer = grabbedObject.gameObject.layer == 3 ? 8 : 2;

				var pigmentobj = grabbedObject.GetComponent<ColorPigment>();
				if (pigmentobj != null)
				{
					pigmentStats.SetActive(true);
				}
				//jointTransform = AttachJoint(target.GetRigidbody(), hit.point, grabPivot, false);
				jointTransform = AttachJoint(target.GetRigidbody(), (target.tag.Equals("Kapot") ? hit.point : target.GetRigidbody().worldCenterOfMass), grabPivot, false);
			}
		}
	}

	private void TryEnter()
	{
		if (isGrabbing)
		{
			return;
		}
		RaycastHit hit;
		if (Physics.Raycast(cameraPivotTransform.position, cameraPivotTransform.forward * maxGrabDistance, out hit))
		{
			if (!interactableTags.Contains(hit.collider.tag) && Vector3.Distance(cameraPivotTransform.position, hit.point) < maxGrabDistance)
			{
				if (cameraController.GetShouldRotate() && hit.collider.tag == "Fiat" && GameManager.Instance.GetTutorial().GetTutorialIndex() >= 18)
				{
					cameraController.MountFiat(hit.collider.GetComponentInParent<MetlaController>());
				}
				if (cameraController.GetShouldRotate() && hit.collider.tag == "Portal" && GameManager.Instance.GetTutorial().GetTutorialIndex() >= 5)
				{
					//GameManager.Instance.GetTutorial().ProgressTutorial(4);
					GameManager.Instance.GetTutorial().ProgressTutorial("jump");
					GameManager.Instance.ChangeDimensions(2);
				}
				return;
			}
		}
	}

	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			TryGrab();
		}
		else if (isGrabbing && Input.GetMouseButton(0))
		{
			HandleObjectHolding();
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			TryEnter();
		}
		else
		{
			StopGrabbing();
		}
		GrabbableFrameCheck();
	}

	private void GrabbableFrameCheck()
	{
		if (grabbedObject == null)
		{
			grabPivot.transform.localPosition = startingGrabPivotPoint;
			return;
		}
		if (!((IGrabbable)grabbedObject).GetIsGrabbed())
		{
			((IGrabbable)grabbedObject).SetIsGrabbed(false);
			StopGrabbing();
		}
	}

	public void StopGrabbing()
	{
		if (grabbedObject == null)
		{
			isGrabbing = false;
			return;
		}
		//grabbedObject.GetRigidbody().useGravity = true;
		((IGrabbable)grabbedObject).SetIsGrabbed(false);
		((IGrabbable)grabbedObject).SetMaintainDirection(false);
		grabbedObject.gameObject.layer = preservedLayer;
		try
		{
			Destroy(jointTransform.gameObject);
			//var monsterObj = grabbedObject.GetComponent<PigmentMonster>();
			//if (monsterObj != null)
			//{
			//	monsterStats.SetActive(false);
			//}
			var pigmentObj = grabbedObject.GetComponent<ColorPigment>();
			if (pigmentObj != null)
			{
				pigmentStats.SetActive(false);
			}

		}
		catch { }
		Physics.IgnoreCollision(GetComponent<Collider>(), grabbedObject.GetRigidbody().GetComponent<Collider>(), false);
		grabbedObject = null;
		isGrabbing = false;
		Destroy(jointTransform.gameObject);
	}

	private void LateUpdate()
	{
		HandleInput();
		CastNpc();
	}

	private JointDrive CreateJoint(bool useFixed, bool isKapot = false)
	{
		JointDrive drive = new JointDrive();
		drive.positionSpring = grabStrenght * (isKapot ? 1 : 10);
		drive.positionDamper = useFixed ? fixedAxisStability : grabpStability * (isKapot ? 1 : 10);
		drive.maximumForce = Mathf.Infinity;
		return drive;
	}

	private Transform AttachJoint(Rigidbody rb, Vector3 attachmentPosition, Transform goParent, bool useFixed)
	{
		//GameObject go = new GameObject("Attachment Point");
		GameObject go = new GameObject("Attachment Point");
		//go.hideFlags = HideFlags.HideInHierarchy;

		bool isKapot = rb.gameObject.tag == "Kapot";
		Debug.Log(isKapot);

		if (isKapot)
		{
			goParent.position = attachmentPosition;
			grabPivot.transform.position = attachmentPosition;
		}
		if (useFixed) //O sorrow
		{
			goParent.position = attachmentPosition;
			grabPivot.transform.position = attachmentPosition;
			Vector3 rotAxis = ((FixedAxis)grabbedObject).GetRotationAxis();
			Vector3 rotCenter = ((FixedAxis)grabbedObject).GetRotationCenter();
			Plane rotationPlane = new Plane(grabbedObject.transform.TransformDirection(rotAxis), rotCenter);
			fixedRotatorPivot.position = rotCenter + ((rotationPlane.ClosestPointOnPlane(grabPivot.position) - rotCenter).normalized * ((FixedAxis)grabbedObject).GetRotationRadius());
			//fixedRotatorPivot.eulerAngles = new Vector3(fixedRotatorPivot.eulerAngles.x * rotAxis.x,fixedRotatorPivot.eulerAngles.y * rotAxis.y,fixedRotatorPivot.eulerAngles.z * rotAxis.z);
			fixedRotatorPivot.rotation = Quaternion.LookRotation((fixedRotatorPivot.position - rotCenter).normalized, grabbedObject.transform.up);
		}
		go.transform.parent = goParent;
		go.transform.localPosition = Vector3.zero;
		try
		{
			var newRb = go.GetComponent<Rigidbody>();
			newRb.isKinematic = true;
		}
		catch
		{
			var newRb = go.AddComponent<Rigidbody>();
			newRb.isKinematic = true;
		}
		go.transform.position = attachmentPosition;
		var joint = go.AddComponent<ConfigurableJoint>();

		PivotedItem pivoted = rb.GetComponent<PivotedItem>();

		if (pivoted != null)
		{
			go.transform.position = rb.transform.position;
			go.transform.rotation = rb.transform.rotation;
		}
		joint.connectedBody = rb;
		joint.configuredInWorldSpace = true;
		joint.breakForce = 10f;
		joint.xDrive = CreateJoint(useFixed, isKapot);
		joint.yDrive = CreateJoint(useFixed, isKapot);
		joint.zDrive = CreateJoint(useFixed, isKapot);
		joint.slerpDrive = CreateJoint(useFixed, isKapot);
		joint.rotationDriveMode = RotationDriveMode.Slerp;
		go.transform.localPosition = Vector3.zero;
		if (pivoted != null)
		{
			Debug.Log("Grabbing Pivoted");
			Transform pivot = pivoted.GetPivot();
			go.transform.localRotation = pivot.localRotation;
			go.transform.position = pivot.position;
		}
		return go.transform;
	}

	public float GetGrabDistance() => maxGrabDistance;

	private void HandleObjectHolding()
	{
		//Rigidbody rb = grabbedObject.GetRigidbody();
		//rb.useGravity = false;
		//Vector3 direction = (grabPivot.position - grabbedObject.transform.position);
		//rb.AddForce(direction * grabStrenght * Time.deltaTime, ForceMode.Force);
		//grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position,grabPivot.position, Time.deltaTime * Mathf.Clamp(grabStrenght - rb.mass,0,grabStrenght));
		if (grabbedObject is FixedAxis)
		{
			Vector3 rotAxis = ((FixedAxis)grabbedObject).GetRotationAxis();
			Vector3 rotCenter = ((FixedAxis)grabbedObject).GetRotationCenter();
			Plane rotationPlane = new Plane(grabbedObject.transform.TransformDirection(rotAxis), rotCenter);
			fixedRotatorPivot.position = rotCenter + ((rotationPlane.ClosestPointOnPlane(grabPivot.position) - rotCenter).normalized * ((FixedAxis)grabbedObject).GetRotationRadius());
			//fixedRotatorPivot.eulerAngles = new Vector3(fixedRotatorPivot.eulerAngles.x * rotAxis.x,fixedRotatorPivot.eulerAngles.y * rotAxis.y,fixedRotatorPivot.eulerAngles.z * rotAxis.z);
			fixedRotatorPivot.rotation = Quaternion.LookRotation((fixedRotatorPivot.position - rotCenter).normalized, grabbedObject.transform.up);
		}
		else
		{
			jointTransform.position = grabPivot.position;
		}

		if (grabbedObject == null) { return; }

		if (!cameraController.GetShouldRotate() && !GetComponent<PlayerController>().GetCanWalk())
		{
			Debug.Log("Projecting");
			Plane grabbedPlane = new Plane(cameraController.transform.forward, grabbedObject.transform.position);
			grabPivot.transform.position = grabbedPlane.ClosestPointOnPlane(Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(Vector3.Distance(grabbedObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)))); //(Vector3.Distance(grabbedObject.transform.position,Camera.main.ScreenToWorldPoint(Input.mousePosition)));
		}

		Physics.IgnoreCollision(GetComponent<Collider>(), grabbedObject.GetRigidbody().GetComponent<Collider>(), true);
		//var monsterObj = grabbedObject.GetComponent<PigmentMonster>();

		var colorPig = grabbedObject.GetComponent<ColorPigment>();
		if (colorPig != null)
		{
			pigmentFields[0].text = LanguageManager.Instance.GetTranslatable("ui.pigment_stats.color") + $"{colorPig.GetColor().maxColorComponent}";
			pigmentFields[1].text = LanguageManager.Instance.GetTranslatable("ui.pigment_stats.volume") + $"{colorPig.GetVolume()}";
		}
	}

}