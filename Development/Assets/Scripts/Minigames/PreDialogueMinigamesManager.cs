using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreDialogueMinigamesManager : Singleton<PreDialogueMinigamesManager> {
	
	public List<PreDialogueMinigame> minigames;
	int currentMinigame = -1;

	private NPC tempNPC;
	private bool loopMinigame;
	
	// Use this for initialization
	void Awake () {
		instance = this;
		
		foreach(PreDialogueMinigame minigame in minigames)
		{
			minigame.gameObject.SetActive(false);
		}
		loopMinigame = false;
	}
	
	/// <summary>
	/// Start a random minigame
	/// </summary>
	public void PreDialogueMinigameStart(NPC npc)
	{
		tempNPC = npc;
		if(npc.includePreDialogueMinigame == NPC.IncludePreDialogueMinigame.NONE) {
			currentMinigame = -1;
			Player.instance.TalkNPC(npc, true);
		} else if(npc.includePreDialogueMinigame == NPC.IncludePreDialogueMinigame.EYE_CONTACT || npc.includePreDialogueMinigame == NPC.IncludePreDialogueMinigame.PLACEMENT_CONTACT) {
			//currentMinigame = Random.Range(0, minigames.Count-1);
			currentMinigame = (int)npc.includePreDialogueMinigame;
			//Debug.Log ("includePreDialogueMinigame: " + npc.includePreDialogueMinigame);
			//Debug.Log ("Minigame count: " + minigames.Count);
			//Debug.Log ("Current Mini Game: " + currentMinigame);			
			minigames[currentMinigame].gameObject.SetActive(true);
			minigames[currentMinigame].PreDialogueMinigameStart(npc);
			Player.instance.PreDialogueMinigameStart(npc);
		} else if(npc.includePreDialogueMinigame == NPC.IncludePreDialogueMinigame.EYE_AND_PLACEMENT_CONTACT){
			// play the placement contact minigame first, then after it is completed, play the eye contact minigame
			if(currentMinigame != (int)NPC.IncludePreDialogueMinigame.PLACEMENT_CONTACT) {
				currentMinigame = (int)NPC.IncludePreDialogueMinigame.PLACEMENT_CONTACT;
				loopMinigame = true;
			} else {
				currentMinigame = (int)NPC.IncludePreDialogueMinigame.EYE_CONTACT;
				loopMinigame = false;
			}

			minigames[currentMinigame].gameObject.SetActive(true);
			minigames[currentMinigame].PreDialogueMinigameStart(npc);

			Player.instance.PreDialogueMinigameStart(npc);
		}
	}
	
	/// <summary>
	/// Return from a minigame
	/// </summary>
	public void PreDialogueMiniGameReturn()
	{	
		if (currentMinigame != -1)
		{
			DialogueWindow.instance.HidePreDialogueMinigame();
			minigames[currentMinigame].gameObject.SetActive(false);
		}
		//Debug.Log ("Deactivate minigame");

		if(loopMinigame == true) {
			PreDialogueMinigameStart(tempNPC);
		} else {
			//Debug.Log ("Start talking");
			Player.instance.TalkNPC(tempNPC, true);
			//Debug.Log ("Finished talking");
		}

	}

	void OnDisable()
	{
		foreach (PreDialogueMinigame minigame in minigames) {
			minigame.gameObject.SetActive(false);
		}
		loopMinigame = false;
	}
}
