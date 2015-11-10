using UnityEngine;
using System.Collections;

public class CustomCharacterButton : MonoBehaviour {
	/*public CustomLevel customLevel;
	public UITexture glowSprite;
	public Transform selectedTransform;
	public UILabel numCharactersLabel;
	
	
	Vector3 initialLoc;
	string name;
	static int numCharacters = 0;
	static int MAXCHARACTERS = 4;
	
	// Use this for initialization
	void Start () {
		name = this.gameObject.name;	
		initialLoc = this.gameObject.transform.position;
	}
	
	//Changes location of character depending of state and updates label
	void ChangeLocation(bool isSelected){
		if(!isSelected){
			numCharacters--;
			transform.position = initialLoc;
		}
		else{
			numCharacters++;
			transform.position = selectedTransform.position;
		}
		
		glowSprite.enabled = isSelected;
		UpdateNumberCharacter();
	}
	
	//updates label with available characters
	void UpdateNumberCharacter(){
		numCharactersLabel.text = numCharacters.ToString() + "/" + MAXCHARACTERS.ToString();
	}
	
	//Defaults to initial states
	public void Reset(){
		glowSprite.enabled = false;
		transform.position = initialLoc;
	}
	
	// Use this for initialization
	void OnPress(bool pressed){
		if(pressed){
			
			switch(name){
				case "Amy_Button":
					customLevel.amySelected = !customLevel.amySelected;
					ChangeLocation(customLevel.amySelected);
//					GetComponent<UIButtonScale>().enabled = !GetComponent<UIButtonScale>().enabled;
//					this.transform.localScale = scale;
					//glowSprite.enabled = customLevel.amySelected;
					break;
				
				case "Max_Button":
					customLevel.maxSelected = !customLevel.maxSelected;
					ChangeLocation(customLevel.maxSelected);
//					GetComponent<UIButtonScale>().enabled = !GetComponent<UIButtonScale>().enabled;
//					this.transform.localScale = scale;
					//glowSprite.enabled = customLevel.maxSelected;
					break;
				
				case "Jake_Button":
					customLevel.jakeSelected = !customLevel.jakeSelected;
					ChangeLocation(customLevel.jakeSelected);
//					GetComponent<UIButtonScale>().enabled = !GetComponent<UIButtonScale>().enabled;
//					this.transform.localScale = scale;
					//glowSprite.enabled = customLevel.jakeSelected;
					break;		
			}

		}
		
	}*/
	
}
