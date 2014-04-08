using UnityEngine;
using System.Collections;

public class ChangeTowerPosition : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//		float _width = ((GameObject.Find ("ImageTarget-PlatformRight").transform.position.x == 0) ? GameObject.Find ("ImageTarget-PlatformLeft").transform.position.x : GameObject.Find ("ImageTarget-PlatformRight").transform.position.x) / 100;
		float _x = GameObject.Find ("ImageTarget-Tower").transform.position.x * 0.999769f;
		//find right platform x set to 0, find left platform in terms of x, width = left.x, position = left.x/2
//		print (_x);
		
		transform.position = new Vector3(_x, transform.position.y, transform.position.z);
	}
}
