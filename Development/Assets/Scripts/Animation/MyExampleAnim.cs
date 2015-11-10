using UnityEngine;
using System.Collections;

public class MyExampleAnim : MonoBehaviour {
	SCAnimationEffects myAnim;
	
	
	// Use this for initialization
	void Start () {
		
		myAnim = GetComponent<SCAnimationEffects>();
		//myAnim.IntializeScaleLerp(new Vector3(transform.localScale.x + 100, transform.localScale.y + 100, transform.localScale.z));
		myAnim.animationCompleteDelegate = ScaleCompleteDelegate;
		myAnim.PlayAnimation(); 
		
	}
	
	void ScaleCompleteDelegate(SCAnimationEffects theScript, string typeAnimation){
		if(myAnim == theScript){
				if(typeAnimation == AnimationPrefix.SCALE_LERP){
				myAnim.animationCompleteDelegate = null;
				Vector3 loc = transform.localPosition;
				myAnim.InitializePositionLerp(new Vector3(loc.x - 100, loc.y - 100, 0), false);
				myAnim.IntializeScaleLerp(new Vector3(transform.localScale.x - 40, transform.localScale.y - 40, transform.localScale.z));
				myAnim.PlayAnimation();
			}
		}
	}
	

}
