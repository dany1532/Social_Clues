using UnityEngine;
using System.Collections;

public class LoginMenuOption : MonoBehaviour {
	
	public int id;
	public UILabel childName;
	public UISprite logo;
	public UIStretch logoStretch;
	public LoginMenuOption parentMenuOption;
	public LoginMenu menu;
	public DeleteUserButton deleteButton;
	
	public void Start()
	{
		if (deleteButton != null)
			deleteButton.Hide();
	}
	
	/// <summary>
	/// Sets the user information from database record
	/// </summary>
	/// <param name='userInfo'>
	/// User information from database
	/// </param>
	public void SetUser (DBUserInfo userInfo)
	{
		id = userInfo.UserID;
		
		// Update login screen info
		childName.text = userInfo.UserName;
	
	    logo.spriteName = userInfo.ToyFilename;
		logo.color = new Color(userInfo.ToyColorR/255.0f,(float)userInfo.ToyColorG/255.0f,(float)userInfo.ToyColorB/255.0f);
		logoStretch.initialSize = new Vector2(logo.mInner.width, logo.mInner.height);
		
		// Update parent screen info
		if (parentMenuOption != null)
		{
			parentMenuOption.SetUser(userInfo);
		}
	}
	
	/// <summary>
	/// Sets the user information from database record
	/// </summary>
	/// <param name='user'>
	/// User information from database
	/// </param>
	public void SetUser (LoginMenuOption userInfo)
	{
		if (userInfo != null)
		{
			id = userInfo.id;
			
			// Update login screen info
			childName.text = userInfo.childName.text;
		
		    logo.spriteName = userInfo.logo.spriteName;
			logo.color = userInfo.logo.color;
			logoStretch.initialSize = userInfo.logoStretch.initialSize;
			
			// Update parent screen info
			if (parentMenuOption != null)
			{
				parentMenuOption.SetUser(userInfo);
			}
		}
	}

	public void Reset ()
	{
		id = -1;
		
		// Update login screen info
		childName.text = "New Player";
	
	    logo.spriteName = "PlusSignEmpty";
		logo.color = Color.white;
		logoStretch.initialSize = new Vector2(logo.mInner.width, logo.mInner.height);
		
		// Update parent screen info
		if (parentMenuOption != null)
		{
			parentMenuOption.SetUser(this);
		}
	}
}
