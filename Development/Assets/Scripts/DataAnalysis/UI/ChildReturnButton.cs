using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildReturnButton : MonoBehaviour {
	DBIncompleteLevel dbIncompleteLevel;
	List<DBUserNPCStatus> statusNPCList;
	public bool displayWorldMap;
	
	// Use this for initialization
	void Start () {
		if (ApplicationState.Instance.previousLevel != ApplicationState.LevelNames.MAIN_MENU)
		{
			if (ApplicationState.Instance.previousLevel != ApplicationState.LevelNames.CHILDS_MENU)
			{
			  #if !UNITY_WEBPLAYER
				dbIncompleteLevel = MainDatabase.Instance.getIncompleteLevel(ApplicationState.Instance.userID);
				statusNPCList 	  = MainDatabase.Instance.GetUserNPCStatus(ApplicationState.Instance.userID);
			  #endif
				
				
				if(statusNPCList == null || IsLevelComplete()){
					if(ApplicationState.Instance.isStoryMode)
					{
						displayWorldMap = true;
					}
					else
					{
						if (transform.parent != null)
						{
							transform.parent.gameObject.SetActive(false);
						}
					}
				}
			}
			else
			{
				if (transform.parent != null)
					transform.parent.gameObject.SetActive(false);
			}
		}
	}
	
	void OnClick(){
		if (ApplicationState.Instance.previousLevel != ApplicationState.LevelNames.MAIN_MENU)
		{
			if(displayWorldMap){
				ApplicationState.Instance.LoadLevelWithLoading(ApplicationState.LevelNames.WORLDMAP);
			}
			else{
				LoadGame();
			}
		}
		else
		{
			ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.MAIN_MENU, MenuButton.MenuType.UserMenu);
		}
	}
	
	public void LoadGame(){
		ApplicationState.Instance.LoadingLevel();
		ApplicationState.Instance.LoadLevelWithLoading(dbIncompleteLevel.LevelName);	
	}
	
	public bool IsLevelComplete(){
		for(int i = 0; i < statusNPCList.Count; i++){
			if(statusNPCList[i].Status == 0)
				return false;
		}
		return true;
	}
	
	public bool IsFirstTimeUser(){
		if(dbIncompleteLevel.LevelPlayID == -1)
			return true;
		else
			return false;

	}
	

}
