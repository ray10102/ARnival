using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public abstract class AGameManager<T> : MonoBehaviour where T : ASpawnable
{
	[HideInInspector]
	public bool shouldSpawn;
	[HideInInspector]
	public float startTime;
	
	public float[] spawnTimes;
	public float gameTime = 45f;
	
	[SerializeField] protected GameObject gameParent;
	[SerializeField] protected GameObject gameOverUI;
	
	[SerializeField] protected TextMeshProUGUI timeText, highScoreText;

	[SerializeField] protected string gameKey;

	[SerializeField] protected Grabbable grabbable;

	[SerializeField] protected Animator canvasesAnim;

	[SerializeField] protected TextAsset levelFile;

	[SerializeField] protected Transform floor;

	[SerializeField] protected BallSpawner ballSpawner;
	
	protected T[] targets;

	protected int spawnTimeIndex = 0;

	private bool animateCanvases;

	private GameObject backboard;

	#region Debug Fields
		
	#endregion

	public void StartGame()
	{
		StartGame(false);
	}
	
	public void StartGame(bool restarting)
	{
		animateCanvases = !restarting;
		Debug.Log("Starting game");
		gameOverUI.SetActive(false);
		ScoreKeeper.instance.Reset();
		if (!restarting)
		{
			ResetPosition();
		}

		spawnTimeIndex = 0;
		targets = FindObjectsOfType<T>();
		StartCoroutine("Countdown", "SpawnTargets");
		timeText.text = "Time:\n" + gameTime.ToString("n2");
		if (grabbable)
			grabbable.Reset();
		if (ballSpawner) 
			ballSpawner.Reset();
		float highscore = PlayerPrefs.GetFloat(gameKey + "_HIGHSCORE");
		highScoreText.text = "HIGHSCORE:\n" + highscore.ToString(highscore % 1 < 0.0001 ? "N0" : "N2");
	}
	
	protected void EndGame()
	{
		BackgroundMusicManager.Pitch(1f);
		if (grabbable)
			grabbable.Reset();
		if (ballSpawner) 
			ballSpawner.Reset();
		backboard.SetActive(true);
		gameOverUI.SetActive(true);
		if (PlayerPrefs.GetFloat(gameKey + "_HIGHSCORE") < ScoreKeeper.instance.score)
		{
			PlayerPrefs.SetFloat(gameKey + "_HIGHSCORE", ScoreKeeper.instance.score);
			highScoreText.text = "HIGHSCORE:\n" + ScoreKeeper.instance.score.ToString(ScoreKeeper.instance.score % 1 < 0.0001 ? "N0" : "N2");
		}
		
		for (int j = 0; j < targets.Length; j++) {
			if (targets[j].isSpawned)
			{
				targets[j].Despawn();
			}
		}

		ScoreKeeper.isGameRunning = false;
	}
	
	public void RestartActiveGame()
	{
		if (enabled)
		{
			StartGame(true);
		}
	}

	public void QuitApplication()
	{
		Application.Quit();
	}

	public void DespawnGame()
	{
		if (grabbable)
			grabbable.Reset();
		if (ballSpawner) 
			ballSpawner.Reset();
		if (enabled)
		{
			canvasesAnim.SetBool("up", false);
			gameParent.GetComponent<Animator>().SetTrigger("despawn");
		}
		ScoreKeeper.isGameRunning = false;
	}

	IEnumerator Countdown(string nextCoroutine = "")
	{
		yield return new WaitForSeconds(2f);
		if (animateCanvases)
		{
			canvasesAnim.SetBool("up", true);
			animateCanvases = false;
		}

		CountDownManager.Three();
		yield return new WaitForSeconds(1f);
		CountDownManager.Two();
		yield return new WaitForSeconds(1f);
		CountDownManager.One();
		yield return new WaitForSeconds(1f);
		CountDownManager.Go();
		ScoreKeeper.isGameRunning = true;
		
		if (nextCoroutine != "")
		{
			StartCoroutine(nextCoroutine);
		}
	}
	
	protected void ResetPosition()
	{
		Vector3 cameraRotation = Camera.main.gameObject.transform.eulerAngles;
		gameParent.transform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
		canvasesAnim.transform.rotation = gameParent.transform.rotation;
		Vector3 cameraPosition = Camera.main.gameObject.transform.position;
		gameParent.transform.position = new Vector3(cameraPosition.x, floor.position.y + .0001f, cameraPosition.z);
		gameParent.transform.Translate(Vector3.forward * .35f, Space.Self);
		floor.position = new Vector3(cameraPosition.x, floor.position.y, cameraPosition.z);
		canvasesAnim.transform.position = gameParent.transform.position;
	}
	
	IEnumerator SpawnTargets()
	{
		shouldSpawn = true;
		startTime = Time.time;
		bool hasPitched = false;
		while (shouldSpawn)
		{
			while (spawnTimeIndex < spawnTimes.Length && spawnTimes[spawnTimeIndex] < Time.time - startTime)
			{
				spawnTimeIndex++;
				int i = Random.Range(0, targets.Length);
				for (int j = 0; j < targets.Length; j++) {
					if (!targets[(j + i) % targets.Length].isSpawned)
					{
						targets[i].Spawn(this);
						Debug.Log("spawning");
						break;
					}
				}
			}

			timeText.text = "Time:\n" + Mathf.Clamp(gameTime - (Time.time - startTime), 0f, gameTime).ToString("n2");
			float runTime = Time.time - startTime;
			if (gameTime - runTime < 10.3f && !hasPitched)
			{
				Debug.Log("tryna pitch");
				BackgroundMusicManager.Pitch(Mathf.Pow(1.05946f, 3));
				hasPitched = true;
			}
			if (Time.time - startTime < gameTime)
			{
				yield return null;
			}
			else
			{
				shouldSpawn = false;
			}
		}

		EndGame();
	}
	
	void Update()
	{
		if (!levelFile && Input.GetKeyDown(KeyCode.Space))
		{
			RecordTime();
		}
	}

	void Start()
	{
		backboard = GameObject.Find("Raycast UI Backboard");
		if (levelFile)
		{
			string[] times = levelFile.text.Split(',');
			int offset = 0;
			spawnTimes = new float[times.Length];
			for (int i = 0; i < times.Length; i++)
			{
				if (!float.TryParse(times[i], out spawnTimes[i - offset]))
				{
					offset++;
				}
			}
		}
		else
		{
			spawnTimes = new float[1000];
			for (int i = 0; i < 1000; i++)
			{
				spawnTimes[i] = 10000;
			}
		}

		//Debug.Log("Inactivating");
		gameParent.SetActive(false);
		//Debug.Log("Inactivated");
		
	}
	
	private void RecordTime()
	{
		string path = "Assets/Resources/test.txt";

		//Write some text to the test.txt file
		StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine(Time.time - startTime + ",");
		writer.Close();

		spawnTimes[spawnTimeIndex] = Time.time - startTime;
	}

}
