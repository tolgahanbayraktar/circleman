using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
	private Vector2 _velocity;
	private BoxCollider2D _boxCollider;
	private Vector3 _localScale;
	private Vector3 _rayCastBottomRight;
	private Vector3 _rayCastBottomLeft;
	private Vector3 _rayCastTopLeft;
	private Transform _transform;
	private const float SkinWidth = 0.02f;
	private const int TotalHorizontalRays = 8;
	private const int TotalVerticalRays = 4;
	private float _verticalDistanceBetweenRays;
	private float _horizontalDistanceBetweenRays;
	private ControllerParameters2D _OverrideParameters;
	private float _jumpIn;

	public LayerMask PlatformMask;

	public Vector2 Velocity{ get { return _velocity; } }

	public ControllerStated State{ get; private set; }

	public ControllerParameters2D DefaultParameters;

	public ControllerParameters2D Parameters  { get { return _OverrideParameters ?? DefaultParameters; } }

	public void Awake ()
	{
		State = new ControllerStated ();

		_boxCollider = GetComponent<BoxCollider2D> ();
		_localScale = transform.localScale;
		_transform = transform;

		var ColliderHeight = _boxCollider.size.y * Mathf.Abs (_transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = ColliderHeight / (TotalHorizontalRays - 1);

		var ColliderWidth = _boxCollider.size.x * Mathf.Abs (_transform.localScale.x) - (2 * SkinWidth);
		_horizontalDistanceBetweenRays = ColliderWidth / (TotalVerticalRays - 1);
	}

	public void LateUpdate ()
	{
		_jumpIn -= Time.deltaTime;

		_velocity.y += Parameters.Gravity * Time.deltaTime;

		Move (Velocity * Time.deltaTime);

	}

	private void Move (Vector2 deltaMovement)
	{
		State.Reset ();

		CalculateRayOrigins ();

		MoveHorizontally (ref deltaMovement);
		MoveVertically (ref deltaMovement);

		_transform.Translate (deltaMovement, Space.World);

		// Debug.Log (Time.deltaTime);

		if (Time.deltaTime > 0) {
			_velocity = deltaMovement / Time.deltaTime;
		}
	}

	public bool CanJump {
		get {
			if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehaviour.CanJumpAnywhere) {
				return _jumpIn <= 0;
			}

			if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehaviour.CanJumpOnGround) {
				return State.IsGrounded;
			}

			return false;
		}
	}

	public void Jump ()
	{
		AddForce (new Vector2 (0, Parameters.JumpMagnitude));

		_jumpIn = Parameters.JumpFrequency;
	}

	public void AddForce (Vector2 force)
	{
		_velocity += force;
	}

	private void MoveHorizontally (ref Vector2 deltaMovement)
	{
		var isGoingRight = deltaMovement.x > 0;
		var rayDistance = Mathf.Abs (deltaMovement.x) + SkinWidth;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		var rayOrigin = isGoingRight ? _rayCastBottomRight : _rayCastBottomLeft;

		for (var i = 0; i < TotalHorizontalRays; i++) {
			
			var rayVector = new Vector2 (rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));

			var raycastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, PlatformMask);
			if (!raycastHit)
				continue;

			if (i == 0 && HandleHorizontalSlope (ref deltaMovement, Vector2.Angle (raycastHit.normal, Vector2.up), isGoingRight))
				break;
		
			deltaMovement.x = raycastHit.point.x - rayVector.x;

			if (isGoingRight) {
				deltaMovement.x -= SkinWidth;
				State.IsCollidingRight = true;
			} else {
				deltaMovement.x += SkinWidth;
				State.IsCollidingLeft = true;
			}
		} //for
	}

	private bool HandleHorizontalSlope (ref Vector2 deltaMovement, float angle, bool isGoingRight)
	{
		if (Mathf.RoundToInt (angle) == 90)
			return false;

		// TODO: tekrar bakacam direkt false gönderecem
		if (angle > Parameters.SlopeLimit) {
			deltaMovement.x = 0;
			return true;
		}

		if (deltaMovement.y > .2f)
			return true;
		
		deltaMovement.y = Mathf.Abs (Mathf.Tan (angle * Mathf.Deg2Rad) * deltaMovement.x);
		State.IsMovingUpSlope = true;
		State.IsCollidingBelow = true;
		return true;


	}

	private void MoveVertically (ref Vector2 deltaMovement)
	{
		var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs (deltaMovement.y) + SkinWidth;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;

		var rayOrigin = isGoingUp ? _rayCastTopLeft : _rayCastBottomLeft;
		rayOrigin.x += deltaMovement.x;

		for (var i = 0; i < TotalVerticalRays; i++) {

			var rayVector = new Vector2 (rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);

			var raycastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, PlatformMask);
			if (!raycastHit)
				continue;

			deltaMovement.y = raycastHit.point.y - rayVector.y;

			if (isGoingUp) {
				deltaMovement.y -= SkinWidth;
				State.IsCollidingAbove = true;
			} else {
				deltaMovement.y += SkinWidth;
				State.IsCollidingBelow = true;
			}
		} //for

	}

	public void SetHorizontalForce (float x)
	{
		_velocity.x = x;
	}

	private void CalculateRayOrigins ()
	{
		var size = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.x), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		var center = new Vector2 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		//Debug.Log (_transform.position);

		_rayCastBottomRight = _transform.position + new Vector3 (center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);

		_rayCastBottomLeft = _transform.position + new Vector3 (center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);

		_rayCastTopLeft = _transform.position + new Vector3 (center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);

		//Debug.Log (_rayCastBottomRight);
	}


}
