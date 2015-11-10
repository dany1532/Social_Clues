using UnityEngine;
using System.Collections;

public class AnotherExample : MonoBehaviour {
	SCAnimationEffects myAnim;
	
	// Use this for initialization
	void Start () {
	myAnim = GetComponent<SCAnimationEffects>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.A)){
			myAnim.Reset();
			myAnim.pos_Anim.playAnimation = true;
			myAnim.PlayAnimation();
		}
		
		if(Input.GetKeyDown(KeyCode.S)){
			myAnim.Reset();
			myAnim.spin_Anim.playAnimation = true;
			myAnim.PlayAnimation();
		}
		
		if(Input.GetKeyDown(KeyCode.D)){
			myAnim.Reset();
			myAnim.scale_Anim.playAnimation = true;
			myAnim.PlayAnimation();
		}
		
		if(Input.GetKeyDown(KeyCode.F)){
			myAnim.Reset();
			myAnim.fadein_Anim.playAnimation = true;
			myAnim.PlayAnimation();
		}
		
		if(Input.GetKeyDown(KeyCode.G)){
			myAnim.Reset();
			myAnim.fadeout_Anim.playAnimation = true;
			myAnim.PlayAnimation();
		}
		
		if(Input.GetKeyDown(KeyCode.H)){
			myAnim.Reset();
			myAnim.colorChange_Anim.playAnimation = true;
			myAnim.PlayAnimation();
		}
		
		if(Input.GetKeyDown(KeyCode.J)){
			myAnim.Reset();
			myAnim.shake_Anim.playAnimation = true;
			myAnim.PlayAnimation();
		}
			/*pos_Anim.playAnimation = false;

		
		spin_Anim.playAnimation = false;

		
		scale_Anim.playAnimation = false;

		
		fadein_Anim.playAnimation = false;

		
		colorChange_Anim.playAnimation = false;

		
		fadeout_Anim.playAnimation = false;

		
		colorChange_Anim.playAnimation = false;

		
		shake_Anim.playAnimation = false;*/
	}
}
