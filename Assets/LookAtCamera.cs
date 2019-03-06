using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Camera.main)
		{
			Vector3 cameraPos = Camera.main.transform.position;
			Vector3 lookPos = new Vector3(cameraPos.x, transform.position.y, cameraPos.z);
			transform.LookAt(lookPos);
		}
	}
}
