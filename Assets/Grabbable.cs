using System.Collections;
using System.Collections.Generic;
using MagicLeap;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Grabbable : MonoBehaviour
{
	private static ControllerVisualizer controllerVisualizer;

	private bool isBeingGrabbed;
	private bool canBeGrabbed;

	private Transform originalParent;
	private Vector3 originalLocalPos;

	void Awake()
	{
		MLInput.OnTriggerDown += OnGrab;
		MLInput.OnTriggerUp += OnRelease;
		
		// cache original state values
		originalLocalPos = transform.localPosition;
		originalParent = transform.parent;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col)
	{
		ControllerVisualizer controller = col.GetComponent<ControllerVisualizer>();
		if (controller)
		{
			if (controllerVisualizer == null)
			{
				controllerVisualizer = controller; // save a static ref to this so we can use it later
			}
			controller.SetVisibility(false);
			canBeGrabbed = true;
		}
	}

	void OnTriggerExit(Collider col)
	{
		ControllerVisualizer controller = col.GetComponent<ControllerVisualizer>();
		if (controller)
		{
			controller.SetVisibility(true);
			canBeGrabbed = false;
		}
	}

	void OnGrab(byte controller_id, float value)
	{
		if (canBeGrabbed && !isBeingGrabbed)
		{
			Grab();
		}
	}

	void OnRelease(byte controller_id, float value)
	{
		if (isBeingGrabbed)
		{
			Release();
		}
	}

	private void Grab()
	{
		if (controllerVisualizer == null)
		{
			controllerVisualizer = FindObjectOfType<ControllerVisualizer>();
		}
		
		transform.parent = controllerVisualizer.transform;
	}
	
	private void Release()
	{
		transform.parent = originalParent;
	}

	public void Reset()
	{
		transform.parent = originalParent;
		transform.localPosition = originalLocalPos;
	}
}
