using System.Collections;
using System.Collections.Generic;
using MSA;
using UnityEngine;

public class Creature : ASpawnable
{
	[SerializeField] private Animator normal, hit;
	
	private MeshRenderer model;

	[SerializeField] private float stayAliveTimeStart = 5f;
	[SerializeField] private float stayAliveTimeEnd = 1f;

	[SerializeField] private AudioSource hitSoundSource, spawnSoundSource;
	[SerializeField] private AudioClip[] hitSounds, spawnSounds;

	private float stayAliveTime;

	public override void Spawn<T>(AGameManager<T> manager)
	{
		isSpawned = true;
		spawnTime = Time.time;
		normal.gameObject.SetActive(true);
		hit.gameObject.SetActive(false);
		normal.SetBool("spawned", true);
		stayAliveTime = Mathf.Lerp(stayAliveTimeStart, stayAliveTimeEnd,
			(Time.time - manager.startTime) / manager.gameTime);
		spawnSoundSource.clip = spawnSounds[Random.Range(0, spawnSounds.Length)];
		spawnSoundSource.Play();
	}

	public override void Despawn()
	{
		if(normal.isActiveAndEnabled)
			normal.SetBool("spawned", false);
		if(hit.isActiveAndEnabled)
			hit.SetBool("spawned", false);
		isSpawned = false;
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
		spawnSoundSource.loop = false;
		hitSoundSource.loop = false;
		Collider col = GetComponentInChildren<Collider>();
		if (!(col && col.isTrigger))
		{
			Debug.LogWarning(gameObject.name + " does not have a trigger collider attached!");
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if (isSpawned && col.name == "Mallet Collider")
		{
			hit.gameObject.SetActive(true);
			normal.gameObject.SetActive(false);
			hit.SetBool("spawned", false);
			ScoreKeeper.instance.AddPoints(1);
			hitSoundSource.clip = hitSounds[Random.Range(0, hitSounds.Length)];
			hitSoundSource.Play();
		}
	}
	
	#endregion
}
