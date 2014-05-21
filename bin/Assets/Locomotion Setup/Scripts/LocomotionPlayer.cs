 /// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class LocomotionPlayer : MonoBehaviour {

    protected Animator animator;

    public float speed = 0;
    public float direction = 0;
    private Locomotion locomotion = null;

	// Use this for initialization
	void Start () 
	{
        animator = GetComponent<Animator>();
        locomotion = new Locomotion(animator);
	}
    
	void Update () 
	{
        if (animator && Camera.main)
		{
//            JoystickToEvents.Do(transform,Camera.main.transform, ref speed, ref direction);
//			direction = 1;
//			direction = (direction < 90 || direction > 270) ? 90 : 270;
//			print (direction);



//			if (Input.GetKeyDown(KeyCode.A))
//				transform.rotation = Quaternion.Euler(0, -90, 0);
//			if (Input.GetKeyDown(KeyCode.D))
//				transform.rotation = Quaternion.Euler(0, 90, 0);
//			if (Input.GetKey(KeyCode.A))
//				locomotion.Do(6, 0);
//			if (Input.GetKey(KeyCode.D))
//				locomotion.Do(6, 0);
//			if (Input.GetKeyUp(KeyCode.A))
//				locomotion.Do(-60, 0);
//			if (Input.GetKeyUp(KeyCode.D))
//				locomotion.Do(-60, 0);
		}		
	}
}
