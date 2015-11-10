using UnityEngine;
using System.Collections;

public class ShakeAnimationFX : MonoBehaviour {
	public bool playAnimation = false;
	public bool isActive = false;
	public float shake = 2.0f;
	private float m_shake = 0f;
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	public AnimationCompleteDelegate animationCompleteDelegate;
	private Vector3 initialPos;
	public float delay = 0.0f;
	
	public delegate void AnimationCompleteDelegate(ShakeAnimationFX scAnim, string whatEffect);
	
	void  OnCompleteAnimation(string whatAnimation){
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this, whatAnimation);
	}
	// Use this for initialization
	void Start () {
		//PlayAnimation();
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive)
			ApplyShakeAnim(Time.deltaTime);
	}
	
	public void PlayAnimation(){
		m_shake = shake;
		initialPos = this.transform.localPosition;
		isActive = true;
		//Invoke("PlayDelayedAnimation", delay);
	}
	
	void PlayDelayedAnimation()
	{
		if(playAnimation){
			initialPos = this.transform.localPosition;
			isActive = true;
		}
	}
	
	void ApplyShakeAnim(float delta){
		Vector3 shakeVector = transform.localPosition;
		
		if(m_shake > 0){
			shakeVector += Random.insideUnitSphere * shakeAmount;
			shakeVector.z = transform.localPosition.z;
			transform.localPosition = shakeVector;
			m_shake -= delta * decreaseFactor;
		}
		
		else{
			m_shake = shake;
			isActive = false;
			this.transform.localPosition = initialPos;
		}
	}
}
