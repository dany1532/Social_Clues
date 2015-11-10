using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadGameManager : MonoBehaviour
{
    DBIncompleteLevel dbIncompleteLevel;
    List<DBUserNPCStatus> statusNPCList;
    bool hasIncompleteLevel = false;
    public UISprite loadButton;

    void Awake()
    {
    }
    // Use this for initialization
    void Start()
    {
        #if !UNITY_WEBPLAYER
        dbIncompleteLevel = MainDatabase.Instance.getIncompleteLevel(ApplicationState.Instance.userID);
        statusNPCList = MainDatabase.Instance.GetUserNPCStatus(ApplicationState.Instance.userID);
        #endif
		
        hasIncompleteLevel = !IsLevelComplete();
    }
	
    public bool IsFirstTimeUser()
    {
        if (dbIncompleteLevel.LevelPlayID == -1)
            return true;
        else
            return false;

    }
	
    public bool IsLevelComplete()
    {
		if (statusNPCList == null) return false; 
        for (int i = 0; i < statusNPCList.Count; i++)
        {
            if (statusNPCList [i].Status == 0)
                return false;
        }
        return true;
    }
	
    public int GetLevelPlayId()
    {
        return dbIncompleteLevel.LevelPlayID;	
    }
	
    public List<DBUserNPCStatus> GetNPCs_Status()
    {
        return statusNPCList;	
    }
	
    public void LoadGame()
    {
		ApplicationState.Instance.isStoryMode = true;
        if (HasLeveToLoad())
        {
			
            DontDestroyOnLoad(this.gameObject);
            ApplicationState.Instance.LoadLevelWithLoading(dbIncompleteLevel.LevelName);
        } else
            NewGame();
    }
	
    public void LoadLevelSetUpScene()
    {
		ApplicationState.Instance.isStoryMode = false;
        ApplicationState.Instance.LoadLevelWithLoading("Level_SetUpScene");	
        Destroy(this.gameObject);
    }
	
    public void NewGame()
    {
		ApplicationState.Instance.isStoryMode = true;
        ApplicationState.Instance.LoadLevelWithLoading("GameIntroCutscene");
        Destroy(this.gameObject);
    }
	
    // Update is called once per frame
    public bool HasLeveToLoad()
    {
        return dbIncompleteLevel != null && dbIncompleteLevel.LevelPlayID > -1 && hasIncompleteLevel;
    }
}
