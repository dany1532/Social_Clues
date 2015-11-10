using UnityEngine;
using System.Collections;

public class UserMenuParentLogin : MonoBehaviour {
	public LoginMenu loginMenu;
	public LoginMenuOption userMenuLoginOption;
	public MenuController menuController;
	
	// Use this for initialization
	void Start () {
	
		if (ApplicationState.Instance.previousLevel != ApplicationState.LevelNames.MAIN_MENU)
		{
			loginMenu.LoadUsers();
			userMenuLoginOption.SetUser(loginMenu.GetActiveUserInfo());
			ApplicationState.Instance.ClearPreviousLevel();
		}
	}
}
