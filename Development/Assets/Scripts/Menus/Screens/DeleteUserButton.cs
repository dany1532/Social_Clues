using UnityEngine;
using System.Collections;

public class DeleteUserButton : MonoBehaviour {
	
	public LoginMenu loginMenu;
	public LoginMenuOption user;
	
	void OnClick()
	{
#if !UNITY_WEBPLAYER
		MainDatabase.Instance.DeleteUser(user.id);
#endif
		// Reload users
		loginMenu.LoadUsers();
	}
	
	public void Hide ()
	{
		this.gameObject.SetActive(false);
	}
	
	public void Show ()
	{
		if (user.id > 0)
			this.gameObject.SetActive(true);
	}
}
