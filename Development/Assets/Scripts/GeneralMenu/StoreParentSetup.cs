using UnityEngine;
using System.Collections;

public class StoreParentSetup : MonoBehaviour {
	public MenuController menuController;
	
	public UIInput parentName;
	public UIInput parentPassword;
	public UIInput retypeParentPassword;
	public UIInput parentEmail;
	
	public void ResetData ()
	{
		parentName.text = "Parent's name";
		parentPassword.text = "password";
		retypeParentPassword.text = "password";
		parentEmail.text = "SocialCluesAnalytics@gmail.com";
	}
	
	void OnEnable()
	{
		Clear();
	}
	
	public void Clear ()
	{
		ResetData ();
	}
		
	/// <summary>
	/// Handle click event: save record and proceed to user menu
	/// </summary>
	void OnClick ()
	{
		ResetData ();
		menuController.enableMenu(MenuButton.MenuType.Users);
	}
	
	/// <summary>
	/// Save user data in database
	/// </summary>
	void SaveUserData()
	{
		
	}
}
