using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NPC Controller to be assigned to every NPC in the game
/// </summary>
public class NPC : MonoBehaviour
{
	
	#region Conversation
    // Intro for the player from Sherlock
    public bool showedIntro = false;
    public Dialogue intro;
    public Dialogue prompt;
	
    // Conversation tree for current NPC
    public ConversationTree conversationTree;
    public int conversationParts = 5;
	#endregion
	
    // Initial and final portrait of the NPC
    public Texture portrait;
    public Texture finalSprite1;
    public Texture finalSprite2;
	
    //found and missing toy
    public Texture missingToy;
	public UISprite additionalToyObject;
    public UIStretch additionalToyStretch;
    public Transform toyPosition;
    public Vector3 toyWorldPosition;
	public NPCs.ToyPositionIndex toyPositionIndex = NPCs.ToyPositionIndex.NONE;
    public Color foundToyColor = Color.white;
	public float npcHeight = 1;
    public float playerHeight = 1;
    public float eyeContactArrowLength = 1;
    public float eyeContactAngleOffset = 0;
    // Interaction state of the NPC, related to the player
    public enum InteractingState
    {
        INACTIVE = -1, // NPC is inactive, and not available to the player
        WAITING_PLAYER = 0, // NPC is wating for the player to interact
        INTERACTING_WITH_PLAYER = 2,
        TALKING_PLAYER = 8, // NPC is interacting with the player
        COMPLETED_TASK = 16 // NPC has completed talking with player
    }
    public InteractingState interactingState = InteractingState.WAITING_PLAYER;
	
    //public bool includePreDialogueMinigame = true;
    public enum IncludePreDialogueMinigame
    {
        NONE = -1,
        EYE_CONTACT = 0,
        PLACEMENT_CONTACT = 1,
        EYE_AND_PLACEMENT_CONTACT = 2
    }
    public IncludePreDialogueMinigame includePreDialogueMinigame = IncludePreDialogueMinigame.NONE;
	public Dialogue preDialogueMinigameSetup;
	
	#region Display data
    //Owner's dialogue bubble color
    public Color npc1Color; 
    //Companion's dialogue bubble color
    public Color npc2Color; 
    //Companion's dialogue bubble color
    public Color npc3Color;
    // The static sprite of the NPC
    public UITexture sprite;
    public UITexture sprite2;
    // The texures the NPC will go through during his / her conersation with the player
    public List<Texture> sprites;
    //The textures the NPC will go through during the waiting event
    public List<Texture> animationSprites;
    // Current texture
    public int currentSprite = -1;
    // Glowing outline of the NPC
    public GameObject glow;
    // The texturethe player has at the end of the dialogue with the NPC
    public Texture playerRewardPortrait;
	public float celebratingTime = 10;
    public CutScene cutScene;
	#endregion
    // Event to be triggered when player is close to the NPC the first time
    public SCEvent nearPlayerEvent;
	
    // The minigame associated with this NPC
    public string miniGameName;
    public Transform minigameLocation;
    [System.NonSerializedAttribute]
    public Minigame
        miniGame;
    public bool walkToMinigame = false;
	
    //InteractionID for Database:Interaction(table). New Interaction for every new or replayed interaction
    public int interactionID;
	
    private bool loadedMinigame = false;
	public NPCDialogueAnimation npcAnimations;
	public NPCDialogueAnimation	sideNPCAnimations;

	public int npcDatabaseId = -1;

    // Use this for initialization
    void Start()
    {
        // Update to the first sprite
        //UpdateToNextSprite();
		
        // If the NPC is waiting for the player to interact
        if (interactingState == InteractingState.WAITING_PLAYER)
			// Then make the NPC glow
            Glow();

		if (toyPositionIndex == NPCs.ToyPositionIndex.NONE) {
				toyWorldPosition = GameManager.Instance.mainCamera.ViewportToWorldPoint (Vector3.zero);
		} else {
			toyWorldPosition = NPCs.Instance.getToyPostion (toyPositionIndex);
			toyPosition.position = toyWorldPosition;
			toyPosition.gameObject.SetActive(false);
		}
	}
	
