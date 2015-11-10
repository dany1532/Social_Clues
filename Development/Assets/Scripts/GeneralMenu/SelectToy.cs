using UnityEngine;
using System.Collections;

public class SelectToy : MonoBehaviour {
	public int toyID = -1;
	
	public UISprite background;
	
	public UISprite toySprite;
	public UIStretch toyStretch;
	
	private StoreUserSetup userData;
	
	/// <summary>
	/// Load toy information
	/// </summary>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	/// <param name='toyName'>
	/// Toy name.
	/// </param>
	/// <param name='toyColor'>
	/// Toy color.
	/// </param>
	public void LoadToy(int id, string toyName, Color toyColor, StoreUserSetup user)
	{
		userData = user;
		
		if (id > 0)
		{
			toyID = id;
			
			toySprite.spriteName = toyName;
			toySprite.color = toyColor;
			
			toyStretch.initialSize = new Vector2(toySprite.innerUV.width, toySprite.innerUV.height);
			
			this.collider.enabled = true;
		}
		else
		{
			toyID = -1;
			toySprite.gameObject.SetActive(false);
			background.color = Color.gray;
			this.collider.enabled = false;
		}
	}
	
	/// <summary>
	/// Handle click event
	/// </summary>
	void OnClick ()
	{
		// Update selected toy for user and change background color
		userData.selectedToy = this;
		background.color = Color.green;
	}
	
	/// <summary>
	/// Unselect
	/// </summary>
	public void Unselect ()
	{
		// Update selected toy for user and change background color
		background.color = Color.white;
	}
}
