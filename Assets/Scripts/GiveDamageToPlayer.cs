using UnityEngine;
using System.Collections;

public class GiveDamageToPlayer : MonoBehaviour
{
	public int DamageToGive=25;
	public Vector2 _lastPosition;
	public Vector2 _velocity;

	public void LateUpdate(){
		_velocity = (_lastPosition - (Vector2)transform.position) / Time.deltaTime;
		_lastPosition = transform.position;
	}

	public void OnTriggerEnter2D (Collider2D other)
	{
		var Player = other.GetComponent<Player> ();

		if (Player== null)
			return;

		Player.TakeDamage (DamageToGive);

		var controller = Player.GetComponent<CharacterController2D> ();
		var totalVelocity = controller.Velocity + _velocity;
		controller.SetForce (new Vector2(
			-1*Mathf.Sign(totalVelocity.x) * Mathf.Clamp (Mathf.Abs (totalVelocity.x) * 6, 10, 40),
			-1*Mathf.Sign(totalVelocity.y) * Mathf.Clamp (Mathf.Abs (totalVelocity.y) * 2, 0, 15)));
	}

}
