using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma strict

public class Wheel_Ship : MonoBehaviour {
	public Camera sceneCamera;
	public Pirate_Ship myShip;
	
	List<Transform> listChildren;
	
	bool dragging = false;
	bool canRotate = false;
	public bool isHoldingTreasure = false;
	
	public Vector3 arrowDirection;
	public Transform wheel_Dir;
	
	public bool movementAIEnabled = false;
	bool goingRight = true;
	bool stopWheel = true;
	
	public float angleAI;
	float angleDuration = 3f;
	float maxAngle = 170f;
	float minAngle = 10f;
	float lerpAngle = 0f;
	
	float lerpSpriteAngle = 0f;
	
	public Selfish_Sam_Manager manager;
	public AIHand movementAIHand;
	public Transform handAnchor;
	
	public enum WheelState
	{
		NONE, DRAGGING, RELEASING
	}
	
	WheelState myState = WheelState.NONE;
	
	
	void Start()
	{
		listChildren = new List<Transform>();
		
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Selfish_Sam_Manager>();
		
		myShip = GameObject.Find("PirateShip").GetComponent<Pirate_Ship>();
	
		foreach (Transform child in transform)
  			listChildren.Add(child);
		
	}
	
//	void OnGUI()
//	{
//		if(Event.current.type == EventType.MouseDrag && canRotate)
//			dragging = true;
//	}
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0) && !dragging)
		{
			ParentChildren(false);
			RotateWheel();
			ParentChildren(true);
		}
		
		if(movementAIHand.transform.position != handAnchor.position)
			movementAIHand.transform.position = handAnchor.position;
//		else if(Input.GetMouseButtonUp(0))
//			dragging = false;

	}
	
	void LateUpdate()
	{
		if(!ApplicationState.Instance.isPaused()){
			if(!movementAIEnabled)
				RotateWheel();
			else
				RotateWheelSprite();
		}

	}
	
	void ParentChildren(bool isParent)
	{
		foreach(Transform child in listChildren)
		{
			if(isParent)
				child.parent = this.transform;
			else
				child.parent = null;
		}
	}
	
	float CalculateShipVelocity(float angle)
	{
		float result;
		
		if(angle >= 92f){
			result = (angle - 90) / 160f;
		}
		
		else if(angle <= 88f)
			result = - (90f - angle) / 160f;
		
		else{
			result = 0f;	
		}
			
		if(result > myShip.maxHorizontalSpeed)
			result = myShip.maxHorizontalSpeed;
		
		return result;
	}
	
	void RotateWheel()
	{
		//Shoot ray into the screen from the mouse position
		Ray ray = sceneCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		
		
		
		//Check that it hits the collider 
    	if (collider.Raycast (ray, out hit, 3f) && Input.GetMouseButton(0) && !isHoldingTreasure) 
		{
			myState = WheelState.DRAGGING;
			//canRotate = true;
			//Will be used for creation of plane
			Vector3 forward = Vector3.forward;
			
			Plane playerPlane = new Plane(-forward, transform.position);
			
			float hitdist = 0.0f;
			
			playerPlane.Raycast(ray, out hitdist);
			
        	// Get the point along the ray that hits the calculated distance.
        	Vector3 targetPoint = ray.GetPoint(hitdist);
			
			//Where the "Wheel" will point to..
        	Vector3 aimPoint = targetPoint - transform.position;
			
			Vector3 prevDirection = new Vector3();
			
			prevDirection = transform.up;
			transform.up = aimPoint;
			
			float angle = Vector2.Angle(Vector2.right, (Vector2) wheel_Dir.up);
			
			if( wheel_Dir.up.y <= 0 || angle > 170f || angle < 10f)
			{
				//Debug.Log("Can't Rotate");
				transform.up = prevDirection;
				
			}
			
			else
				myShip.SteerShip(CalculateShipVelocity(angle));
			
    	}
		
		else if (myState == WheelState.DRAGGING)
		{
			myShip.WheelReloadCannon();
			myState = WheelState.NONE;
		}
	}
	
	public void EnableMovementAI()
	{
		angleAI = 90f;
		lerpAngle = 0f;
		movementAIEnabled = true;
		//goingRight = true;
		movementAIHand.ChangeUnselectHandTexture();
		
		RotateWheel_AI();
		//InvokeRepeating("RotateWheel_AI", 0f, 0.5f);
	}
	
	public void DisableMovementAI()
	{
		angleAI = 90f;
		lerpAngle = 0f;
		movementAIEnabled = false;
		goingRight = true;
		movementAIHand.Reset();
		
		myShip.SteerShip(CalculateShipVelocity(angleAI));
	}
	
	public void moveToOtherSide_AI()
	{
		goingRight = !goingRight;
		if(movementAIEnabled)
		{
			
			angleAI = 90f;
			lerpAngle = 0f;
		}
	}
	
	public void StopWheel_AI()
	{
		stopWheel = true;
		angleAI = 90f;
		lerpAngle = 0f;
		
		if(manager.activateMovementAI)
			movementAIHand.ChangeUnselectHandTexture();
		
		RotateWheel_AI();
	}
	
	
	
	public void StartWheel_AI(){
		stopWheel = false;
	}
	
	public void RotateWheel_AI()
	{
		if(manager.activateMovementAI)
		{
			if(!movementAIEnabled){
				EnableMovementAI();
			}
			
			if(!stopWheel)
			{
				if(goingRight)
				{
					if(lerpAngle < 1f)
					{
						lerpAngle += Time.deltaTime / angleDuration;
						angleAI = Mathf.Lerp(90f, minAngle, lerpAngle);
						movementAIHand.ChangePressedHandTexture();
						
						if (lerpAngle > 1f)
							lerpAngle = 1;
						
					}
					else
						movementAIHand.ChangeUnselectHandTexture();
					
					
				}
				
				else
				{
					if(lerpAngle < 1f)
					{
						lerpAngle += Time.deltaTime / angleDuration;
						angleAI = Mathf.Lerp(90, maxAngle, lerpAngle);
						movementAIHand.ChangePressedHandTexture();
						
						if (lerpAngle > 1f)
							lerpAngle = 1;
					}
					
					else
						movementAIHand.ChangeUnselectHandTexture();
				}
			}
				
			myShip.SteerShip(CalculateShipVelocity(angleAI));
		}
		else
			DisableMovementAI();

	}
	
	void RotateWheelSprite()
	{
		if(!stopWheel)
		{
			if(goingRight)
			{
				float angle = Vector2.Angle(Vector2.right, (Vector2) wheel_Dir.up);
				
			
				if(angle > 30f)
				{
					transform.Rotate( new Vector3(0,0, -1f));
					
				}
				
				
			}
			
			else
			{
				float angle = Vector2.Angle(Vector2.right, (Vector2) wheel_Dir.up);
			
				
				if(angle < 170f)
				{
					transform.Rotate( new Vector3(0,0, 1f));
					
				}
			}
		}
		
		else
		{
			float angle = Vector2.Angle(Vector2.right, (Vector2) wheel_Dir.up);
			if(angle > 92f)
				transform.Rotate(new Vector3(0,0, -2f));
			
			else if (angle < 88f)
				transform.Rotate( new Vector3(0,0, 2f));

			
			

		}
		
		
	}
	

		
		
}
