using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game manager: Holds the camera settings, active minigame and UI Settings
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// Camera settings
    /// </summary>
    class CameraSettings
    {
        // Camera position
        Vector3 position;
        // Camera rotation
        Quaternion rotation;
        // View port size
        float viewPortSize;
        // Near plane z
        float nearPlane;
        // Far plane z
        float farPlane;
		
        // Camera refernece
        Camera defaultCamera;
		
        /// <summary>
        /// Initializes a new instance of the <see cref="GameManager.CameraSettings"/> class from an existing camera
        /// </summary>
        /// <param name='newCamera'>
        /// Camera to be used to copy over the camera setings
        /// </param>
        public CameraSettings(Camera newCamera)
        {
            if (newCamera == null)
                return;
            // Copy over initial camera settings
            position = newCamera.transform.position;
            rotation = newCamera.transform.rotation;
            viewPortSize = newCamera.orthographicSize;
            nearPlane = newCamera.nearClipPlane;
            farPlane = newCamera.farClipPlane;
			
            // Keep reference ofcamera
            defaultCamera = newCamera;
        }
		
        /// <summary>
        /// Update camera to either default camera or settings of this camera
        /// </summary>
        /// <param name='newCamera'>
        /// A camera that its settings will be copied over
        /// </param>
        public void UpdateCamera(Camera newCamera)
        {
            // If a camera was passed as a parameter
            if (newCamera != null)
            {
                // update default camera to match the given camera settings
                defaultCamera.transform.position = newCamera.transform.position;
                defaultCamera.transform.rotation = newCamera.transform.rotation;
                defaultCamera.orthographicSize = newCamera.orthographicSize;
                defaultCamera.nearClipPlane = newCamera.nearClipPlane;
                defaultCamera.farClipPlane = newCamera.farClipPlane;
            } else // otherwise set the camera back to the default settings
            {
                defaultCamera.transform.position = position;
                defaultCamera.transform.rotation = rotation;
                defaultCamera.orthographicSize = viewPortSize;
                defaultCamera.nearClipPlane = nearPlane;
                defaultCamera.farClipPlane = farPlane;
            }
        }
    }
	
    // In game cameras and their camera settings
    // Main camera and settings
    public Camera mainCamera;
    CameraSettings mainCameraSettings;
    // UI camera and settings
    public Camera uiCamera;
    CameraSettings uiCameraSetings;
	
    // UI object and manager
    public Transform UIPanel;
    [System.NonSerialized]
    public GameObject
        OT;
	
    // Conversation tree for current level, used to introduce the level
    ConversationTree conversation;
	
    // Current minigame, if it's played
    public Minigame minigame;
	
    public CafeteriaIntroAnimation introAnim;
    public bool wantPlayIntro = false;
	
    // Background audio for current level
    public AudioClip backgroundAudio;
	
    // Whether to load the level from prefabs-
    public bool useLoadFromPrefabs = false;
	
    public int LevelPlayID = -1;
    // Prefabs to be used to load the scene
    [System.SerializableAttribute]
    public class LoadPrefab
    {
        public Transform transform;
        public GameObject prefab;
        public bool makeChild;
        public Vector3 scale = new Vector3(1, 1, 1);
        public bool forcePosition = false;
		
        public bool Load()
        {
            if (prefab != null)
            {
                if (this.makeChild)
                {
                    GameObject go = Instantiate(this.prefab) as GameObject;
                    go.transform.parent = this.transform;
					
                    if (!forcePosition)
                        go.transform.localPosition = Vector3.zero;
					
                    go.transform.localScale = this.scale;
                } else if (this.transform != null)
                {
                    GameObject go = Instantiate(this.prefab, this.transform.position, this.transform.rotation) as GameObject;
                    go.transform.localScale = this.scale;
                    Destroy(transform.gameObject);
                } else
                {
                    GameObject go = Instantiate(this.prefab) as GameObject;
                    go.transform.localScale = this.scale;
                }
				
                return true;
            }
			
            return false;
        }
    }
    public List<LoadPrefab> loadPrefabs;
	public Transform playerPosition;

	public Dialogue introPete;
	public Dialogue introKate;
	bool introLevel = true;

    void Awake()
    {
        instance = this;
		
        // Allow application to runInBackground, so it doesn't stop
        Application.runInBackground = true;
		
        // Load prefabs of objects in the beginning of the level
        if (useLoadFromPrefabs)
        {
            foreach (LoadPrefab loadPrefab in loadPrefabs)
            {
                loadPrefab.Load();
            }
        }
    }
	
    // Use this for initialization
    void Start()
    {
		
        // Make UI Camera child of the Main Camera
        //uiCamera.transform.parent = mainCamera.transform;
        //uiCamera.transform.localEulerAngles = Vector3.zero;
        //uiCamera.transform.localPosition = Vector3.zero;
		
        // Copy settings of in-game cameras
        mainCameraSettings = new CameraSettings(mainCamera);
        uiCameraSetings = new CameraSettings(uiCamera);
		
        // Start playing background music
        //if (backgroundAudio != null)
        //	AudioManager.Instance.PlayMusic(backgroundAudio, .1f);
        AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.musicVolume);
		
        // get the conversation tree for the level
        conversation = gameObject.GetComponent<ConversationTree>();
		if (Application.loadedLevelName == "Bedroom" && ApplicationState.instance.isStoryMode) {
				introLevel = false;
		}else{
			if (ApplicationState.instance.selectedCharacter == "Pete" && introPete != null)
					conversation.root = introPete;
			else if (ApplicationState.instance.selectedCharacter == "Kate" && introKate != null)
					conversation.root = introKate;
		}

        if (!wantPlayIntro)
        {
            PrepareIntro();
        }
