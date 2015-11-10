using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Dialogue window that shows all dialogues, except Sherlocks, and trigger voice over, etc
/// </summary>
public class DialogueWindow : MonoBehaviour
{
	
    // Static instance of the dialogue window
    public static DialogueWindow instance;
	
	#region Display elements
    /// <summary>
    /// Display element which includes sprite, text, and final answer color
    /// </summary>
    [System.Serializable]
    public class DisplayElement
    {
        public UISprite sprite;
        public UILabel text;
        public Color finalAnswerColor;
    }
	
    // Display elements for emotions, scenarios (image + text) and text only replies
    public List<InteractiveReplyElement> emotions;
    public List<InteractiveReplyElement> imageText;
    public List<InteractiveReplyElement> textOnly;
    public List<InteractiveReplyElement> imageTextBox;
	
    // Text sprite for dialogues with only a single element
    public UILabel singleDialogueElement;
	
    // Sprite with player portrait
    public UITexture playerSprite;
    [System.Serializable]
    public class CharacterTexture
    {
        // Sprite with npc portrait
        public UITexture texture;
        public UIStretch stretch;
        public NPCDialogueAnimation animation;
        public RepeatNPCDialogue repeatDialogue;

        public bool eventTriggered = false;

        public void EnableRepeatButton(bool value)
        {
            if (repeatDialogue != null)
                repeatDialogue.enabled = value;
        }
    }
    public Transform playerParentTransform;
    public CharacterTexture playerTexture;
    public CharacterTexture npcSingle;
    public CharacterTexture npc1;
    public CharacterTexture npc2;
	
    // Display elements background sprite
    public BlendTextures background;
    //Display dialogue bubble background
    public DialogueBubble dialogueBubble;
    public Transform npcArrow;
    //Display timer
    public TimerAnimation timer;
	#endregion
	
	#region Current Dialogue information
    // Current Dialogue
    Dialogue dialogue;
    // Last NPC Dialogue
    Dialogue lastNPCDialogue;
    // Current dialogue display type
    Dialogue.DialogueDisplayType displayType = Dialogue.DialogueDisplayType.TEXT_ONLY;
    // Progress in current dialogue
    public DialogueProgress progressBar;
	
    // Total number of available boxes to be clicked
    int activeElements;
    // Active reply
    [System.NonSerializedAttribute]
    public InteractiveReplyElement
        activeReply;
	#endregion

	#region Actions
    public RepeatDialogue repeatButton;
    public SkipDialogue skipButton;
	#endregion
    //Timer delegate
    private TimerAnimation myTimer;

	public UISprite received2DToy;
	public UISprite reach2DToy;

    public SoundFXLibrary soundFXLibrary;

    void Awake()
    {
        // Save static instance of the object
        instance = this;
        playerParentTransform = playerTexture.texture.transform.parent;
    }
	
    // Use this for initialization
    void Start()
    {
		
        // Initialize all elements in window
        InitializeElements(textOnly);
        InitializeElements(imageText);
        InitializeElements(emotions);
        InitializeElements(imageTextBox);
        singleDialogueElement.transform.parent.gameObject.SetActive(false);
		
        // Deactivate dialogue window in the beginning of the game
        this.gameObject.SetActive(false);
    }

    void EnableRepeatNPCButtons(bool value)
    {
        repeatButton.SetActive(value);
        npcSingle.EnableRepeatButton(value);
        npc1.EnableRepeatButton(value);
        npc2.EnableRepeatButton(value);
    }

    void OnEnable()
    {
        EnableRepeatNPCButtons(false);
    }

    void OnDestroy()
    {
        instance = null;
    }

    /*
	// Update is called once per frame
	void Update () {
		// Input handling from user. If there are available elements
		Debug.Log (dialogue.text);
		if (activeElements == 0)
		{
			// Handle input if there are no active elements on screen
			if (Input.GetMouseButtonDown(0))
			{
				// If there are no replies left
				if (dialogue != null && dialogue.remainingReplies == 0)
					// then reply to current dialouge, by going to the next dialogue
					dialogue.Reply(-1);
			}
		}
	}
	*/
	
	#region Display Element Actions
    /// <summary>
    /// Initializes display elements: Resets back color and hides them
    /// </summary>
    /// <param name='elements'>
    /// Display elements to be initialized
    /// </param>
    public void InitializeElements(List<InteractiveReplyElement> elements)
    {
        // Go through each element
        for (int counter = 0; counter < elements.Count; counter++)
        {
            // Reset background color and deactivate game object
            elements [counter].Hide();
            elements [counter].Reset();
        }		
    }
	
