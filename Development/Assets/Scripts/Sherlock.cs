using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manager class for Sherlock
/// </summary>
public class Sherlock : Singleton<Sherlock> {
	
    public NPCAnimations defaultAnimations;
    public NPCAnimations activeAnimations;
    public NPCDialogueAnimation animationController;
    
    public UITexture foregroundTexture;
	public UITexture backgroundTexture;
	public UITexture portrait;
    
    // Animated sprite
    //public OTAnimatingSprite animationSprite;
    
    // Text Dialog sprite
    public UILabel sherlokTextSpriteRight;
    public GameObject sherlokDialogueBoxRight;
    public UILabel sherlokTextSpriteDown;
    public GameObject sherlokDialogueBoxDown;
    UILabel sherlokTextSprite;
    GameObject sherlokDialogueBox;
    
    public enum side{RIGHT, DOWN}
    
    // Current node of the dialogue
    public Dialogue currentNode;
    
    public delegate void CallBackDelegate();
    CallBackDelegate callBackDelegate;
    
	// Use this for initialization
	void Awake () {
		instance = this;
        foregroundTexture.gameObject.SetActive(false);

        sherlokTextSpriteDown.enabled = false;
        sherlokDialogueBoxDown.SetActive(false);
        sherlokTextSpriteRight.enabled = false;
		sherlokDialogueBoxRight.SetActive(false);
		
		//automatically set speech bubble position to RIGHT
		SetBubblePosition(side.RIGHT);
	}
    
    void Start()
    {        
        // Make Sherlock idle and hide the dialogue window
        PlayAnimation(NPCAnimations.AnimationIndex.NEUTRAL);
        HideDialogue();
    }

	
	/// <summary>
	/// Sets the position of Sherlock's Speech Bubble to the right or bottom side of the screen.
	/// </summary>
	/// <param name='pos'>
	/// New Position of speech bubble
	/// </param>/
	public void SetBubblePosition(side pos){
		if (pos.Equals(side.DOWN))
		{
			sherlokTextSpriteDown.enabled=true;
			sherlokDialogueBoxDown.SetActive(true);
			sherlokTextSprite = sherlokTextSpriteDown;
			sherlokDialogueBox = sherlokDialogueBoxDown;
			sherlokTextSpriteRight.enabled=false;
			sherlokDialogueBoxRight.SetActive(false);
		}
		else if (pos.Equals(side.RIGHT))
		{
			sherlokTextSpriteRight.enabled=true;
			sherlokDialogueBoxRight.SetActive(true);
			sherlokTextSprite = sherlokTextSpriteRight;
			sherlokDialogueBox = sherlokDialogueBoxRight;
			sherlokTextSpriteDown.enabled=false;
			sherlokDialogueBoxDown.SetActive(false);
		}
	}

	
	/***
	 * Handle different animations
	***/
	#region Animations
	/// <summary>
	/// Play idle animation
	/// </summary>
	NPCAnimations.AnimationSequence IdleAnimation(){
        return defaultAnimations.animations[0];
    }
	
	/// <summary>
	/// Play talking animation
	/// </summary>
	NPCAnimations.AnimationSequence TalkingAnimation(){
        return defaultAnimations.animations[1];
    }

    public void PlayAnimation (NPCAnimations.AnimationIndex animationIndex)
    {
        NPCAnimations.AnimationSequence animationSequence = null;
        
        if (animationIndex == NPCAnimations.AnimationIndex.TALKING)
            animationSequence = TalkingAnimation();
        else if (animationIndex == NPCAnimations.AnimationIndex.NEUTRAL)
            animationSequence = IdleAnimation();
        else
            animationSequence = activeAnimations.RetrieveAnimationSequence(animationIndex);
        
        List<Texture> animationTextures = animationSequence.textures;
        if (animationTextures.Count > 0)
        {
            animationController.StopAnimation();
            animationController.SetAnimationList(animationTextures);
            animationController.PlayAnimation();
            animationController.SetSpeed(animationSequence.speed);
        }
        else
        {
            Debug.LogWarning("Error retrieving animation " + animationIndex.ToString());
        }
    }
	#endregion
	
