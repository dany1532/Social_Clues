using UnityEngine;
using System.Collections;

public class tiledTextureTrain : MonoBehaviour {

	float initalTextureY;
	float currTextureY;
 	UITexture myTexture;
	float lerpParameter = 0f;
	public float movSpeed = 15f;

	// Use this for initialization
	void Start () {
		myTexture = GetComponent<UITexture>();
		initalTextureY = myTexture.uvRect.y;
		currTextureY = initalTextureY; 
	}
	
	// Update is called once per frame
	void Update () {

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
