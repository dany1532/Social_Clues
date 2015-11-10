using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomLevel : MonoBehaviour
{
	
    public enum LevelSelect
    {
        NONE,
        CAFETERIA,
        BEDROOM,
        CLASSROOM}
    ;
    public enum ModeSelect
    {
        LEVELSELECT,
        CHARACTERSELECT,
        NONE}
    ;
	
    public int levelSelect;
    public ModeSelect modeSelect;
	
    public GameObject levelSetUp;
    public GameObject characterSetUp;
	
    public CharacterSelectionTable charactersTable;
	public ConfirmCharacterButton confirmButton;
    public List<CharacterSelected_Button> chosenCharacters;
	
    public GameObject createButton;
    public GameObject returnButton;

    List<LevelCharactersInfo.Character> customCharacterList;
    private List<int> customCharacterPositions;
    public LevelCreationInfo currentLevelInfo;
	
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
		
        GameObject loadManager = GameObject.Find("LoadGame_Manager");
        if (loadManager != null)
            Destroy(loadManager);
    }
	
    // Use this for initialization
    void Start()
    {		
        //Start in the level select window
        levelSelect = 0;
        modeSelect = ModeSelect.LEVELSELECT;
        SetVisibleLevelSelect(true);
    }
		
    //Return to the Level Select window
    public void ReturnToLevelSelection()
    {
        if (returnButton != null)
        {
            ResetCharacterSelection();
            createButton.SetActive(false);
            SetVisibleLevelSelect(true);
            levelSelect = 0;
            currentLevelInfo = null;
            modeSelect = ModeSelect.LEVELSELECT;
        }
    }
	
    //Return to MainMenu
    public void ReturnToMainMenu()
    {
        if (ApplicationState.Instance.previousLevel == ApplicationState.LevelNames.MAIN_MENU)
            ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.MAIN_MENU, MenuButton.MenuType.UserMenu);
        else
            ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.CHILDS_MENU, MenuButton.MenuType.Adventure);
        
        Destroy(this.gameObject);
    }
	
	//Return to MainMenu
	public void ReturnToHomeMenu()
	{
		if (ApplicationState.Instance.previousLevel == ApplicationState.LevelNames.MAIN_MENU)
			ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.MAIN_MENU);
		else
			ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.CHILDS_MENU);
		
		Destroy(this.gameObject);
	}

    //Destroy character table
    void ResetCharacterSelection()
    {
        charactersTable.destroyTable();	
		confirmButton.Default();
    }
	
    //Loads the appropiate level
    public void CreateLevel()
    {
		
        //the selected level is the cafeteria
        if (currentLevelInfo != null)
        {
            levelSetUp = null;
            characterSetUp = null;
            createButton = null;
            CreateCustomCharacterList();
            ApplicationState.Instance.LoadLevelWithLoading(currentLevelInfo.levelName);	
        }
    }
	
    //Depending on the characters selected it will be added to the list
    void CreateCustomCharacterList()
    {
        customCharacterList = new List<LevelCharactersInfo.Character>();
		
        //Add the chosen characters and their respective positions
        for (int i = 0; i < chosenCharacters.Count; i++)
        {
            if (chosenCharacters [i].IsDisplayingCharacter())
            {
                customCharacterList.Add(chosenCharacters [i].charInfo);
            }
        }
		
        customCharacterList.Sort(new LevelCharactersInfo.CharacterComparer());

        int currentEmptySlot = 0;
        int emptySlots = 0;
        for (int slot = 0, maxSlots = customCharacterList.Count ; slot < maxSlots; slot++)
        {
            if (customCharacterList [slot].positionId > -1)
            {
                emptySlots |= (1 << customCharacterList [slot].positionId);
            } else
            {
                for (int availableSlot = currentEmptySlot; availableSlot < maxSlots; availableSlot++)
                {
                    if ((emptySlots & (1 << availableSlot)) == 0)
                    {
                        currentEmptySlot = availableSlot + 1;
						customCharacterList [slot].positionId = availableSlot;
						emptySlots |= (1 << availableSlot);
                        break;
                    }
                }
            }
        }
    }

    //Used for the NPCs script to get the character's that were chosen
    public List<LevelCharactersInfo.Character> GetCustomCharactersList()
    {
        return  customCharacterList;	
    }
	
    public List<int> GetCustomCharacterIndexPositions()
    {
        return customCharacterPositions;	
    }
	
    //Makes visible/invisible objects related to choosing the characters
    void SetVisibleLevelSelect(bool visible)
    {
        levelSetUp.SetActive(visible);
        characterSetUp.SetActive(!visible);
    }

    public void SetupCharacterSelection(LevelCreationInfo levelInfo)
    {
        currentLevelInfo = levelInfo;
        currentLevelInfo.LoadCharacters();
        
        SetVisibleLevelSelect(false);
        modeSelect = ModeSelect.CHARACTERSELECT;

        charactersTable.SetCharactersList(currentLevelInfo.levelCharacters);
        charactersTable.CreateTable(currentLevelInfo.levelCharacters);
    }

}
