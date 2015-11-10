using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreDialogueMinigame : MonoBehaviour {
 
    public NPCAnimations.AnimationIndex animationIndex;
	public SCEvent returnEvent;
	
	public ConversationTree instructions;
	
	/// <summary>
	/// Start the pre-dialogue minigame
	/// </summary>
	/// <param name='npc'>
	/// Npc associated with minigame
	/// </param>
	public void PreDialogueMinigameStart(NPC npc)
	{		
		DialogueWindow.instance.ShowPreDialogueMinigame(animationIndex, npc.GetConversationRoot());
		Invoke("ShowInstructions", 1);
		BroadcastMessage("MinigameStart",SendMessageOptions.DontRequireReceiver);
	}
	
	/// <summary>
	/// Shows the instructions for the minigame
	/// </summary>
	void ShowInstructions()
	{
		Sherlock.Instance.PlaySequenceInstructions(instructions.root, null);
	}
}