    // Update is called once per frame
    public void StartInteraction()
    {
        // If NPC is waiting for the player to interact
        if (interactingState == InteractingState.WAITING_PLAYER)
        {
            if (Player.instance.talkToNPC)
            {
                // and it's the first time he has been close to the NPC
                if (Player.instance.interactionState != Player.InteractionState.NEAR_NPC)
                {
                    GameManager.Instance.CancelUserPrompt();
                    Player.instance.talkToNPC = false;
                    // Hide Sherlocks' dialogue window
                    Sherlock.Instance.StartCoroutine(Sherlock.Instance.SetDialogue(null));
                    // Notify player that is near the NPC
                    Player.instance.interactionState = Player.InteractionState.NEAR_NPC;
					
                    Player.instance.SetCutscene(true);
					
                    Player.instance.interactingNPC = this;
                    // Enable the input on the NPCs
                    NPCs.Instance.EnableInput();
								
                    GameObject cutSceneGO = null;	
                    // If there is an event to be triggered when player is near the NPC
                    if (nearPlayerEvent != null)
                    {
                        // Load cut scene prefab and instantiate it
                        GameObject cutScenePrefab = ResourceManager.LoadNPCCutScene(NPCName());
                        if (cutScenePrefab != null)
                        {
                            cutSceneGO = Instantiate(cutScenePrefab) as GameObject;
                            cutSceneGO.transform.parent = GameManager.Instance.UIPanel;
                            cutScene = cutSceneGO.GetComponent<CutScene>();
							
                            // Update references in cut scene to NPC
                            nearPlayerEvent.ClearObjectsToNotify();
                            nearPlayerEvent.AddObjectToNotify(cutSceneGO);
							
                            foreach (SCEvent cutSceneEvent in cutSceneGO.GetComponentsInChildren<SCEvent>(true))
                            {
                                List<GameObject> gameObjects = cutSceneEvent.gameObjectsToNotify;
                                for (int i = 0, count = gameObjects.Count; i < count; i++)
                                    if (gameObjects [i] == null)
                                    {
                                        gameObjects [i] = this.gameObject;
                                    }
                            }
							
                            // Trigger it
                            nearPlayerEvent.TriggerEvent(true);
                        } else
                        {
                            Debug.LogWarning("No cut scene prefab found for " + NPCName());
                        }
                    }
					
                    // Load conversation prefab and instantiate it
                    GameObject conversationPrefab = ResourceManager.LoadNPCConversation(NPCName());
                    if (conversationPrefab != null)
                    {
                        GameObject conversationRoot = Instantiate(conversationPrefab) as GameObject;
                        conversationRoot.transform.parent = this.transform;
                        conversationTree = conversationRoot.GetComponent<ConversationTree>();
						
                        // Update references in conversation events to NPC
                        foreach (SCEvent conversationEvent in conversationRoot.GetComponentsInChildren<SCEvent>(true))
                        {
                            int nullGO = 0;
                            List<GameObject> gameObjects = conversationEvent.gameObjectsToNotify;
                            for (int i = 0, count = gameObjects.Count; i < count; i++)
                                if (gameObjects [i] == null)
                                {
                                    nullGO++;
                                    if (nullGO == 2)
                                        gameObjects [i] = cutSceneGO;
                                    else
                                        gameObjects [i] = this.gameObject;
                                }
                        }

						GameObject voiceOversPrefab = ResourceManager.LoadNPCVoiceOver(NPCName(), ApplicationState.Instance.selectedCharacter);
						DialogueVoiceOver voiceOver =  (DialogueVoiceOver) ((voiceOversPrefab != null) ? voiceOversPrefab.GetComponent<DialogueVoiceOver>() : null);
						// Upate owner of each dialogue to NPC
                        foreach (Dialogue dialogue in conversationRoot.GetComponentsInChildren<Dialogue>(true))
                        {
                            dialogue.owner = this;

							if (voiceOver != null && dialogue.voiceOverID != DialogueVoiceOver.VoiceOverID.NATIVE && dialogue.voiceOverID != DialogueVoiceOver.VoiceOverID.NONE)
							{
								dialogue.voiceOver = voiceOver.RetrieveVoiceOverAudio(dialogue.voiceOverID);
							}
                        }

						foreach (Reply reply in conversationRoot.GetComponentsInChildren<Reply>(true))
						{							
							if (voiceOver != null && reply.voiceOverID != DialogueVoiceOver.VoiceOverID.NATIVE && reply.voiceOverID != DialogueVoiceOver.VoiceOverID.NONE)
							{
								reply.voiceOver = voiceOver.RetrieveVoiceOverAudio(reply.voiceOverID);
							}
						}

						voiceOversPrefab = null;
						Resources.UnloadUnusedAssets();
                    } else
                    {
                        Debug.LogWarning("No conversation prefab found for " + NPCName());
                    }
                }
            }
			// else if the player is not close to the NPC but the player was interacting with the NPC before
			else if (Player.instance.interactingNPC == this)
            {
                // Notify plyer that he is away from the NPC
                Player.instance.interactionState = Player.InteractionState.NONE;
                Player.instance.interactingNPC = null;
                NPCs.Instance.EnableInput();
                //Player.instance.SetGoToCharacter(true);
                Player.instance.Navigate(transform.position, false);
            } else
            {
                //Player.instance.interactionBubble.IgnoreNPC();
                //Player.instance.SetGoToCharacter(true);
                Player.instance.SetInteractiveBubbleNPC(this);
                Player.instance.Navigate(transform.position, false);
            }
        } else
        {
            if (interactingState == InteractingState.INTERACTING_WITH_PLAYER || interactingState == InteractingState.TALKING_PLAYER)
            {
                //if (Player.instance.PlayerDistance(transform.position) < 3f)
                if (Player.instance.GetDistance(this.transform.position) < 0.02f)
                {
					// Then talk with the player
                    Player.instance.TalkNPC(this);
                }
                else
                {
                    //Player.instance.SetGoToCharacter(true);
                    Player.instance.Navigate(transform.position, false);
                }
            }
        }
    }

