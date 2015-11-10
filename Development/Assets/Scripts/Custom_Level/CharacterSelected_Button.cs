using UnityEngine;
using System.Collections;

public class CharacterSelected_Button : MonoBehaviour
{
	
    public CharacterSelectionTable char_Table;
    public ConfirmCharacterButton confirmButton;
    public UISprite myBorder;
	public UISprite selectedCharater;
    public LevelCharactersInfo.Character charInfo;
	public GameObject okayButton;

    UISprite myIcon;
    UIStretch myStretch;
    UISprite myBackground;
    UISprite cancelSprite;
    UILabel nameLabel;
    UILabel backgroundLabel;
	
    string backgroundSpriteName;
    string backgroundSpriteName_Blank = "Yellow Character Selected Bubble";

    public GameObject createButton;
	
    //Find respective components and set to default state
    void Start()
    {		
        myIcon = transform.FindChild("Icon").GetComponent<UISprite>();
        myStretch = transform.FindChild("Icon").GetComponent<UIStretch>();
        myBackground = transform.FindChild("Background").GetComponent<UISprite>();
        cancelSprite = transform.FindChild("Cancel_button").GetComponent<UISprite>();
        nameLabel = transform.FindChild("Label").GetComponent<UILabel>();
        backgroundLabel = transform.FindChild("BackgroundLabel").GetComponent<UILabel>();
		
        myIcon.enabled = false;
        nameLabel.enabled = false;
        myBorder.enabled = false;
        cancelSprite.enabled = false;
		
        backgroundSpriteName = myBackground.spriteName;
    }
	
    //When slot/cross is selected, enable correspoding slot in table and set to default state
    void OnPress(bool pressed)
    {
        if (!pressed && myIcon.enabled)
        {
			
            myIcon.enabled = false;
            nameLabel.enabled = false;
            myBorder.enabled = false;
            cancelSprite.enabled = false;
			//Checking if a charater is selected
			if(selectedCharater.enabled == true)
				okayButton.SetActive(true); // if character is selected and cancel button is pressed
            backgroundLabel.enabled = true;
            myBackground.spriteName = backgroundSpriteName;
			
            char_Table.Enable_Slot(charInfo.npcName);
            confirmButton.occupiedSlots--;

            if (confirmButton.occupiedSlots <= 0)
                GameObject.Find("CreateLevel_Button").SetActive(false);
            //if(confirmButton.HasAvailableSlots())
            //confirmButton.gameObject.SetActive(true);
        }
    }
	
    //Character has been confirmed, display character sprite
    public void SetSprite(LevelCharactersInfo.Character info)
    {
        charInfo = info;	
		
        myIcon.spriteName = charInfo.npcTexture;
        myIcon.enabled = true;
		
        myStretch.initialSize.Set(myIcon.GetAtlasSprite().inner.width, myIcon.GetAtlasSprite().inner.height);
		
        nameLabel.text = charInfo.npcName;
        nameLabel.enabled = true;
		
        myBorder.enabled = true;
		
        cancelSprite.enabled = true;
		
        backgroundLabel.enabled = false;
        myBackground.spriteName = backgroundSpriteName_Blank;
    }
	
    //Used when returning to the level selection screen
    public void Default()
    {
        myIcon.enabled = false;	
		
        nameLabel.text = "";
        nameLabel.enabled = false;
		
        cancelSprite.enabled = false;
        myBorder.enabled = false;
		
        backgroundLabel.enabled = true;
        myBackground.spriteName = backgroundSpriteName;
    }
	
    //Used to check where to display a new confirmed character
    public bool IsDisplayingCharacter()
    {
        if (myIcon.enabled)
            return true;
        else
            return false;

    }
	
	

	
}
