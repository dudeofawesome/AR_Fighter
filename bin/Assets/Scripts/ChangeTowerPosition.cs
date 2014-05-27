using UnityEngine;
using System.Collections;

public class ChangeTowerPosition : MonoBehaviour {

	enum TowerType {DOJO, TURRET, BOAT};
	[SerializeField]
	private TowerType towerType = TowerType.DOJO;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject _tower = GameObject.Find ("ImageTarget-Tower");
		Vector3 _pos;
		Vector3 _rot;
		if (_tower.transform.position.z < 10 && _tower.transform.position.z > -10){
			_pos = new Vector3 (_tower.transform.position.x * 0.999769f, 0.1827326f, 4.5f);
			_rot = new Vector3(0f, 0f, 0f);
		}
		else{
			_pos = _tower.transform.position;
			_rot = _tower.transform.rotation.eulerAngles;
			_rot = new Vector3(0, _tower.transform.rotation.eulerAngles.y + 180, 0);
		}
		transform.position = _pos;
		transform.rotation = Quaternion.Euler(_rot.x, _rot.y, _rot.z);
	}
}
