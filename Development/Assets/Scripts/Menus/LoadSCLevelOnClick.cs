using UnityEngine;
using System.Collections;

public class LoadSCLevelOnClick : MonoBehaviour {
		
	public ApplicationState.LevelNames levelName;
	public MenuButton.MenuType submenu = MenuButton.MenuType.None;
	
	public bool showLoadingScreen = false;
	public bool resetTimeScale = false;
	
	void OnClick ()
	{
		if (resetTimeScale)
			Time.timeScale = 1;
		
		if (showLoadingScreen)
		{
			ApplicationState.Instance.LoadLevelWithLoading(levelName, submenu);
		}
		else
		{
			ApplicationState.Instance.LoadLevel(levelName, submenu);
		}
	}
}
