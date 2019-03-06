using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterGameManager : AGameManager
{
	[SerializeField] private GameObject gameParent;
	private int targetCount;
	
	public override void StartGame()
	{
		BlasterTarget[] targets = FindObjectsOfType<BlasterTarget>();
		foreach (BlasterTarget target in targets)
		{
			target.gameObject.SetActive(true);
		}

		targetCount = targets.Length;
		
		Blaster.Reset();
		
		ScoreKeeper.instance.OnScoreUpdated += HandleScoreUpdated;
	}

	public void HandleScoreUpdated(int score)
	{
		if (score >= targetCount)
		{
			EndGame();
		}
	}
}
