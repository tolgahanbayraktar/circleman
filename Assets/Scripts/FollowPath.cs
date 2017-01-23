using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour
{

	public PathDefinition _pathDefinition;
	public float speed = 5f;
	public float maxDistanceToGoal = 0.1f;
	private IEnumerator<Transform> _currentPoint;

	public enum FollowType
	{
		MoveTowards,
		Lerp

	}

	public FollowType type = FollowType.MoveTowards;

	public void Start ()
	{

		if (_pathDefinition == null) {
			Debug.LogError ("Path defination cannot be null", gameObject);
			return;
		}
			
		_currentPoint = _pathDefinition.GetPathEnumerator ();
		_currentPoint.MoveNext ();
		transform.position = _currentPoint.Current.position;

	}

	public void Update ()
	{

		if (_currentPoint == null || _currentPoint.Current == null)
			return;

		if (type == FollowType.MoveTowards) {
			transform.position = Vector3.MoveTowards (transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
		} else if (type == FollowType.Lerp) {
			transform.position = Vector3.Lerp (transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
		}

		var distanceSquared = (transform.position - _currentPoint.Current.position).sqrMagnitude; 

		if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal) {
			_currentPoint.MoveNext ();
		}
	}
}
