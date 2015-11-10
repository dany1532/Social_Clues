using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginMenu : MonoBehaviour {
	
	public List<LoginMenuOption> loginMenuOptions;

	// Use this for initialization
	void Start () {
		LoadUsers();
	}
	
	/// <summary>
	/// Load users from database
	/// </summary>
	public void LoadUsers()
	{
		// getting all existingUsers and loading them in the user selection menu
		MainDatabase.Instance.ConnectDB();
		List<DBUserInfo> ExistingUser = MainDatabase.Instance.getUserInfo();
		
		for (int i = 0; i < ExistingUser.Count; i++)
		{
			loginMenuOptions[i].SetUser(ExistingUser[i]);
		}
		
		for( int i = ExistingUser.Count ; i < loginMenuOptions.Count ; i++)
		{
			loginMenuOptions[i].Reset();
		}
	}

	public LoginMenuOption GetActiveUserInfo ()
	{
		foreach (LoginMenuOption user in loginMenuOptions)
		{
			if (user.id == ApplicationState.Instance.userID)
				return user;
		}
		return null;
	}
}
