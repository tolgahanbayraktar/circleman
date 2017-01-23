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
	private static readonly float SlopeLimitTangant = Mathf.Tan (75f * Mathf.Deg2Rad);
	private Vector3 _activeGlobalPlatformPoint;
	private Vector3 _activeLocalPlatyformPoint;
	private GameObject _lastStandingOn;

	public LayerMask PlatformMask;

	public Vector2 Velocity{ get { return _velocity; } }

	public ControllerStated State{ get; private set; }

	public ControllerParameters2D DefaultParameters;

	public ControllerParameters2D Parameters  { get { return _OverrideParameters ?? DefaultParameters; } }

	public GameObject StandingOn{ get; private set; }

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
		var wasGrounded = State.IsCollidingBelow;

		State.Reset ();
		HandlePlatforms ();

		CalculateRayOrigins ();

		if (deltaMovement.y < 0 && wasGrounded)
			HandleVerticalSlope (ref deltaMovement);

		if (Mathf.Abs (deltaMovement.x) > 0.001f)
			MoveHorizontally (ref deltaMovement);
		
		MoveVertically (ref deltaMovement);

		CorrectHorizontalPlacement (ref deltaMovement, true);
		CorrectHorizontalPlacement (ref deltaMovement, false);
			
		_transform.Translate (deltaMovement, Space.World);

		// Debug.Log (Time.deltaTime);

		if (Time.deltaTime > 0) {
			_velocity = deltaMovement / Time.deltaTime;
		}

		if (State.IsMovingUpSlope)
			_velocity.y = 0;

		if (StandingOn != null) {
			_activeGlobalPlatformPoint = transform.position;
			_activeLocalPlatyformPoint = StandingOn.transform.InverseTransformPoint (transform.position);

			if (_lastStandingOn != StandingOn) {
				StandingOn.SendMessage ("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = StandingOn; 
				//Debug.Log (StandingOn);
			}
		}
	}

	private void HandlePlatforms ()
	{
		if (StandingOn != null) {
			var newGlobalPlatformPoint = StandingOn.transform.TransformPoint (_activeLocalPlatyformPoint);
			var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
			if (moveDistance != Vector3.zero)
				transform.Translate (moveDistance, Space.World);

			StandingOn = null;
		}
	}

	private void CorrectHorizontalPlacement (ref Vector2 deltaMovement, bool isRight)
	{
		var halfWidth = (_boxCollider.size.x * _localScale.x) / 2f;
		var rayOrigin = isRight ? _rayCastBottomRight : _rayCastBottomLeft;
		if (isRight)
			rayOrigin.x += SkinWidth - halfWidth;
		else
			rayOrigin.x += -SkinWidth + halfWidth;

		var rayDirection = isRight ? Vector2.right : -Vector2.right;
		var offset = 0f;

		for (var i = 1; i < TotalHorizontalRays - 1; i++) {
			var rayVector = new Vector2 (rayOrigin.x + deltaMovement.x, deltaMovement.y + rayOrigin.y + i * (_verticalDistanceBetweenRays));

			var raycastHit = Physics2D.Raycast (rayVector, rayDirection, halfWidth, PlatformMask);

			if (!raycastHit)
				continue;

			offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
		} //for

		deltaMovement.x += offset;
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

			if (State.RayVectorDebug)
				Debug.DrawRay (rayVector, rayDirection, Color.yellow);

			var raycastHit = Physics2D.Raycast (rayVector, rayDirection, rayDistance, PlatformMask);
			if (!raycastHit)
				continue;

			// Platrofmrlar üzerinde iken 
			if (!isGoingUp) {
				StandingOn = raycastHit.collider.gameObject;
				//Debug.Log (StandingOn.name);
			}

			deltaMovement.y = raycastHit.point.y - rayVector.y;

			if (isGoingUp) {
				deltaMovement.y -= SkinWidth;
				State.IsCollidingAbove = true;
			} else {
				deltaMovement.y += SkinWidth;
				State.IsCollidingBelow = true;
			}

			if (!isGoingUp && deltaMovement.y > 0.0001f)
				State.IsMovingUpSlope = true;

		} //for

	}

	private void MoveHorizontally (ref Vector2 deltaMovement)
	{
		var isGoingRight = deltaMovement.x > 0;
		var rayDistance = Mathf.Abs (deltaMovement.x) + SkinWidth;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		var rayOrigin = isGoingRight ? _rayCastBottomRight : _rayCastBottomLeft;

		for (var i = 0; i < TotalHorizontalRays; i++) {

			var rayVector = new Vector2 (rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));

			if (State.RayVectorDebug)
				Debug.DrawRay (rayVector, rayDirection, Color.yellow);
			
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

		if (_jumpIn > 0)
			return true;

		deltaMovement.y = Mathf.Abs (Mathf.Tan (angle * Mathf.Deg2Rad) * deltaMovement.x);
		State.IsMovingUpSlope = true;
		State.IsCollidingBelow = true;
		return true;


	}

	private void HandleVerticalSlope (ref Vector2 deltaMovement)
	{
		var center = (_rayCastBottomLeft.x + _rayCastBottomRight.x) / 2;
		var direction = -Vector2.up;
		var slopeDistance = SlopeLimitTangant * (_rayCastBottomRight.x - center);
		var slopeRayVector = new Vector2 (center, _rayCastBottomLeft.y);



		var rayCastHit = Physics2D.Raycast (slopeRayVector, direction, slopeDistance, PlatformMask);

		if (!rayCastHit)
			return;

		var isMovingDownSlope = Mathf.Sign (rayCastHit.normal.x) == Mathf.Sign (deltaMovement.x);
		if (!isMovingDownSlope)
			return;

		var angle = Vector2.Angle (rayCastHit.normal, Vector2.up);
		if (Mathf.Abs (angle) < 0.0001f)
			return;

		State.IsMovingDownSlope = true;
		State.SlopeAngle = angle;

		deltaMovement.y = rayCastHit.point.y - slopeRayVector.y;
			
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

	public void SetVerticalForce (float y)
	{
		_velocity.y = y;
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

	public void OnTriggerEnter2D (Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D> ();
		if (parameters == null)
			return;

		_OverrideParameters = parameters.Parameters;

	}

	public void OnTriggerExit2D (Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
		if (parameters == null)
			return;

		_OverrideParameters = null;

	}
}
