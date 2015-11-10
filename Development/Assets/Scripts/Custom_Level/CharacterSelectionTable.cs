//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Storage container that stores items.
/// </summary>

//[AddComponentMenu("NGUI/Examples/UI Item Storage")]
public class CharacterSelectionTable : MonoBehaviour
{

    /// <summary>
    /// Maximum number of rows to create.
    /// </summary>

    public int maxRows = 4;

    /// <summary>
    /// Maximum number of columns to create.
    /// </summary>

    public int maxColumns = 4;

    /// <summary>
    /// Template used to create inventory icons.
    /// </summary>

    public GameObject template;
	
    public List<string> cafeteriaCharactersNames;
    public List<string> cafeteriaCharactersSprites;
    public List<int>    cafeteriaExclusivePositions;
	
    public List<string> neutralCharactersNames;
    public List<string> neutralCharactersSprites;

    List<CharacterInformation> charInfoList;
	
    public CustomLevel customLevel;
    public ConfirmCharacterButton confirmButton;

    /// <summary>
    /// Background widget to scale after the item slots have been created.
    /// </summary>

    public UIWidget background;

    /// <summary>
    /// Spacing between icons.
    /// </summary>

    public int spacing = 128;

    /// <summary>
    /// Padding around the border.
    /// </summary>

    public int padding = 10;
	
    List<string> listCharacters;
    List<Character_Slot_Button> slots;

    UISprite lastBorder;
    void Start()
    {
        GenerateSlots();
    }

    void GenerateSlots()
    {
        if (slots != null)
            return;

        Bounds b = new Bounds();
        slots = new List<Character_Slot_Button>(); 

        for (int x = 0; x < maxColumns; ++x)
        {
            for (int y = 0; y < maxRows; ++y)
            {
                //Set it as a child of this object
                GameObject go = AddChild(gameObject, template);
                
                //Sets the postion of the slot
                Transform t = go.transform;
                t.localPosition = new Vector3(padding + (x + 0.5f) * spacing, -padding - (y + 0.5f) * (spacing + 15), 0f);
                
                //Get the script of the slot, to give info
                Character_Slot_Button char_button = go.GetComponent<Character_Slot_Button>();
                
                char_button.Set_EmptySprite();
                go.name = "Empty_Slot";
                slots.Add(char_button);
                
                b.Encapsulate(new Vector3(padding * 2f + (x + 1) * spacing, -padding * 2f - (y + 2) * spacing, 0f));                
            }//end x
        }//endy
        
        if (background != null)
            background.transform.localScale = b.size;
    }

    //Used to find and disable a slot on the table
    public void Disable_Slot(string nameCharacter)
    {
        Transform slot = this.transform.FindChild(nameCharacter);
        slot.gameObject.GetComponent<Character_Slot_Button>().Disable_CharacterSlot();
        lastBorder = null;
    }
	
    //Used to find and enable a slot on the table
    public void Enable_Slot(string nameCharacter)
    {
        Transform slot = this.transform.FindChild(nameCharacter);
        slot.gameObject.GetComponent<Character_Slot_Button>().Enable_CharacterSlot();
    }
	
    //Sets/Gets the character's names and respective sprites to be used for the slots
    public void SetCharactersList(LevelCharactersInfo characters)
    {
        List<string> charNames = new List<string>();
        List<string> charSprites = new List<string>();
        List<int> charIndexPosition = new List<int>();
		
        charInfoList = new List<CharacterInformation>();
		
        //Use the cafeteria list
        if (customLevel.levelSelect == 1)
        {
			
            for (int i = 0; i < cafeteriaCharactersNames.Count; i++)
            {
                charNames.Add(cafeteriaCharactersNames [i]);
                charSprites.Add(cafeteriaCharactersSprites [i]);
                charIndexPosition.Add(cafeteriaExclusivePositions [i]);
            }
        }
		
		//Another level list
		else
        {
            charNames = cafeteriaCharactersNames;
            charSprites = cafeteriaCharactersSprites;
        }
		
        //Add the names and sprites of the characters neutral of the level
        for (int i = 0; i < neutralCharactersNames.Count; i++)
        {
            charNames.Add(neutralCharactersNames [i]);
            charSprites.Add(neutralCharactersSprites [i]);
        }
		
        //Fills the character info to be given to the slots
        for (int i = 0; i < charNames.Count; i++)
        {
            CharacterInformation charInfo = new CharacterInformation();
            charInfo.name = charNames [i];
            charInfo.spriteName = charSprites [i];
			
            if (i < charIndexPosition.Count)
                charInfo.characterIndexPosition = charIndexPosition [i];
            else
                charInfo.characterIndexPosition = i + 1;
			
            charInfoList.Add(charInfo);
        }
    }
	
    //Used when returning to the level selection screen (Default state)
    public void destroyTable()
    {
        confirmButton.Default();
		
        foreach (Character_Slot_Button slot in slots)
        {
            slot.Set_EmptySprite();
            slot.gameObject.name = "Empty_Slot";
        }	
		
		
    }
	
    //Hacky way of switching between border selected slots
    public void SwitchSelectedSlot(UISprite sprite)
    {
        if (lastBorder != null && lastBorder != sprite)
        {
            lastBorder.enabled = false;
            lastBorder.gameObject.transform.parent.GetComponent<UIButtonScale>().enabled = true;
        }
		
        lastBorder = sprite;
    }
	
    //Creates an row X column of slots using the given characters
    public void CreateTable(LevelCharactersInfo characters)
    {
        int characterIndex = 0;
        Bounds b = new Bounds();

		List<LevelCharactersInfo.Character> activeCharacterList = characters.levelSpecificCharacters;
        GenerateSlots();

		//Deleting all assigned toys to generate random toys
		MainDatabase.Instance.DeleteAllAssignedToy();

        for (int counter = 0, length = slots.Count; counter < length; ++counter)
        {
            if (characterIndex < activeCharacterList.Count)
            {
                slots [counter].gameObject.name = activeCharacterList [characterIndex].npcName;
                slots [counter].SetInfo(activeCharacterList [characterIndex]);
                slots [counter].SetExclusiveCharacterBackground();
                slots [counter].SetConfirmButton(confirmButton);
				Debug.Log(activeCharacterList [characterIndex].npcName);
				
				//Assigning a random toy
				DBToyInfo newToy = MainDatabase.Instance.GetRandomToy(ApplicationState.Instance.userID);
				activeCharacterList [characterIndex].toyInfo = newToy;
				int npcID = MainDatabase.Instance.getIDs("Select NPCID from NPC where NPCName ='" +activeCharacterList [characterIndex].npcName+ "';");
				MainDatabase.Instance.InsertAssignedToy(ApplicationState.Instance.userID, newToy.ToyID, newToy.ToyName ,npcID);

                characterIndex++;

            } else
            {
                slots [counter].Set_EmptySprite();
                slots [counter].gameObject.name = "Empty_Slot";
            }

            if (counter == (maxRows - 1))
            {
                characterIndex = 0;
                activeCharacterList = characters.otherCharacters;
            }
        }//endy
    }
	
    public GameObject AddChild(GameObject parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = prefab.transform.localScale;
            go.layer = parent.layer;
        }
        return go;
    }
}