	#region Dialogue
    /// <summary>
    /// Have Sherlock introduce the character to the player
    /// </summary>
    public void PlayIntro()
    {
        if (!showedIntro)
        {
            //showedIntro = true;
			
            if (intro != null)
                Sherlock.Instance.PlaySequenceInstructions(intro, null);
        }
    }
	
    /// <summary>
    /// Returns the NPCs Name
    /// </summary>
    /// <returns>
    /// The NPCs name
    /// </returns>
    public string NPCName()
    {
        if (this.name.Contains("(Clone)"))
            return this.name.Substring(0, this.name.Length - 7);
        else
            return this.name;
    }

    public Dialogue GetConversationRoot()
    {
        return conversationTree.root;
    }
	
    /// <summary>
    /// Starts the conversation with the player
    /// </summary>
    public void StartConversation()
    {
        if (conversationTree.root == conversationTree.currentNode)
        {
            DialogueWindow.instance.progressBar.ClearStars(conversationParts);
            DialogueWindow.instance.UpdateNPCArrow(conversationTree.currentNode);
            DialogueWindow.instance.UpdateNPCPortrait(conversationTree.currentNode);
					
            #if !UNITY_WEBPLAYER
            if (npcDatabaseId != -1)
            {
                MainDatabase.Instance.InsertSql("INSERT INTO Interaction (NPCID,LevelPlayID,Completed)VALUES(" + npcDatabaseId + "," + GameManager.Instance.LevelPlayID + ",'False');");
                interactionID = MainDatabase.Instance.getIDs("Select Max(InteractionID) from Interaction;");
                MainDatabase.Instance.InsertSql("INSERT INTO InteractionTime (InteractionID,StartTime)VALUES(" + interactionID + ",datetime('now','localtime'));");
            }
            #endif
			
        }
		
        // Set NPC interaction state to talking with the player
        interactingState = InteractingState.TALKING_PLAYER;
        // Set dialogue to current conversation node
        SetDialogue(conversationTree.currentNode);
        // Disable input to all other NPCs
        NPCs.Instance.DisableInput();

    }

