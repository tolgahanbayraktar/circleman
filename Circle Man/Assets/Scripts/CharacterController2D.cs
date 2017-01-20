using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
	private Vector2 _velocity;
	private BoxCollider2D _boxCollider;
	private Vector3 _localScale;
	private Vector3 _rayCastBottomRight;
	private Vector3 _rayCastBottomLeft;
	private Transform _transform;
	private const float SkinWidth = 0.02f;
	private const int TotalHorizontalRays = 8;
	private float _verticalDistanceBetweenRays;

	public LayerMask PlatformMask;

	public Vector2 Velocity{ get { return _velocity; } }

	public void Awake ()
	{
		_boxCollider = GetComponent<BoxCollider2D> ();
		_localScale = transform.localScale;
		_transform = transform;

		var ColliderHeight = _boxCollider.size.y * Mathf.Abs (_transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = ColliderHeight / (TotalHorizontalRays - 1);
	}

	public void LateUpdate ()
	{
		
		Move (Velocity * Time.deltaTime);
	}

	private void Move (Vector2 deltaMovement)
	{
		CalculateRayOrigins ();

		MoveHorizontally (ref deltaMovement);

		_transform.Translate (deltaMovement, Space.World);

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

			if (!raycastHit) {
				continue;
			}
		
			deltaMovement.x = raycastHit.point.x - rayVector.x;

			if (isGoingRight)
				deltaMovement.x -= SkinWidth;
			else
				deltaMovement.x += SkinWidth;
		}
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
	}


}
