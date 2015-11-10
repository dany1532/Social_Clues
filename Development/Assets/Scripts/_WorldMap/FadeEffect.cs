using UnityEngine;
using System.Collections;

public class FadeEffect : MonoBehaviour 
{
	public enum FadeType{ FADEIN, FADEOUT};
	
	public FadeType fadeType = FadeType.FADEIN;
	public bool isActive = false;
	public float duration = 2.5f;
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	//private Transform scTransform;
	private UITexture scSprite;
	
	//must hide from inspector
	public float t = 0f;
	
	public delegate void AnimationCompleteDelegate(FadeEffect scAnim, string whatEffect);
	
	void  OnCompleteAnimation(string whatAnimation)
	{
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this, whatAnimation);
	}
	// Use this for initialization
	void Awake () 
	{
		//scTransform = this.transform;
		scSprite = GetComponent<UITexture>();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isActive && fadeType == FadeType.FADEOUT)
			ApplyFadeOutAnim(Time.deltaTime);
		
		else if(isActive && fadeType == FadeType.FADEIN)
			ApplyFadeInAnim(Time.deltaTime);
	}
	
	public void PlayFadeOut(){
		fadeType = FadeType.FADEOUT;
		Reset();
		isActive = true;		
	}
	
	public void PlayFadeIn()
	{
		fadeType = FadeType.FADEIN;
		Reset();
		isActive = true;	
	}
	
	public void Reset()
	{
		if(fadeType == FadeType.FADEOUT)
			scSprite.alpha = 1f;
		else
			scSprite.alpha = 0;
		
	}
	
	void ApplyFadeOutAnim(float delta)
	{
		float alpha = scSprite.alpha;
		t += delta/duration;
		t = Mathf.Clamp(t, 0, 1);
		
		scSprite.alpha = Mathf.SmoothStep(1f, 0f, t);
		
		if(scSprite.alpha == 0f){
			scSprite.alpha = 0f;
			isActive = false;
			t = 0;
			OnCompleteAnimation("FadeOut");
		}
		
	}
	
	void ApplyFadeInAnim(float delta)
	{
		float alpha = scSprite.alpha;
		t += delta/duration;
		t = Mathf.Clamp(t, 0, 1);
		
		scSprite.alpha = Mathf.SmoothStep(0f,1f,t);
		
		if(scSprite.alpha == 1){
			scSprite.alpha = 1;
			isActive = false;
			t = 0;
			OnCompleteAnimation("FadeIn");
		}
	}

}
