using UnityEngine;
using System.Collections;

public class SortParticalSystem : MonoBehaviour {

	public string LayerName = "Particles";


	public void Awake(){
		
	}

	// Use this for initialization
	void Start () {
		
		GetComponent<ParticleSystem> ().GetComponent<Renderer> ().sortingLayerName = LayerName;
	}

}
