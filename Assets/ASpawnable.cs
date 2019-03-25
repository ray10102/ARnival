using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ASpawnable : MonoBehaviour
{
	[HideInInspector]
	public bool isSpawned;
	protected float spawnTime;

	public abstract void Spawn<T>(AGameManager<T> manager) where T : ASpawnable;

	public abstract void Despawn();
}
