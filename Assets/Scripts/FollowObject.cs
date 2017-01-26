using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{

	public Vector2 offset;
	public Transform following;

	public void Update ()
	{
		transform.position = following.position + (Vector3)offset;
	}
}
