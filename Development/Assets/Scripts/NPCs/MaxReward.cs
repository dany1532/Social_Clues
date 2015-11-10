using UnityEngine;
using System.Collections;

public class MaxReward : MonoBehaviour {
	public NPC npc;
	
	public void OnPress(bool pressed)
	{
		if (pressed)
		{
			
			GameObject go = GameObject.Find("BedroomManager");
			
			if(go != null)
			{
				BedroomLevelManager manager = go.GetComponent<BedroomLevelManager>();
				manager.StoryEndDialogue(transform.position);
			}
			
			else{
				this.gameObject.SetActive(false);
				npc.OnQuestCompleted();
			}
		}
	}
}
