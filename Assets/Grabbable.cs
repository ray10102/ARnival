using System;
using System.Collections;
using System.Collections.Generic;
using MagicLeap;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Grabbable : MonoBehaviour
{
	public Action OnGrabbableReleased;
	public Action OnGrabbableGrabbed;
	
	private static ControllerVisualizer controllerVisualizer;

	public bool isBeingGrabbed;
	private bool canBeGrabbed;

	private Transform originalParent;
	private Vector3 originalLocalPos;

	[SerializeField]
	private MLInputControllerButton grabButton = MLInputControllerButton.Bumper;
	[SerializeField]
	private bool useTrigger;

	void Awake()
	{
		if (useTrigger)
		{
			MLInput.OnTriggerDown += OnTriggerDown;
			MLInput.OnTriggerUp += OnTriggerUp;
		}
		else
		{
			MLInput.OnControllerButtonDown += OnButtonDown;
		}

		// cache original state values
		originalLocalPos = transform.localPosition;
		originalParent = transform.parent;
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

	void OnButtonDown(byte controller_id, MLInputControllerButton button)
	{
		if (button == grabButton)
		{
			if (canBeGrabbed && !isBeingGrabbed)
			{
				Grab();
			}
			else if (isBeingGrabbed)
			{
				Release();
			}
		}
	}

	void OnTriggerDown(byte controller_id, float value)
	{
		if (canBeGrabbed && !isBeingGrabbed)
		{
			Grab();
		}
	}
	
	void OnTriggerUp(byte controller_id, float value)
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
		OnGrabbableGrabbed.Invoke();
	}
	
	private void Release()
	{
		transform.parent = originalParent;
		OnGrabbableReleased.Invoke();
	}

	public void Reset()
	{
		transform.parent = originalParent;
		transform.localPosition = originalLocalPos;
	}
}
