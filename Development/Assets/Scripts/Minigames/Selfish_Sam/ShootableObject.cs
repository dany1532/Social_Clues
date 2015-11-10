using UnityEngine;
using System.Collections;

public class ShootableObject : MonoBehaviour {
	public Camera sceneCamera;
	Pirate_Ship ship;
	
	
	void Start(){
		sceneCamera = GameObject.FindGameObjectWithTag("MinigameCamera").GetComponent<Camera>();
		ship = GameObject.Find("PirateShip").GetComponent<Pirate_Ship>();
	}

	void OnPress(bool pressed)
	{

//		if(!ship.manager.activateShootingAI && pressed)
//		{
//			
//			Vector3 mousePos = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
//			ship.ShootAtPosition(mousePos);
//			
//		}
	}
}
