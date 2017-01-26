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
}
