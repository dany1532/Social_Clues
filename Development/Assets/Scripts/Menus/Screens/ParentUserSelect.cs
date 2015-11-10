using UnityEngine;
using System.Collections;

public class ParentUserSelect : MonoBehaviour {
	
	private MenuController menuController;
	public LoginMenuOption SelectedChild;
	private LoginMenuOption userInfo;
	
	public void Awake()
	{
		userInfo = GetComponent<LoginMenuOption>();
		menuController = GameObject.FindObjectOfType(typeof(MenuController)) as MenuController;
	}
	
	/// <summary>
	/// Sets the user.
	/// </summary>
	/// <param name='userInfo'>
	/// User info.
	/// </param>
	public bool SetSelectedChild()
	{
		if (userInfo.id > 0)
		{
			ApplicationState.Instance.selectedCharacter = MainDatabase.Instance.GetUserCharacter(userInfo.id);

			ApplicationState.Instance.userID = userInfo.id;
			SelectedChild.SetUser(userInfo);
			return true;
		}
		
		return false;
	}
}
