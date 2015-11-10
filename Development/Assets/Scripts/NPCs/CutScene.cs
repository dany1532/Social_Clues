using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cut scene which includes all the clips of the scene, and events triggered at the cutscene
/// </summary>
public class CutScene : MonoBehaviour {
		
	// Cut scene game object
	public GameObject cutScene;
	
	// Clips that make the scene
	public List<SceneClip> clips;
	// Current clip
	int currentClip = 0;
	
	// Event triggered when clsing scene
	public SCEvent eventOnClose;
	// Event triggere when we are playing the scene for a second time
	public SCEvent eventOnReplay;
	
	// Time before we close scene after last clip
	public float closeSceneDelay = 2.0f;
	
	// if we have played the cut scene once already
	public bool playedOnce = false;

	bool skipActivated = false;

	SkipCutScene skipCutSceneButton;

	// Use this for initialization
	void Start () {
		skipCutSceneButton = GameObject.FindObjectOfType(typeof(SkipCutScene)) as SkipCutScene;
		skipCutSceneButton.SetActive(false);

		// Go through all te clips in the scene, deactivate them, and make them a child of the cut scene
		foreach(SceneClip scene in clips)
		{
			scene.gameObject.SetActive(false);
			scene.cutScene = this;
			
		}
		// Deactivate whole cutscene
		this.gameObject.SetActive(false);
	}
	
	/// <summary>
	/// Start playin the cut scene
	/// </summary>
	public void PlayCutScene()
	{
		GameObject dialogueWindow = GameObject.FindGameObjectWithTag("DialogueWindow");
		if(dialogueWindow != null)
			dialogueWindow.SetActive(false);
	    AudioManager.Instance.LowerMusicSound ();
		skipActivated = false;
		if (playedOnce) skipCutSceneButton.SetActive(true);
		// If there are more clips to be played
		if (currentClip < clips.Count)
		{
			// Hide Sherlocks speech bubble
			Sherlock.Instance.HideDialogue();
			
			// Go to next clip
			GoToNextClip();
			
			// Deactivate the HUD
			HUD.Instance.gameObject.SetActive(false);
		}
	}
	
		/// <summary>
	/// Start playin the cut scene
	/// </summary>
	public void PlayIntroCutScene()
	{
		gameObject.SetActive(true);
		
		skipActivated = false;
		if (playedOnce) skipCutSceneButton.SetActive(true);
		// If there are more clips to be played
		if (currentClip < clips.Count)
		{
			// Go to next clip
			GoToNextIntroClip();
			
			// Deactivate the HUD
			HUD.Instance.gameObject.SetActive(false);
		}
	}
	
	/// <summary>
	/// Start playing next scene
	/// </summary>
	public void GoToNextIntroClip()
	{
		if (skipActivated) return;

		// If there are more clips
		if (currentClip < clips.Count)
		{
			// Activate clip
			clips[currentClip].isIntro = true;
			
			clips[currentClip].ActivateClip();
			// and go to next one
			currentClip++;
		}
		else // otherwise close cut scene after a delay
		{
			Invoke("CloseScene", closeSceneDelay);
		}
	}
	
	
	/// <summary>
	/// Start playing next scene
	/// </summary>
	public void GoToNextClip()
	{
		if (skipActivated) return;

		// If there are more clips
		if (currentClip < clips.Count)
		{
			// Activate clip
			clips[currentClip].ActivateClip();
			// and go to next one
			currentClip++;
		}
		else // otherwise close cut scene after a delay
		{
			Invoke("CloseScene", closeSceneDelay);
		}
	}

	/// <summary>
	/// Close the cut scene
	/// </summary>
	public void CloseScene()
	{	
		// Go through all clips in the cut scene
		foreach(SceneClip scene in clips)
		{
			scene.ResetClip();
		}
		
		// If this is the first time we play the cut scene
		if (playedOnce == false)
		{
			// Remember that we have already played the scene once
			playedOnce = true;
			
			// If there is an event we need to trigger when closing scene
			if (eventOnClose != null)
				// Trigger event
				eventOnClose.TriggerEvent(true);
		}
		else // otherwise if we already played the scene once and this is a replay
		{
			/// If there is an event we need to trigger after the replay of the scene (i.e. playing scene during dialogue)
			if (eventOnReplay != null)
				// Trigger event
				eventOnReplay.TriggerEvent(true);
		}
		skipActivated = false;
	}
	
	/// <summary>
	/// Hides all the UI elements of the cut scene
	/// </summary>
	public void Hide()
	{
		// Set current clip back to 0, for next play
		currentClip = 0;
		
		// Go through all clips in the cut scene
		foreach(SceneClip scene in clips)
		{
			scene.ResetClip();
			// and deactivate them
			scene.gameObject.SetActive(false);
		}
		
		// Deactivate complete cut scene
		this.gameObject.SetActive(false);
	}

	public void Skip ()
	{
		CancelInvoke();
		skipActivated = true;
		AudioManager.Instance.StopVoiceOver();
		//AudioManager.Instance.StopSoundFXs();

		// Go through all clips in the cut scene
		foreach(SceneClip scene in clips)
		{
			scene.gameObject.SetActive(true);
			scene.ForceEndClip();
		}
		currentClip = clips.Count;
		CancelInvoke();
		Invoke("CloseScene", closeSceneDelay);
	}
}
