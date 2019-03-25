using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
	public enum BackgroundMusic
	{
		Main,
		Target,
		Ball,
		Creature,
	}
	[SerializeField] private AudioClip main, target, ball, creature;

	private AudioSource audioSource;

	private static BackgroundMusicManager instance;

	private float totalLerpTime = .2f;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogError("There are two background music manager in this scene!");
		}
	}

	public static void Pitch(float endPitch)
	{
		instance.StartPitchCoroutine(endPitch);
		Debug.Log("pitch");
	}

	private void StartPitchCoroutine(float endPitch)
	{
		StartCoroutine("PitchCoroutine", endPitch);
	}

	IEnumerator PitchCoroutine(float endPitch)
	{
		float startPitch = audioSource.pitch;
		float startTime = Time.time;
		while (audioSource.pitch != endPitch)
		{
			audioSource.pitch = Mathf.Lerp(startPitch, endPitch, (Time.time - startTime) / totalLerpTime);
			Debug.Log("Start pitch: " + startPitch + " End pitch: " + endPitch + " totalLerpTime: " + totalLerpTime + " fraction: " + (Time.time - startTime / totalLerpTime) + " " +  audioSource.pitch);
			yield return null;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = main;
		audioSource.loop = true;
		Play();
	}

	private void Play()
	{
		if (!audioSource.isPlaying)
		{
			audioSource.Play();
		}
	}

	public void PlayMain()
	{
		Play(BackgroundMusic.Main);
	}

	public void PlayTarget()
	{
		Play(BackgroundMusic.Target);
	}

	public void PlayBall()
	{
		Play(BackgroundMusic.Ball);
	}

	public void PlayCreature()
	{
		Play(BackgroundMusic.Creature);
	}
	

	public static void Play(BackgroundMusic music)
	{
		switch (music)
		{
			case BackgroundMusic.Main:
				instance.audioSource.clip = instance.main;
				instance.Play();
				break;
			case BackgroundMusic.Target:
				instance.audioSource.clip = instance.target;
				instance.Play();
				break;
			case BackgroundMusic.Ball:
				instance.audioSource.clip = instance.ball;
				instance.Play();
				break;
			case BackgroundMusic.Creature:
				instance.audioSource.clip = instance.creature;
				instance.Play();
				break;
		}
	}
}
