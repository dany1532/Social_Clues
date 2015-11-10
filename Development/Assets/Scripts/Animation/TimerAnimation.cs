using UnityEngine;
using System.Collections;

public class TimerAnimation : MonoBehaviour {
	
	public UISprite timerSprite;
	public UISprite clockSprite;
	public AnimationCompleteDelegate animationCompleteDelegate;
	public float duration;
	public float newDelta = 0f;
	public bool canPlay = false;
	
	public delegate void AnimationCompleteDelegate(TimerAnimation anim);
		

	// Update is called once per frame
	void Update () {
		if(canPlay)
			ApplyAnimation(Time.deltaTime);
	}
	
	//Dissapears timer counterclockwise
	void ApplyAnimation(float delta){
		newDelta +=	delta/duration;
		//newDelta = Mathf.Clamp(newDelta, 0f, 1f);
		
		
		//timerSprite.fillAmount -= delta;
		timerSprite.fillAmount = Mathf.Lerp(1, 0, newDelta);
		//timerSprite.fillAmount = Mathf.SmoothStep(timerSprite.fillAmount,0f,newDelta);
		if(timerSprite.fillAmount <= 0f){
			Debug.Log("Ending timer animation");
			newDelta = 0f;
			canPlay = false;
			clockSprite.enabled = false;
			OnCompleteAnimation();
		}
	}
	
	//When the animation is complete, mark it as complete
	void  OnCompleteAnimation(){
		
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this);
	}
	
	//Displays the timer and the clock sprites
	public void DisplayTimer(){
		timerSprite.fillAmount = 1f;
		clockSprite.enabled = true;
	}
	
	//Regular timer with (no animation)
	public void StartTimer_NoDisplay(){
		Debug.Log("Starting no display timer");
		Invoke("StopTimer_NoDisplay", duration);	
	}
	
	//Stops the regular timer (no animation)
	public void StopTimer_NoDisplay(){
		Debug.Log("Ending no display timer");
		newDelta = 0f;
		canPlay = false;
		clockSprite.enabled = false;
		OnCompleteAnimation();
	}
	
	//Set the duration of the timer
	public void SetDuration(float time){
		float timeDuration = time;
		duration = timeDuration;
	}
	
	//Activate animation
	public void PlayTimerAnimation(){
		Debug.Log("starting display timer");
		canPlay = true;
	}
}
