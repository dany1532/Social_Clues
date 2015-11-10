using UnityEngine;
using System.Collections;


//Script that handles the Character_Portrait buttons
public class Character_Slot_Button : MonoBehaviour
{
	
    public UISprite myIconSprite;
    public ConfirmCharacterButton confirmButton;
	
	
    UISprite myBorder;
    UISprite myCheck;
    UIButtonScale buttonScale;
	
    bool empty = false;
    bool selected = false;
	
    LevelCharactersInfo.Character myInfo;
	
    void Awake()
    {
        buttonScale = GetComponent<UIButtonScale>();
        myBorder = transform.FindChild("Border").GetComponent<UISprite>();
        myCheck = transform.FindChild("Check").GetComponent<UISprite>();	
    }
	
    void OnPress(bool pressed)
    {
		
        //If character can be selected, give info to the confirm button to display corresponding info of character
        if (pressed == false && !empty && !selected)
        {	
            confirmButton.Set_CurrentCharacter(myInfo);
            myBorder.enabled = true;
            GameObject.Find("CharacterSelection").GetComponent<CharacterSelectionTable>().SwitchSelectedSlot(myBorder);
            buttonScale.enabled = false;
        }
    }
	
    //Set the info given by the CharacterSelectionTable
    public void SetInfo(LevelCharactersInfo.Character info)
    {
        myInfo = info;
        if (info != null)
        {
            Enable_CharacterSlot();
        } else
        {
            Set_EmptySprite();
        }
    }
	
    public void SetExclusiveCharacterBackground()
    {
        transform.FindChild("Background").GetComponent<UISlicedSprite>().color = Color.yellow;	
    }
	
    //Set reference to the confirm button
    public void SetConfirmButton(ConfirmCharacterButton button)
    {
        confirmButton = button;	
    }
	
    //Character already selected, can't be choosen twice
    public void Disable_CharacterSlot()
    {
        myIconSprite.spriteName = myInfo.npcTexture + "_gray";
        myBorder.enabled = false;
        myCheck.enabled = true;
		
        buttonScale.enabled = false;
        selected = true;
    }
	
    //Character can be selected again
    public void Enable_CharacterSlot()
    {
		
        buttonScale.enabled = true;
        myIconSprite.spriteName = myInfo.npcTexture;
        myCheck.enabled = false;
        selected = false;
        empty = false;
    }
	
    public void EnableButtonScale()
    {
        buttonScale.enabled = true;	
    }
	
    //Just an empty slot
    public void Set_EmptySprite()
    {
        //myIconSprite.enabled = false;
        myIconSprite.spriteName = "Unknown_Character_Select_Icon";
        //transform.FindChild("Background").GetComponent<UISlicedSprite>().spriteName = "Unknown Character Select Icon";
        //transform.FindChild("Background").GetComponent<UISlicedSprite>().enabled = true;

        buttonScale.enabled = false;
		myBorder.enabled = false;
        empty = true;
    }
	
	
}
