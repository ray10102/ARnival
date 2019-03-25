using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.XR.MagicLeap;

public class Ring : MonoBehaviour
{
	[SerializeField]
	private float ringRadius = .1f;
	
	private Grabbable grabbable;
	private float spawnDelay = 1f;

	[SerializeField]
	private GameObject ringPrefab_;

	private static GameObject ringPrefab;

	[SerializeField] private Transform ringsParent_;

	private static Transform ringsParent;

	private static Vector3 ringSpawnPosition;

	private Rigidbody rigid;

	private Vector3 lastPosition;
	private float lastPositionTime;

	private Vector3 velocity;

	[SerializeField]
	private Transform pegsParent;

	private bool scored;
	
	// Use this for initialization
	void Awake () {
		if (ringPrefab_ && ringPrefab == null)
		{
			ringPrefab = ringPrefab_;
		}

		if (ringsParent_ && ringsParent == null)
		{
			ringsParent = ringsParent_;
		}
		
		ringSpawnPosition = ringPrefab.transform.localPosition;
	}

	void Start()
	{
		rigid = GetComponent<Rigidbody>();
		grabbable = GetComponent<Grabbable>();
		if (!grabbable)
		{
			Debug.LogError(gameObject.name + ":Ring does not have a Grabbable attached!");
		}
		
		if (!rigid)
		{
			Debug.LogError(gameObject.name + ":Ring does not have a Rigidbody attached!");
		}

		grabbable.OnGrabbableReleased += OnRingReleased;
		grabbable.OnGrabbableGrabbed += StartSpawnNew;
		lastPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		velocity = (transform.position - lastPosition) / Time.deltaTime;
		lastPosition = transform.position;
		
		if (!rigid.isKinematic && rigid.velocity == Vector3.zero && !scored)
		{
			foreach (Transform peg in pegsParent) // NOT OPTIMIZED
			{
				if (Vector3.Distance(peg.position, transform.position) < ringRadius)
				{
					Score(peg.GetComponent<Bucket>());
					scored = true;
				}
			}
		}
	}

	private void Score(Bucket bucket)
	{
		if (bucket == null)
		{
			Debug.LogError("No peg found");
			return;
		}

		ScoreKeeper.instance.AddPoints((int) bucket.bucketType);
	}

	private void StartSpawnNew()
	{
		StartCoroutine("SpawnNewCoroutine");
	}

	private void OnRingReleased()
	{
		if (rigid && grabbable.isBeingGrabbed)
		{
			rigid.isKinematic = false;
			rigid.useGravity = true;
			rigid.constraints = RigidbodyConstraints.None;
			rigid.AddForce(velocity * 50f);
		}
	}

	IEnumerator SpawnNewCoroutine()
	{
		yield return new WaitForSeconds(spawnDelay);
		SpawnNew();
	}

	private void SpawnNew()
	{
		GameObject newRing = Instantiate(ringPrefab, ringsParent);
		newRing.transform.localPosition = ringSpawnPosition;
	}
}
