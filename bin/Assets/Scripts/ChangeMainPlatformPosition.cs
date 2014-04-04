using UnityEngine;
using System.Collections;

public class ChangeMainPlatformPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float _width = ((GameObject.Find ("ImageTarget-PlatformRight").transform.position.x == 0) ? GameObject.Find ("ImageTarget-PlatformLeft").transform.position.x : GameObject.Find ("ImageTarget-PlatformRight").transform.position.x) / 10;
		//find right platform x set to 0, find left platform in terms of x, width = left.x, position = left.x/2

		transform.localScale = new Vector3(_width * 2, 0.02f, 0.976f);
		transform.position = new Vector3(_width * 2, transform.position.y, transform.position.z);
	}
}
