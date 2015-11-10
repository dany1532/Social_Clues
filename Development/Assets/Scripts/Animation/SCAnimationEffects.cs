using UnityEngine;
using System.Collections;



public class SCAnimationEffects : MonoBehaviour {
 	Transform scTransform;
	Rigidbody scRigidbody;
	UISprite  scSprite;
	
    public SubMenuLerpPositionClass pos_Anim;
	public SubMenuSpinClass 		spin_Anim;
	public SubMenuScaleLerpClass	scale_Anim;
	public SubMenuFadeInClass		fadein_Anim;
	public SubMenuFadeOutClass      fadeout_Anim;
	public SubMenuChangeColorClass  colorChange_Anim;
	public SubMenuShakeClass		shake_Anim;
	
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	// Use this for initialization
	void Start () {
		scTransform = this.transform;
		scSprite = GetComponent<UISprite>();
	}
	
	// Update is called once per frame
	void Update () {
		if(spin_Anim.isActive){
			ApplySpin(Time.deltaTime);
		}
		
		if(pos_Anim.isActive){
			ApplyLerp(Time.deltaTime);
		}
		
		if(scale_Anim.isActive){
			ApplyScaleAnim(Time.deltaTime);
		}
			
		if(fadein_Anim.isActive){
			ApplyFadeInAnim(Time.deltaTime);	
		}
		
		if(fadeout_Anim.isActive){
			ApplyFadeOutAnim(Time.deltaTime);	
		}
		
		if(colorChange_Anim.isActive){
			ApplyColorChangeAnim(Time.deltaTime);	
		}
		
		if(shake_Anim.isActive){
			ApplyShakeAnim(Time.deltaTime);	
		}
	}
	
	public void PlayAnimation(){
		scSprite = GetComponent<UISprite>();
		
		if(pos_Anim.playAnimation){
			pos_Anim.target = pos_Anim.target;
			pos_Anim.boomerangReturnPos = transform.localPosition;
			pos_Anim.isActive = true;
		}
		
		if(spin_Anim.playAnimation){
			spin_Anim.isActive = true;	
			Invoke("EndSpin", spin_Anim.duration);
		}
		
		if(scale_Anim.playAnimation){
			scale_Anim.isActive = true;	
		}
		
		if(fadein_Anim.playAnimation){
			Debug.Log("here?");
			scSprite.alpha = 0;
			fadein_Anim.isActive = true;	
		}
		
		if(colorChange_Anim.playAnimation){
			colorChange_Anim.isActive = true;	
		}
		
		if(fadeout_Anim.playAnimation){
			fadeout_Anim.isActive = true;	
		}
		
		if(colorChange_Anim.playAnimation){
			colorChange_Anim.isActive = true;	
		}
		
		if(shake_Anim.playAnimation){
			shake_Anim.isActive = true;	
		}
	}
	
	public void Reset(){
		pos_Anim.playAnimation = false;

		
		spin_Anim.playAnimation = false;

		
		scale_Anim.playAnimation = false;

		
		fadein_Anim.playAnimation = false;

		
		colorChange_Anim.playAnimation = false;

		
		fadeout_Anim.playAnimation = false;

		
		colorChange_Anim.playAnimation = false;

		
		shake_Anim.playAnimation = false;

	}
	
	
	public void InitializePositionLerp(Vector3 targetVec, bool wantBoomerang){
		pos_Anim.playAnimation = true;
		pos_Anim.target = targetVec;
		pos_Anim.boomerang = wantBoomerang;
	}
	
	public void InitializePositionLerp(Vector3 targetVec, float duration, bool wantBoomerang){
		pos_Anim.playAnimation = true;
		pos_Anim.target = targetVec;
		pos_Anim.duration = duration;
		pos_Anim.boomerang = wantBoomerang;
	}
	
	
	public void IntializeScaleLerp(Vector3 targetVec){
		scale_Anim.playAnimation = true;
		scale_Anim.destinationScale = targetVec;
	}
	
	public void IntializeScaleLerp(Vector3 targetVec, float duration){
		scale_Anim.playAnimation = true;
		scale_Anim.destinationScale = targetVec;
		scale_Anim.duration = duration;
	}
	
	public void InitializeSpin(float duration, int numberOfSpins, bool clockwise){
		spin_Anim.playAnimation = true;
		
	}
	
	public void EndSpin(){
		spin_Anim.endSpin = true;	
	}
	
