using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{

	private List<IPlayerRespawnListener> _listeners;

	public void Awake ()
	{
		_listeners = new List<IPlayerRespawnListener> ();
	}

	public void SpawnPlayer (Player player)
	{
		player.RespawnAt (transform);
		foreach (var listener in _listeners) {
			listener.OnPlayerRespawnInThisCheckpoint ();
		}
	}

	// Yakın checkpointin içerisine yıldızları aktarıyoruz
	public void AssignObjectToCheckpoint (IPlayerRespawnListener listener)
	{
		_listeners.Add (listener);
	}

	public void PlayerHitCheckpoint ()
	{
		// Yazılacak yazı ve Resources'daki skin adı
		//FloatingText.Show ("Checkpoint!", "CheckpointText", new CenteredTextPositioner (.5f));

		StartCoroutine(PlayerHitCheckPointCo(LevelManager.Instantiate.CurrentTimeBonus));
	}

	public IEnumerator PlayerHitCheckPointCo (int bonus)
	{
		FloatingText.Show ("Checkpoint!", "CheckpointText", new CenteredTextPositioner (.5f));
		yield return new WaitForSeconds (.5f);
		FloatingText.Show (string.Format ("+{0} stime bonus!", bonus), "CheckPointText", new CenteredTextPositioner (.5f));
	}
}
