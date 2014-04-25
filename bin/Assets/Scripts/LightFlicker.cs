using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		float r = Random.Range(-2, 2) * 0.5f;
		if (GetComponent<Light>().intensity - r >= 2 && GetComponent<Light>().intensity - r <= 5)
			GetComponent<Light>().intensity += r;
		else
			GetComponent<Light>().intensity = 3.5f;
	}
}