    /// <summary>
    /// Clears display elements by resetting text, background color and Deactivating them
    /// </summary>
    /// <param name='elements'>
    /// Display elements to be cleared
    /// </param>
    void ClearElements(List<InteractiveReplyElement> elements)
    {
        // Go through all elements
        for (int counter = 0; counter < elements.Count; counter++)
        {
            // Clear text, Reset background color and deactivate game object
            elements [counter].Reset();
        }
    }
	
    /// <summary>
    /// Enables the given elements based on remaining replies in current dialoge
    /// </summary>
    /// <param name='currentNode'>
    /// Current dialogue
    /// </param>
    /// <param name='elements'>
    /// Display elements to be enabled
    /// </param>
    void EnableAvailableElements(Dialogue currentNode, List<InteractiveReplyElement> elements)
    {
        int counter;
        // Go through all elements that are available as a reply
        for (counter = 0; counter < Mathf.Max(4, currentNode.remainingReplies); counter++)
        {
            // reset their background
            elements [counter].Reset();
            elements [counter].Show();
        }
		
        // Disable reset of the elements (not enough replies to be displayed)
        for (; counter < elements.Count; counter++)
        {
            elements [counter].Hide();
        }
    }
	
    /// <summary>
    /// Disables display elements.
    /// </summary>
    /// <param name='elements'>
    /// Display elements to be disabled
    /// </param>
    void DisableElements(List<InteractiveReplyElement> elements)
    {
        // Go through all elements to be disabled
        for (int counter = 0; counter < elements.Count; counter++)
        {
            // Reset background color and deactivate game object
            elements [counter].Reset();
            elements [counter].Hide();
        }
    }
	
