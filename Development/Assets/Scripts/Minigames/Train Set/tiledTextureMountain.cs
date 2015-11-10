using UnityEngine;
using System.Collections;

public class tiledTextureMountain : MonoBehaviour {
	
	float initalTextureX;
	float currTextureX;
	UITexture myTexture;
	float lerpParameter = 0f;
	public float movSpeed = 15f;
	
	// Use this for initialization
	void Start () {
		myTexture = GetComponent<UITexture>();
		initalTextureX = myTexture.uvRect.x;
		currTextureX = initalTextureX; 
	}
	
	// Update is called once per frame
	void Update () {
		
		if(lerpParameter < 1){
			lerpParameter += Time.deltaTime/movSpeed;
			Rect newRect = myTexture.uvRect;
			newRect.x = Mathf.Lerp(0, 1, lerpParameter);
			myTexture.uvRect = newRect;
		}
		
		else{
			lerpParameter = 0;
			Rect newRect = myTexture.uvRect;
			newRect.x = initalTextureX;
			myTexture.uvRect = newRect;
		}
		
	}
}