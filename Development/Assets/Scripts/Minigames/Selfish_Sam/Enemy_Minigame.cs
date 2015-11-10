using UnityEngine;
using System.Collections;

public class Enemy_Minigame : MonoBehaviour {
	public Pirate_Ship ship;
	AIHand enemyHand;
	Selfish_Sam_Manager manager;
	public float treasureChaseSpeed = 0.00001f;
	public bool wasCaptured = false;
	public VerticalMovement myMovement;
	public TweenScale myScale;
	
	void Start()
	{
		ship = GameObject.Find("PirateShip").GetComponent<Pirate_Ship>();
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Selfish_Sam_Manager>();
	}
	
	void Update()
	{
		if(manager.activateShootingAI && transform.position.y < 0.6f)
		{
			if(enemyHand == null){
				enemyHand = GameObject.FindGameObjectWithTag("EnemyAIHand").GetComponent<AIHand>();	
				enemyHand.FollowEnemy(this);
			}	
		}
		
		if (!wasCaptured && transform.position.y > -0.15f)
			MoveToShip ();
	}
	
	void MoveToShip()
	{
		Vector3 shipDir = ship.transform.position - transform.position;
		shipDir.Normalize();
		transform.Translate(shipDir * treasureChaseSpeed * Time.deltaTime);
	}
	
	
	void OnTriggerEnter(Collider col)
	{
		if(col.collider.name == "CannonBall" && !wasCaptured)
		{
			wasCaptured = true;
			myMovement.Stop();
			myScale.enabled = true;
			//Destroy(col.gameObject);
			Invoke("EnemyCaptured", 1.0f);
			//Destroy(gameObject);
		}
	}
	
//	void OnCollisionEnter(Collision col)
//	{
//		if(col.collider.name == "CannonBall" && !wasCaptured)
//		{
//			wasCaptured = true;
//			myMovement.Stop();
//			Destroy(col.gameObject);
//			Invoke("EnemyCaptured", 1.0f);
//			//Destroy(gameObject);
//		}
//	}
	
	void EnemyCaptured()
	{
		Destroy(gameObject);
	}
}