    public void DisableReplyColliders()
    {
        List<InteractiveReplyElement> elements = null;
        if (displayType == Dialogue.DialogueDisplayType.EMOTION)
            elements = emotions;
        else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT)
            elements = imageText;
        else if (displayType == Dialogue.DialogueDisplayType.TEXT_ONLY)
            elements = textOnly;
        else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT_BOX)
            elements = imageTextBox;
        else
            Debug.Log("No display type");
        // Go through all elements to be disabled
        if (elements != null)
        {
            for (int counter = 0; counter < elements.Count; counter++)
            {
                elements [counter].gameObject.collider.enabled = false;
            }
        }
    }
	
    public void EnableReplyCollidersAfterReply(List<InteractiveReplyElement> elements)
    {
        if (elements != null)
        {
            for (int counter = 0; counter < elements.Count; counter++)
            {
                elements [counter].gameObject.collider.enabled = true;
            }
        }
    }
	#endregion
	
    public void ShowPreDialogueMinigame(NPCAnimations.AnimationIndex playerMinigameTexture, Dialogue firstDialogue)
    {
		npcArrow.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
        GameManager.Instance.mainCamera.enabled = false;
        // Hide the HUD
        HUD.Instance.gameObject.SetActive(false);
		
        // Clear all elements of the window
        InitializeElements(emotions);
        InitializeElements(imageText);
        InitializeElements(imageTextBox);
        InitializeElements(textOnly);
        singleDialogueElement.transform.parent.gameObject.SetActive(false);
		
        if (playerMinigameTexture != NPCAnimations.AnimationIndex.NONE)
        {
			DialogueWindow.instance.dialogue = Player.instance.interactingNPC.conversationTree.root;
			playerTexture.animation.StopAnimation();
            UpdatePlayerTexture(playerMinigameTexture);
        }
		npc1.animation.StopAnimation ();
		npc2.animation.StopAnimation ();
		npcSingle.animation.StopAnimation ();
        UpdateNPCPortrait(firstDialogue);
		
        this.gameObject.SetActive(true);
    }
		
    public void HidePreDialogueMinigame()
    {
        if (playerTexture != null)
        {
			UpdatePlayerTexture(NPCAnimations.AnimationIndex.LISTENING);
        }
        progressBar.gameObject.SetActive(true);
    }
	
    //Checks if the player is idle (for the waiting event)
    bool checkIfIdle(Dialogue currentNode)
    {
        bool isIdle = false;
		
        if (currentNode != null)
        {
            if (currentNode.type != Dialogue.DialogueType.IDLE)
                isIdle = false;
            else
                isIdle = true;
        } else
            isIdle = false;
		
        return isIdle;
    }

    public void PlaySoundFX (NPCAnimations.AnimationIndex soundFX)
    {
        AudioClip audioClip = soundFXLibrary.RetrieveAudioClip(soundFX);
        if (audioClip != null)
            AudioManager.Instance.Play(audioClip, this.transform, 1, false);
    }
	
    /// <summary>
    /// Shows the dialogue window with a given dialogue
    /// </summary>
    /// <param name='currentNode'>
    /// Dialogue to be displayed
    /// </param>
    public void ShowWindow(Dialogue currentNode = null)
    {
        EnableRepeatNPCButtons(false);

        dialogue = currentNode;

        // If no dialogue was given, or the text of the given dialogue is not set
        if ((currentNode == null || currentNode.text == "") && !checkIfIdle(currentNode))
        {
            lastNPCDialogue = null;

            GameManager.Instance.mainCamera.enabled = true;
            // Display back the HUD
            HUD.Instance.gameObject.SetActive(true);
            // Return Sherlock to idle animation
            Sherlock.Instance.PlayAnimation(NPCAnimations.AnimationIndex.NEUTRAL);
            // Clear number of active elements in window
            activeElements = 0;
			
            // Hide Display window
            if (currentNode == null || currentNode.name != "ReplayCutScene")
                gameObject.SetActive(false);
			
            singleDialogueElement.transform.parent.gameObject.SetActive(false);
            NPCDialogueAnimation anim = npcSingle.texture.gameObject.GetComponent<NPCDialogueAnimation>();
            if (anim != null)
                anim.StopAnimation();
			
            // If current dialoue has an event associated with it
            if (currentNode != null && currentNode.eventCauser != null)
            {
                // Then trigger the event
                currentNode.eventCauser.TriggerEvent(true);
            }
			AudioManager.Instance.RestoreMusicSound();
        }
		//If the dialogue type is idle, then start the timer event
		else if (checkIfIdle(currentNode))
        {
            StartCoroutine(ShowWindowCoroutine(currentNode));
        } else
        {
            GameManager.Instance.mainCamera.enabled = false;
			
            NPCDialogueAnimation anim = npcSingle.animation;//.texture.gameObject.GetComponent<NPCDialogueAnimation>();
            if (anim != null && npcSingle.eventTriggered)
            {
                anim.StopAnimation();
                npcSingle.eventTriggered = false;
            }
            // Start coroutine to display given dialogue
            StartCoroutine(ShowWindowCoroutine(currentNode));	
        }
    }
	
    public void RepeatNPCDialogue()
    {
        if (dialogue.speaker != Dialogue.Speaker.NPC && lastNPCDialogue != null)
        {
			lastNPCDialogue.firstRepetition = true;
			dialogue.firstRepetition = dialogue.forceRepetition;
            ShowWindow(lastNPCDialogue);
        }
    }
	
    public void RepeatDialogue()
    {
        dialogue.firstRepetition = true;
        ShowWindow(dialogue);
    }
	
    //timer animation has completed, go to next dialogue
    void TimerComplete(TimerAnimation timerAnim)
    {
        if (timerAnim == timer)
        {
            //Debug.Log("Timer: "+ dialogue);
            timer.animationCompleteDelegate = null;
            //dialogue.owner.UpdateToNextSprite();
            dialogue.owner.SetDialogue(dialogue.nextDialogue);
        }
    }

    /// <summary>
    /// Updates the NPC portrait.
    /// </summary>
    /// <param name='currentDialogue'>
    /// Current dialogue.
    /// </param>
    public void UpdateNPCPortrait(Dialogue currentDialogue)
    {
        if (currentDialogue != null)
        {
			if (dialogue == null) dialogue = currentDialogue;
            if (currentDialogue.owner.conversationTree.animations == null || currentDialogue.npc1Animation == NPCAnimations.AnimationIndex.NONE)
            {
                if ((currentDialogue.npc2Portrait == null))
                {
					UpdateNPCPortrait(currentDialogue.npc1Portrait, 0, currentDialogue.npcNo);
					if (currentDialogue.npc1Portrait != null)
						Player.instance.interactingNPC.UpdateToSprite (currentDialogue.npc1Portrait);
    				
                } else
                {
                    UpdateNPCPortrait(currentDialogue.npc1Portrait, 1, currentDialogue.npcNo);
					if (currentDialogue.npc1Portrait != null)
						Player.instance.interactingNPC.UpdateToSprite (currentDialogue.npc1Portrait);
                    UpdateNPCPortrait(currentDialogue.npc2Portrait, 2, currentDialogue.npcNo);
                }
            } else
            {
                if ((currentDialogue.npc2Animation == NPCAnimations.AnimationIndex.NONE))
                {
                    NPCAnimations.AnimationSequence npc1Anim = currentDialogue.owner.conversationTree.animations.RetrieveAnimationSequence(currentDialogue.npc1Animation);
                    List<Texture> npc1Tex = npc1Anim.textures;
                    if (npc1Tex.Count > 0)
                    {
                        npcSingle.animation.StopAnimation();
                        npcSingle.animation.SetAnimationList(npc1Tex);
                        npcSingle.animation.PlayAnimation();
						npcSingle.animation.SetSpeed(npc1Anim.speed);
						Player.instance.interactingNPC.UpdateAnimation(npc1Tex, true);
                        UpdateNPCPortrait(npc1Tex [0], 0, currentDialogue.npcNo);
                    } else
                    {
                        Debug.LogError("No texures were found for index " + currentDialogue.npc1Animation.ToString());
                    }
                } else
                {
                    NPCAnimations.AnimationSequence npc1Anim = currentDialogue.owner.conversationTree.animations.RetrieveAnimationSequence(currentDialogue.npc1Animation);
                    List<Texture> npc1Tex = npc1Anim.textures;
                    if (npc1Tex.Count > 0)
                    {
                        npc1.animation.StopAnimation();
                        npc1.animation.SetAnimationList(npc1Tex);
                        npc1.animation.PlayAnimation();
						npc1.animation.SetSpeed(npc1Anim.speed);
						Player.instance.interactingNPC.UpdateAnimation(npc1Tex, true);
                        UpdateNPCPortrait(npc1Tex [0], 1, currentDialogue.npcNo);
                    } else
                    {
                        Debug.LogError("No texures were found for index " + currentDialogue.npc1Animation.ToString());
                    }
                    NPCAnimations.AnimationSequence npc2Anim = currentDialogue.owner.conversationTree.animations2.RetrieveAnimationSequence(currentDialogue.npc2Animation);
                    List<Texture> npc2Tex = npc2Anim.textures;
                    if (npc2Tex.Count > 0)
                    {
                        npc2.animation.StopAnimation();
                        npc2.animation.SetAnimationList(npc2Tex);
                        npc2.animation.PlayAnimation();
						npc2.animation.SetSpeed(npc2Anim.speed);
						Player.instance.interactingNPC.UpdateAnimation(npc2Tex, false);
						UpdateNPCPortrait(npc2Tex [0], 2, currentDialogue.npcNo);
                    } else
                    {
                        Debug.LogError("No texures were found for index " + currentDialogue.npc1Animation.ToString());
                    }
                }
            }
            SetHeight(currentDialogue);
        }
    }
	
    //Sets the npc height through the dialogue
    private void SetHeight(Dialogue currentDialogue)
    {
        float newHeight;
        float playerHeight = .6f;//playerTexture.stretch.relativeSize.y;
        //npc1.texture.pivot = UIWidget.Pivot.Bottom;
        //npc2.texture.pivot = UIWidget.Pivot.Bottom;
		
        if (Player.instance.interactingNPC != null)
        {
            if ((currentDialogue.npc2Animation == NPCAnimations.AnimationIndex.NONE))
            {
                newHeight = playerHeight * Player.instance.interactingNPC.npcHeight;
                npcSingle.stretch.relativeSize = new Vector2(npcSingle.stretch.relativeSize.x, newHeight);
            }
            else
            {
                playerHeight = .55f;
                newHeight = playerHeight * Player.instance.interactingNPC.npcHeight;
                
                npc1.stretch.relativeSize = new Vector2(npc1.stretch.relativeSize.x, newHeight);
                npc2.stretch.relativeSize = new Vector2(npc2.stretch.relativeSize.x, newHeight);
            }
        }
    }
	
    /// <summary>
    /// Updates the NPC portrait.
    /// </summary>
    /// <param name='newTexure'>
    /// New texture for the NPC
    /// </param>
    private void UpdateNPCPortrait(Texture newTexure, int npcId, int npcNo)
    {
        CharacterTexture npcTexture = (npcId == 0) ? npcSingle : ((npcId == 1) ? npc1 : npc2);
		
        if (newTexure != null) {
			if (npcId == 0) {
				if (npcNo == 1) {
						UpdatePlayerTexture (null, 0.6f);
						npcSingle.stretch.relativeSize.y = 0.6f;
				} else {
						UpdatePlayerTexture (null, 0.55f);
						npcSingle.stretch.relativeSize.y = 0.55f;
				}

				npc1.texture.transform.parent.gameObject.SetActive (false);
				npcSingle.texture.gameObject.SetActive (true);

			} else {
				npc1.texture.transform.parent.gameObject.SetActive (true);
				npcSingle.texture.gameObject.SetActive (false);
				UpdatePlayerTexture (null, 0.55f);
			}

			npcTexture.texture.gameObject.SetActive (true);

			// Update the portrait of the NPC
			npcTexture.texture.mainTexture = newTexure;

			if (Player.instance != null && Player.instance.interactingNPC != null)

			npcTexture.stretch.initialSize = new Vector2 (newTexure.width, newTexure.height);
		}
    }
	
    /// <summary>
    /// Updates the NPC arrow based on current dialogue
    /// </summary>
    /// <param name='dialogue'>
    /// Dialogue.
    /// </param>
    public void UpdateNPCArrow(Dialogue dialogue)
    {
        if (dialogue != null)
        {
            if (dialogue.npcUnderArrow == Dialogue.DialogueLocation.NPC1)
            {
                npcArrow.gameObject.SetActive(true);
                npcArrow.localPosition = new Vector3(npc1.texture.transform.localPosition.x + dialogue.arrowOffset.x, npcArrow.localPosition.y + dialogue.arrowOffset.y, npcArrow.localPosition.z);
            } else if (dialogue.npcUnderArrow == Dialogue.DialogueLocation.NPC2)
            {
                npcArrow.gameObject.SetActive(true);
                npcArrow.localPosition = new Vector3(npc2.texture.transform.localPosition.x + dialogue.arrowOffset.x, npcArrow.localPosition.y + dialogue.arrowOffset.y, npcArrow.localPosition.z);
            } else
                npcArrow.gameObject.SetActive(false);
			
            return;
        }
    }
	
	public void UpdatePlayerTexture(NPCAnimations.AnimationIndex animationIndex, float stretch = -1)
	{
        if (dialogue.owner != null) 
            playerParentTransform.localScale = Vector3.one * dialogue.owner.playerHeight;
        
		//if (stretch > 0)
		//	playerTexture.stretch.relativeSize.y = stretch;
		
		if (animationIndex != NPCAnimations.AnimationIndex.NONE)
		{
			dialogue.owner.conversationTree.playerAnimations.animations.PlayAnimationSequence(animationIndex, playerTexture);

			if(animationIndex == NPCAnimations.AnimationIndex.RECEIVE_TOY)
			{
				received2DToy.spriteName = dialogue.owner.missingToy.name;
				received2DToy.color = dialogue.owner.foundToyColor;
				received2DToy.gameObject.SetActive(true);
				reach2DToy.gameObject.SetActive(false);
                
                progressBar.PlayStarParticle(received2DToy.transform.position);
			}
			else
			{
				received2DToy.gameObject.SetActive(false);

				if (animationIndex == NPCAnimations.AnimationIndex.REACH_FOR_TOY)
				{
					reach2DToy.spriteName = dialogue.owner.missingToy.name;
					reach2DToy.color = dialogue.owner.foundToyColor;
					reach2DToy.gameObject.SetActive(true);
				}
				else
				{
					reach2DToy.gameObject.SetActive(false);
				}
			}
		}
	}

	public void UpdatePlayerTexture(Texture updatedPlayerTexture, float stretch = -1)
    {
        if (dialogue.owner != null) 
            playerParentTransform.localScale = Vector3.one * dialogue.owner.playerHeight;
        
     //if (stretch > 0)
     //  playerTexture.stretch.relativeSize.y = stretch;

		if (updatedPlayerTexture != null)
		{
            playerTexture.stretch.initialSize = new Vector2(updatedPlayerTexture.width, updatedPlayerTexture.height);
            playerTexture.texture.mainTexture = updatedPlayerTexture;
			if(updatedPlayerTexture.name == "PeteWithMarker")
			{
				received2DToy.spriteName = dialogue.owner.missingToy.name;
				received2DToy.color = dialogue.owner.foundToyColor;
				received2DToy.gameObject.SetActive(true);
			}
			else
			{
				received2DToy.gameObject.SetActive(false);
			}
        }
	}
	
	public void SkipShowingOptions()
    {
        AudioManager.Instance.StopVoiceOver();
        StopCoroutine("DisplayOptions");
        ForceShowOptions();
    }

    void ForceShowOptions()
    {
        int currentBox = 0;
		
        for (int i = 0; i < dialogue.replies.Count && currentBox < 4; i++)
        {
            // If reply is available to be selected by the player
            if (dialogue.replies [i].available == true)
            {
                // Display the reply data depending to current dialogue display type
                // If dialogue is in emotion phase
                if (displayType == Dialogue.DialogueDisplayType.EMOTION)
                {
                    // Update elements from reply
                    emotions [currentBox].Show();
                    emotions [currentBox++].Init(dialogue.replies [i]);
                }
				// If diallogue in image + text (scenarios) phase
				else
				if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT)
                {
                    // Update elements from reply
                    imageText [currentBox].Show();
                    imageText [currentBox++].Init(dialogue.replies [i]);
                }
				// If dialogue in text only (conversation) phase
				else
				if (displayType == Dialogue.DialogueDisplayType.TEXT_ONLY)
                {
                    // Update elements from reply
                    textOnly [currentBox].Show();
                    textOnly [currentBox++].Init(dialogue.replies [i]);
                }
				// If diallogue in image + text (scenarios) phase
				else
				if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT_BOX)
                {
                    // Update elements from reply
                    imageTextBox [currentBox].Show();
                    imageTextBox [currentBox++].Init(dialogue.replies [i]);
                }
            } else// else if reply is not available to be selected (alreay selectedin the past)
            {
                // Just go to next reply and skip one display element
                currentBox++;
            }
        }
        skipButton.SetActive(false);
        repeatButton.SetActive(true);
        EnableRepeatNPCButtons(true);
    }
	
    IEnumerator DisplayOptions(Dialogue currentDialogue)
    {
        int currentBox = 0;
        skipButton.SetActive(true);

        for (int i = 0; i < currentDialogue.replies.Count && currentBox < 4; i++)
        {
            // If reply is available to be selected by the player
            if (currentDialogue.replies [i].available == true)
            {
                // Display the reply data depending to current dialogue display type
                // If dialogue is in emotion phase
                if (displayType == Dialogue.DialogueDisplayType.EMOTION)
                {
                    // Update elements from reply
                    emotions [currentBox].Show();
                    emotions [currentBox++].Init(currentDialogue.replies [i]);
                }
				// If diallogue in image + text (scenarios) phase
				else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT)
                {
                    // Update elements from reply
                    imageText [currentBox].Show();
                    imageText [currentBox++].Init(currentDialogue.replies [i]);
                }
				// If dialogue in text only (conversation) phase
				else if (displayType == Dialogue.DialogueDisplayType.TEXT_ONLY)
                {
                    // Update elements from reply
                    textOnly [currentBox].Show();
                    textOnly [currentBox++].Init(currentDialogue.replies [i]);
                }
				// If diallogue in image + text (scenarios) phase
				else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT_BOX)
                {
                    // Update elements from reply
                    imageTextBox [currentBox].Show();
                    imageTextBox [currentBox++].Init(currentDialogue.replies [i]);
                }
                if (!UserSettings.Instance.instantAnswer)
                    DisableReplyColliders();

                // If the reply has not been repeated before
                if (currentDialogue.firstRepetition)
                {
                    // and it has a voice over
                    if (currentDialogue.replies [i].voiceOver != null)
                    {
                        // Play voice over
                        AudioManager.Instance.PlayVoiceOver(currentDialogue.replies [i].voiceOver, 1);

						if (displayType == Dialogue.DialogueDisplayType.EMOTION || displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT)
							Sherlock.Instance.PlayAnimation(NPCAnimations.AnimationIndex.TALKING);

                        // and wait until it finished
                        yield return new WaitForSeconds(currentDialogue.replies [i].voiceOver.length);

						if (displayType == Dialogue.DialogueDisplayType.EMOTION || displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT)
							Sherlock.Instance.PlayAnimation(NPCAnimations.AnimationIndex.NEUTRAL);

						yield return new WaitForSeconds(0.5f);

					} else// otherwise just wait for 0.5 second
                    {
                        yield return new WaitForSeconds(1.5f);
                    }
                }
                yield return new WaitForSeconds(UserSettings.Instance.optionsProcessingTime);
            } else// else if reply is not available to be selected (alreay selectedin the past)
            {
                // Just go to next reply and skip one display element
                currentBox++;
            }
        }
		
        skipButton.SetActive(false);
        if (displayType == Dialogue.DialogueDisplayType.EMOTION)
            EnableReplyCollidersAfterReply(emotions);
        else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT)
            EnableReplyCollidersAfterReply(imageText);
        else if (displayType == Dialogue.DialogueDisplayType.TEXT_ONLY)
            EnableReplyCollidersAfterReply(textOnly);
        else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT_BOX)
            EnableReplyCollidersAfterReply(imageTextBox);

        EnableRepeatNPCButtons(true);
    }
	
    /// <summary>
    /// Shows the dialogue window given a dialogue
    /// </summary>
    /// <param name='currentDialogue'>
    /// Dialogue to be displayed
    /// </param>
    private IEnumerator ShowWindowCoroutine(Dialogue currentDialogue = null)
    {
        EnableRepeatNPCButtons(false);
        // Get display type of the dialogue
        displayType = dialogue.displayType;
		
        // Hide the HUD
        HUD.Instance.gameObject.SetActive(false);
		
        // If current dialogue is a milestone
        if (currentDialogue.dialogueMilestone)
        {
            // Reward player with a star
            progressBar.AddStar();
        }
		
        UpdateNPCPortrait(currentDialogue);
		
        // If current dialogue force window to clear
        if (currentDialogue.clearWindow && (currentDialogue.firstRepetition || currentDialogue.forceRepetition))
        {			
            // Clear all elements of the window
            InitializeElements(emotions);
            InitializeElements(imageText);
            InitializeElements(imageTextBox);
            InitializeElements(textOnly);
            singleDialogueElement.transform.parent.gameObject.SetActive(false);
        }
		
        #region Update Portrait / Animations
        // If player portrait during voice over is set
		if ((currentDialogue.audioPortrait != null || currentDialogue.playerFinalAnimation != NPCAnimations.AnimationIndex.NONE) && currentDialogue.playerHasReplied == false)
        {
			if (currentDialogue.playerFinalAnimation != NPCAnimations.AnimationIndex.NONE)
				UpdatePlayerTexture(currentDialogue.playerFinalAnimation);
			else
	            // then update player portrait
	            UpdatePlayerTexture(currentDialogue.audioPortrait);
        }
		
        // If current dialogue speaker is Sherlock or if it's a players reply
        if ((currentDialogue.speaker == Dialogue.Speaker.PLAYER && currentDialogue.remainingReplies > 0) || currentDialogue.speaker == Dialogue.Speaker.SHERLOK)
        {	
            // if current dialogue is not Sherlock giving an instruction
            if (!(currentDialogue.speaker == Dialogue.Speaker.SHERLOK && currentDialogue.type == Dialogue.DialogueType.INSTRUCTION))
            {
                // Swich the background texture to the current background dialogue
                //background.SwitchToTexture(dialogue.background);
                dialogueBubble.SetDialogueBubble(currentDialogue);
                //DebugInfo.Instance.UpdateDebugText("Update texture to " + dialogue.background.name);
            }
            if (currentDialogue.playerHasReplied != true)
            {	
                // Show Sherlocks dialogue window
                Sherlock.Instance.StartCoroutine(Sherlock.Instance.SetDialogue(currentDialogue));
                // If there is a voice over we need to play
                if (currentDialogue.voiceOver != null && currentDialogue.firstRepetition)
                {
                    // then wait until voice over has finished

                    yield return new WaitForSeconds(currentDialogue.voiceOver.length + 0.5f);
                } else
                {
                    // then wait until voice over has finished
                    yield return new WaitForSeconds(1.0f);
                }
            } else
            {
                Sherlock.Instance.SetText(currentDialogue.text);
            }
        }
		//If current speaker is the player and he is idle, start the waiting timer
		else if (currentDialogue.speaker == Dialogue.Speaker.PLAYER && currentDialogue.type == Dialogue.DialogueType.IDLE)
        {
            // Hide Sherlocks dialogue window
            Sherlock.Instance.HideDialogue();
            // Update NPC portrait to current dialogues' portrait of NPC
            UpdateNPCPortrait(currentDialogue);
            //Clears the white textbox
            singleDialogueElement.transform.parent.gameObject.SetActive(false);	
            //clears background dialogue bubble
            if (!currentDialogue.showBackgroundTexture)
                dialogueBubble.ClearTexture();
            //background.ClearTexture();
			
            //displays the timer
            if (currentDialogue.displayTimer)
                timer.DisplayTimer();
			
            //Set the duration of the timer
            timer.SetDuration(currentDialogue.timerDuration);
			
            //Delegate: Know when the animation/timer ends
            //timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<TimerAnimation>();
            timer.animationCompleteDelegate = TimerComplete;	
			
            //Play animation
            if (currentDialogue.displayTimer)
                timer.PlayTimerAnimation();
			
			//Start regular timer
			else
                timer.StartTimer_NoDisplay();
			
            //Play the NPC's animation
            NPCDialogueAnimation anim = npcSingle.texture.gameObject.GetComponent<NPCDialogueAnimation>();
            if (anim != null)
            {
                anim.SetAnimationList(currentDialogue.owner.animationSprites);
				anim.SetFPS(4);
                anim.PlayAnimation();
                npcSingle.eventTriggered = true;
            }
        } else // If current speaker is an NPC or the player, replying back to the NPC directly (no reply choice)
        {
            if (currentDialogue.speaker == Dialogue.Speaker.NPC)
            {
                if (!(lastNPCDialogue != null && lastNPCDialogue.nextDialogue != null && lastNPCDialogue.nextDialogue == currentDialogue))
                    lastNPCDialogue = currentDialogue;
            }

            // Hide Sherlocks dialogue window
            Sherlock.Instance.HideDialogue();
			
            // Update NPC portrait to current dialogues' portrait of NPC
            UpdateNPCPortrait(dialogue);
            // Swich the background texture to the current background dialogue
            //background.SwitchToTexture(dialogue.background);
            dialogueBubble.SetDialogueBubble(dialogue);
        }
		
        UpdateNPCArrow(currentDialogue);
        #endregion
		
        #region Update / Show Replies
        // If current dialogue force window to clear
        if (currentDialogue.clearWindow)
        {
            // Reset active box
            activeReply = null;
            // If there are available replies for the player
            if (currentDialogue.remainingReplies > 0)
            {
                InitializeElements(emotions);
                InitializeElements(imageText);
                InitializeElements(imageTextBox);
                InitializeElements(textOnly);
                // Enable the appropriate display elements
                if (displayType == Dialogue.DialogueDisplayType.EMOTION)
                    EnableAvailableElements(currentDialogue, emotions);
                else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT)
                    EnableAvailableElements(currentDialogue, imageText);
                else if (displayType == Dialogue.DialogueDisplayType.TEXT_ONLY)
                    EnableAvailableElements(currentDialogue, textOnly);
                else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT_BOX)
                    EnableAvailableElements(currentDialogue, imageTextBox);
				
                singleDialogueElement.transform.parent.gameObject.SetActive(false);
            } else
            {
                // Otherwise disable all display elements
                if (displayType == Dialogue.DialogueDisplayType.EMOTION)
                    DisableElements(emotions);
                else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT)
                    DisableElements(imageText);
                else if (displayType == Dialogue.DialogueDisplayType.TEXT_ONLY)
                    DisableElements(textOnly);
                else if (displayType == Dialogue.DialogueDisplayType.IMAGE_TEXT_BOX)
                    DisableElements(imageTextBox);
				
                // If speaker is NPC or player that has no remaining replies (to select from) and it is not idle
                if ((currentDialogue.speaker == Dialogue.Speaker.NPC || currentDialogue.speaker == Dialogue.Speaker.PLAYER)
                    && currentDialogue.type != Dialogue.DialogueType.IDLE)
                {
                    // Enable single dialogue element
                    singleDialogueElement.transform.parent.gameObject.SetActive(true);
                    // and display text of NPC / player
                    singleDialogueElement.text = currentDialogue.text;
                } else // otherwise do nothing
                    singleDialogueElement.transform.parent.gameObject.SetActive(false);
            }
		
            // If the player portrait for this dialogue is set
			if (currentDialogue.playerInitialAnimation != NPCAnimations.AnimationIndex.NONE)
			{
				UpdatePlayerTexture(currentDialogue.playerInitialAnimation);
			}
            else if (currentDialogue.playerPortrait != null)
            {
                // Update the player portraitto match the current dialogue
                UpdatePlayerTexture(currentDialogue.playerPortrait);
            }

            yield return StartCoroutine("DisplayOptions", currentDialogue);

            activeElements = currentDialogue.remainingReplies;	
        } else
        {
            activeElements = 0;
        }
        #endregion
		
        currentDialogue.startReplyingTimer = System.DateTime.Now;
		
        // Activate display window
        this.gameObject.SetActive(true);
		
        // If speaker is NPC or player that has no remaining replies (to select from) and there is a voice over to be played
        if ((currentDialogue.speaker == Dialogue.Speaker.NPC || (currentDialogue.speaker == Dialogue.Speaker.PLAYER && currentDialogue.replies.Count == 0)) && currentDialogue.voiceOver != null && currentDialogue.firstRepetition)
        {
            // Play voice over
            AudioManager.Instance.PlayVoiceOver(currentDialogue.voiceOver, 1);
            // and waituntil it finishes
            yield return new WaitForSeconds(currentDialogue.voiceOver.length);
        } else if (currentDialogue.speaker == Dialogue.Speaker.NPC || (currentDialogue.speaker == Dialogue.Speaker.PLAYER && currentDialogue.replies.Count == 0))
        {
            yield return new WaitForSeconds(0.5f);
        }
		
        if (currentDialogue.transitionDelay > 0)
            yield return new WaitForSeconds(currentDialogue.transitionDelay);
        
        if (currentDialogue.type != Dialogue.DialogueType.QUESTION)
        {
            if (currentDialogue.speaker != Dialogue.Speaker.SHERLOK)
                yield return new WaitForSeconds(0.25f);
            currentDialogue.Reply(-1);
        }
			
        currentDialogue.firstRepetition = false || currentDialogue.forceRepetition;
    }
}
