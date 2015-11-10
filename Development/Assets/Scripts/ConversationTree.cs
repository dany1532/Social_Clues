using UnityEngine;
using System.Collections;

/// <summary>
/// Conversation tree to be used for dialogues, instructions and other statements
/// </summary>
[System.Serializable]
public class ConversationTree : MonoBehaviour
{
	
    // Root of the dialogue
    public Dialogue root;
    // Current node of the dialogue
    public Dialogue currentNode;

	public NPCAnimations animations;
	public NPCAnimations animations2;
    public PlayerAnimations playerAnimations;

    // Use this for initialization
    void Awake()
    {
        // Set current node in tree
        currentNode = root;
		if (animations == null)
			animations = GetComponent<NPCAnimations> ();
		if (playerAnimations == null)
			playerAnimations = GetComponent<PlayerAnimations> ();
    }	
	
    /// <summary>
    /// Get event assciated with current dialogue node
    /// </summary>
    /// <returns>
    /// The event assciated with current dialogue node
    /// </returns>
    public SCEvent GetCurrentEvent()
    {
        // If current node is set
        if (currentNode != null)
			// return associated event
            return currentNode.eventCauser;
		
        return null;
    }
	
    /// <summary>
    /// Set conersation to a specific dialoge
    /// </summary>
    /// <param name='dialogue'>
    /// Dialogue to be set in conversation tree
    /// </param>
    public void SetDialogue(Dialogue dialogue)
    {
        currentNode = dialogue;
    }
	
    /// <summary>
    /// Set current dialogue node to next dialoge in the conversation
    /// </summary>
    public void ContinueToNextDialogue()
    {
        // If the current node is set
        if (currentNode != null)
			// set current node to next dialogue
            currentNode = (currentNode.nextDialogue != null) ? currentNode.nextDialogue : currentNode.eventCauser.parameter;
    }
	
    /// <summary>
    /// Start playing conversation tree from conversation root
    /// </summary>
    /// <returns>
    /// True if the current dialogue was succesfully displayed on screen
    /// </returns>
    public bool StartConversation()
    {
        // Show dialogue starting from root
        return ShowCurrentDialogue(root);
    }

    public bool ShowCurrentDialogue(Sentence dialogueReply)
    {
        return false;
    }
	
    /// <summary>
    /// Shows the current dialogue
    /// </summary>
    /// <returns>
    /// True if the current dialogue was succesfully displayed on screen
    /// </returns>
    /// <param name='dialogueReply'>
    /// Dialogue to be dsplayed
    /// </param>
    public bool ShowCurrentDialogue(Dialogue dialogueReply)
    {
        CancelInvoke();
		
        // Set current dialogue to given dialogue
        currentNode = dialogueReply;
		
        // If current dialogue is set and Sherlock gives an instruction
        if (currentNode != null && currentNode.speaker == Dialogue.Speaker.SHERLOK && currentNode.type == Dialogue.DialogueType.INSTRUCTION)
        {
            // If current dialogue includes text
            //if (currentNode.text != "")
            // Then show Sherlock dialogue ext
            Sherlock.Instance.StartCoroutine(Sherlock.Instance.SetDialogue(currentNode));
			
            // If current dialoue has an event associated with it
            if (currentNode.eventCauser != null)
            {
                // Then trigger the event
                currentNode.eventCauser.TriggerEvent(true);
            }
			
            // Return if Sherlock was able to display the dialogue
            return Sherlock.Instance.isValid();
        }
		// If current dialogue is set and NPC gives an instruction
		else if (currentNode != null && currentNode.speaker == Dialogue.Speaker.NPC && currentNode.type == Dialogue.DialogueType.INSTRUCTION)
        {			
			// If current dialogue includes text
            if (currentNode.voiceOver != null)
            {
                AudioManager.Instance.PlayVoiceOver(currentNode.voiceOver, 1); 		
                return true;
            }
			
            // Return if Sherlock was able to display the dialogue
            return false;
        }
		// If the dialogue window has been set
		else if (DialogueWindow.instance != null)
        {
            // If the current dialogue is set, and there is an event associated with it, which is not an atomic operation
            if (currentNode != null && currentNode.eventCauser != null && currentNode.eventCauser.atomicOperation == false)
            {
                // Then trigger the event
                currentNode.eventCauser.TriggerEvent(true);
            }
			
            // Activate dialogue window
            DialogueWindow.instance.gameObject.SetActive(true);
            // And show the dialogue
            DialogueWindow.instance.ShowWindow(currentNode);
			
            return true;
        } else // Else if no dialogue was found then display warning message
            Debug.LogWarning("Dialogue Window hasn't been initialized");
        // If the dialoge was not displayed then return false
        return false;
    }

    /// <summary>
    /// Show the next dialogue in the conversation
    /// </summary>
    /// <returns>
    /// True if the next dialogue was succesfully displayed on screen
    /// </returns>
    public bool ShowNextDialogue()
    {		
        // If current node is set
        if (currentNode != null)
			// Show next dialogue
            return ShowCurrentDialogue(currentNode.nextDialogue);
        else
			// Reached end of conversation, hide conversation
            return ShowCurrentDialogue((Dialogue)null);
    }
}