#if !UNITY_WEBPLAYER
        string connectstring = MainDatabase.Instance.ConnectDB();
		
        GameObject loadManager = GameObject.Find("LoadGame_Manager");
		
        if (loadManager != null)
        {
            LevelPlayID = loadManager.GetComponent<LoadGameManager>().GetLevelPlayId();
            Destroy(loadManager);
        } else if (ApplicationState.Instance.previousLevel == ApplicationState.LevelNames.PLAY_ANALYTICS)
        {
            DBIncompleteLevel incompleteLevel = MainDatabase.Instance.getIncompleteLevel(ApplicationState.Instance.userID);
            if (incompleteLevel != null)
                LevelPlayID = incompleteLevel.LevelPlayID;
        }
		
        if (LevelPlayID == -1)
        {
			
			int levelID = MainDatabase.Instance.getIDs("Select LevelID from Level Where LevelName = '" + Application.loadedLevelName + "';");
            MainDatabase.Instance.InsertSql("INSERT INTO LevelPlay (LevelID,UserID)VALUES(" + levelID + "," + ApplicationState.Instance.userID + ");");
			
            LevelPlayID = MainDatabase.Instance.getIDs("Select Max(LevelPlayID) from LevelPlay;");
        }
		
        MainDatabase.Instance.InsertSql("INSERT INTO LevelPlayInstance (LevelPlayID,StartTime)VALUES(" + LevelPlayID + ",datetime('now','localtime'));");
		
        if (introAnim != null)
        {
            if (wantPlayIntro)
            {
                introAnim.gameObject.SetActive(true);
                introAnim.PrepareIntroAnimation();
                Invoke("PrepareIntro", 2f);
            } else
            {
                introAnim.gameObject.SetActive(false);	
            }
        }
