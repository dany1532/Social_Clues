using UnityEngine;
using System.Collections;

public class PirateCollision_AI : MonoBehaviour {
	public Wheel_Ship wheel;

//	void OnCollisionStay(Collision col)
//	{
//		if(col.gameObject.name == "Rock")
//		{
//			Debug.Log("yay");
//		}
//	}
	
	void OnTriggerStay(Collider col)
	{
		if(col.tag == "Rock")
		{
			if(wheel.manager.activateMovementAI){
				wheel.StartWheel_AI();
				wheel.RotateWheel_AI();	
			}
		}
	}
	
	void OnTriggerExit(Collider col)
	{
		if(col.tag == "Rock")
		{
			if(wheel.manager.activateMovementAI)
			{
				wheel.StopWheel_AI();
			}
		}
	}
}
