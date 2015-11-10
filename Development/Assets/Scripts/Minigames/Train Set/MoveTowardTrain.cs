using UnityEngine;
using System.Collections;

public class MoveTowardTrain : MonoBehaviour {

	float backSpeed = 0.1f;
	float downSpeed = 0.00001f;
	public Vector3 currPos;
	//public GameObject yHeight; 
	Vector3 currY; 
	float lerp; 
	public float correctY = -18.0f; 
	public float lerpSpeed = 2.0f, maxY = 2.0f; 

	//Camera sceneCamera;
	//public GameObject mainCam; 
	// Use this for initialization
	void Start () {
		//sceneCamera = mainCam.GetComponent<Camera>(); 

		//correctY = yHeight.transform.position.y; 
		correctY = -18.0f;
		 
	}

	// Update is called once per frame
	void Update () {
		currPos = gameObject.transform.position; 

		//Debug.Log (gameObject.name + currPos); 
		/*Vector3 viewPoint = sceneCamera.WorldToViewportPoint(transform.position);
		
		if(viewPoint.z < 0){
			//if(gameObject.name == "Enemy") Selfish_Sam_Manager.currentEnemies--;
			Destroy(gameObject);
		}*/
		if (currPos.z < 0)
		{
			Destroy(this.gameObject); 
		}

		transform.Translate(Vector3.back * backSpeed * Time.deltaTime, Space.World);
		//transform.Translate(Vector3.down * downSpeed * Time.deltaTime, Space.World);

		//Move up over the grass, then forward
		/*if (currPos.y < correctY )
		{

			transform.Translate(Vector3.up * backSpeed * Time.deltaTime, Space.World);
		}
		else if (currPos.y >= correctY)
		{
			transform.Translate(Vector3.back * backSpeed * Time.deltaTime, Space.World);
		}*/
		/*
		lerp = Time.time * lerpSpeed;
		//offsetLerp = (Time.time*lerpSpeed) + offset; 
		currY = new Vector3(currPos.x, Mathf.PingPong(lerp, maxY) - 18.0f , currPos.z); 
		this.gameObject.transform.position = currY; 
		*/
	}
}
