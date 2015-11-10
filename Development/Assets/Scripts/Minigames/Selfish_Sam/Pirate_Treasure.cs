using UnityEngine;
using System.Collections;

public class Pirate_Treasure : MonoBehaviour 
{
	public UISprite treasureFill;
	public float fillTreasureDuration= 5f;
	public float treasureChaseSpeed = 1f;
	bool holdTreasure = false;
	bool treasureFilled = false;
	float lerpParameter = 0f;
	
	Pirate_Ship ship;
	Selfish_Sam_Manager manager;
	public InteractiveScale myIntScale;
	AIHand treasureHand;
	
	void Start()
	{
		ship = GameObject.FindGameObjectWithTag("PirateShip").GetComponent<Pirate_Ship>();
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Selfish_Sam_Manager>();
		//myIntScale = GetComponent<InteractiveScale>();
	}

	
	void OnPress(bool pressed)
	{
		if(!manager.activateTreasureAI){
			TreasurePressed(pressed);
		}
	}
	
	public void TreasurePressed(bool pressed)
	{
		if(pressed)
		{
			if(lerpParameter > 0) lerpParameter = 1f - lerpParameter;
			else   lerpParameter = 0f;
			holdTreasure = true;
			myIntScale.Play(true);
			ship.StopWheel();
		}
		else
		{
			lerpParameter = 1f - lerpParameter;
			holdTreasure = false;
			myIntScale.Play(false);
			ship.EnableWheel();
			ship.StartReloading();
		}
	}
	
	void Update()
	{
		if(manager.activateTreasureAI && transform.position.y < 0.6f)
		{
			if(treasureHand == null){
				treasureHand = GameObject.FindGameObjectWithTag("TreasureAIHand").GetComponent<AIHand>();	
				treasureHand.FollowTreasure(this);
			}
				
		}
		
		if(!treasureFilled)
			DoTreasureFill();
		else
			MoveToShip();
	}
	
//	void OnCollisionEnter(Collision col)
//	{
//		if(col.collider.tag == "PirateShip")
//		{
//			col.gameObject.GetComponent<Pirate_Ship>().getTreasure();
//			Destroy(gameObject);
//		}
//	}
	
	void OnTriggerEnter(Collider col)
	{
		if(treasureFilled && col.tag == "PirateShip")
		{
			col.gameObject.GetComponent<Pirate_Ship>().getTreasure();
			Destroy(gameObject);
		}
	}
	
	void OnTriggerStay(Collider col)
	{
		if(treasureFilled && col.tag == "PirateShip")
		{
			col.gameObject.GetComponent<Pirate_Ship>().getTreasure();
			Destroy(gameObject);
		}
	}
	
	void MoveToShip()
	{
		Vector3 shipDir = ship.transform.position - transform.position;
		shipDir.Normalize();
		transform.Translate(shipDir * treasureChaseSpeed * Time.deltaTime);
	}
	
	void DoTreasureFill()
	{
		
		if(holdTreasure &&  lerpParameter < 1)
		{
			LerpTreasureFill(0,1, true);
		}
		
		else if(!holdTreasure && lerpParameter > 0)
		{
			LerpTreasureFill(1,0, false);
		}
	}
	
	void LerpTreasureFill(int initial, int end, bool filling)
	{
		lerpParameter += Time.deltaTime/fillTreasureDuration;
		
		if(lerpParameter > 1 && filling ) 
		{
			lerpParameter = 1;
			treasureFilled = true;
			treasureFill.enabled = false;
			ship.EnableWheel();
			myIntScale.Play(false);
			
			if(manager.activateTreasureAI){
				treasureHand.UnselectTreasure();
			}
		}
		else if(lerpParameter < 0) lerpParameter = 0;
		
		treasureFill.fillAmount = Mathf.Lerp(initial, end, lerpParameter);
		
		
	}
}
