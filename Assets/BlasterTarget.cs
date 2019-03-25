using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class BlasterTarget : ASpawnable
{
	public Material baseMaterial, hitMaterial;

	private Animator anim;
	private MeshRenderer model;

	[SerializeField] private float stayAliveTimeStart = 5f;
	[SerializeField] private float stayAliveTimeEnd = 1f;

	private AudioSource audioSource;
	[SerializeField]
	private AudioClip hitSound, spawnSound;

	private float stayAliveTime;

	private bool canBeHit;

	public override void Spawn<T>(AGameManager<T> manager)
	{
		isSpawned = true;
		spawnTime = Time.time;
		model.material = baseMaterial;
		anim.SetTrigger("spawn");
		stayAliveTime = Mathf.Lerp(stayAliveTimeStart, stayAliveTimeEnd,
			(Time.time - manager.startTime) / manager.gameTime);
		audioSource.clip = spawnSound;
		audioSource.Play();
		canBeHit = true;
	}

	public override void Despawn()
	{
		anim.SetTrigger("despawn");
		isSpawned = false;
		canBeHit = false;
	}

	#region Lifecycle
	
	void Update()
	{
		if (isSpawned && Time.time - spawnTime > stayAliveTime)
		{
			Despawn();
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		Collider col = GetComponentInChildren<Collider>();
		if (!(col && col.isTrigger))
		{
			Debug.LogWarning(gameObject.name + " does not have a trigger collider attached!");
		}

		model = GetComponentInChildren<MeshRenderer>();

		anim = GetComponent<Animator>();

		audioSource = GetComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.loop = false;
	}
	
	void OnTriggerEnter(Collider col)
	{

		if (col.GetComponent<BlasterBullet>() && isSpawned && canBeHit)
		{
			anim.SetTrigger("hit");
			model.material = hitMaterial;
			ScoreKeeper.instance.AddPoints(1);
			audioSource.clip = hitSound;
			audioSource.Play();
			canBeHit = false;
		}
	}
	
	#endregion
}
