using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Blaster : MonoBehaviour
{
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private GameObject bulletsParent;
	[SerializeField] private float fireRate = .3f;

	private Vector3 bulletFirePosition;

	private Grabbable grabbable;

	private static Blaster instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		bulletFirePosition = bulletPrefab.transform.localPosition;
		bulletPrefab.SetActive(false);

		MLInput.OnTriggerDown += OnTriggerDown;
	}

	void Start()
	{
		grabbable = GetComponent<Grabbable>();
		if (!grabbable)
		{
			Debug.LogError(gameObject.name + ":Blaster does not have a grabbable attached!");
		}

		grabbable.OnGrabbableReleased += HandleGrabbableReleased;
	}

	public static void Reset()
	{
		instance.grabbable.Reset();
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
				child.gameObject.transform.localPosition = bulletFirePosition;
				child.gameObject.transform.localRotation = Quaternion.identity;
				child.gameObject.SetActive(true);
				return;
			}
		}

		// Instantiate new bullets if necessary
		GameObject bullet = Instantiate(bulletPrefab, bulletsParent.transform);
		bullet.transform.localPosition = bulletFirePosition;
		bullet.transform.localRotation = Quaternion.identity;
	}
}
