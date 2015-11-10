using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class LevelAnimation : MonoBehaviour {

	bool animationPlaying = false;
	int animationIndex = 0;
	public float speed = 1; 

	// Update is called once per frame
	void OnPress(bool press)
	{
		if (press == true && animationPlaying == false) {
			InputManager.Instance.ReceivedUIInput();

			AnimationState animState = GetAnimationClip();
			if (animState != null)
			{
				animState.speed = speed;
				animation.Play(animState.name);
				animationPlaying = true;
				Invoke ("AnimationEnd", animState.length / speed);
			}

			animationIndex++;
			if (animationIndex == animation.GetClipCount())
				animationIndex = 0;
		}
	}
	AnimationState GetAnimationClip()
	 {
		int index = 0;
		foreach (AnimationState anim in animation) {
			if (index == animationIndex)
				return anim;
			index++;	
		}
		return null;
	}

	void AnimationEnd()
	{
		animationPlaying = false;
	}
}
