using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The NPCs in the current level
/// </summary>
public class NPCs : Singleton<NPCs>
{
    //public List<NPC> nPCPrefabs;
    public List<LevelCharactersInfo.Character> npcNames;
    // List with all the NPCs
    public List<NPC> nPCs;
	public List<Transform> positions;
    private bool preloadToys = false;

	public enum ToyPositionIndex{
		NONE = -999,
		GENERAL = 0,
		FAR_LEFT = 1,
		FAR_RIGHT,
		NEAR_LEFT,
		NEAR_RIGHT,
		BOX = 16,
		SHELF = 32,
		WATER = 48,
		FLOOR = 64,
        CORNER = 80
	}
	[System.Serializable]
	public class ToyPosition{
		public ToyPositionIndex index = ToyPositionIndex.GENERAL;
		public Transform position;
		public bool used = false;
	}
	public List<ToyPosition> toyPositions;

    void Awake()
    {
     
        GameObject customLevel = GameObject.Find("Custom_Level");
        GameObject loadGameManager = GameObject.Find("LoadGame_Manager");
        string npcName;
     
        //if coming from the custom level, get the names of the characters selected
        if (customLevel != null)
        {
			preloadToys = true;
            npcNames = customLevel.GetComponent<CustomLevel>().GetCustomCharactersList();
            InstantiateCharacters(true);
            UpdateNPCStatus();

            Destroy(customLevel);
            return;
        }
     
        //if coming from loading previous gameplay, get the names of the characters and their status
        else if (loadGameManager != null || ApplicationState.Instance.previousLevel == ApplicationState.LevelNames.PLAY_ANALYTICS)
        {
            preloadToys = true;
            DBIncompleteLevel incompleteLevel = MainDatabase.Instance.getIncompleteLevel(ApplicationState.Instance.userID);

            if ((loadGameManager != null && loadGameManager.GetComponent<LoadGameManager>().HasLeveToLoad()) || ApplicationState.Instance.previousLevel == ApplicationState.LevelNames.PLAY_ANALYTICS)
            {
                npcNames = new List<LevelCharactersInfo.Character>();

                List<DBUserNPCStatus> npcStatusList;
                if (ApplicationState.Instance.previousLevel == ApplicationState.LevelNames.PLAY_ANALYTICS)
                    npcStatusList = MainDatabase.Instance.GetUserNPCStatus(ApplicationState.Instance.userID);
                else
                    npcStatusList = loadGameManager.GetComponent<LoadGameManager>().GetNPCs_Status();
         
                int levelID = MainDatabase.Instance.getIDs("SELECT LevelID FROM LevelPlay WHERE LevelPlayID ='" + incompleteLevel.LevelPlayID + "';");
                for (int i = 0; i < npcStatusList.Count; i++)
                {
                    if (npcStatusList [i].Status > -1)
                    {
                        DBNPCMiniGameInfo characterDBInfo = MainDatabase.Instance.getNPCInfo(npcStatusList [i].NPCID);
                 
                        if (characterDBInfo == null)
                            continue;
                        
                        LevelCharactersInfo.Character character = new LevelCharactersInfo.Character();
                        character.npcName = characterDBInfo.NPCName;
                        character.npcId = npcStatusList [i].NPCID;
                        character.positionId = MainDatabase.Instance.getIDs("SELECT PositionIndex FROM LevelNPCs WHERE NPCID = '" + character.npcId + "' AND LevelID = '" + levelID + "';");

                        npcNames.Add(character);
                    }
                }
                
                int currentEmptySlot = 0;
                int emptySlots = 0;
                for (int slot = 0, maxSlots = npcNames.Count; slot < maxSlots; slot++)
                {
                    if (npcNames [slot].positionId > -1)
                    {
                        emptySlots |= (1 << npcNames [slot].positionId);
                    } else
                    {
                        for (int availableSlot = currentEmptySlot; availableSlot < maxSlots; availableSlot++)
                        {
                            if ((emptySlots & (1 << availableSlot)) == 0)
                            {
                                currentEmptySlot = availableSlot + 1;
                                npcNames [slot].positionId = availableSlot;
                                break;
                            }
                        }
                    }
                }
                
                InstantiateCharacters(false);
                SetNPCStatus(npcStatusList);
                return;
            }
        }



        //Instantitate the default characters
        npcNames = MainDatabase.Instance.GetLevelsInfoExtended(MainDatabase.Instance.getLevelFromName(Application.loadedLevelName)).levelSpecificCharacters;
        InstantiateCharacters(false);
        UpdateNPCStatus();
    }
 
