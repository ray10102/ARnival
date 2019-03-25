using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using Image = UnityEngine.UI.Image;

public class SimpleRaycasterCursor : MonoBehaviour
{
	[SerializeField] private GameObject cursor;
	[SerializeField] private float maxDistance;
	[SerializeField] private LayerMask layers;

	private LineRenderer laser;
	private RaycastHit result;
	private PhysicsRaycastButton button;

	void Awake()
	{
		MLInput.OnTriggerDown += OnTriggerDown;
		MLInput.OnTriggerUp += OnTriggerUp;
	}

	void Start()
	{
		laser = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!ScoreKeeper.isGameRunning)
		{
			if (Physics.Raycast(transform.position, transform.forward, out result, maxDistance, layers))
			{
				laser.enabled = true;
				laser.SetPositions(new Vector3[] { transform.position, transform.position + transform.forward * .1f, result.point });
				/*cursor.SetActive(true);
				cursor.transform.position = result.point;
				cursor.transform.rotation = Quaternion.LookRotation(result.normal);
				cursor.transform.localScale = Vector3.one * result.distance / maxDistance * 2;*/
				button = result.collider.GetComponent<PhysicsRaycastButton>();
			}
			else
			{
				laser.enabled = true;
				laser.SetPositions(new Vector3[] { transform.position, transform.position + transform.forward * .1f, transform.position + transform.forward * maxDistance });
			}

			if (button)
			{
				PhysicsRaycastButton.focused = button;
			}
			else
			{
				PhysicsRaycastButton.focused = null;
			}
		}
		else
		{
			laser.enabled = false;
		}
	}

	void OnTriggerDown(byte controller_id, float value)
	{
		if (PhysicsRaycastButton.focused != null)
		{
			PhysicsRaycastButton.focused.state = PhysicsRaycastButton.ButtonState.PRESSED;
		}
	}
	
	void OnTriggerUp(byte controller_id, float value)
	{
		if (PhysicsRaycastButton.focused != null)
		{
			PhysicsRaycastButton.focused.OnButtonPressed();
			PhysicsRaycastButton.focused.state = PhysicsRaycastButton.ButtonState.NONE;
			PhysicsRaycastButton.focused = null;
		}	
	}
}
