using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Blaster : MonoBehaviour
{
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private GameObject bulletsParent;
	[SerializeField] private float fireRate = .3f;
	[SerializeField] private AudioClip fireSound;

	private AudioSource audioSource;
	
	private ControllerConnectionHandler controllerConnectionHandler;

	private Vector3 bulletFirePosition
	{
		get { return bulletPrefab.transform.position; }
	}
	
	private Quaternion bulletFireRotation
	{
		get { return bulletPrefab.transform.rotation; }
	}

	private Grabbable grabbable;

	private static Blaster instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		bulletPrefab.SetActive(false);

		MLInput.OnTriggerDown += OnTriggerDown;
		MLInput.OnTriggerUp += OnTriggerUp;
	}

	void Start()
	{
		grabbable = GetComponent<Grabbable>();
		if (!grabbable)
		{
			Debug.LogError(gameObject.name + ":Blaster does not have a grabbable attached!");
		}

		grabbable.OnGrabbableReleased += HandleGrabbableReleased;

		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Fire();
		}
	}

	private void OnTriggerDown(byte controller_id, float value)
	{
		if (grabbable.isBeingGrabbed)
		{
			StartCoroutine("FireBlasterCoroutine");
		}
	}

	private void OnTriggerUp(byte controller_id, float value)
	{
		StopCoroutine("FireBlasterCoroutine");
	}

	public void HandleGrabbableReleased()
	{
		StopCoroutine("FireBlasterCoroutine");
	}

	IEnumerator FireBlasterCoroutine()
	{
		while (true)
		{
			Fire();
			yield return new WaitForSeconds(fireRate);
		}
	}

	private void Fire()
	{
		foreach (Transform child in bulletsParent.transform)
		{
			if (!child.gameObject.activeSelf)
			{
				// Try to use inactivated bullets instead of instantiating new ones
				child.gameObject.transform.position = bulletFirePosition;
				child.gameObject.transform.rotation = bulletFireRotation;
				child.gameObject.SetActive(true);
				audioSource.PlayOneShot(fireSound);
				return;
			}
		}

		// Instantiate new bullets if necessary
		audioSource.PlayOneShot(fireSound);
		GameObject bullet = Instantiate(bulletPrefab, bulletsParent.transform);
		bullet.SetActive(true);
		bullet.transform.position = bulletFirePosition;
		bullet.transform.rotation = bulletFireRotation;
	}
}