	public delegate void AnimationCompleteDelegate(SCAnimationEffects scAnim, string whatEffect);

	
	void ApplySpin(float delta){
		
		delta *= Mathf.Rad2Deg * Mathf.PI * 2f;
		
		Quaternion offset = Quaternion.Euler(spin_Anim.rotationsPerSecond * delta);
		

		if(spin_Anim.endSpin){
			scTransform.rotation = NGUIMath.SpringLerp(scTransform.rotation, spin_Anim.finalSpin,
														0.1f, delta);
			
			
			if(scTransform.rotation == Quaternion.identity){
				spin_Anim.endSpin = false;
				spin_Anim.isActive = false;
			}
		}
		
		else{
			scTransform.rotation = scTransform.rotation * offset;
		}
	}
	
	void ApplyLerp(float delta){
		
		pos_Anim.t += delta/pos_Anim.duration;
		pos_Anim.t = Mathf.Clamp(pos_Anim.t, 0, 1);
		
		//if (pos_Anim.threshold == 0f) {
			//pos_Anim.threshold = (pos_Anim.target - scTransform.localPosition).magnitude * 0.001f;
		//}
		
		//if there is no boomerang effect or hasn't started
		if(!pos_Anim.startBoomerang){
			
			//scTransform.localPosition = NGUIMath.SpringLerp(scTransform.localPosition, pos_Anim.target, 
			//											pos_Anim.strength, delta);
			
			scTransform.localPosition = Vector3.Lerp(scTransform.localPosition, pos_Anim.target,
														pos_Anim.t);
	
			if (pos_Anim.threshold >= (pos_Anim.target - scTransform.localPosition).magnitude){
				scTransform.localPosition = pos_Anim.target;
				
				if(pos_Anim.boomerang) {pos_Anim.startBoomerang = true; pos_Anim.t = 0f;}
				
				else{
					pos_Anim.playAnimation = false;
					pos_Anim.isActive = false;
					pos_Anim.t = 0f;
					OnCompleteAnimation(AnimationPrefix.POSITION_LERP);
					if(spin_Anim.isActive) {spin_Anim.endSpin = true;}
				}
			}
		}
		
		
		else {
			//scTransform.localPosition = NGUIMath.SpringLerp(scTransform.localPosition, pos_Anim.boomerangReturnPos, 
													//	pos_Anim.strength, delta);
			
			scTransform.localPosition = Vector3.Lerp(scTransform.localPosition, pos_Anim.boomerangReturnPos,
														pos_Anim.t);
			
			if (pos_Anim.threshold >= (pos_Anim.boomerangReturnPos - scTransform.localPosition).magnitude){
				scTransform.localPosition = pos_Anim.boomerangReturnPos;
				pos_Anim.isActive = false;
				pos_Anim.startBoomerang = false;
				pos_Anim.t = 0f;
				OnCompleteAnimation(AnimationPrefix.POSITION_LERP);
			}
		}
	}
	
	void ApplyScaleAnim(float delta){
		scale_Anim.t += delta/scale_Anim.duration;
		scale_Anim.t = Mathf.Clamp(scale_Anim.t, 0, 1);
		
		//if (scale_Anim.threshold == 0f) {
			//scale_Anim.threshold = (scale_Anim.destinationScale - scTransform.localScale).magnitude * 0.001f;
		//}
		
		//scTransform.localScale = NGUIMath.SpringLerp(scTransform.localScale, scale_Anim.destinationScale,
		//											scale_Anim.strength, delta);
		
		scTransform.localScale = Vector3.Lerp(scTransform.localScale, scale_Anim.destinationScale,
														scale_Anim.t);
		
		if(scale_Anim.threshold >= (scale_Anim.destinationScale - scTransform.localScale).magnitude){
			Debug.Log("now");
			scale_Anim.isActive = false;
			scale_Anim.playAnimation = false;
			scale_Anim.t = 0f;
			OnCompleteAnimation(AnimationPrefix.SCALE_LERP);
		}
	}
	
	void ApplyFadeInAnim(float delta){
		float alpha = scSprite.alpha;
		fadein_Anim.t += delta/fadein_Anim.duration;
		
		//scSprite.alpha = NGUIMath.SpringLerp(alpha, 1, fadein_Anim.strength, delta);
		scSprite.alpha = Mathf.Lerp(alpha, 1, fadein_Anim.t);
		
		if(scSprite.alpha > fadein_Anim.threshold){
			scSprite.alpha = 1;
			fadein_Anim.isActive = false;
			fadein_Anim.t = 0;
		}
		
		
	}
	
