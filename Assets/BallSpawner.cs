using System.Collections;
using System.Collections.Generic;
using MagicLeap;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class BallSpawner : MonoBehaviour
{
	[SerializeField] private GameObject ballPrefab;
	private ControllerVisualizer controllerVisualizer;
	[SerializeField]
	private GameObject ballParent;

	private GameObject ballBeingGrabbed;
	
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField] private AudioClip ballSpawnSound;

	private Rigidbody rigid;

	private Vector3 lastPosition;
	private float lastPositionTime;

	private Vector3 velocity;
	
	[SerializeField]
	public Transform bucketParent;
	
	private ControllerConnectionHandler controllerConnectionHandler;


	void Start()
	{
		ControllerVisualizer controller = FindObjectOfType<ControllerVisualizer>();
		if (controller)
		{
			if (controllerVisualizer == null)
			{
				controllerVisualizer = controller; // save a static ref to this so we can use it later
			}
		}
	}
	
	void OnTriggerDown(byte controller_id, float value)
	{
		if (ScoreKeeper.isGameRunning)
		{
			foreach (Transform child in ballParent.transform)
			{
				if (!child.gameObject.activeSelf)
				{
					ballBeingGrabbed = child.gameObject;

					// Try to use inactivated bullets instead of instantiating new ones
					ballBeingGrabbed.transform.position = controllerVisualizer.transform.position;
					ballBeingGrabbed.transform.rotation = controllerVisualizer.transform.rotation;
					ballBeingGrabbed.SetActive(true);

					rigid = ballBeingGrabbed.GetComponent<Rigidbody>();

					if (!rigid)
					{
						Debug.LogError(gameObject.name + ":Ring does not have a Rigidbody attached!");
					}
					else
					{
						rigid.isKinematic = true;
					}

					Ball ball = ballBeingGrabbed.GetComponent<Ball>();
					ball.bucketParent = bucketParent;
					ball.scored = false;


					lastPosition = transform.position;
					audioSource.PlayOneShot(ballSpawnSound);
					
					if (!controllerConnectionHandler)
						controllerConnectionHandler = FindObjectOfType<ControllerConnectionHandler>();
					if (controllerConnectionHandler)
					{
						MLInputController controller = controllerConnectionHandler.ConnectedController;
						if (controller != null)
						{
							controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Click,
								MLInputControllerFeedbackIntensity.Medium);
						}
					}
					return;
				}
			}

			// Instantiate new bullets if necessary
			audioSource.PlayOneShot(ballSpawnSound);
			ballBeingGrabbed = Instantiate(ballPrefab, ballParent.transform);
			ballBeingGrabbed.SetActive(true);
			ballBeingGrabbed.transform.position = controllerVisualizer.transform.position;
			ballBeingGrabbed.transform.rotation = controllerVisualizer.transform.rotation;

			rigid = ballBeingGrabbed.GetComponent<Rigidbody>();

			if (!rigid)
			{
				Debug.LogError(gameObject.name + ":Ring does not have a Rigidbody attached!");
			}
			else
			{
				rigid.isKinematic = true;
			}

			Ball ball2 = ballBeingGrabbed.GetComponent<Ball>();
			ball2.bucketParent = bucketParent;
			ball2.scored = false;

			lastPosition = transform.position;
			
			if (!controllerConnectionHandler)
				controllerConnectionHandler = FindObjectOfType<ControllerConnectionHandler>();
			if (controllerConnectionHandler)
			{
				MLInputController controller = controllerConnectionHandler.ConnectedController;
				if (controller != null)
				{
					controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz,
						MLInputControllerFeedbackIntensity.Low);
				}
			}
		}
	}
	
	void OnTriggerUp(byte controller_id, float value)
	{
		if (rigid)
		{
			rigid.isKinematic = false;
			rigid.useGravity = true;
			rigid.constraints = RigidbodyConstraints.None;
			rigid.AddForce(velocity * 55f);
			Ball ball = rigid.GetComponent<Ball>();
			ball.thrownFrom = rigid.transform.position;
			ball.frameReleased = Time.frameCount;
			rigid = null;
			ballBeingGrabbed = null;
		}
	}

	private void ConnectHandlers()
	{
		MLInput.OnTriggerDown += OnTriggerDown;
		MLInput.OnTriggerUp += OnTriggerUp;
	}
	
	private void DisconnectHandlers()
	{
		MLInput.OnTriggerDown -= OnTriggerDown;
		MLInput.OnTriggerUp -= OnTriggerUp;
	}
	
	void OnEnable()
	{
		ConnectHandlers();
	}

	void OnDisable()
	{
		DisconnectHandlers();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (ballBeingGrabbed)
			{
				OnTriggerUp(1, 1f);
			}
			else
			{
				OnTriggerDown(2, 2f);
			}
		}
		if (ballBeingGrabbed)
		{

			if (controllerVisualizer == null)
			{
				controllerVisualizer = FindObjectOfType<ControllerVisualizer>();
			}

			ballBeingGrabbed.transform.position = controllerVisualizer.transform.position;
			ballBeingGrabbed.transform.rotation = controllerVisualizer.transform.rotation;
			velocity = (controllerVisualizer.transform.position - lastPosition) / Time.deltaTime;
			lastPosition = controllerVisualizer.transform.position;
		}
	}

	public void Reset()
	{
		foreach (Transform child in ballParent.transform)
		{
			child.gameObject.SetActive(false);
		}
	}
	
}
