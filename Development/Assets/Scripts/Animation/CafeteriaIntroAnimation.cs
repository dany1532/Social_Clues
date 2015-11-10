using UnityEngine;
using System.Collections;

public class CafeteriaIntroAnimation : MonoBehaviour {
	public ScaleAnimationFX scaleFX;
	public SwingingObjects doorsAnim;
	
	Vector3 finalScale = new Vector3(10.13f, 10.13f, 10.13f);
	Vector3 playerFinalPos;
	//Vector3 playerBehindDoorPos = new Vector3(22.19f, -0.45f, -78.57f);
	Vector3 initialScale;
	public Vector3 playerStartingOffset = new Vector3(10,0,0);
	public bool playerWalks = true;

	void Awake(){
		initialScale = this.transform.localScale;	
	}
	
	public void PrepareIntroAnimation(){
		Sherlock.Instance.HideDialogue();
		scaleFX.IntializeScaleLerp(initialScale, finalScale);
		scaleFX.animationCompleteDelegate = AnimationCompleted;

		if (playerWalks) {
			playerFinalPos = Player.instance.transform.position;

			Vector3 playerBehindDoorLoc = Player.instance.transform.position;
			playerBehindDoorLoc += playerStartingOffset;
			Player.instance.transform.position = playerBehindDoorLoc;

			Player.instance.PlayIntroAnimation (playerFinalPos);
			doorsAnim.PlayDoorOpeningIntro ();
		}
		Invoke("PlayIntro", 2f);
	}
	
	public void PlayIntro (){
		
		scaleFX.PlayAnimation();
		
	}
	
	private void AnimationCompleted(ScaleAnimationFX anim, string what){
		transform.localScale = initialScale;
		gameObject.SetActive(false);
	}
}


