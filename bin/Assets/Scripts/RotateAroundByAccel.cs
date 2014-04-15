using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Accell-Look")]
public class RotateAroundByAccel : MonoBehaviour {
	
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationX = 0F;
	float rotationY = 0F;
	
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
		transform.rotation = Quaternion.Euler(Input.acceleration * 5);
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
}