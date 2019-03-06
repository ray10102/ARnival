using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Ring : MonoBehaviour
{
	private Grabbable grabbable;
	private float spawnDelay;

	[SerializeField]
	private GameObject ringPrefab;

	private Vector3 ringSpawnPosition;

	private Rigidbody rigid;
	
	// Use this for initialization
	void Awake () {
		ringSpawnPosition = ringPrefab.transform.localPosition;
	}

	void Start()
	{
		rigid = GetComponent<Rigidbody>();
		grabbable = GetComponent<Grabbable>();
		if (!grabbable)
		{
			Debug.LogError(gameObject.name + ":Blaster does not have a Grabbable attached!");
		}
		
		if (!rigid)
		{
			Debug.LogError(gameObject.name + ":Blaster does not have a Rigidbody attached!");
		}

		grabbable.OnGrabbableReleased += OnRingReleased;
		grabbable.OnGrabbableGrabbed += StartSpawnNew;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void StartSpawnNew()
	{
		StartCoroutine("SpawnNewCoroutine");
	}

	private void OnRingReleased()
	{
		if (rigid)
		{
			rigid.isKinematic = false;
			rigid.useGravity = true;
		}
	}

	IEnumerator SpawnNewCoroutine()
	{
		yield return new WaitForSeconds(spawnDelay);
		SpawnNew();
	}

	private void SpawnNew()
	{
		Instantiate(ringPrefab, ringSpawnPosition, Quaternion.identity);
	}
}
