using UnityEngine;
using System.Collections;

public class FadeInAnimationFX : MonoBehaviour {
	public bool playAnimation = false;
	public bool isActive = false;
	public float duration = 2.5f;
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	private Transform scTransform;
	private UISprite scSprite;
	
	//must hide from inspector
	//public Vector3 target;
	public float t = 0f;
	
	public delegate void AnimationCompleteDelegate(FadeInAnimationFX scAnim, string whatEffect);

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
			ApplyFadeInAnim(Time.deltaTime);
	}
	
	public void PlayAnimation(){
		if(playAnimation){
			scSprite.alpha = 0;
			isActive = true;	
		}
	}
	
	void ApplyFadeInAnim(float delta){
		float alpha = scSprite.alpha;
		t += delta/duration;
		t = Mathf.Clamp(t, 0, 1);
		
		//scSprite.alpha = NGUIMath.SpringLerp(alpha, 1, fadein_Anim.strength, delta);
		//scSprite.alpha = Mathf.Lerp(alpha, 1, t);
		scSprite.alpha = Mathf.SmoothStep(alpha,1,t);
		
		if(scSprite.alpha == 1){
			scSprite.alpha = 1;
			isActive = false;
			t = 0;
		}
	}
}
