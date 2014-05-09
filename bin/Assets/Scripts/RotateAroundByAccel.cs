using UnityEngine;
using System.Collections;
using Holoville.HOTween;

[AddComponentMenu("Camera-Control/Accell-Look")]
public class RotateAroundByAccel : MonoBehaviour {
	
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float x = 0F;
	public float y = 0F;
	public float z = 0F;
	
	void Update ()
	{
//		if (axes == RotationAxes.MouseXAndY)
//		{
//			rotationX += Input.acceleration.y * sensitivityX;
//			rotationX = Mathf.Clamp (rotationX, minimumX, maximumX);
//			
//			rotationY += Input.acceleration.z * sensitivityY;
//			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
//
//			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
//		}
//		else if (axes == RotationAxes.MouseX)
//		{
//			transform.Rotate(0, Input.acceleration.z * sensitivityX, 0);
//		}
//		else
//		{
//			rotationY += Input.acceleration.y * sensitivityY;
//			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
//			
//			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
//		}
		Vector3 r = Input.acceleration;
		// r.x = r.y;
		// r.y = Input.acceleration.x * 8;
		// r.z = 0;
		r.x = Mathf.Clamp((r.y) + 1, 0, 1);
		r.y = 90 + Input.acceleration.x * 5;
		r.z = 0;

		// transform.rotation = Quaternion.Euler(r * 10);
		HOTween.To(transform, 1, "rotation", Quaternion.Euler(r * 10));
	}
	
	void Start ()
	{
		HOTween.Init(false, false, true);
		HOTween.EnableOverwriteManager();

		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
}