using UnityEngine;
using System.Collections;

public class StarAnimationFX : MonoBehaviour {
	
	public PositionAnimationFX posFX;
	public ScaleAnimationFX scaleFX;
	public RotationAnimationFX rotFX;
	
	public Vector3 initialPos;
	public Vector3 finalPos;
	public Vector3 initialScale;
	public Vector3 centerScale;
	public Vector3 finalScale;
	
	Quaternion initialRot;
	
	public StarCompleteDelegate starCompleteDelegate;
	public float returnDuration = 1.0f;
	public float initialDuration = 0.5f;
	public float stillDuration = 0.75f;
	
	public delegate void StarCompleteDelegate(StarAnimationFX anim);
	
	public GameObject startAnimationPrefab;
	
	// Use this for initialization
	void Awake () {
		initialPos = this.transform.localPosition;
		initialScale = this.transform.localScale;
		initialRot = this.transform.localRotation;
	}
	
	public void PlayAnimation(){
		this.transform.localPosition = initialPos;
		this.transform.localScale = initialScale;
		this.transform.localRotation = initialRot;
		
		rotFX.Reset();
		posFX.Reset();
		scaleFX.Reset();
		
		scaleFX.duration = initialDuration;
		scaleFX.IntializeScaleLerp(initialScale, centerScale);
		scaleFX.animationCompleteDelegate = EndAnimation;
		
		scaleFX.PlayAnimation();
	}
	
	public void SetFinalPos(Vector3 pos){
		finalPos = pos;	
		finalPos.z = -3f;
	}
	
	private void EndAnimation(ScaleAnimationFX anim, string what){
		/*
		GameObject starParticle = GameObject.Instantiate(startAnimationPrefab) as GameObject;
		starParticle.transform.parent = this.transform;
		starParticle.transform.localPosition = new Vector3(0, 0, 1);
		starParticle.transform.parent = null;
		*/
		Invoke("StartFinalAnimation", stillDuration);
	}
	
	private void StartFinalAnimation()
	{
		scaleFX.animationCompleteDelegate = null;
		
		scaleFX.Reset();
		posFX.Reset();
		rotFX.Reset();
		
		posFX.InitializePositionLerp(initialPos, finalPos, false);
		posFX.duration = returnDuration;
		
		scaleFX.IntializeScaleLerp(centerScale, finalScale);
		scaleFX.duration = returnDuration;
		
		rotFX.animationCompleteDelegate = AnimationCompleted;
		rotFX.duration = returnDuration;
		
		posFX.PlayAnimation();
		scaleFX.PlayAnimation();
		rotFX.PlayAnimation();
	}
	
	private void AnimationCompleted(RotationAnimationFX anim, string what){
		rotFX.animationCompleteDelegate = null;	
		posFX.animationCompleteDelegate = null;
		
		rotFX.Reset();
		scaleFX.Reset();
		posFX.Reset();
		
		OnCompleteAnimation();
	}
	
	void  OnCompleteAnimation(){
		if(starCompleteDelegate != null)
			starCompleteDelegate(this);
	}
}
