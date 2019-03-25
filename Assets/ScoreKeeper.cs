using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
	public float score { get; private set; }
	public TextMeshProUGUI scoreText;

	public static ScoreKeeper instance;

	public static bool isGameRunning;

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
		if (scoreText)
		{
			scoreText.text = "Score:\n0";
		}
	}

	public void AddPoints(float points)
	{
		if (isGameRunning)
		{
			score += points;
			if (scoreText)
			{
				scoreText.text = "Score:\n" + score.ToString(score % 1 < 0.0001 ? "N0" : "N2");
			}
		}
	}
}
