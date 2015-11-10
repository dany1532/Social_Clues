using UnityEngine;
using System.Collections;

public class ChangeColorAnimationFX : MonoBehaviour {
	public bool playAnimation = false;
	public bool isActive = false;
	public float duration = 2.5f;
	public Color to;
	public AnimationCompleteDelegate animationCompleteDelegate;
	
	private UISprite scSprite;
	
	//must hide from inspector
	//public Vector3 target;
	//public float threshold = .9f;
	public float t = 0f;
	
	
	public delegate void AnimationCompleteDelegate(ChangeColorAnimationFX scAnim, string whatEffect);
	
	void  OnCompleteAnimation(string whatAnimation){
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this, whatAnimation);
	}
	// Use this for initialization
	void Start () {
		scSprite = GetComponent<UISprite>();
		PlayAnimation();
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive)
			ApplyColorChangeAnim(Time.deltaTime);
	}
	
	public void PlayAnimation(){
		if(playAnimation)
			isActive = true;
	}
	
	void ApplyColorChangeAnim(float delta){
		
		Color color = scSprite.color;
		t += delta/duration;
		t = Mathf.Clamp(t, 0, 1);
		
		//scSprite.color = NGUIMath.SpringLerp(scSprite.color, colorChange_Anim.to, 
													//colorChange_Anim.strength, delta);
		
		scSprite.color = Color.Lerp(color, to, t);
	
		
		//scSprite.color = Color.Lerp(scSprite.color, colorChange_Anim.to, 
		
		//Vector3 vecTo = new Vector3(colorChange_Anim.to.r, colorChange_Anim.to.g, colorChange_Anim.to.b);
		//pos_Anim.threshold >= (pos_Anim.target - scTransform.localPosition).magnitude
		if(color == to){
			isActive = false;
			t = 0f;
		}
		
	}
}
