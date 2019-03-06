using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : MonoBehaviour
{
	[SerializeField] private GameObject bulletPrefab;

	private Vector3 bulletFirePosition;

	void Awake()
	{
		bulletFirePosition = bulletPrefab.transform.localPosition;
		bulletPrefab.SetActive(false);
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
