using UnityEngine;
using System.Collections;

public class InteractiveScale : MonoBehaviour {
	public bool wantLoop = false;
	bool holdingInteractable = false;
	public float scaleFactor = 1.2f;
	public float scaleDuration= 0.1f;
	float lerpParameter = 0f;
	Vector3 initialScale;
	Vector3 finalScale;
	bool switchLerp = false;
	public bool playOnStart = false;
	
	void Start()
	{
		initialScale = transform.localScale;
		finalScale = transform.localScale * scaleFactor;
		
		if(playOnStart)
		{
			Play(true);
		}
	}
	
	
	public void Play(bool pressed)
	{
		if(pressed)
		{
			if(lerpParameter > 0) lerpParameter = 1f - lerpParameter;
			holdingInteractable = true;
		}
		
		else
		{
			if(lerpParameter < 1) lerpParameter = 1f - lerpParameter;
			
			else 
				lerpParameter = 1;
			
			holdingInteractable = false;
		}
	}
	
	public void Stop()
	{
		holdingInteractable = false;
	}
	
	void Update()
	{
		if(!wantLoop){
			if(holdingInteractable &&  lerpParameter < 1)
				LerpScale(initialScale, finalScale);
			
			else if(!holdingInteractable && lerpParameter > 0)
				LerpScale(finalScale, initialScale);
		}
		
		else if(wantLoop && holdingInteractable)
		{
			if(lerpParameter < 1){
				if(!switchLerp)
					LerpScale(initialScale, finalScale);
				
				if(switchLerp)
					LerpScale(finalScale, initialScale);
			}	
		}
		
		else
			transform.localScale = initialScale;
			
	}
	
	void LerpScale(Vector3 initial, Vector3 end)
	{
		lerpParameter += Time.deltaTime/scaleDuration;
		
		transform.localScale = Vector3.Lerp(initial, end, lerpParameter);
		
		if(lerpParameter > 1 ) lerpParameter = 1;
		else if(lerpParameter < 0) lerpParameter = 0;
		
		if(wantLoop && lerpParameter == 1){
			lerpParameter = 0;
			switchLerp = !switchLerp;
		}
		else if(wantLoop && lerpParameter == 0)
			switchLerp = !switchLerp;
	}
}
