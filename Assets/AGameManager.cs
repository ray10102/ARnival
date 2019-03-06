using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class AGameManager : MonoBehaviour
{
	[SerializeField]
	private GameObject GameOverUI;
	[SerializeField]
	private TextMeshProUGUI scoreText;

	public abstract void StartGame();
	
	protected void EndGame()
	{
		GameOverUI.SetActive(true);
		scoreText.text = ScoreKeeper.instance.score.ToString();
	}
}
