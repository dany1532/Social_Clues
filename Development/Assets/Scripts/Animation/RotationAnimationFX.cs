using UnityEngine;
using System.Collections;

public class RotationAnimationFX : MonoBehaviour {
	public bool playAnimation = false;
	public bool isActive = false;
	
	public float duration = 1f;
	public float spins = 1f;
    float rotationsPerSecond = 90f;
	
	bool endSpin = false;

	public AnimationCompleteDelegate animationCompleteDelegate;
	
	private Transform scTransform;
	
	public float rotationAngle = 360;
	private Vector3 initialRotation;
	
	public enum AxisRotation{ XAXIS,
							  YAXIS,
							  ZAXIS }
	public AxisRotation myAxis = AxisRotation.ZAXIS;
	
	public delegate void AnimationCompleteDelegate(RotationAnimationFX scAnim, string whatEffect);
	
	void  OnCompleteAnimation(string whatAnimation){
		if(animationCompleteDelegate != null)
			animationCompleteDelegate(this, whatAnimation);
	}

	
	void Start(){
		initialRotation = transform.localEulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive)
			ApplySpin(Time.deltaTime);
	}
	
	
	private void ApplySpin(float delta){
		
		if(endSpin){
			endSpin = false;
			isActive = false;
			OnCompleteAnimation(AnimationPrefix.SPIN_FX);
			
		}
		
		else{
			if(myAxis == AxisRotation.ZAXIS)
				transform.Rotate (0, 0, rotationsPerSecond * delta);	
			
			else if(myAxis == AxisRotation.YAXIS)
				transform.Rotate (0, rotationsPerSecond * delta, 0);
			
			else if(myAxis == AxisRotation.XAXIS)
				transform.Rotate (rotationsPerSecond * delta, 0, 0);
		}
	}
	
	public void EndSpin(){
		endSpin = true;	
	}
	
	public void PlayAnimation(){
		transform.localEulerAngles = initialRotation;
		
		//how much should it rotate each frame to get to final rotation in the expected time
		rotationsPerSecond = rotationAngle/duration; 
		
		//Set to one if only one spin, two if two spins during that duration, etc...
		rotationsPerSecond *= spins;
		
		isActive = true;
		Invoke("EndSpin", duration/*+0.001f*/);

	}
	
	public void Reset(){
		isActive = false;
		endSpin = false;
		playAnimation = false;	
	}
	
	public void Reset(Vector3 _initialRotation, float _rotationAngle, float _spins){
		initialRotation = _initialRotation;
		rotationAngle = _rotationAngle;
		spins = _spins;
		isActive = false;
		endSpin = false;
		playAnimation = false;	
	}
}
