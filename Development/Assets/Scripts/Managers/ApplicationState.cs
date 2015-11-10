using UnityEngine;
using System.Collections;

public class ApplicationState : Singleton<ApplicationState>
{
    public static float currentBuildNo = 1.2f;
    public float buildNo;
	
    UILabel currentBuildLabel;
    public int loadingLevelIdx = -1;
    public int _userID = -1;
    public int userID
    {
        get { return _userID; }
        set
        {
            _userID = value;
            UserSettings.Instance.loadUserSettings();
        }
    }
	public string selectedCharacter = "Pete";
    public string loadingLevelName = string.Empty;
	
    public LevelNames currentLevel { get; private set; }
    public LevelNames previousLevel { get; private set; }
    public MenuButton.MenuType loadingSubmenu = MenuButton.MenuType.None;
	
    public HUDFPS fps;
    public bool showBuildLabel;
    public bool showFPS;
	public bool isStoryMode;
	public bool presentationBuild;
    
    public enum LevelNames
    {
        MAIN_MENU = 0,
        CHILDS_MENU,
        PLAY_ANALYTICS,
        LEVEL_COMPLETE,
        MINIGAME_COMPLETE,
		PARENT_ANALYTICS,
		ACHIEVEMENTS,
        LEVEL_SETUP = 512,
        MAIN_LEVELS = 1024,
		CAFETERIA = 1025,
		CLASSROOM = 1026,
		WORLDMAP = 1060, 
		BEDROOM = 1061
    }
	
    private string GetLevelName(LevelNames levelName)
    {
        switch (levelName)
        {
            case LevelNames.MAIN_MENU:
                return "Menu";
            case LevelNames.CHILDS_MENU:
                return "ChildMenu";
            case LevelNames.PLAY_ANALYTICS:
                return "PlayAnalytics";
            case LevelNames.LEVEL_COMPLETE:
                return "LevelComplete";
            case LevelNames.MINIGAME_COMPLETE:
                return "MinigameComplete";
            case LevelNames.LEVEL_SETUP:
                return "Level_SetUpScene";
            case LevelNames.MAIN_LEVELS:
                return "Cafeteria";
            case LevelNames.CAFETERIA:
                return "Cafeteria";
			case LevelNames.PARENT_ANALYTICS:
				return "AnalyticsHUB";
			case LevelNames.WORLDMAP:
				return "WorldMap";
			case LevelNames.BEDROOM:
				return "Bedroom";
			case LevelNames.CLASSROOM:
				return "Classroom";
			case LevelNames.ACHIEVEMENTS:
				return "Achievements";
		}
        return "Error";
    }

	public LevelNames GetLevelFromName (string levelToLoad)
	{
		
		switch (levelToLoad)
		{
			case "Menu":
				return LevelNames.MAIN_MENU;
			case "ChildMenu":
				return LevelNames.CHILDS_MENU;
			case "PlayAnalytics":
				return LevelNames.PLAY_ANALYTICS;
			case "LevelComplete":
				return LevelNames.LEVEL_COMPLETE;
			case "MinigameComplete":
				return LevelNames.MINIGAME_COMPLETE;
			case "Level_SetUpScene":
				return LevelNames.LEVEL_SETUP;
			case "Cafeteria":
				return LevelNames.CAFETERIA;
			case "AnalyticsHUB":
				return LevelNames.PARENT_ANALYTICS;
			case "WorldMap":
				return LevelNames.WORLDMAP;
			case "Bedroom":
				return LevelNames.BEDROOM;
			case "Classroom":
				return LevelNames.CLASSROOM;
			case "Achievements":
				return LevelNames.ACHIEVEMENTS;
		}
		return LevelNames.MAIN_MENU;
	}
	
	bool pause = false;
	
