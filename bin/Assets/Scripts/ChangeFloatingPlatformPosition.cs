using UnityEngine;
using System.Collections;

public class ChangeFloatingPlatformPosition : MonoBehaviour {

	enum PlatformType {TURF, CASTLEWALL, PIER};
	[SerializeField]
	private PlatformType platformType = PlatformType.TURF;

	private GameObject platform = null;
	private GameObject tower = null;

	// Use this for initialization
	void Start () {
		platform = GameObject.Find ("ImageTarget-FloatingPlatform");
		tower = GameObject.Find ("ImageTarget-Tower");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 _pos;
		Vector3 _rot;
		if (platform.transform.position.z < 10 && platform.transform.position.z > -10){
			_pos = new Vector3 (tower.transform.position.x * 0.999769f, 7f, 0f);
			switch (platformType) {
				case PlatformType.TURF :
					_pos.x = platform.transform.position.x * 0.999769f;
					_pos.y = 7;
				break;
				case PlatformType.CASTLEWALL :
					if (Mathf.Abs(platform.transform.position.x - tower.transform.position.x) < 20) {
					      _pos.x = tower.transform.position.x + 9;
						_pos.y = 0;
					}
					else {
						_pos.x = platform.transform.position.x * 0.999769f;
						_pos.y = 0;
					}
				break;
				case PlatformType.PIER :
					_pos.x = platform.transform.position.x * 0.999769f;
				break;
			}
			_rot = new Vector3(-90f, 0f, 0f);
		}
		else {
			_pos = platform.transform.position;
			_pos.y = 7;
			_rot = new Vector3(-90, platform.transform.rotation.eulerAngles.y + 180, 0);
		}
		transform.position = _pos;
		transform.rotation = Quaternion.Euler(_rot.x, _rot.y, _rot.z);
	}
}
