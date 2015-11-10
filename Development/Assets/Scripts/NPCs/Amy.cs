using UnityEngine;
using System.Collections;

/// <summary>
/// Cry Girl NPC (aka Amy)
/// </summary>
public class Amy : MonoBehaviour {
	
	#region Tear dropets animation
	// Animated tears of NPC
	/*
	public OTSprite tearLeft;
	public OTSprite tearRight;
	private OTSprite currentTear;
	private OTSprite otherTear;
	*/
	// Drop speed of tears
	public float dropSpeed = 0.005f;
	// Drop frequency of tears
	public float dropFrequency = 0.8f;
	// Time tears take to fade out
	public float fadeOutTime = 0.4f;
	// If the NPC is crying
	public bool cry = false;
	// The parent game object of all the tears
	public GameObject cryingSprites;
	
	// How long has the current tear has been displayed
	private float viewTime = .0f;
	
	// Initial z position of tear drops
	private float initialY;
	#endregion
	
	// NPC controller associated with the CryGirl
	private NPC npc;
	
	// The crying audio clip
	public AudioClip crying;
	// The crying audio source, after she starts crying
	private AudioSource cryingSource;
	
	void Start()
	{
		// Get the associated NPC controller
		npc = this.gameObject.GetComponent<NPC>();
		/*
		if (tearRight != null && tearLeft != null && cryingSprites != null)
		{
			// Get the initial Y position of the tear drops
			initialY = tearRight.transform.localPosition.y;
			
			// Set the alpha value of the right tear drop to 0
			tearRight.alpha = 0;
			// Set the current tear drops
			currentTear = tearLeft;
			otherTear = tearRight;
			
			// If the girl is not crying the deactivate tears
			if (cry)
				InvokeRepeating("Cry", 0, 0.1f);
			else
				cryingSprites.SetActive(false);
		}
		*/
	}
	
	/// <summary>
	/// Show the tear drops
	/// </summary>
	void Cry () {
		// If the tear drop view time is more that the fade out time
		if (viewTime > fadeOutTime)
		{
			/*
			// Fade out current tear
			currentTear.alpha = Mathf.Lerp(currentTear.alpha, 0, 6 * fadeOutTime * Time.deltaTime);
			
			// If view time is more than the drop frequency
			if (viewTime > dropFrequency)
			{
				// Move second tear drop to initial z position
				otherTear.transform.localPosition = new Vector3(otherTear.transform.localPosition.x, initialY, otherTear.transform.localPosition.z);
				
				// Switch tear drops
				otherTear = currentTear;
				if (currentTear == tearLeft)
					currentTear = tearRight;
				else
					currentTear = tearLeft;
				
				// Change alpha of current tear
				currentTear.alpha = 1;
			}
			*/
		}
		/*
		// Drop the tear drop
		currentTear.transform.localPosition += Vector3.down * dropSpeed * Time.deltaTime;
		// Fade the second tear drop down to zero
		otherTear.alpha = Mathf.Lerp(otherTear.alpha, 0, fadeOutTime);
		*/
		// Update the view time of the tear drop
		if (viewTime < dropFrequency)
			viewTime += 0.1f;
		else{
			viewTime = 0.0f;
		}
	}
	
	/// <summary>
	/// Start the audio of the girl crying
	/// </summary>
	public void StartCrying()
	{
		cryingSource = AudioManager.Instance.Play(crying, this.transform, 1, true).source;
	}
	
	/// <summary>
	/// Stop the audio of the girl crying
	/// </summary>
	public void StopCrying()
	{
		// If there is a crying audio
		if (cryingSource != null)
			AudioManager.Instance.stopSound(cryingSource);
		
		// Stop crying
		cry = false;
		
		// Cancel the crying tear drops
		CancelInvoke("Cry");
	}
	
	// Handle all the game related events
	#region Events	
	/// <summary>
	/// Handle Event: Emotion
	/// </summary>
	public void EmotionEvent()
	{
		// Start crying
		StartCrying();
	}
	
	/// <summary>
	/// Handle Event: PlayCutScene
	/// </summary>
	public void PlayCutScene()
	{
		// Stop crying
		StopCrying();
	}
	
	/// <summary>
	/// HandleEvent: ConversationStartEvent
	/// </summary>
	public void ConversationStartEvent()
	{
		// Stop Crying
		StopCrying();
	}
	#endregion
}