	/***
	 * Handle dialogue events
	***/
	#region Dialogue
	/// <summary>
	/// Check if Sherlock dialogue sprite has been initialized and thus can be used
	/// </summary>
	/// <returns>
	/// bool: if sherlock dialogue has been initialized
	/// </returns>
	public bool isValid ()
	{
		return sherlokTextSprite != null;
	}
	
	/// <summary>
	/// Sets the text for Sherlock's dialogue box
	/// </summary>
	/// <param name='text'>
	/// Text to be displayed
	/// </param>/
	public void SetText(string text)
	{
		if (text != string.Empty)
		{
			sherlokTextSprite.gameObject.SetActive(true);
			sherlokDialogueBox.gameObject.SetActive(true);
			sherlokTextSprite.text = text;
		}
		else
		{
			sherlokTextSprite.gameObject.SetActive(false);
			sherlokDialogueBox.gameObject.SetActive(false);
		}
	}
	
	/// <summary>
	/// Update Sherlocks dialogue box
	/// </summary>
	/// <param name='dialogue'>
	/// Dialogue: The dialogue to be used to display text
	/// </param>
	public IEnumerator SetDialogue (Dialogue dialogue, NPCAnimations.AnimationIndex animationIndex = NPCAnimations.AnimationIndex.TALKING)
	{
		if (isValid() && dialogue != null)
		{
			CancelInvoke("HideDialogue");
			if (dialogue.showWindow)
			{
				SetText(dialogue.text);
			}
                
            if (dialogue.sherlockAnimation != NPCAnimations.AnimationIndex.NEUTRAL)
                PlayAnimation(dialogue.sherlockAnimation);
            else
                PlayAnimation(animationIndex);
			
			if (dialogue.voiceOver != null && dialogue.firstRepetition)
			{
				AudioManager.Instance.PlayVoiceOver(dialogue.voiceOver, 1);
				yield return new WaitForSeconds(dialogue.voiceOver.length);
			}
			else
				yield return new WaitForSeconds(1);
		}
		else
		{
			if (sherlokTextSprite != null)
				SetText(string.Empty);
		}
        PlayAnimation(NPCAnimations.AnimationIndex.NEUTRAL);
	}
	
	/// <summary>
	/// Hide sherlocks dialogue box
	/// </summary>
	public void HideDialogue ()
	{
		// if text sprite is set
		if (sherlokTextSprite != null)
			// Disable parent game object, which holds all Sherlock dialogue components
			SetText(string.Empty);
        PlayAnimation(NPCAnimations.AnimationIndex.NEUTRAL);
	}
	#endregion
	
	#region Give Sequence of instructions
	
	
	/// <summary>
	/// Play a sequence of consecutive instructions from Sherlock starting from the root
	/// </summary>
	public void PlaySequenceInstructions ()
	{
		PlaySequenceInstructions(currentNode, null);
	}
		
	/// <summary>
	/// Play a sequence of consecutive instructions from Sherlock starting from the given dialogue
	/// </summary>
	/// <param name='instruction'>
	/// Dialogue to play the sequence of instructions from
	/// </param>
	public void PlaySequenceInstructions(Dialogue instruction, CallBackDelegate callBack)
	{
		CancelInvoke();
		
		currentNode = instruction;
		callBackDelegate = callBack;
		
		if (currentNode != null)
		{
			StartCoroutine(SetDialogue(currentNode));
			
			if (currentNode.voiceOver != null)
				Invoke("PlayNextInstructionInSequence", currentNode.voiceOver.length + currentNode.text.Length * 0.1f * 0.2f);
			else
				Invoke("PlayNextInstructionInSequence", 1f);
		}
		else
		{
			Invoke("HideDialogue", 1f);
			if (callBackDelegate != null)
				callBackDelegate();
		}
	}
	
	/// <summary>
	/// Plays the next instruction in the sequence by Sherlock
	/// </summary>
	private void PlayNextInstructionInSequence()
	{
		PlaySequenceInstructions(currentNode.nextDialogue, callBackDelegate);
	}
	#endregion
}