    public void ResetState()
    {
        interactingState = InteractingState.WAITING_PLAYER;
        NPCs.Instance.EnableInput();
        Glow();
    }
	
    public void StartEvent()
    {
        interactingState = InteractingState.INTERACTING_WITH_PLAYER;
        NPCs.Instance.DisableInput();
    }
	
    /// <summary>
    /// Sets current dialogue in conversation tree
    /// </summary>
    /// <param name='dialogueReply'>
    /// Dialogue to be used for conversation
    /// </param>
    public void SetDialogue(Dialogue dialogueReply)
    {
        // If a dialogue was given in the function
        if (dialogueReply != null)
        {
            // then start the conversation with given dialogue
            conversationTree.ShowCurrentDialogue(dialogueReply);
        } else
        {
            // Otherwise hide dialogue window
            DialogueWindow.instance.ShowWindow(null);
            // Set state to waiting player
            interactingState = InteractingState.WAITING_PLAYER;
        }
    }

    /// <summary>
    /// Sets current dialogue in conversation tree
    /// </summary>
    /// <param name='dialogueReply'>
    /// Dialogue to be used for conversation
    /// </param>
    public void SetDialogue(Sentence dialogueReply)
    {
        // If a dialogue was given in the function
        if (dialogueReply != null)
        {
            // then start the conversation with given dialogue
            conversationTree.ShowCurrentDialogue(dialogueReply);
        } else
        {
            // Otherwise hide dialogue window
            DialogueWindow.instance.ShowWindow(null);
            // Set state to waiting player
            interactingState = InteractingState.WAITING_PLAYER;
        }
    }
	#endregion
	
	#region Sprite Managing
    /// <summary>
    /// Updates the texture of the NPC to the next one
    /// </summary>
    /// <returns>
    /// The sprite with the texture of the NPC
    /// </returns>
    public Texture UpdateToNextSprite()
    {
        // Increase the current texture
        currentSprite++;
		
        // If the current sprite is within the available textures
        if (currentSprite >= 0 && currentSprite < sprites.Count)
        {
            // and the static sprite is set
            if (sprite != null)
            {
                sprite.mainTexture = sprites [currentSprite];
                // Remove glowing effect around NPC
                RemoveGlow();
            }
        } else
        {
            Debug.Log(string.Format("Sprite for {0} out of range", gameObject.name));
            currentSprite--;
        }
		
        return sprites [currentSprite];
    }

    public void UpdateToSprite(Texture npc1Portrait)
    {
		if (npcAnimations != null)
			npcAnimations.StopAnimation();

        sprite.mainTexture = npc1Portrait;
		
        // Remove glowing effect around NPC
        RemoveGlow();
    }

	public void UpdateAnimation(List<Texture> animationList, bool primaryNPC = true)
	{
		if (primaryNPC) {
				if (npcAnimations != null) {
						npcAnimations.StopAnimation ();
						npcAnimations.SetAnimationList (animationList);
						npcAnimations.PlayAnimation ();

						// Remove glowing effect around NPC
						RemoveGlow ();
				} else if (animationList != null && animationList.Count > 0) {
						UpdateToSprite (animationList [0]);
				}
		} else {
			if (sideNPCAnimations != null) {
				sideNPCAnimations.StopAnimation ();
				sideNPCAnimations.SetAnimationList (animationList);
				sideNPCAnimations.PlayAnimation ();
			}
		}
	}
	
	public Texture GetSprite()
    {
        return sprites [currentSprite];
    }
	
    /// <summary>
    /// Gets the current sprite of the NPC
    /// </summary>
    /// <returns>
    /// The current sprite.
    /// </returns>
    public Texture GetCurrentSprite()
    {
        if (conversationTree != null)
        {
            if (conversationTree.currentNode.npc1Portrait != null)
                return conversationTree.currentNode.npc1Portrait;
        }
		
        return sprite.mainTexture;
    }
	
    /// <summary>
    /// Start glowing effect around NPC
    /// </summary>
    public void Glow()
    {
        // Activate glowing gameobject
        if (glow)
            glow.SetActive(true);
    }
	
