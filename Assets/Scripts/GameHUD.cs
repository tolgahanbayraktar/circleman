using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour
{

	public GUISkin skin;

	public void OnGUI ()
	{
		GUI.skin = skin;

		GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
		{
			GUILayout.BeginVertical (skin.GetStyle ("GameHud"));
			{
				GUILayout.Label (string.Format ("Points: {0}", GameManager.Instance.points), skin.GetStyle ("PointsText"));

				var time = LevelManager.Instantiate.RunningTime;

				GUILayout.Label (string.Format ("{0:00}:{1:00} with {2} bonus", 
					time.Minutes + (time.Hours * 60), 
					time.Seconds,
					LevelManager.Instantiate.CurrentTimeBonus
				), skin.GetStyle ("TimeText"));
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndArea ();
	}
}
