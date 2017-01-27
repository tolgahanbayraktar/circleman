﻿using UnityEngine;
using System.Collections;

public class PointStar : MonoBehaviour , IPlayerRespawnListener
{

	public GameObject Effect;

	public int PointsToAdd = 10;

	public void OnTriggerEnter2D (Collider2D other)
	{
		if (other.GetComponent<Player> () == null)
			return;
		
		GameManager.Instance.AddPoints (PointsToAdd);
		Instantiate (Effect, transform.position, transform.rotation);

		gameObject.SetActive (false);

		/* Her yıldızı alınca üzerinde aldığımız 
		 * puan yazacak kayacak kaybolacak
		 */
		FloatingText.Show (string.Format ("+{0}", PointsToAdd), "PointStarText",
			new FromWorldPointTextPositioner (Camera.main, transform.position, 1.5f, 50));


	}

	public void OnPlayerRespawnInThisCheckpoint ()
	{
		gameObject.SetActive (true);
	}

}
