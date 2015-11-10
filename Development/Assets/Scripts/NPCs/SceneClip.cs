using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A scene clip within a cut scene
/// </summary>
public class SceneClip : MonoBehaviour {
	#region Cut Scene information
	// The cut scene the clip belongs in
	[System.NonSerialized]
	public CutScene cutScene;
	
	public float clipDelay = 1.0f;
	#endregion
	
	#region Cut Scene media to be played
	// Audio clips to be played during clip
	public List<AudioClip> clips;
	// Current audio clip being played
	public int currentClip = 0;
	
	// Conversation to be played during scene
	public ConversationTree conversation;
	
	// Sound effet to be played during scene
	public AudioClip soundEffect;
	
	// If animation FX has finished
	bool animationHasFinsihed;
	// If audio clips have finished
	bool audioClipsHaveFinsihed;
	#endregion
	
	// Event triggered at the beginning or at the end of the scene
	public SCEvent sceneEvent;
	// Trigger event either begin or end of the scene
	public bool endOfClipEvent = false;
	
	public bool isIntro = false;
	
	public CutsceneAnimationFX animationEffect;

	void Awake()
	{
		// Plat cut scene animation
		animationEffect = this.GetComponent<CutsceneAnimationFX>();
	}

	/// <summary>
	/// Activates the scene
	/// </summary>
	public void ActivateClip()
	{
		// Activate clips' game object
		gameObject.SetActive(true);
	
		if (animationEffect != null)
		{
			animationHasFinsihed = false;
			//Activate clip animation if it has one
			animationEffect.PlayAnimation();
		}
		else
		{
			animationHasFinsihed = true;
		}
		
		// Start playing audio / dialogue of the clip
		audioClipsHaveFinsihed = false;
		//PlayCurrentClip();
		
		// If there is an event set for the beginning of the clip
		if (sceneEvent != null && !endOfClipEvent)
			// then trigger event
			sceneEvent.TriggerEvent(true);
	}
	
	/// <summary>
	/// Play sound effect associated with script
	/// </summary>
	public void PlaySFX()
	{
		// If sound effect is set
		if (soundEffect != null)
		{
			// play sound effect
			AudioManager.Instance.Play(soundEffect, GameManager.Instance.mainCamera.transform, 1.0f, false);
		}
	}
	
	/// <summary>
	/// Play the current clip
	/// </summary>
	public void PlayCurrentClip ()
	{
		// If there are more audio cips to be played
		if (currentClip < clips.Count)
		{
			// If this is the first clip
			if (currentClip	== 0)
				// Start conversation
				conversation.StartConversation();
			else
				// Otherwise go to the next dialogue
				conversation.ShowNextDialogue();
			
			// Move to next clip
			currentClip++;
			
			// If there is a clip set
			if (clips[currentClip-1] != null)
			{
				// Play clip
				Invoke("PlayCurrentClip", clips[currentClip-1].length);
				return;
			}
		}
		
		// All audio clips have finished end clip
		// Singal the end of the audio clips
		audioClipsHaveFinsihed = true;
		// If there are no audio clips, end current clip
		
		if(!isIntro){
			EndClip();
		}
		
		else
			EndIntroClip();
	}
	
	public void EndIntroClip()
	{
		if (animationHasFinsihed && audioClipsHaveFinsihed)
		{
			// If there is an event set to be played at the end of the clip
			if (sceneEvent != null && endOfClipEvent)
				// then trigger event
				sceneEvent.TriggerEvent(true);
			
			
			// Move to the next clip in the scene with a delay
			cutScene.Invoke("GoToNextIntroClip", clipDelay);
		}
	}
	
	/// <summary>
	/// Ends the clip
	/// </summary>
	public void EndClip()
	{
		if (animationHasFinsihed && audioClipsHaveFinsihed)
		{
			// If there is an event set to be played at the end of the clip
			if (sceneEvent != null && endOfClipEvent)
				// then trigger event
				sceneEvent.TriggerEvent(true);
			
			// Set Sherlocks dialogue to null (i.e. close dialgue window)
			Sherlock.Instance.StartCoroutine(Sherlock.Instance.SetDialogue(null));
			Sherlock.Instance.HideDialogue();
			
			// Move to the next clip in the scene with a delay
			cutScene.Invoke("GoToNextClip", clipDelay);
		}
	}
	
	/// <summary>
	/// Receive signal that audio clips have ended
	/// </summary>
	public bool HasAudioFinished ()
	{
		return audioClipsHaveFinsihed;
	}
	
	/// <summary>
	/// Receive signal that animation has ended
	/// </summary>
	public void AnimationEnd ()
	{
		animationHasFinsihed = true;
		
		if(!isIntro)
			EndClip();
		else
			EndIntroClip();
	}
	
	public void ResetClip()
	{
		animationEffect.ResetAnimation();
		currentClip = 0;
		animationHasFinsihed = false;
		audioClipsHaveFinsihed = false;
	}

	public void ForceEndClip()
	{
		CancelInvoke();
		animationEffect.Cancel();
		transform.localPosition = animationEffect.finalCutscenePos;
		transform.localScale = animationEffect.finalScale;
	}
}
