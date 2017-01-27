using UnityEngine;

public class FromWorldPointTextPositioner : IFloatingTextPositioner {

	// yazının kayma hızı
	private readonly Camera _camera;
	private readonly Vector3 _worldPosition;
	private readonly float _speed;
	private float _timeToLive;
	private float _yOffseet;


	public FromWorldPointTextPositioner (Camera camera, Vector3 worldPosition, float timeToLive, float speed)
	{
		_camera = camera;
		_worldPosition = worldPosition;
		_timeToLive = timeToLive;
		_speed = speed;


	}

	//interface
	public bool GetPosition (ref Vector2 position, GUIContent content, Vector2 size)
	{
		
		if ((_timeToLive -= Time.deltaTime) < 0)
			return false;


		var screenPosition = _camera.WorldToScreenPoint (_worldPosition);
		position.x = screenPosition.x- (size.x / 2);

		// Camera kordinati ile unity kordinati farklı olduğu için x,y yi
		// önce px cinsine çeviriyoruz _camera.WorldToScreenPoint ile
		// unity kordinatı sol alttan başladığı için ve camera sol üstten 
		// başladığı için x posisyonunda sıkıntı yok ama y için Ekrarnın
		// yüksekliğinden çıkartmak gerekiyor.
		position.y = Screen.height - screenPosition.y - _yOffseet;
		_yOffseet += Time.deltaTime * _speed;

		return true;

	}

}
