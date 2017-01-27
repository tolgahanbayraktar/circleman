using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelManager : MonoBehaviour
{
	private List<Checkpoint> _checkpoints;
	private int _currentCheckpointIndex;
	private DateTime _started;
	private int _savedPoints;

	public int BonusCutoffSeconds = 10;
	public int BonusSecondMultiplier = 3;

	// Zaman bonusu kazandık mı?
	public int CurrentTimeBonus {
		get { 
			var secondDifferece = (int)(BonusCutoffSeconds - RunningTime.TotalSeconds);
			return Mathf.Max (0, secondDifferece) * BonusSecondMultiplier;
		}
	}

	public TimeSpan RunningTime{ get { return DateTime.UtcNow - _started; } }

	public static LevelManager Instantiate { get; private set; }

	public Player Player{ get; private set; }



	public void	Awake ()
	{
		Instantiate = this;
	}

	public void Start ()
	{
		_checkpoints = FindObjectsOfType<Checkpoint> ().OrderBy (t => t.transform.position.x).ToList ();
		_currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1;
		Player = FindObjectOfType<Player> ();
		_started = DateTime.UtcNow;

		// Sistemdeki yıldızları topluyoruz
		var listeners = FindObjectsOfType<MonoBehaviour> ().OfType<IPlayerRespawnListener> ();

		foreach (var listener in listeners) {
			for (var i = _checkpoints.Count - 1; i >= 0; i--) {
				var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints [i].transform.position.x;

				if (distance < 0)
					continue;
				
				_checkpoints [i].AssignObjectToCheckpoint (listener);
				break;
			}
		}
	}

	public void Update ()
	{

		// Checkpointlerin son noktası ise
		var isAtLastpoint = _currentCheckpointIndex + 1 >= _checkpoints.Count;
		if (isAtLastpoint)
			return;

		var distanceToNextCheckpoint = _checkpoints [_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;

		if (distanceToNextCheckpoint >= 0)
			return;

		// Bir sonraki checkpointe ulaştık 
		_currentCheckpointIndex++;

		_checkpoints [_currentCheckpointIndex].PlayerHitCheckpoint ();

		GameManager.Instance.AddPoints (CurrentTimeBonus);
		_savedPoints = GameManager.Instance.points;
		_started = DateTime.UtcNow;
	}

	public void KillPlayer ()
	{
		StartCoroutine (KillPlayerCo ());

	}

	private IEnumerator KillPlayerCo ()
	{
		Player.Kill ();
		yield return new WaitForSeconds (1f);

		if (_currentCheckpointIndex != -1)
			_checkpoints [_currentCheckpointIndex].SpawnPlayer (Player);

		// Ölmeden önceki puan durumuna tekrar döndür
		_started = DateTime.UtcNow;
		GameManager.Instance.ResetPoints (_savedPoints);
	}
}
