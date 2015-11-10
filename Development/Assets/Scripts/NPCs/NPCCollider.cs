using UnityEngine;
using System.Collections;

/// <summary>
/// NPC collider that triggers talking with player
/// </summary>
public class NPCCollider : MonoBehaviour {
	public NPC npc;
	
	// Check if the sprite was clicked
	void OnPress(bool pressed)
	{
		if (pressed)
		{
			InputManager.Instance.ReceivedUIInput();
			if (npc.interactingState != NPC.InteractingState.COMPLETED_TASK && npc.interactingState != NPC.InteractingState.INACTIVE)
			{
				if(!Player.instance.cutscene)
					npc.StartInteraction();
			}
		}
	}
}
