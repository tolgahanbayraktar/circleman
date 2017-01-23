using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	private Camera cam;
	public BoxCollider2D bounds;
	public Transform player;
	public Vector3 _min, _max;
	public Vector2 margin, smoothing;

	public void Start ()
	{
		_min = bounds.bounds.min;
		_max = bounds.bounds.max;
		cam = GetComponent<Camera> ();
	}

	public void Update ()
	{

		var x = transform.position.x;
		var y = transform.position.y;
		var cameraHalfWidth = cam.orthographicSize * ((float)Screen.width / Screen.height);

		if (Mathf.Abs (x - player.position.x) > margin.x)
			x = Mathf.Lerp (x, player.position.x, smoothing.x * Time.deltaTime);

		if (Mathf.Abs (y - player.position.y) > margin.y)
			y = Mathf.Lerp (y, player.position.y, smoothing.y * Time.deltaTime);
		

		x = Mathf.Clamp (x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, _min.y + cam.orthographicSize, _max.y - cam.orthographicSize);

		transform.position = new Vector3 (x, y, transform.position.z);
	}

}
