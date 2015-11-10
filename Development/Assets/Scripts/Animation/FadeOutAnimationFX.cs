using UnityEngine;
using System.Collections;

public class FadeOutAnimationFX : MonoBehaviour {
	public bool playAnimation = false;
	public bool isActive = false;
	public float duration = 2.5f;
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	private Transform scTransform;
	private UISprite scSprite;
	
	//must hide from inspector
	public float t = 0f;
	
	public delegate void AnimationCompleteDelegate(FadeOutAnimationFX scAnim, string whatEffect);
	
	void  OnCompleteAnimation(string whatAnimation){
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this, whatAnimation);
	}
	// Use this for initialization
	void Start () {
		scTransform = this.transform;
		scSprite = GetComponent<UISprite>();
		PlayAnimation();
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive)
			ApplyFadeOutAnim(Time.deltaTime);
	}
	
	public void PlayAnimation(){
		if(playAnimation){
			isActive = true;	
		}	
	}
	
	public void Reset(){
		scSprite.alpha = 1f;
		playAnimation = false;
	}
	
	void ApplyFadeOutAnim(float delta){
		float alpha = scSprite.alpha;
		t += delta/duration;
		t = Mathf.Clamp(t, 0, 1);
		
		//scSprite.alpha = NGUIMath.SpringLerp(alpha, 0, fadeout_Anim.strength, delta);
		//scSprite.alpha = Mathf.Lerp(alpha, 0, t);
		scSprite.alpha = Mathf.SmoothStep(alpha, 0f, t);
		
		if(scSprite.alpha == 0f){
			scSprite.alpha = 0f;
			isActive = false;
		}
		
	}
}
