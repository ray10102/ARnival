using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BlasterBullet : MonoBehaviour
{
	[SerializeField] private float speed = .5f;
	
	// Use this for initialization
	void Start ()
	{
		Collider col = GetComponent<Collider>();
		if (!(col && col.isTrigger))
		{
			Debug.LogWarning(gameObject.name + " does not have a trigger collider attached!");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.localPosition = transform.localPosition + Vector3.forward * speed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider col)
	{
		// Do something
		if (col.GetComponent<Impactable>())
		{
			gameObject.SetActive(false);
		}

		if (col.GetComponent<BlasterTarget>())
		{
			Debug.Log("Hit target");
			// TODO Handle target hit
		}
	}
}
