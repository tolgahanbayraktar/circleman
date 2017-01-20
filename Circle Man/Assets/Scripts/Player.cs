using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	private float _normalizedHorizontalSpeed;
	private CharacterController2D _controller;
	private bool _isFacingRight;

	public float maxSpeed = 8;

	public void Start ()
	{
		_isFacingRight = transform.localScale.x > 0;
		_controller = GetComponent<CharacterController2D> ();
	}


	public void Update ()
	{
		HandledInput ();
		_controller.SetHorizontalForce (Mathf.Lerp (
			_controller.Velocity.x,
			_normalizedHorizontalSpeed * maxSpeed,
			Time.deltaTime));
	}

	private void HandledInput ()
	{
		Debug.Log (_isFacingRight);

		if (Input.GetKey (KeyCode.D)) {
			_normalizedHorizontalSpeed = 1;
			if (!_isFacingRight)
				Flip ();
		} else if (Input.GetKey (KeyCode.A)) {
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight)
				Flip ();
		} else {
			_normalizedHorizontalSpeed = 0;
		}

	}

	private void Flip ()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}
}
