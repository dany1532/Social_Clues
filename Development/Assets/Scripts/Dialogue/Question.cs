using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Question : MonoBehaviour {

	Sentence sentence;

	#region Display Information
	// The display type of the dialogue
	public enum DialogueDisplayType
	{
		TEXT_ONLY,
		IMAGE_TEXT,
		EMOTION,
		IMAGE_TEXT_BOX
	}
	
	//Used to determine the location of the dialogue bubble
	public enum DialogueLocation
	{
		PLAYER,
		NPC1,
		NPC2,
		NEUTRAL1,
		NEUTRAL2
	}
	
	public DialogueDisplayType displayType = DialogueDisplayType.TEXT_ONLY;
	
	// Who is talking, used to determine dialogue bubble location
	public DialogueLocation dialogueBubbleType;
	
	// Show dialogue background texture during idle time
	public bool showBackgroundTexture = true;
	// Show the dialogue window when displaying the dialogue
	public bool showWindow = true;
	// Clear window before showing the dialogue
	public bool clearWindow = true; 
	
	// Player portrait at the beginning of the dialogue
	public Texture playerPortrait;
	// Player portrait after the audio of the dialogue
	public Texture audioPortrait;
	public Texture npc1Portrait;
	public float npc1Height = 1f;
	public Texture npc2Portrait;
	public float npc2Height = 1f;
	
	// Whether after the end of the dialogue the NPC portrait should be switched t the next one
	public bool switchNPCPortrait = false;
	
	// If the reply is to be removed after chosen
	public bool removeReplyAfterSelected = true;
	// Lets the dialoguewindow know if a wrong reply is selected, so it can override the playerportrait
	[System.NonSerializedAttribute]
	public bool playerHasReplied = false;
	#endregion
	
	//Need to calculate timetaken for a reply
	public System.DateTime startReplyingTimer;
		
	#region Dialogue Type
	// If the dialgue is told by an NPC, the NPC
	public NPC owner;
	// Number of NPCs in current dialogue
	public int npcNo = 1;
	public DialogueLocation npcUnderArrow = DialogueLocation.NEUTRAL1;
	public Vector3 arrowOffset = Vector3.zero;
	
	// The dialogue type
	public enum DialogueType
	{
		STATEMENT,
		INSTRUCTION,
		QUESTION,
		IDLE
	}
	
	public DialogueType type;
	public bool displayTimer;
	public float timerDuration;
	
	#region Replies
	// List of wrong Replies
	public List<Reply> wrongReplies;
	public Reply rightReply;
	// List of replies displayed
	//[System.NonSerializedAttribute]
	public List<Reply> replies = new List<Reply>();
	// How many replies to display
	public int numDisplayedReplies = 0;
	// How many replies are left in the dialogue
	//[System.NonSerializedAttribute]
	public int remainingReplies;
	// If the player needs to click one or two times to select a reply
	public bool singleClick = false;
	#endregion

	#endregion
	
	// Event triggered with the event
	public SCEvent eventCauser;
	
	// If this is a milestone that will reward the player
	public bool dialogueMilestone = false;
	
	private TimerAnimation myTimer;
	
	// Use this for initialization
	void Start () {
		// If the dialogue has replies
		if (wrongReplies != null) {
			remainingReplies = wrongReplies.Count;
			if(rightReply != null) { // Add the right reply
				replies.Add(rightReply);
				rightReply.SetIndex(0);
				rightReply.id = 0;
				remainingReplies++;
			}
			
			int counter;
			for(counter = 0; counter < wrongReplies.Count; counter++)
				wrongReplies[counter].id = counter+1;
			
			Reply tempReply;
			// Randomize the list of wrong replies
			if(rightReply != null) {
				for(counter = 0; counter < wrongReplies.Count ; counter++) {
					int randomindex = Random.Range(counter, wrongReplies.Count);
					tempReply = wrongReplies[randomindex];
					wrongReplies[randomindex] = wrongReplies[counter]; 
					wrongReplies[counter] = tempReply;		
				}
			}
			// Populate the list of replies
			int addedIndex = (rightReply == null) ? 0 : 1;
			for(counter = 0; counter < numDisplayedReplies - addedIndex; counter++) {
				wrongReplies[counter].SetIndex(counter+addedIndex);
				replies.Add(wrongReplies[counter]);
			}
			// Randomize the list of replies
			if(rightReply != null) {
				for(counter = (Mathf.Min(numDisplayedReplies, remainingReplies) - 1); counter > 0; counter--) {
					int k = Random.Range(0, counter + 1);
					//Debug.Log ("Random index:" + k + " Current index:" + counter);
					tempReply = replies[k];
					replies[k] = replies[counter];  
					replies[counter] = tempReply;
					
					replies[counter].SetIndex(counter);
					replies[k].SetIndex(k);
				}
			}
		}
		remainingReplies = Mathf.Min(remainingReplies, 4);
		
		// If there is no portrait after the voice over, then set the voice over to the initial portrait
		if (audioPortrait == null)
			audioPortrait = playerPortrait;
	}
	
	/// <summary>
	/// Event triggered when minigame starts
	/// </summary>
	/// <param name='_owner'>
	/// The NPC that is associated with the minigame and the dialogue
	/// </param>
	public void StartMinigame (NPC _owner) {
		// Set the owner NPC to the NPC assocated with the minigame
		owner = _owner;
		
		// If there is an event  associated with the dialogue
		if (eventCauser != null)
			// Then set the causer of the event to the NPC, and force the player to return to the NPC at the of the event
			eventCauser.SetEventCauser(owner, eventCauser.location != null);
	}
	
	//	//timer animation has completed, go to next dialogue
	//	void TimerComplete(TimerAnimation timerAnim){
	//		if(timerAnim == myTimer){
	//			myTimer = null;
	//			owner.UpdateToNextSprite();
	//			owner.SetDialogue(nextDialogue);
	//		}
	//	}
	
	/// <summary>
	/// One of the replies was selected
	/// </summary>
	/// <param name='availableIndex'>
	/// The index of the reply that was selected
	/// </param>
	public void Reply (int availableIndex)
	{
		//Debug.Log("Here "+this);
		// If there is an event associated with the dialogue, and the event can force the dialogue to be skipped
		if (eventCauser != null && eventCauser.atomicOperation == true)
		{
			// Then trigger event
			eventCauser.TriggerEvent(false);
			
			//if(nextDialogue.type == DialogueType.IDLE)
			//owner.SetDialogue(nextDialogue);
		}
		
		//		//Used when loading a level and the next dialogue is a "Waiting event"
		//		else if (eventCauser != null && eventCauser.atomicOperation == false)
		//		{
		//			// Then trigger event
		//			eventCauser.TriggerEvent(false);
		//			//Trigger the next dialogue
		//			owner.SetDialogue(nextDialogue);
		//			
		//		}
		else // Otherwise move to the dialogue after the reply
		{
			// If the given reply index is valid and there are replies
			if (availableIndex >= 0 && replies != null)
			{
				// If there is at least one reply
				if (availableIndex < replies.Count)
				{
					// Reduce the availableIndexnumber of remaining replies
					if(numDisplayedReplies != 2)
						remainingReplies--;
					// Mark the selected reply as not available
					if(removeReplyAfterSelected)
						replies[availableIndex].available = false;
					// Mark this question as replied
					playerHasReplied = true;
					/*
					if (remainingReplies >= numDisplayedReplies)
					{
						// Swap new wrong reply in
						Reply tempReply = replies[availableIndex];
						replies[availableIndex] = replies[remainingReplies];
						replies[remainingReplies] = tempReply;
						
						replies[remainingReplies].SetIndex(remainingReplies);
						replies[availableIndex].SetIndex(availableIndex);
					}
					*/
					#if !UNITY_WEBPLAYER
					if (rightReply != null)
					{
						//Debug.Log("NPC:"+owner.name+",Conversation:"+this.name+",ReplyIndex"+replies[availableIndex].id);
						//owner.interactionID;
						int replyTimeTaken = System.DateTime.Now.Subtract(startReplyingTimer).Seconds;
						int commID = MainDatabase.Instance.getIDs("SELECT CommID FROM PointCommunication WHERE CommText ='" + this.name + "';");
						int npcDatabaseId;
						int questionID;
						int replyID;
						
						if (commID != -1)
						{
							npcDatabaseId = MainDatabase.Instance.getIDs("Select NPCID from NPC where NPCName ='"+ owner.name.Substring(0,owner.name.Length-7)+"';");
							questionID = MainDatabase.Instance.getIDs("SELECT QuestionID FROM Question WHERE CommID = " + commID + " AND NPCID = " + npcDatabaseId +";");
							//Debug.Log("questionID "+questionID);
							replyID = MainDatabase.Instance.getIDs("SELECT ReplyID FROM Reply WHERE QuestionID = " + questionID + " AND objectID = " + replies[availableIndex].id + ";");
							//Debug.Log("replies.Count: "+replies.Count+" remainingReplies: "+remainingReplies);
							if(replies[availableIndex].id != 0)
							{
								MainDatabase.Instance.InsertSql("INSERT INTO Answer (ReplyID,ReplyType,ReplyTimeTaken,InteractionID)VALUES("+replyID+",1,"+replyTimeTaken+",'"+owner.interactionID+"');");
							}
							else
							{
								MainDatabase.Instance.InsertSql("INSERT INTO Answer (ReplyID,ReplyType,ReplyTimeTaken,InteractionID)VALUES("+replyID+",0,"+replyTimeTaken+",'"+owner.interactionID+"');");
								
								int totalScore = 25*(remainingReplies+1); // 100 = 25*(3+1) if first one is right answer
								string commPointText = this.name;
								//updating score for that interaction
								MainDatabase.Instance.UpdateSql("UPDATE Interaction SET "+commPointText+"Score = "+totalScore+" WHERE InteractionID = "+owner.interactionID+";");
								
								
								for(int i=0;i<replies.Count;i++)
								{
									if(replies[i].available)
									{
										replyID = MainDatabase.Instance.getIDs("SELECT ReplyID FROM Reply WHERE QuestionID = " + questionID + " AND objectID = " + replies[i].id + ";");
										
										if (replyID != -1)
											MainDatabase.Instance.InsertSql("INSERT INTO Answer (ReplyID,ReplyType,ReplyTimeTaken,InteractionID)VALUES("+replyID+",2,0,'"+owner.interactionID+"');");
									}
								}
							}
						}
					}
					
					#endif					
					// Set the dialogue of the NPC to the reply dialogue
					if (owner != null)
					{
						owner.SetDialogue(replies[availableIndex].dialogueReply);
					}
				}
			}
			else if (owner != null && type != DialogueType.IDLE)
			{
				// Set the dialogue of the NPC to the next dialogue
				owner.SetDialogue(sentence.nextDialogue);
			}
		}
	}
	
	public float GetCommulativeDuration ()
	{
		float duration = 0;
		Question currentDialogue = this;
		
		while (currentDialogue != null)
		{
			if (currentDialogue.sentence.voiceOver != null)
				duration += currentDialogue.sentence.voiceOver.length;
			currentDialogue = currentDialogue.sentence.nextDialogue.GetQuestion();
		}
		
		return duration;
	}
}
