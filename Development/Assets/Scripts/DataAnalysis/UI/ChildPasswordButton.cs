using UnityEngine;
using System.Collections;

public class ChildPasswordButton : MonoBehaviour {
	public Tab switchTab;
	
	void OnPress(bool pressed){
		if(!pressed){
			if(name == "Okay_Button"){
				switchTab.SwitchTab();	
			}
			else if(name == "Cancel_Button"){
				switchTab.HideInput();	
			}
		}
	}
}
