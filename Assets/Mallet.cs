using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Mallet : MonoBehaviour
{

	private AudioSource audio;
	[SerializeField]
	private AudioClip malletHitSound;

	private ControllerConnectionHandler controllerConnectionHandler;
	
	// Use this for initialization
	void Start ()
	{
		audio = GetComponentInChildren<AudioSource>();
		controllerConnectionHandler = FindObjectOfType<ControllerConnectionHandler>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Pipe>())
		{
			audio.PlayOneShot(malletHitSound, .7f);
			if (!controllerConnectionHandler)
				controllerConnectionHandler = FindObjectOfType<ControllerConnectionHandler>();
			if (controllerConnectionHandler)
			{
				MLInputController controller = controllerConnectionHandler.ConnectedController;
				if (controller != null)
				{
					controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz,
						MLInputControllerFeedbackIntensity.Medium);
				}
			}
		}
	}
}
