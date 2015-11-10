using UnityEngine;
using System.Collections;

public class VerticalMovement : MonoBehaviour 
{
	public static float startSpeed = 0.13f;
	public static float verticalSpeed = 0.13f;
	Camera sceneCamera;
	Selfish_Sam_Manager manager;
	bool stop = false;
	
	// Use this for initialization
	void Awake(){
		//verticalSpeed = startSpeed;
	}
	
	void Start () {
		//verticalSpeed = startSpeed;
		sceneCamera = GameObject.FindGameObjectWithTag("MinigameCamera").GetComponent<Camera>();
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Selfish_Sam_Manager>();
		//verticalSpeed += verticalSpeed * manager.verticalSpeedIncrease;
	}
	
	public void Stop()
	{
		stop = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!stop){
			Vector3 viewPoint = sceneCamera.WorldToViewportPoint(transform.position);
			
			if(viewPoint.y < 0){
				//if(gameObject.name == "Enemy") Selfish_Sam_Manager.currentEnemies--;
				Destroy(gameObject);
			}
			
			transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
		}
	}
}
