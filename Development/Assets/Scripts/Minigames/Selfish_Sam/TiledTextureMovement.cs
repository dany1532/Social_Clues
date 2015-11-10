using UnityEngine;
using System.Collections;

public class TiledTextureMovement : MonoBehaviour 
{
	public static float startMovSpeed = 25f;
	public static float movSpeed = 25f;
	float lerpParameter = 0f;
	float initalTextureY;
	UITexture myTexture;
	Selfish_Sam_Manager manager;
	
	// Use this for initialization
	void Awake(){
		//movSpeed = startMovSpeed;
	}
	
	void Start () 
	{
		//movSpeed = startMovSpeed;
		myTexture = GetComponent<UITexture>();
		initalTextureY = myTexture.uvRect.y;
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Selfish_Sam_Manager>();
		//movSpeed -= movSpeed * manager.verticalSpeedIncrease;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(lerpParameter < 1){
			lerpParameter += Time.deltaTime/movSpeed;
			Rect newRect = myTexture.uvRect;
			newRect.y = Mathf.Lerp(0, 1, lerpParameter);
			myTexture.uvRect = newRect;
		}
		
		else{
			lerpParameter = 0;
			Rect newRect = myTexture.uvRect;
			newRect.y = initalTextureY;
			myTexture.uvRect = newRect;
		}
			
	}
}
