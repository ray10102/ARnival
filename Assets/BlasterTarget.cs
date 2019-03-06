using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BlasterTarget : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		Collider col = GetComponent<Collider>();
		if (!(col && col.isTrigger))
		{
			Debug.LogWarning(gameObject.name + " does not have a trigger collider attached!");
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		// Do something
		if (col.GetComponent<Impactable>())
		{
			// Play sound
		}

		if (col.GetComponent<BlasterBullet>())
		{
			gameObject.SetActive(false);
		}
	}
}
