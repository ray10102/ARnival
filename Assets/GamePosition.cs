using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePosition : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	}
	
	public void Reset()
	{
		transform.localPosition = new Vector3(Camera.main.transform.position.x, GroundIdentifier.Height, Camera.main.transform.position.z);
	}
}
