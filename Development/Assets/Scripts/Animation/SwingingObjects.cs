using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwingingObjects : MonoBehaviour {
	// are doors swinging
	public bool swinging = false;
	public Vector3 force;
	public float proximity;
	
	// hinge joints for the door
	public List<HingeJoint> objects;
	public List<RotationAnimationFX> anims;
	
	public bool isPlayingOpenningAnimation = false;
	
	public void PlayDoorOpeningIntro(){
		isPlayingOpenningAnimation = true;
		foreach(HingeJoint joint in objects)
		{
			joint.useSpring = false;
		}
		
		foreach(RotationAnimationFX animation in anims){
			animation.PlayAnimation();	
		}
		
		Invoke("CloseDoorIntro", 2.5f);
		
	}
	
	void CloseDoorIntro(){
		isPlayingOpenningAnimation = false;
		foreach(RotationAnimationFX animation in anims){
			animation.Reset(animation.transform.localEulerAngles, -animation.rotationAngle, 1f);
			animation.duration = 0.5f;
			animation.PlayAnimation();	
		}
		Invoke("CloseDoorNatural", 0.35f);
	}
	
	void CloseDoorNatural(){	
		foreach(HingeJoint joint in objects)
		{
			joint.useSpring = true;
		}
		swinging = true;
		// Invoke check methods
		InvokeRepeating("CheckSwing", 0.5f, 0.1f);
	}
	
	/// <summary>
	/// Checks if the doors have stopped swining
	/// </summary>
	void CheckSwing()
	{
		foreach(HingeJoint joint in objects)
		{
			if (joint.velocity > 0.1f)
				return;
		}
		
		swinging = false;
		CancelInvoke("CheckSwing");
	}
	
	/// <summary>
	/// Handle OnPress event.
	/// </summary>
	/// <param name='press'>
	/// Press.
	/// </param>
	void OnPress(bool press)
	{
		if (ApplicationState.Instance.isPaused ())
						return;

		if (press)
		{
			// Mark that an environment element has been clicked to disable player movement
			InputManager.Instance.ReceivedUIInput();
			
			// Check if the door is already swinging and that the player is far away
			if (!swinging && Vector3.Distance(transform.position, Player.instance.transform.position) > proximity)
			{
				swinging = true;
				
				foreach (HingeJoint joint in objects)
				{
					joint.rigidbody.AddTorque(force);
				}
				// Invoke check methods
				InvokeRepeating("CheckSwing", 0.5f, 0.1f);
			}                                    	
		}
	}
}
