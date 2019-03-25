using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

[RequireComponent(typeof(BoxCollider))]
public class PhysicsRaycastButton : MonoBehaviour
{
	public bool click;
	
	[Serializable]
	public struct StateSprite
	{
		public ButtonState state;
		public Sprite sprite;
	}
	
	[SerializeField] private float verticalPadding = 20f, horizontalPadding = 30f;

	[SerializeField] private BoxCollider collider;
	
	[SerializeField] private UnityEvent onClick;

	[SerializeField] private StateSprite[] sprites;
	
	private Dictionary<ButtonState, Sprite> states = new Dictionary<ButtonState, Sprite>();
	
	private Image image;
	
	private static PhysicsRaycastButton focused_;

	private AudioSource canvasAudio;
	[SerializeField]
	private AudioClip clickSound, hoverSound;
	
	private ControllerConnectionHandler controllerConnectionHandler;


	public static PhysicsRaycastButton focused
	{
		get { return focused_; }
		set {
			if (focused_ != value)
			{
				if (focused_ != null)
				{
					focused_.state = ButtonState.NONE;
				}
				focused_ = value;
				if (value != null)
				{
					focused_.state = ButtonState.HOVER;
					if (focused_.canvasAudio != null && focused_.hoverSound != null)
						focused_.canvasAudio.PlayOneShot(focused_.hoverSound, .5f);
				}
			}
		}
	}
	
	public enum ButtonState { NONE, HOVER, PRESSED }

	private ButtonState state_;
	public ButtonState state
	{
		get { return state_; }
		set
		{
			if (state_ != value)
			{
				state_ = value;
				UpdateState();
			}
		}
	}

	void Awake()
	{
		focused = null;
	}
	
	// Use this for initialization
	void Start ()
	{
		canvasAudio = GetComponentInParent<AudioSource>();
		collider = GetComponent<BoxCollider>();
		RectTransform rect = GetComponent<RectTransform>();
		rect.pivot = new Vector2(.5f, .5f);
		collider.size = new Vector3(rect.rect.width + 2 * horizontalPadding, rect.rect.height + 2 * verticalPadding, 1f);
		collider.center = new Vector3(0f, 0f, 0f);

		image = GetComponent<Image>();

		foreach (StateSprite sprite in sprites)
		{
			try
			{
				states.Add(sprite.state, sprite.sprite);
			}
			catch
			{
				Debug.LogWarning("Tried to add duplicate sprite state: " + gameObject.name);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (click)
		{
			OnButtonPressed();
			click = false;
		}
	}

	private void UpdateState()
	{
		Sprite sprite;
		if (states.TryGetValue(state, out sprite))
		{
			image.sprite = sprite;
		}
	}

	public void OnButtonPressed()
	{
		if (onClick != null)
		{
			onClick.Invoke();
			if (canvasAudio != null && clickSound != null)
				canvasAudio.PlayOneShot(clickSound, 1.2f);
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
		}
	}
}
