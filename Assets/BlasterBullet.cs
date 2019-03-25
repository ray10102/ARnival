using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BlasterBullet : MonoBehaviour
{
	[SerializeField] private float speed = 1.5f;

	private float spawnTime;
	
	// Use this for initialization
	void Start ()
	{
		Collider col = GetComponent<Collider>();
		if (!(col && col.isTrigger))
		{
			Debug.LogWarning(gameObject.name + " does not have a trigger collider attached!");
		}
	}

	void OnEnable()
	{
		spawnTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.time - spawnTime > 5f)
		{
			gameObject.SetActive(false);
		}
		transform.position = transform.position + transform.forward * speed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<Blaster>())
		{
			return;
		}
		
		if (col.GetComponent<BlasterTarget>())
		{
			Debug.Log("Hit target");
			// TODO Handle target hit
		}
	}
}