	// Use this for initialization
	void Awake()
	{
		currentBuildNo = buildNo;
		
		if (!showFPS && fps != null)
			Destroy(fps.gameObject);
		
		
		currentBuildLabel = GetComponentInChildren<UILabel>();
		
        if (!showBuildLabel && currentBuildLabel != null)
        {
            Destroy(currentBuildLabel.gameObject);
        }
		
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);	
        instance = this;
        currentLevel = previousLevel = LevelNames.MAIN_MENU;
    }
	
    void Start()
    {
        if (currentBuildLabel != null)
            currentBuildLabel.text = currentBuildNo.ToString() + " (" + MainDatabase.Instance.GetDatabaseVersion() + ")";
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
	
    #region Handle Game States
    /// <summary>
    /// Pause / Unpause game
    /// </summary>
    public void Pause()
    {
        pause = !pause;
        Time.timeScale = (pause) ? 0f : 1f;
        AudioManager.Instance.PauseFX(pause);

        if (pause)
        {
            #if !UNITY_WEBPLAYER
            if (GameManager.WasInitialized())
                MainDatabase.Instance.UpdateSql("UPDATE LevelPlayInstance SET EndTime=datetime('now','localtime') WHERE LevelPlayID=" + GameManager.Instance.LevelPlayID + ";");
            #endif
        } else
        {
            #if !UNITY_WEBPLAYER
            if (GameManager.WasInitialized())
                MainDatabase.Instance.InsertSql("INSERT INTO LevelPlayInstance (LevelPlayID,StartTime)VALUES(" + GameManager.Instance.LevelPlayID + ",datetime('now','localtime'));");
            #endif
        }
        
    }
    
    /// <summary>
    /// Returns if the game is paused or not
    /// </summary>
    /// <returns>
    /// Game is paused
    /// </returns>
    public bool isPaused()
    {
        return pause;
    }

    void OnApplicationPause()
    {
        //Pause();
    }

    void OnApplicationQuit()
    {
        if (GameManager.WasInitialized() && GameManager.Instance.LevelPlayID > 0)
        {
            MainDatabase.Instance.UpdateSql("UPDATE LevelPlayInstance SET EndTime=datetime('now','localtime') WHERE LevelPlayID=" + GameManager.Instance.LevelPlayID + ";");
            MainDatabase.Instance.DisconnectDB();
        }
    }
	#endregion
	
	#region Manage Levels

    public void ClearPreviousLevel()
    {
        previousLevel = currentLevel;
    }

    public void SetNextSubmenu(MenuButton.MenuType submenu)
    {
        loadingSubmenu = submenu;
    }
	
	public void SetCurrent(LevelNames newLevel)
	{
		currentLevel = newLevel;
	}
	

    public void LoadLevel(LevelNames newLevel, MenuButton.MenuType submenu = MenuButton.MenuType.None)
    {
        previousLevel = currentLevel;
        currentLevel = newLevel;
		
        loadingLevelIdx = -1;
        SetNextSubmenu(submenu);
		Application.LoadLevel(GetLevelName(newLevel));
    }
	
    public void LoadLevelWithLoading(int newLevel)
    {
        loadingLevelIdx = newLevel;
        Application.LoadLevel("Loading");
    }
	
    public void LoadLevelWithLoading(LevelNames newLevel, MenuButton.MenuType submenu = MenuButton.MenuType.None)
    {
        previousLevel = currentLevel;
        currentLevel = newLevel;
		
        SetNextSubmenu(submenu);
        loadingLevelIdx = -1;
        loadingLevelName = GetLevelName(newLevel);
        Application.LoadLevel("Loading");
    }
	
    public void LoadLevelWithLoading(string newLevel)
    {
        loadingLevelIdx = -1;
        loadingLevelName = newLevel;
        Application.LoadLevel("Loading");
    }
	
    public UnityEngine.AsyncOperation LoadLevelMain()
    {
#if UNITY_EDITOR
        if (UnityEditorInternal.InternalEditorUtility.HasPro())
        {
#else
		if (true)
		{
#endif
            if (loadingLevelIdx > 0)
                return Application.LoadLevelAsync(loadingLevelIdx);
            else
                return Application.LoadLevelAsync(loadingLevelName);
        } else
            return null;
    }
	
    public void LoadNextLevel()
    {
        if (loadingLevelIdx > -1)
            Application.LoadLevel(loadingLevelIdx);
        else
            Application.LoadLevel(loadingLevelName);
    }
	

    public void LoadLevelAdditive(LevelNames levelComplete)
    {
        Application.LoadLevelAdditive(GetLevelName(levelComplete));
    }

    public void LoadingLevel()
    {
        previousLevel = currentLevel;
        currentLevel = LevelNames.MAIN_LEVELS;
    }
	#endregion

    public bool InLevel()
    {
        return ((int)currentLevel >= (int)LevelNames.MAIN_LEVELS) || (Player.instance != null);
    }

	public bool InMinigame ()
	{
			return GameObject.FindObjectOfType (typeof(Minigame)) != null;
	}
}