	void ApplyFadeOutAnim(float delta){
		float alpha = scSprite.alpha;
		fadeout_Anim.t += delta/fadeout_Anim.duration;
		//scSprite.alpha = NGUIMath.SpringLerp(alpha, 0, fadeout_Anim.strength, delta);
		scSprite.alpha = Mathf.Lerp(alpha, 0, fadeout_Anim.t);
		
		
		if(scSprite.alpha < fadeout_Anim.threshold){
			scSprite.alpha = 0;
			fadeout_Anim.isActive = false;
		}
		
	}
	
	void ApplyColorChangeAnim(float delta){
		Color color = scSprite.color;
		colorChange_Anim.t += delta/colorChange_Anim.duration;
		
		//scSprite.color = NGUIMath.SpringLerp(scSprite.color, colorChange_Anim.to, 
													//colorChange_Anim.strength, delta);
		
		scSprite.color = Color.Lerp(scSprite.color, colorChange_Anim.to, colorChange_Anim.t);
		
		//scSprite.color = Color.Lerp(scSprite.color, colorChange_Anim.to, 
		
		//Vector3 vecTo = new Vector3(colorChange_Anim.to.r, colorChange_Anim.to.g, colorChange_Anim.to.b);
		//pos_Anim.threshold >= (pos_Anim.target - scTransform.localPosition).magnitude
		if(color == colorChange_Anim.to){
			colorChange_Anim.isActive = false;
			colorChange_Anim.t = 0f;
		}
		
	}
	
	void ApplyShakeAnim(float delta){
		Vector3 shakeVector = transform.localPosition;
		
		if(shake_Anim.shake > 0){
			shakeVector += Random.insideUnitSphere * shake_Anim.shakeAmount;
			shakeVector.z = transform.localPosition.z;
			transform.localPosition = shakeVector;
			shake_Anim.shake -= delta * shake_Anim.decreaseFactor;
		}
		
		else{
			shake_Anim.shake = 0;
			shake_Anim.isActive = false;
		}
	}
	
	void  OnCompleteAnimation(string whatAnimation){
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this, whatAnimation);
	}
}



[System.Serializable]
public class SubMenuSpinClass{
	public bool playAnimation = false;
	public bool isActive = false;
	public float duration = 1f;
	public Vector3 rotationsPerSecond = new Vector3(0,0,1f);
	
	//Hide from inspector
	[HideInInspector] public bool endSpin = false;
	public Quaternion finalSpin = Quaternion.Euler(new Vector3(0, 0, 0));
	public int rotation = 0;
}

[System.Serializable]
public class SubMenuLerpPositionClass{
	public bool playAnimation = false;
	public bool boomerang = false;
	public bool isActive = false;
	public float duration = 2.5f;
	public Vector3 target;
	public Vector3 boomerangReturnPos;
	
	//must hide from inspector
	[HideInInspector] public bool startBoomerang = false;
	public float threshold = 0f;
	[HideInInspector] public float t = 0f;
	
}

[System.Serializable]
public class SubMenuScaleLerpClass{
	public bool playAnimation = false;
	public bool isActive = false;
	public float duration = 2.5f;
	public Vector3 destinationScale;
	
	//must hide from inspector
	//public Vector3 target;
	public float threshold = 1f;
	[HideInInspector] public float t = 0f;
	
}

[System.Serializable]
public class SubMenuFadeInClass{
	public bool playAnimation = false;
	public bool isActive = false;
	public float duration = 2.5f;
	
	//must hide from inspector
	//public Vector3 target;
	public float threshold = 0.9f;
	public float t = 0f;
}

[System.Serializable]
public class SubMenuFadeOutClass{
	public bool playAnimation = false;
	public bool isActive = false;
	public float duration = 2.5f;
	
	//must hide from inspector
	//public Vector3 target;
	public float threshold = 0.1f;
	public float t = 0f;
}

[System.Serializable]
public class SubMenuChangeColorClass{
	public bool playAnimation = false;
	public bool isActive = false;
	public float duration = 2.5f;
	public Color to;
	
	//must hide from inspector
	//public Vector3 target;
	public float threshold = .9f;
	public float t = 0f;
}

[System.Serializable]
public class SubMenuShakeClass{
	public bool playAnimation = false;
	public bool isActive = false;
	public float shake = 2.0f;
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	//must hide from inspector
	//public Vector3 target;
	//public float threshold = .9f;
}


