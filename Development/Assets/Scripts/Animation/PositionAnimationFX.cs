using UnityEngine;
using System.Collections;

public class PositionAnimationFX : MonoBehaviour {
	public bool playAnimation = false;
	public bool boomerang = false;
	public float duration = 1.5f;
	public Vector3 target;
	public Vector3 boomerangReturnPos;
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	private Transform scTransform;
	private Vector3 initialPosition;
	
	public bool useLocalPosition = true;
	
	//must hide from inspector
	[HideInInspector] public bool startBoomerang = false;
	public float threshold = 0.01f;
	[HideInInspector] public float t = 0f;
	public bool isActive = false;
	
	
	
	public delegate void AnimationCompleteDelegate(PositionAnimationFX scAnim, string whatEffect);
	
	void  OnCompleteAnimation(string whatAnimation){
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this, whatAnimation);
	}
	
	// Use this for initialization
	void Start () {
		scTransform = this.transform;
		if (useLocalPosition)
			initialPosition = transform.localPosition;
		else
			initialPosition = transform.position;
	}
	
	
	// Update is called once per frame
	void Update () {
		if(isActive)
			ApplyAnimation(Time.deltaTime);
	}
	
	
	
	void ApplyAnimation(float delta){
		
		t += delta/duration;
		t = Mathf.Clamp(t, 0, 1);
		
		//if (pos_Anim.threshold == 0f) {
			//pos_Anim.threshold = (pos_Anim.target - scTransform.localPosition).magnitude * 0.001f;
		//}
		
		//if there is no boomerang effect or hasn't started
		if(!startBoomerang){
			
			//scTransform.localPosition = NGUIMath.SpringLerp(scTransform.localPosition, pos_Anim.target, 
			//											pos_Anim.strength, delta);
			
			if (useLocalPosition)
				scTransform.localPosition = Vector3.Lerp(initialPosition, target, t);
			else
				scTransform.position = Vector3.Lerp(initialPosition, target, t);
	
			if (threshold >= (target - (Vector3)((useLocalPosition) ? scTransform.localPosition : scTransform.position)).magnitude)
			{
				if (useLocalPosition)
					scTransform.localPosition = target;
				else
					scTransform.position = target;
				
				if(boomerang) {startBoomerang = true; t = 0f;}
				
				else{
					playAnimation = false;
					isActive = false;
					t = 0f;
					OnCompleteAnimation(AnimationPrefix.POSITION_LERP);
					//if(spin_Anim.isActive) {spin_Anim.endSpin = true;}
				}
			}
		}
		
		
		else {
			//scTransform.localPosition = NGUIMath.SpringLerp(scTransform.localPosition, pos_Anim.boomerangReturnPos, 
													//	pos_Anim.strength, delta);
			
			if (useLocalPosition)
				scTransform.localPosition = Vector3.Lerp(initialPosition, boomerangReturnPos, t);
			else
				scTransform.position = Vector3.Lerp(initialPosition, boomerangReturnPos, t);
			
			if (threshold >= (boomerangReturnPos - (Vector3)((useLocalPosition) ? scTransform.localPosition : scTransform.position)).magnitude)
			{
				if (useLocalPosition)
					scTransform.localPosition = boomerangReturnPos;
				else
					scTransform.position = boomerangReturnPos;
				
				isActive = false;
				startBoomerang = false;
				t = 0f;
				OnCompleteAnimation(AnimationPrefix.POSITION_LERP);
			}
		}
	}
	
	
	public void PlayAnimation(){
		if(playAnimation){
			//target = pos_Anim.target;
			if (useLocalPosition)
				boomerangReturnPos = transform.localPosition;
			else
				boomerangReturnPos = transform.position;
			isActive = true;
		}
	}
	
	public void Reset(){
		isActive = false;
		t = 0f;
		playAnimation = false;
	}
	
	public void InitializePositionLerp(Vector3 initialVec, Vector3 targetVec, bool wantBoomerang){
		t = 0f;
		playAnimation = true;
		target = targetVec;
		boomerang = wantBoomerang;
		initialPosition = initialVec;
	}
	
	public void InitializePositionLerp(Vector3 initialVec, Vector3 targetVec, float dur, bool wantBoomerang){
		t = 0f;
		playAnimation = true;
		target = targetVec;
		duration = dur;
		boomerang = wantBoomerang;
		initialPosition = initialVec;
	}
}
