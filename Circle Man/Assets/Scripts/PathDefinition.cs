using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathDefinition : MonoBehaviour {

	public Transform[] points;

	public void OnDrawGizmos(){

		if(points == null || points.Length < 2){
			return;
		}

		for(var i=1; i < points.Length; i++){
			Gizmos.DrawLine (points [i - 1].position, points [i].position);
		}
	}

	public IEnumerator<Transform> GetPathEnumerator(){

		var index = 0;
		var direction = 1;


		while (true) {
			yield return points [index];
			if (index <= 0) {
				direction = 1;
			} else if (index >= points.Length-1) {
				direction = -1;
			}
			index += direction;


		}
	}
}