#endif
    }
	
    public void PrepareIntro()
    {
        if (conversation != null && introLevel)
            Invoke("ShowInstructions", 2f);
        
        PrepareUserPrompt();
    }
    
    public void PrepareUserPrompt()
    {        
        GameObject bedroomManagerGO = GameObject.Find("BedroomManager");
        if (bedroomManagerGO == null || !ApplicationState.Instance.isStoryMode)
            Invoke("ShowPrompt", 35f);	
    }
	
    public void CancelUserPrompt()
    {
        CancelInvoke("ShowPrompt");
    }
    
    /// <summary>
    /// Starts the conversation for the level
    /// </summary>
    void ShowInstructions()
    {
        // Start the conversation
        //conversation.StartConversation();
        Sherlock.Instance.PlaySequenceInstructions(conversation.root, null);
    }
	
    /// <summary>
    /// Shows prompt for player to interact with NPCs
    /// </summary>
    void ShowPrompt()
    {
        Dialogue prompt;
        if (Player.instance != null && Player.instance.interactionState == Player.InteractionState.NONE && Player.instance.interactingNPC == null && Player.instance.cutscene == false && (prompt = NPCs.Instance.GetNextPrompt()) != null)
        {
            Sherlock.Instance.PlaySequenceInstructions(prompt, null);
        } else
        {
            PrepareUserPrompt();
        }
    }
	
    /// <summary>
    /// Update settings of the cameras to match the main camera or the new camera.
    /// </summary>
    /// <param name='newCamera'>
    /// A camera whose settings will be copied to the main camera
    /// </param>
    public void SwitchMainCamera(Camera newCamera)
    {
        // Update the main camera settings based on the new camera
        //mainCameraSettings.UpdateCamera(newCamera);
        // Fade the music back to the background music
        AudioManager.Instance.FadeMusic(backgroundAudio, .2f);
    }
	
    /// <summary>
    /// Update UI camera to match new camera, and update main camera to this settings
    /// </summary>
    /// <param name='newCamera'>
    /// A camera whose settings will be copied to the UI camera
    /// </param>
    public void SwitchUICamera(Camera newCamera)
    {
        // If no camera was passed in as a parameter
        //if (newCamera == null)
        // then just update to default settings of the main camera
        //	SwitchMainCamera(null);
		
        // Update main and UI camera to match new camera settings
        //uiCameraSetings.UpdateCamera(newCamera);
    }
	
    /// <summary>
    /// Switch camera to show minigame, and start the minigame
    /// </summary>
    public void ShowMinigame(bool returnToCauser = true)
    {
        // If minigame is set
        if (minigame != null)
        {
            // Update the UI camera transformation
            //uiCamera.transform.parent = null;
            //uiCamera.transform.position = Vector3.zero;
            //uiCamera.transform.rotation = Quaternion.identity;
			
            // Activate minigame gameobject if it's disabled
            minigame.gameObject.SetActive(true);
            // Start minigame
            minigame.StartMiniGame(returnToCauser);
        }
    }
	
    public bool InConversation()
    {
        return DialogueWindow.instance != null && DialogueWindow.instance.gameObject.activeSelf == true;
    }

    public bool InMinigame()
    {
        return (minigame != null && minigame.gameObject.activeSelf == true);
    }

    public static void DestroyGameLevel()
    {
        Sherlock.DestroyInstance();
        NPCs.DestroyInstance();
        PreDialogueMinigamesManager.DestroyInstance();
        Markers.DestroyInstance();
        HUD.DestroyInstance();

        Destroy(Player.instance);
        Player.instance = null;
    }

    public void CompleteLevel()
    {
        #if !UNITY_WEBPLAYER
        MainDatabase.Instance.UpdateSql("UPDATE LevelPlayInstance SET EndTime=datetime('now','localtime') WHERE LevelPlayID=" + GameManager.Instance.LevelPlayID + ";");
        #endif
        //mainCamera.gameObject.GetComponent<UICamera>().eventReceiverMask = -1;
        HUD.DestroyInstance();
        //Player player = Player.instance;
        //Player.instance = null;
        //Destroy (player.gameObject);
        ApplicationState.Instance.LoadLevelAdditive(ApplicationState.LevelNames.LEVEL_COMPLETE);
    }

    public void InitializeLevelCompleteSequence(LevelComplete levelComplete)
    {
        int npcNo = NPCs.Instance.GetNPsNo();
        levelComplete.ResetToys(npcNo);
		
        for (int i = 0; i < npcNo; i++)
            levelComplete.SetToy(NPCs.Instance.GetNPCToy(i));
    }
}