    /// <summary>
    /// Removes glowing effect around NPC
    /// </summary>
    public void RemoveGlow()
    {
        // Deactivate glowing gameobject
        //if (glow)
        glow.SetActive(false);
    }
	#endregion
	
	#region Events
    /// <summary>
    /// Handle Event: Cut Scene Return
    /// </summary>
    public void CutSceneReturn()
    {
        // Switch the NPC sprite to the next one
        //UpdateToNextSprite();
        // Start glowing effect around NPC
        Glow();
        // Start Pre dialgue minigame
        PreDialogueMinigamesManager.Instance.PreDialogueMinigameStart(this);
		
        cutScene.Hide();
    }
	
    /// <summary>
    /// Handle Event: CutSceneReplay
    /// </summary>
    public void CutSceneReplay()
    {
        // Go to the next dialoge (show the question again)
		conversationTree.ContinueToNextDialogue();
        // Start talking with the player again
        Player.instance.TalkNPC(this, true);
        cutScene.Hide();
    }
	
    /// <summary>
    /// Handle Event: PreLoadMinigame
    /// </summary>
    public void PreLoadMinigame(Dialogue nextDialogue = null)
    {		
        Application.LoadLevelAdditive(miniGameName);
		
        // Hide the dialogue window
        DialogueWindow.instance.ShowWindow(nextDialogue);
    }
	
    public void PreLoadMinigame_Timer()
    {
        Application.LoadLevelAdditive(miniGameName);
    }
	
    /// <summary>
    /// Handle Event: LoadMinigame.
    /// </summary>
    public void LoadMinigame()
    {
        if (minigameLocation != null && loadedMinigame == false)
        {
            loadedMinigame = true;
            //conversationTree.ContinueToNextDialogue();
            // Tell player to move to target location for the event
            Player.instance.MoveToEvent(minigameLocation);
            DialogueWindow.instance.ShowWindow(null);
        } else
        {
            StartMinigame();
			
            if (!loadedMinigame)
                DialogueWindow.instance.ShowWindow(null);
        }
    }
	
    public void StartMinigame()
	{
		//conversationTree.ContinueToNextDialogue();
		#if !UNITY_WEBPLAYER
        MainDatabase.Instance.UpdateSql("UPDATE InteractionTime SET EndTime=datetime('now','localtime') WHERE InteractionID=" + interactionID + ";");
        #endif		
        miniGame.gameObject.SetActive(true);
		
        // Set current minigame in level to the minigame of the crying girl
        GameManager.Instance.minigame = miniGame;
        // Start the minigame
        GameManager.Instance.ShowMinigame(walkToMinigame);
    }

    /// <summary>
    /// Handle Event: OnQuestCompleted
    /// </summary>
    public void OnQuestCompleted()
    {
        if (miniGame != null)
        {
            ResourceManager.UnloadAsset(miniGame.gameObject);
            Destroy(miniGame.gameObject);
        }

        Destroy(conversationTree.gameObject);
        conversationTree = null;

        Destroy(cutScene.gameObject);
        cutScene = null;

        Resources.UnloadUnusedAssets();

		InputManager.Instance.ReceivedUIInput();
		Player.instance.PlayRewardPortrait(playerRewardPortrait, missingToy, foundToyColor, celebratingTime);
		
        //Add interaction Completed in interaction table
        //RecordAnswers("Emotions");
        //RecordAnswers("Comprehension");
        //RecordAnswers("ConversationStart");
        //RecordAnswers("MaintainConversation");
        //RecordAnswers("ProblemSolving");
        interactionID = MainDatabase.Instance.getIDs("Select Max(InteractionID) from Interaction;");
        MainDatabase.Instance.UpdateSql("UPDATE Interaction SET Completed='True' WHERE InteractionID=" + interactionID + ";");
		int NPCID = MainDatabase.Instance.getIDs("Select NPCID from Interaction where InteractionID=" + interactionID + ";");
        //changing the status of NPC to completed by assigning to 1
        MainDatabase.Instance.ChangeUserNPCStatus(ApplicationState.Instance.userID, NPCID, 1);
		//Adding toy to userToy table.
		DBToyInfo toyAchieved = MainDatabase.Instance.getToyInfoByInteractionID(interactionID);
		MainDatabase.Instance.InsertSql("INSERT INTO UserToy (UserID,ToyID,InteractionID) VALUES ('" + ApplicationState.Instance.userID + "','"+toyAchieved.ToyID+ "','"+interactionID+ "');");

        // Remove glowing effect around NPC
        RemoveGlow();
        Markers.Instance.OnQuestCompleted(toyWorldPosition, NPCs.Instance.GetNpcID(this));
        interactingState = InteractingState.COMPLETED_TASK;
        NPCs.Instance.UpdateNPCStatus();

        NPCs.Instance.EnableInput();
        GameManager.Instance.PrepareUserPrompt();
    }

