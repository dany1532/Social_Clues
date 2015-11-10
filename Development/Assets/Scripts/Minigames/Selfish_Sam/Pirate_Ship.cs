using UnityEngine;
using System.Collections;

public class Pirate_Ship : MonoBehaviour 
{
	public Selfish_Sam_Manager manager;
	
	public float maxHorizontalSpeed = 0.5f;
	float currentHorizontalVelocity = 0.0f;
	
	public Wheel_Ship myWheel;
	
	public GameObject cannonBall;
	public Transform cannonTransform;
	public Transform cannonSpriteTransform;
	public Transform kablamPos;
	
	public float reloadingDuration = 0.6f;
	bool reloadCannon = false;
	
	public UITexture kablamTexture;
	public SCSpriteAnimation cannonAnim;
	
	GameObject lastCollision;
    Vector3 lastTranslation;
	
	public Camera sceneCamera;
	bool invincibilityFrames = false;
	public AudioClip cannonFX;
	public AudioClip crashFX;
    
	// Use this for initialization
	void Start () 
	{
		sceneCamera = GameObject.FindGameObjectWithTag("MinigameCamera").GetComponent<Camera>();
		kablamTexture.GetComponent<InteractiveScale>().Play(true);
        lastCollision = null;
        lastTranslation = Vector3.zero;
	}
	
	
	public void SteerShip(float velocity)
	{
		currentHorizontalVelocity = velocity;
	}
	
	public void ShootAtPosition(Vector3 inputPos)
	{
		if(!reloadCannon)
		{
			AudioManager.Instance.Play(cannonFX, transform, 0.1f, false);
			GameObject go = AddChild(transform.parent.gameObject, cannonBall);
			go.name = "CannonBall";
			
			Vector3 pos = cannonTransform.position;
			inputPos.z = cannonTransform.position.z;
			go.transform.position = pos;
			
			Vector3 aim = inputPos - cannonTransform.position;
			cannonSpriteTransform.up = aim;
			
			go.GetComponent<Cannon_ball>().SetCannonBall(inputPos, cannonTransform);
			//go.transform.rotation = cannonSpriteTransform.rotation;
			cannonAnim.Play();
			cannonAnim.GetComponent<UITexture>().enabled = true;
			StartReloading();
			
		}
		
	}
	
	public void StartReloading()
	{
		reloadCannon = true;	
		Invoke("ReloadCannonBall", reloadingDuration);
	}
	
	public void WheelReloadCannon()
	{
		reloadCannon = true;
		Invoke("ReloadCannonBall", 0.1f);
	}
	
	void ReloadCannonBall()
	{
		cannonAnim.GetComponent<UITexture>().enabled = false;
		reloadCannon = false;
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(!invincibilityFrames)
		{
			if(other.tag == "Rock")
			{
				shipCollidedAnim();
				manager.ShipCollide();
	            //other.collider.enabled = false;
			}
			
			else if(other.name == "Enemy")
			{
				if(!other.GetComponent<Enemy_Minigame>().wasCaptured)
				{
					shipCollidedAnim();
					manager.ShipCollide();
		            Destroy(other.gameObject);
				}
				
			}
	        else if(other.name == "Boundary_Beach")
			{
				myWheel.moveToOtherSide_AI();
			}
		}
	}
    
    void OnTriggerStay(Collider other)
    {
        if(other.name == "Boundary_Beach")
        {
            Vector3 boundPosition = Vector3.zero;
            if (other.transform.localPosition.x > 0)
            {
                boundPosition.x = other.transform.position.x - 0.75f * other.bounds.size.x;
                boundPosition.z = transform.position.z;
                transform.position = boundPosition;
            }
            else
            {
                boundPosition.x = other.transform.position.x + 0.75f * other.bounds.size.x;
                boundPosition.z = transform.position.z;
                transform.position = boundPosition;
            }
        }
    }
	
	public void getTreasure(){
		manager.updateTreasureBar();
	}
	
	public void shipCollidedAnim()
	{
		AudioManager.Instance.Play(crashFX, transform, 0.1f, false);
		kablamTexture.enabled = true;
		invincibilityFrames = true;
		Invoke("stopShipCollidedAnim", 2f);
	}
	
	public void stopShipCollidedAnim(){
		kablamTexture.enabled = false;	
		invincibilityFrames = false;
	}
	
	public void StopWheel()
	{
		if(!manager.activateTreasureAI){
			myWheel.isHoldingTreasure = true;	
		}
	}
	
	public void EnableWheel()
	{
		if(!manager.activateTreasureAI){
			myWheel.isHoldingTreasure = false;
		}
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		kablamTexture.transform.position = kablamPos.position;
		
		//cannonSpriteTransform.position = cannonTransform.position;
		//cannonAnim.transform.position = cannonTransform.position;
		//cannonAnim.transform.Translate(cannonSpriteTransform.up * (0.1f));
		
		//rigidbody.velocity = Vector3.zero;
        lastTranslation.x = currentHorizontalVelocity * Time.deltaTime;
		transform.Translate(lastTranslation);
	
		//rigidbody.velocity = new Vector3(currentHorizontalVelocity * Time.deltaTime, 0, 0);
	}
	
	void LateUpdate()
	{
		if(InputManager.Instance.HasReceivedClick() && !manager.activateShootingAI)
		{
			Vector3 mousePos = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
			ShootAtPosition(mousePos);
		}
	}
	
	public GameObject AddChild (GameObject parent, GameObject prefab)
	{
		GameObject go = GameObject.Instantiate(prefab) as GameObject;

		if (go != null && parent != null)
		{
			go.layer = parent.layer;
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = prefab.transform.localScale;
			
		}
		
		return go;
	}
}
