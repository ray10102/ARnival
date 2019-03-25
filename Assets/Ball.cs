using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

	private Rigidbody rigid;
	[SerializeField]
	private float bucketRadius;
	
	public Transform bucketParent;

	public bool scored = false;

	private AudioSource audioSource;

	[SerializeField] private AudioClip[] ballSuccess, ballFail;

	[SerializeField] private AudioClip[] ballBounceSounds;

	public Vector3 thrownFrom;
	public int frameReleased;
	
	// Use this for initialization
	void Start ()
	{
		audioSource = GetComponent<AudioSource>();
		rigid = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision other)
	{
		audioSource.clip = ballBounceSounds[Random.Range(0, ballBounceSounds.Length)];
		audioSource.Play();
	}

	private void Score(Bucket bucket, Vector3 from, Vector3 to)
	{
		if (bucket == null)
		{
			Debug.LogError("No bucket found");
			return;
		}
		
		float distance = Vector2.Distance(new Vector2(from.x, from.z), new Vector2(to.x, to.z));
		int bucketScore = (int) bucket.bucketType;
		int score = bucketScore > 0 ? (distance > 0.6f ? 2 : 1) * bucketScore : (distance > 0.6f ? 1 : 2) * bucketScore;
		ScoreKeeper.instance.AddPoints(score);
		if (audioSource)
		{
			if ((int) bucket.bucketType > 0 && ballSuccess.Length > 0)
				audioSource.PlayOneShot(ballSuccess[Random.Range(0, ballSuccess.Length)]);
			else if (ballFail.Length > 0)
				audioSource.PlayOneShot(ballFail[Random.Range(0, ballSuccess.Length)]);
		}
	}

	void OnEnabled()
	{
		scored = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!rigid.isKinematic && rigid.velocity == Vector3.zero && !scored && frameReleased != 0 && Time.frameCount > frameReleased + 1)
		{
			Debug.Log(frameReleased + " " + Time.frameCount);
			foreach (Transform bucket in bucketParent) // NOT OPTIMIZED
			{
				if (Vector3.Distance(new Vector3(bucket.position.x, rigid.transform.position.y, bucket.position.z),
					    rigid.transform.position) < bucketRadius)
				{
					Score(bucket.GetComponent<Bucket>(), thrownFrom, rigid.transform.position);
					scored = true;
					return;
				}
			}
			// even if we didnt score, mark as scored so we stop evaluating
			scored = true;
		}
	}
}
