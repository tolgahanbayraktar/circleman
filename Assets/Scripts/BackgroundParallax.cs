using UnityEngine;
using System.Collections;

public class BackgroundParallax : MonoBehaviour
{

	public Transform[] Backgrounds;
	public float ParallaxScale = 0.5f;
	private Vector3 _lastPosition;
	public float Smoothing = 2.0f;
	public float ParallaxReductionFactor=3.0f;

	public void Start ()
	{
		_lastPosition = transform.position;
	}

	public void Update ()
	{
		var parallax = (_lastPosition.x - transform.position.x) * ParallaxScale;

		for (var i = 0; i < Backgrounds.Length; i++) {

			var backgroundTargetPosition = Backgrounds [i].position.x + parallax*(i*ParallaxReductionFactor+1);

			Backgrounds [i].position = Vector3.Lerp (
				Backgrounds [i].position, 
				new Vector3 (backgroundTargetPosition, Backgrounds [i].position.y, Backgrounds [i].position.z),
				Smoothing * Time.deltaTime);
		}

		_lastPosition = transform.position;
	}
}
