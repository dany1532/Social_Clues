using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PositionAnimationFX))]
[RequireComponent(typeof(ScaleAnimationFX))]
public class CutsceneAnimationFX : MonoBehaviour {
	PositionAnimationFX posFX;
	ScaleAnimationFX scaleFX;
	ShakeAnimationFX shakeFX;
	SceneClip clip;
	
	public Vector3 centerCutscenePos;
	public Vector3 finalCutscenePos;
	public float initialDuration = 0.5f;
	public float finalDuration = 1.5f;
	public float sceneStillDuration = 2f;
	
	Vector3 initialCutscenePos;
	Vector3 initialScale = new Vector3(86f,86f,1f);
	public Vector3 finalScale = new Vector3(45f,45f,1f);
	
	public bool willShake = false;
	
	// Use this for initialization
	void Awake () {
		posFX = GetComponent<PositionAnimationFX>();
		scaleFX = GetComponent<ScaleAnimationFX>();
		if(willShake) shakeFX = GetComponent<ShakeAnimationFX>();
		clip = GetComponent<SceneClip>();
		initialCutscenePos = transform.localPosition;
		initialScale = transform.localScale;
	}
	
	public void ResetAnimation()
	{
		transform.localPosition = initialCutscenePos;
		transform.localScale = initialScale;
	}
	
	/// <summary>
	/// Play initial cut scene animation with clip going to the center of the screen
	/// </summary>
	public void PlayAnimation(){
		// Set initial position and scale
		//transform.localPosition = initialCutscenePos;
		//transform.localScale = initialScale;
		
		// Reset the animation FXs
		posFX.Reset();
		scaleFX.Reset();
		
		// Start move position FX
		posFX.animationCompleteDelegate = AnimationStill;
		posFX.InitializePositionLerp(initialCutscenePos, centerCutscenePos, initialDuration, false);
		posFX.PlayAnimation();
	}
	
	/// <summary>
	/// Start animation still (when in center position) related FXs
	/// </summary>
	private void AnimationStill(PositionAnimationFX anim, string what)
	{
		if(posFX == anim)
		{
			if(willShake) shakeFX.PlayAnimation();
			clip.PlaySFX();
			clip.PlayCurrentClip();
			Invoke ("FinalAnimation", sceneStillDuration);
		}
	}
	
	/// <summary>
	/// Play final animation for cut scene, with image returning down to its final position
	/// </summary>
	private void FinalAnimation(){
		if (!clip.HasAudioFinished())
		{
			Invoke ("FinalAnimation", 0.1f);
			return;
		}
		
		// Set final postion
		posFX.animationCompleteDelegate = null;
		posFX.Reset();
		posFX.InitializePositionLerp(centerCutscenePos, finalCutscenePos, false);
		posFX.duration = scaleFX.duration = initialDuration;
		
		// Set final scale
		scaleFX.IntializeScaleLerp(initialScale, finalScale);
		scaleFX.animationCompleteDelegate = CloseClip;
		
		// Play final animation
		posFX.PlayAnimation();
		scaleFX.PlayAnimation();
	}
	
	/// <summary>
	/// Signal the end of the animation to the clip
	/// </summary>
	private void CloseClip(ScaleAnimationFX anim, string what)
	{
		clip.AnimationEnd();
	}

	public void Cancel ()
	{
		CancelInvoke ();
		ResetAnimation();
		if (posFX != null) posFX.Reset();
		if (scaleFX != null) scaleFX.Reset();
	}
}
