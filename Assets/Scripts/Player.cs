using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	private float _normalizedHorizontalSpeed;
	private CharacterController2D _controller;
	private bool _isFacingRight;
	private Collider2D _collider;
	public float maxSpeed = 8;
	public float SpeedAccelerationOnGround = 10f;
	public float SpeedAccelerationInAir = 5f;
	public GameObject OuchEffect;
	public bool IsDead { get; private set; }
	public int MaxHealt = 100;
	public int Healt { get; private set;}

	public void Awake(){
		Healt = MaxHealt;
	}

	public void Start ()
	{
		_isFacingRight = transform.localScale.x > 0;
		_controller = GetComponent<CharacterController2D> ();
		_collider = GetComponent<Collider2D> ();
	}


	public void Update ()
	{
		if (!IsDead)
			HandledInput ();

		var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

		if (IsDead)
			_controller.SetHorizontalForce (0);
		else
			_controller.SetHorizontalForce (Mathf.Lerp (
				_controller.Velocity.x,
				_normalizedHorizontalSpeed * maxSpeed,
				Time.deltaTime * movementFactor));
	}

	public void Kill ()
	{
		_controller.HandleCollisions = false;
		_collider.enabled = false;
		IsDead = true;
		_controller.SetForce (new Vector2 (0, 10));
		Healt = 0;
	}

	public void RespawnAt (Transform spawnPoint)
	{
		if (!_isFacingRight)
			Flip ();

		IsDead = false;
		_collider.enabled = true;
		_controller.HandleCollisions = true;
		transform.position = spawnPoint.position;
		Healt = MaxHealt;
	}


	public void TakeDamage (int damage)
	{
		Instantiate (OuchEffect, transform.position, transform.rotation);
		Healt -= damage;

		FloatingText.Show (string.Format ("-{0}", damage), "PlayerTakeDamageText",
			new FromWorldPointTextPositioner (Camera.main, transform.position, 2f, 60f));
		
		if (Healt <= 0)
			LevelManager.Instantiate.KillPlayer ();
	}

	private void HandledInput ()
	{
		
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

		if (_controller.CanJump && Input.GetKeyDown (KeyCode.Space)) {
			_controller.Jump ();
		}

	}

	private void Flip ()
	{
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}


}
