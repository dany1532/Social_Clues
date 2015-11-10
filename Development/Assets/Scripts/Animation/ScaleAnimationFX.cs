using UnityEngine;
using System.Collections;

public class ScaleAnimationFX : MonoBehaviour {
	public bool playAnimation = false;
	public float duration = 2.5f;
	
	public Vector3 initialScale;
	public Vector3 destinationScale;
	
	Transform scTransform;
	
	//must hide from inspector
	//public Vector3 target;
	public float threshold = 1f;
	public float t = 0f;
	public bool isActive = false;
	
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	public delegate void AnimationCompleteDelegate(ScaleAnimationFX scAnim, string whatEffect);
	
	void  OnCompleteAnimation(string whatAnimation){
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this, whatAnimation);
	}
	
	// Use this for initialization
	void Start () {
		scTransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive)
			ApplyScaleAnim(Time.deltaTime);
	}
	
	public void IntializeScaleLerp(Vector3 initialVec, Vector3 targetVec){
		playAnimation = true;
		initialScale = initialVec;
		destinationScale = targetVec;
	}
	
	public void IntializeScaleLerp(Vector3 initialVec, Vector3 targetVec, float duration){
		playAnimation = true;
		initialScale = initialVec;
		destinationScale = targetVec;
		duration = duration;
	}
	
	public void PlayAnimation(){
		isActive = true;	
	}
	
	public void Reset(){
		isActive = false;
		t = 0f;
		playAnimation = false;	
	}
	
	void ApplyScaleAnim(float delta){
		t += delta/duration;
		t = Mathf.Clamp(t, 0, 1);
		
		scTransform.localScale = Vector3.Lerp(initialScale, destinationScale, t);
		
		
		//if(threshold >= (destinationScale - scTransform.localScale).magnitude){
		if(scTransform.localScale == destinationScale){
			isActive = false;
			playAnimation = false;
			t = 0f;
			OnCompleteAnimation(AnimationPrefix.SCALE_LERP);
		}
	}
}