    /// <summary>
    /// Gets the number of npcs in level
    /// </summary>
    /// <returns>
    /// The number of npcs in level
    /// </returns>
    public int GetNPsNo()
    {
        return nPCs.Count;
    }
 
    /// <summary>
    /// Gets the NPCs' toy.
    /// </summary>
    /// <returns>
    /// The NPC toy.
    /// </returns>
    /// <param name='index'>
    /// NPC index
    /// </param>
    public Toy GetNPCToy(int index)
    {
        if (index >= 0 && index < nPCs.Count)
        {
            return new Toy(nPCs [index].missingToy.name, nPCs [index].missingToy.name, nPCs [index].foundToyColor);
        }
     
        return null;
    }
 
    //Load characters from the resource folder using their names
    public void InstantiateCharacters(bool dynamicPositioning)
    {
        string pathName;
        int count;

        //Delete all Assigned toys and so that we can randomly assigne toys in Instantiate when loading default characters
        //When loading from Analytics or saved game
        if (!preloadToys)
            MainDatabase.Instance.DeleteAllAssignedToy();

        for (count = 0; count < npcNames.Count; count++)
        {
         
         
            //Create path using npc name, create prefab and make it a child
            pathName = "NPCs/" + npcNames [count].npcName + "/" + npcNames [count].npcName;
            GameObject npcPrefab = (GameObject)Instantiate(Resources.Load(pathName, typeof(GameObject)));
         
            //if (dynamicPositioning)
            //{
            npcPrefab.transform.position = GetPositionAtIndex(npcNames [count].positionId);  
            //}
         
            npcPrefab.transform.parent = transform;
         
            //Add npc to the list
            NPC newNPC = npcPrefab.GetComponent<NPC>();
            nPCs.Add(newNPC);

         #if !UNITY_WEBPLAYER
            newNPC.npcDatabaseId = MainDatabase.Instance.getIDs("Select NPCID from NPC where NPCName ='" + npcNames [count].npcName + "';");

            if (!preloadToys)//When loading from Analytics or saved game
            {
                DBToyInfo newToy = MainDatabase.instance.GetRandomToy(ApplicationState.Instance.userID);
				MainDatabase.instance.InsertAssignedToy(ApplicationState.Instance.userID, newToy.ToyID, newToy.ToyName ,newNPC.npcDatabaseId);
                newNPC.missingToy.name = newToy.Filename;
                newNPC.foundToyColor = new Color(newToy.ToyColorR / 255.0f, newToy.ToyColorG / 255.0f, newToy.ToyColorB / 255.0f);
            } else
            {
                int toyID = MainDatabase.instance.getIDs("SELECT ToyID FROM ToyAssigned WHERE UserID = " + ApplicationState.instance.userID + " AND NPCID = " + newNPC.npcDatabaseId + ";");
                DBToyInfo newToy = MainDatabase.instance.getToyInfoByID(toyID);
                newNPC.missingToy.name = newToy.Filename;
                newNPC.foundToyColor = new Color(newToy.ToyColorR / 255.0f, newToy.ToyColorG / 255.0f, newToy.ToyColorB / 255.0f);
            }
         #endif
            if (newNPC.additionalToyObject)
            {
                newNPC.additionalToyObject.spriteName = newNPC.missingToy.name;
                newNPC.additionalToyObject.color = newNPC.foundToyColor;
                newNPC.additionalToyStretch = newNPC.additionalToyObject.gameObject.GetComponent<UIStretch>();
                if (newNPC.additionalToyStretch != null)
                    newNPC.additionalToyStretch.initialSize = new Vector2(newNPC.additionalToyObject.mInner.width, newNPC.additionalToyObject.mInner.height);
            }

            //Add the toys
            if (count < Markers.Instance.markers.Count)
            {
                Markers.Instance.ReplaceMarker(count, newNPC.missingToy.name, newNPC.foundToyColor);
            }
        }
     
        //something about the toy
        for (; count < Markers.Instance.markers.Count; count++)
            Markers.Instance.HideMarker(count);
    }
 