    void RecordAnswers(string category)
    {
        int replyTimeTaken = 1;
        int commID = MainDatabase.Instance.getIDs("SELECT CommID FROM PointCommunication WHERE CommText ='" + category + "';");
        int questionID;
        int replyID;
		
        if (commID != -1)
        {
            questionID = MainDatabase.Instance.getIDs("SELECT QuestionID FROM Question WHERE CommID = " + commID + " AND NPCID = " + npcDatabaseId + ";");
            replyID = MainDatabase.Instance.getIDs("SELECT ReplyID FROM Reply WHERE QuestionID = " + questionID + " AND Points = 2;");
            MainDatabase.Instance.InsertSql("INSERT INTO Answer (ReplyID,ReplyType,ReplyTimeTaken,InteractionID)VALUES(" + replyID + ",1," + replyTimeTaken + ",'" + interactionID + "');");
            MainDatabase.Instance.InsertSql("INSERT INTO Answer (ReplyID,ReplyType,ReplyTimeTaken,InteractionID)VALUES(" + (replyID - 1) + ",1," + replyTimeTaken + ",'" + interactionID + "');");
            MainDatabase.Instance.InsertSql("INSERT INTO Answer (ReplyID,ReplyType,ReplyTimeTaken,InteractionID)VALUES(" + (replyID - 2) + ",1," + replyTimeTaken + ",'" + interactionID + "');");
            MainDatabase.Instance.InsertSql("INSERT INTO Answer (ReplyID,ReplyType,ReplyTimeTaken,InteractionID)VALUES(" + (replyID - 3) + ",1," + replyTimeTaken + ",'" + interactionID + "');");
        }
    }
	
    //Set NPC to completed state (used for when loading)
    public void NPCCompletedStatus()
    {
		
        if (miniGame != null)
            Destroy(miniGame.gameObject);
        if (conversationTree != null)
            Destroy(conversationTree.gameObject);
        if (cutScene != null)
            Destroy(cutScene.gameObject);
		
        RemoveGlow();
        Markers.Instance.DisplayNPCToyCompleted(NPCs.Instance.GetNpcID(this));
        interactingState = InteractingState.COMPLETED_TASK;
        if (finalSprite1 != null)
            sprite.mainTexture = finalSprite1;
        if (finalSprite2 != null)
            sprite2.mainTexture = finalSprite2;
        NPCs.Instance.EnableInput();
    }
	
    /// <summary>
    /// Handle Event: ProblemSolvingFailure
    /// </summary>
    /// <param name='dialogue'>
    /// Which dialogue should the NPC continue with the player
    /// </param>
    public void ProblemSolvingFailure(Dialogue dialogue)
    {
        // Clear NPC dialogue
        SetDialogue((Dialogue)null);
        // Set NPC conversation dialogue to the one given
        conversationTree.SetDialogue(dialogue);
        Player.instance.ResetState();
        NPCs.Instance.EnableInput();
    }
	
    /// <summary>
    /// Raises the dialogue complete event.
    /// Used mainly for Eddie
    /// </summary>
    public void OnDialogueComplete()
    {
        if (sprite2 != null)
            sprite2.mainTexture = finalSprite2;

		if (toyPositionIndex != NPCs.ToyPositionIndex.NONE) {
			toyPosition.gameObject.SetActive (true);
			conversationTree.ShowNextDialogue ();
			Player.instance.ResetState ();
		}
    }
	#endregion
}
