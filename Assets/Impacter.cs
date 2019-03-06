﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Impacter : MonoBehaviour
{
	[SerializeField]
	private AudioClip hitSound;
	
	// Use this for initialization
	void Start () {
		// Disable if no sound attached
		if (hitSound == null)
		{
			Debug.LogWarning(gameObject.name + ":Impacter has no hit sound attached. Disabling component.");
			this.enabled = false;
		}
		
	}
	
	void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<Impacter>())
		{
			
		}
	}
}