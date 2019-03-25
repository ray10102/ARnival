using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : ASpawnable
{
	[SerializeField] private Material grey, red, green;

	[SerializeField] private float greenStartProbability = .3f;
	[SerializeField] private float greenEndProbability = .8f;
	
	[SerializeField] private float stayAliveTime = 5f;

	public BucketType bucketType;

	public enum BucketType
	{
		Red = -1,
		Grey = 1,
		Green = 5
	}

	private MeshRenderer meshRenderer;
	
	// Use this for initialization
	void Start ()
	{
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		bucketType = BucketType.Grey;
		meshRenderer.material = grey;
	}

	public override void Spawn<T>(AGameManager<T> manager)
	{
		float greenProbability =
			Mathf.Lerp(greenStartProbability, greenEndProbability, manager.startTime / manager.gameTime);
		if (Random.Range(0f, 1f) < greenProbability)
		{
			meshRenderer.material = green;
			bucketType = BucketType.Green;
		}
		else
		{
			meshRenderer.material = red;
			bucketType = BucketType.Red;
		}

		spawnTime = Time.time;
		isSpawned = true;
	}

	public override void Despawn()
	{
		meshRenderer.material = grey;
		bucketType = BucketType.Grey;
		isSpawned = false;
	}
	
	void Update()
	{
		if (isSpawned && Time.time - spawnTime > stayAliveTime)
		{
			Despawn();
		}
	}
}
