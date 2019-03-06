using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
	public int score { get; private set; }
	public Action<int> OnScoreUpdated;

	public static ScoreKeeper instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
	
	public void Reset()
	{
		score = 0;
		OnScoreUpdated.Invoke(score);
	}

	public void AddPoints(int points)
	{
		score += points;
		OnScoreUpdated.Invoke(score);
	}
}
