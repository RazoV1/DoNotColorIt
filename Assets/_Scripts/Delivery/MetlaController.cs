using Assets._Scripts.Audio;
using Assets._Scripts.Events;
using Assets._Scripts.Game.SaveSystem;
using Assets._Scripts.PlayerController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Delivery
{
	public class MetlaController : MonoBehaviour, ISavable
	{
		[Header("!!!ЛЮТИЙ ДРИФТ МОД!!!")]
		[SerializeField] private bool lutiiDrift;
		[SerializeField] private bool shouldAutocorrect;
		[Header("Настройки")]
		[SerializeField] private GameObject kapot;
		[SerializeField] private float maxSpeed;
		[SerializeField] private float acceleration;
		[SerializeField] private float currentSpeed;
		[SerializeField] private Transform pivot;
		[SerializeField] private List<Collider> collidersToCull;
		[SerializeField] private float lift;
		[SerializeField] private float upAmount;

		private AudioSource source;
		private CameraController rider;

		[SerializeField] private float responsiveness;

		[SerializeField] private float yawAmount;
		[SerializeField] private float rollAmoutn;
		[SerializeField] private float pitchAmount;
		[SerializeField] private Color color;

		[SerializeField] private float autocorrectAmount;

		private Rigidbody rb;
		private float verticalInput;
		private float preferedSpeed;
		private Vector2 preferredRotation;
        private float mouseVerticalInput;

        [SerializeField] private bool isMounted = false;

		[SerializeField] private PlayerHinter hinter;

		public Transform GetPivot() => pivot;

		private void ToggleHints()
		{
			Debug.Log($"<color=yellow>{!isMounted}");
			hinter.SetShouldHint(!isMounted);
		}

		public void SetIsMounted(bool isMounted, CameraController rider = null)
		{
			if (isMounted)
			{
				GameplayEvents.OnMount.Invoke();
				GameManager.Instance.GetTutorial().ProgressTutorial("carSit");
			}
			rb.useGravity = !isMounted;
			this.isMounted = isMounted;

			ToggleHints();

			kapot.transform.localEulerAngles = Vector3.zero;
			kapot.GetComponent<MeshCollider>().enabled = !isMounted;
			collidersToCull.ForEach(x => x.enabled = !isMounted);
			if (this.rider != null && !isMounted)
			{
				this.rider.UnmountFiat(this);
				this.rider = null;
			}
			else
			{
				this.rider = rider;
			}
		}

		private void Awake()
		{
			SubscribeToSaveEvent();
		}

		private void TiltAutocorrect()
		{
			if (!shouldAutocorrect) return;
			Quaternion currentQuaternion = transform.rotation;
			Quaternion neededQuaternion = Quaternion.Euler(currentQuaternion.eulerAngles.x, currentQuaternion.eulerAngles.y, 0);

		    transform.rotation = Quaternion.Lerp(currentQuaternion, neededQuaternion, Time.deltaTime * autocorrectAmount);
		}

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			SyncData(new SavablePrefab());
			Cursor.lockState = CursorLockMode.Locked;
			source = GetComponent<AudioSource>();
			source.resource = AudioManager.Instance.CarFlight;
			source.Play();
			kapot.transform.localEulerAngles = Vector3.zero;
		}

		private void HandleSound()
		{
			source.volume = Mathf.Lerp(source.volume, Mathf.Clamp(currentSpeed / maxSpeed, 1, 1.25f), Time.deltaTime * 3f);
			source.pitch = Mathf.Lerp(source.pitch, Mathf.Clamp(currentSpeed / maxSpeed + 0.5f, 0.6f, 1.1f), Time.deltaTime);
		}

		private void HandleFlight()
		{
            rb.AddForce(-transform.forward * currentSpeed);
            rb.AddTorque(transform.up * yawAmount * preferredRotation.x * responsiveness);
            rb.AddTorque(transform.right * pitchAmount * preferredRotation.y * responsiveness);
            rb.AddForce(-transform.forward * maxSpeed * (verticalInput < 0 ? verticalInput : 0));

            rb.AddForce(transform.up * (Input.GetKey(KeyCode.Space) ? 1f : 0f) * upAmount);
            rb.AddForce(-transform.up * (Input.GetKey(KeyCode.LeftControl) ? 1f : 0f) * upAmount);
            //rb.AddForce(-transform.forward * currentSpeed);
            //rb.AddTorque(transform.up * yawAmount * preferredRotation.x * responsiveness);
            //rb.AddTorque(transform.right * pitchAmount * preferredRotation.y * responsiveness);
            //rb.AddTorque(transform.forward * rollAmoutn * Input.GetAxis("Horizontal") * responsiveness);
            //rb.AddForce(-transform.forward * maxSpeed * (verticalInput < 0 ? verticalInput : 0));
            //rb.linearDamping = currentSpeed / maxSpeed;

            if (lutiiDrift)
			{
				rb.linearDamping = currentSpeed / maxSpeed;
				rb.AddForce(transform.up * (currentSpeed / maxSpeed) * lift);
				rb.AddForce(Vector3.down * 10f * rb.mass * (1 - currentSpeed / maxSpeed));
			}
			else
			{
				rb.AddForce(transform.up * (Input.GetKey(KeyCode.Space) ? 1f : 0f) * upAmount);
				rb.AddForce(-transform.up * (Input.GetKey(KeyCode.LeftControl) ? 1f : 0f) * upAmount);
			}
		}

		private void HandleInput()
		{
			verticalInput = Input.GetAxis("Vertical");
			preferedSpeed = Mathf.Clamp(maxSpeed * verticalInput, 0, maxSpeed);
			currentSpeed = Mathf.Lerp(currentSpeed, preferedSpeed, Time.deltaTime * acceleration);
            mouseVerticalInput = Input.GetAxis("Mouse Y");
            preferredRotation = new Vector2(Input.GetAxis("Horizontal"), mouseVerticalInput * 10);
        }

		private void CheckUnmount()
		{
			if (!isMounted) return;
			if (Input.GetKeyDown(KeyCode.E) && rb.linearVelocity.magnitude < 2f)
			{
				RaycastHit h;
				if (!Physics.Raycast(transform.position, transform.up * -3, out h) || h.collider.gameObject.layer == 4)
				{
					return;
				}
				SetIsMounted(false);
			}
		}

		private void FixedUpdate()
		{
			kapot.transform.localEulerAngles = new Vector3(kapot.transform.localEulerAngles.x, 0, 0);
			HandleSound();
			if (!isMounted) return;
			HandleInput();
			HandleFlight();
			TiltAutocorrect();
		}

		private void Update()
		{
			CheckUnmount();
		}

		public void SubscribeToSaveEvent()
		{
			SaveEvents.OnSaveEvent.AddListener(SaveData);
		}

		public void SaveData()
		{
			SaveManager saveManager = SaveManager.Instance;
			saveManager.SaveFloat("fiatX", transform.position.x);
			saveManager.SaveFloat("fiatY", transform.position.y);
			saveManager.SaveFloat("fiatZ", transform.position.z);

			saveManager.SaveFloat("fiatQX", transform.rotation.x);
			saveManager.SaveFloat("fiatQY", transform.rotation.y);
			saveManager.SaveFloat("fiatQZ", transform.rotation.z);
			saveManager.SaveFloat("fiatQW", transform.rotation.w);

			saveManager.SaveFloat("isMounted", isMounted ? 1f : 0f);
		}

		public void SyncData(SavablePrefab data)
		{
			SaveManager saveManager = SaveManager.Instance;

			float x = saveManager.GetFloat("fiatX");
			float y = saveManager.GetFloat("fiatY");
			float z = saveManager.GetFloat("fiatZ");

			float qx = saveManager.GetFloat("fiatQX");
			float qy = saveManager.GetFloat("fiatQY");
			float qz = saveManager.GetFloat("fiatQZ");
			float qw = saveManager.GetFloat("fiatQW");

			Vector3 savedPos = new Vector3(x, y, z);
			Quaternion savedRotation = new Quaternion(qx, qy, qz, qw);

			if (qx == qw && qz == qy && qw == qy)
			{
				savedRotation = new Quaternion(0f, -0.0026346636f, 0, 0.999996603f);
			}
			if (x == y && x == z && z == 0)
			{
				return;
			}
			this.isMounted = saveManager.GetFloat("isMounted") == 1f;
			if (isMounted)
			{
				GameObject.FindObjectOfType<CameraController>().MountFiat(this);
				ToggleHints();
			}


			try
			{
				collidersToCull.ForEach(x => x.enabled = false);
				rb.interpolation = RigidbodyInterpolation.None;
				rb.MovePosition(savedPos);
				rb.MoveRotation(savedRotation);
				Debug.Log($"Set pos {x} {y} {z}");


				kapot.transform.localEulerAngles = Vector3.zero;

				rb.interpolation = RigidbodyInterpolation.Interpolate;

				collidersToCull.ForEach(x => x.enabled = true);
			}
			catch
			{
				Debug.Log("<color=red>Не нашли сейв фиата!");
			}
		}
	}
}