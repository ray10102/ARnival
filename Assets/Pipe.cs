using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
	private AudioSource audioSource;

	[SerializeField] private AudioClip[] clips;
	
	// Use this for initialization
	void Start () {
		audioSource = GetComponentInChildren<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.loop = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInParent<Mallet>())
		{
			audioSource.PlayOneShot(clips[transform.GetSiblingIndex()], 1.3f);
		}
	}
}
