using UnityEngine;
using System.Collections;

public class ChangeTowerPosition : MonoBehaviour {

	enum TowerType {DOJO, TURRET, BOAT};
	[SerializeField]
	private TowerType towerType = TowerType.DOJO;

	private GameObject tower = null;

	// Use this for initialization
	void Start () {
		tower = GameObject.Find ("ImageTarget-Tower");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 _pos;
		Vector3 _rot;
		if (tower.transform.position.z < 10 && tower.transform.position.z > -10){
			_pos = new Vector3 (tower.transform.position.x * 0.999769f, 0f, 0f);
			switch (towerType) {
				case TowerType.DOJO :
					_pos = new Vector3 (tower.transform.position.x * 0.999769f, 0.1827326f, 4.5f);
				break;
				case TowerType.TURRET :
					_pos = new Vector3 (tower.transform.position.x * 0.999769f, 0f, 0f);
				break;
				case TowerType.BOAT :
					_pos = new Vector3 (tower.transform.position.x * 0.999769f, 0f, 0f);
				break;
			}
			_rot = new Vector3(0f, 0f, 0f);
		}
		else{
			_pos = tower.transform.position;
			_rot = new Vector3(0, tower.transform.rotation.eulerAngles.y + 180, 0);
		}
		transform.position = _pos;
		transform.rotation = Quaternion.Euler(_rot.x, _rot.y, _rot.z);
	}
}