    //If Loaded NPC has completed status, set corresponding npc to completed state
    void SetNPCStatus(List<DBUserNPCStatus> npcStatusList)
    {
     
        for (int i = 0; i < npcStatusList.Count; i++)
        {
            //if NPC completed...
            if (npcStatusList [i].Status == 1)
                nPCs [i].NPCCompletedStatus();
        }
    }
 
    Vector3 GetPositionAtIndex(int index)
    {
        return positions [index].position;   
    }
 
    /// <summary>
    /// Enables ability for player to click on NPCs that have not completed their task
    /// </summary> 
    public void EnableInput()
    {
        foreach (NPC npc in nPCs)
        {
            if (npc.sprite != null)
                npc.sprite.collider.enabled = npc.interactingState != NPC.InteractingState.COMPLETED_TASK && npc.interactingState != NPC.InteractingState.INACTIVE;
        }
    }
 
    /// <summary>
    /// Disables ability for player to click on all the NPCs
    /// </summary>
    public void DisableInput()
    {
        foreach (NPC npc in nPCs)
        {
            if (npc.sprite != null)
                npc.sprite.collider.enabled = false;
        }
    }
 
    /// <summary>
    /// Gets the next prompt available dialogue prompt for NPC
    /// </summary>
    /// <returns>
    /// The next prompt.
    /// </returns>.
    public Dialogue GetNextPrompt()
    {
        int index = Random.Range(0, nPCs.Count);
     
        for (int i = 0; i < nPCs.Count; i++)
        {
            NPC npc = nPCs [(i + index) % nPCs.Count];
         
            if (npc.interactingState == NPC.InteractingState.WAITING_PLAYER && npc.prompt != null)
                return npc.prompt;
        }
     
        return null;
    }
 
    // Return the index of the npc in the NPC list
    public int GetNpcID(NPC seekingNPC)
    {
        int index = 0;
        foreach (NPC npc in nPCs)
        {
            if (npc == seekingNPC)
                return index;
            index++;
        }
     
        return -1;
    }
    
    // Return the index of the npc in the NPC list
    public int GetNpcID(int index)
    {
        if (index < nPCs.Count)
        {
            return MainDatabase.Instance.getIDs("Select NPCID from NPC where NPCName ='" + nPCs [index].NPCName() + "';");
        }

        return -1;
    }

    public int getNpcStatus(int index)
    {
        if (index < nPCs.Count)
            return (int)((nPCs [index].interactingState == NPC.InteractingState.COMPLETED_TASK) ? 1 : 0);
        return -1;
    }
    
    public bool hasCompletedNPCs()
    {
        for (int i= 0; i < nPCs.Count; i++)
        {
            if (nPCs [i].interactingState == NPC.InteractingState.COMPLETED_TASK)
                return true;
        }

        return false;
    }

    public void UpdateNPCStatus()
    {
        MainDatabase.instance.AddUserNPCList(ApplicationState.Instance.userID, GetNpcID(0), getNpcStatus(0), GetNpcID(1), getNpcStatus(1), GetNpcID(2), getNpcStatus(2), GetNpcID(3), getNpcStatus(3));
    }
	
	public Vector3 getToyPostion(ToyPositionIndex positionIndex)
	{
		if (positionIndex > ToyPositionIndex.GENERAL) {
			for (int counter = 0; counter < toyPositions.Count; counter++) {
				if (toyPositions [counter].index == positionIndex && toyPositions [counter].used == false) {
					toyPositions [counter].used = true;
					return toyPositions [counter].position.position;
				}
			}
		}

		return getRandomToyPostion();
	}

	public Vector3 getRandomToyPostion()
	{
		int index = -1;
		while (index == -1) {
			index = Random.Range(0, toyPositions.Count);
			if (toyPositions[index].index > ToyPositionIndex.GENERAL || toyPositions[index].used == true)
				index = -1;
		}
		toyPositions [index].used = true;
		return toyPositions [index].position.position;
	}
}
