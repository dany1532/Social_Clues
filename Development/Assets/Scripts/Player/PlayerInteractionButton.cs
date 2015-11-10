using UnityEngine;
using System.Collections;

public class PlayerInteractionButton : MonoBehaviour {
	string name;
	public PlayerInteractionBubble bubble;
	
	void Start(){
		name = this.gameObject.name;
	}
	
	void OnPress(bool pressed){
		if(pressed){
			InputManager.Instance.ReceivedUIInput();
			Player.instance.SetFinishedInteraction();
			
			switch(name){
				
				case "Talk_Button":
					bubble.InteractWithNPC();
					break;
				
				case "Ignore_Button":
					bubble.IgnoreNPC();
					break;
			}
		}
	}
}
