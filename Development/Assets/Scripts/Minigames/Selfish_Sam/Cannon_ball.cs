using UnityEngine;
using System.Collections;

public class Cannon_ball : MonoBehaviour {
	//public SCSpriteAnimation myAnim;
	public Texture bouncyBall;
	public Texture captureNet;
	Camera sceneCamera;
	Vector3 target;
	Transform cannon;
	Vector3 cannonDir;
	float cannonVelocity = 1.1f;
	bool canMove = false;
	public Pirate_Ship ship;
	bool capturedEnemy = false;
	public TweenScale myScale;
	
	// Use this for initialization
	void Start () {
		sceneCamera = GameObject.FindGameObjectWithTag("MinigameCamera").GetComponent<Camera>();
		ship = GameObject.Find("PirateShip").GetComponent<Pirate_Ship>();
//		myAnim.Play();
//		Invoke("ShootCannonBall", 0.5f);
	}
	
	void ShootCannonBall(){
		//myAnim.Stop();
		GetComponent<UITexture>().mainTexture = bouncyBall;
		cannonDir = target - cannon.position;
		cannonDir.Normalize();
		canMove = true;
        transform.up = cannonDir;
	}
	
	public void SetCannonBall(Vector3 inputPos, Transform cannonTransform){
		target = inputPos;
		cannon = cannonTransform;
		
		ShootCannonBall();		
	}
	
	
//	void OnCollisionEnter(Collision col){
//		Debug.Log("here?");
//		if(col.collider.name== "Enemy"){
//			Destroy(col.gameObject);
//			Destroy(gameObject);
//		}
//	}
	
	void OnTriggerEnter(Collider col)
	{
		if(!capturedEnemy && col.collider.name== "Enemy")
		{
			capturedEnemy = true;
			ship.manager.updateEnemyBar();
			GetComponent<UITexture>().mainTexture = captureNet;
			Vector3 newPos = col.transform.position;
			newPos.z -= 0.5f;
			transform.position = newPos;
			myScale.enabled = true;
			Invoke("DestroyCannonBall", 1.0f);
			//Destroy(col.gameObject);
			//Destroy(gameObject);
		}
	}
	
	void DestroyCannonBall()
	{
		Destroy(gameObject);	
	}
	
	void Update()
	{
		if(!capturedEnemy)
		{
			Vector3 viewPoint = sceneCamera.WorldToViewportPoint(transform.position);
			
			if(viewPoint.x > 1 || viewPoint.x < 0 || viewPoint.y > 1 || viewPoint.y < 0)
				Destroy(gameObject);
			
			if(canMove)
				transform.Translate(cannonDir * cannonVelocity * Time.deltaTime,Space.World);	
			else
				transform.position = cannon.position;
		}
	}
	

}
