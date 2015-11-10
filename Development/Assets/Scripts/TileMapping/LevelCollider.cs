using UnityEngine;
using System.Collections;

public class LevelCollider : MonoBehaviour {
	static public bool isPressed = false;

	void OnPress(bool pressed){
		if(pressed){
			isPressed = true;	
		}
	}
}
