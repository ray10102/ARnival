using System;
using System.Collections;
using System.Collections.Generic;
using MagicLeap;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

public class Grabbable : MonoBehaviour
{
	public Action OnGrabbableReleased;
	public Action OnGrabbableGrabbed;
	
	private static ControllerVisualizer controllerVisualizer;

	public bool isBeingGrabbed;
	private bool canBeGrabbed;

	public Transform originalParent;
	public Vector3 originalLocalPos;

	[SerializeField]
	private MLInputControllerButton grabButton = MLInputControllerButton.Bumper;
	[SerializeField]
	private bool useTrigger;

	[SerializeField] private Vector3 snapOrientation, snapPositionOffset;

	[SerializeField] private AudioClip grabbedSound;

	private AudioSource grabbbaleAudio;
	
	private void ConnectHandlers()
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
		
	}
	
	private void DisconnectHandlers()
	{
		if (useTrigger)
		{
			MLInput.OnTriggerDown -= OnTriggerDown;
			MLInput.OnTriggerUp -= OnTriggerUp;
		}
		else
		{
			MLInput.OnControllerButtonDown -= OnButtonDown;
		}
	}
	
	void Awake()
	{
		// cache original state values
		originalLocalPos = transform.localPosition;
		originalParent = transform.parent;
		Debug.Log("cached");
	}

	void OnEnable()
	{
		ConnectHandlers();
	}

	void OnDisable()
	{
		DisconnectHandlers();
	}

	void Update()
	{
		
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
		if (canBeGrabbed && !isBeingGrabbed && useTrigger)
		{
			Grab();
		}
	}
	
	void OnTriggerUp(byte controller_id, float value)
	{
		if (isBeingGrabbed && useTrigger)
		{
			Release();
		}
	}

	private void Grab()
	{
		if (OnGrabbableGrabbed != null)
		{
			OnGrabbableGrabbed.Invoke();
		}
		
		if (controllerVisualizer == null)
		{
			controllerVisualizer = FindObjectOfType<ControllerVisualizer>();
		}

		isBeingGrabbed = true;
		transform.parent = controllerVisualizer.transform;
		
		transform.localPosition = Vector3.zero + snapPositionOffset;
		transform.localRotation = Quaternion.Euler(snapOrientation);
	}
	
	private void Release()
	{
		if (OnGrabbableReleased != null)
		{
			OnGrabbableReleased.Invoke();
		}
		transform.parent = originalParent;
		isBeingGrabbed = false;
	}

	public void Reset()
	{
		transform.parent = originalParent;
		transform.localPosition = originalLocalPos;
		transform.localRotation = Quaternion.identity;
		isBeingGrabbed = false;
	}
}
