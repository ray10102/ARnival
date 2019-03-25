using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownManager : MonoBehaviour {
	[SerializeField] private Image countdownImage;
	[SerializeField] private Sprite three, two, one;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip[] clips;

	private static CountDownManager instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogError("There is more than one countdown manager!");
		}
	}

	void Start()
	{
		audioSource.loop = false;
		audioSource.playOnAwake = false;
	}

	public static void Three()
	{
		instance.SwapImage(3);
	}
	
	public static void Two()
	{
		instance.SwapImage(2);
	}
	
	public static void One()
	{
		instance.SwapImage(1);
	}
	
	public static void Go()
	{
		instance.SwapImage(0);
	}

	private void SwapImage(int number)
	{
		countdownImage.gameObject.SetActive(true);
		switch (number)
		{
			case 0:
				countdownImage.sprite = null;
				countdownImage.gameObject.SetActive(false);
				break;
			case 1:
				countdownImage.sprite = one;
				break;
			case 2:
				countdownImage.sprite = two;
				break;
			case 3:
				countdownImage.sprite = three;
				break;
		}

		if (audioSource && number > 0)
		{
			audioSource.clip = clips[number - 1];
			audioSource.Play();
		}
	}
}
