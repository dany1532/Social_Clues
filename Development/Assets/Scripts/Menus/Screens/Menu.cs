using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	public MenuButton.MenuType id;
	private GameObject menu;
	public Color backgroundColorBack = Color.white;
	public Color backgroundColorFront = Color.white;
	public UITexture backgroundForeground;
	public Texture background;
	public MenuButton.MenuType backButton;
	public MenuButton.MenuType homeButton;
	public bool modalMenu = false;
	
	// Use this for initialization
	void Awake () {
		menu = this.gameObject;
	}
	
	/// <summary>
	/// Shw menu
	/// </summary>
	public void Show ()
	{
		menu.SetActive(true);
	}
	
	/// <summary>
	/// Hide menu
	/// </summary>
	public void Hide ()
	{
		menu.SetActive(false);
	}
}
