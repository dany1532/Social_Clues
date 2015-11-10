using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Web;

public class MenuController : MonoBehaviour {
	public MenuButton.MenuType initialMenu = MenuButton.MenuType.Main;
	
	public List<Menu> menus;
	private Dictionary<MenuButton.MenuType, Menu> _menus;
	public MenuButton backButton;
	public MenuButton homeButton;
	public UITexture background;
	public Texture defaultTexture;
	private GameObject currentActiveBackground;
	
	public AudioClip backgroundMusic;
	public float musicVolume = 1f;
	
	private MenuButton.MenuType currentMenuIdx;
	private MenuButton.MenuType previousMenuIdx;
	private bool disabledColliders = false;
	
	// Use this for initialization
	void Awake () {
		Resources.UnloadUnusedAssets();
		previousMenuIdx = MenuButton.MenuType.None;
		_menus = new Dictionary<MenuButton.MenuType, Menu>();
		
		foreach(Menu menu in menus)
		{
			menu.gameObject.SetActive(true);
			_menus.Add(menu.id, menu);
		}
	}
	
	void Start()
	{
		// Enable just the main menu
		currentMenuIdx = MenuButton.MenuType.None;
		if (ApplicationState.Instance.loadingSubmenu == MenuButton.MenuType.None)
			enableMenu (initialMenu);
		else
		{
			enableMenu (ApplicationState.Instance.loadingSubmenu);
			ApplicationState.Instance.loadingSubmenu = MenuButton.MenuType.None;
		}
		
		if (backgroundMusic != null)
		{
			AudioManager.Instance.PlayMusic(backgroundMusic, musicVolume);
			//AudioManager.Instance.PlayBackgroundMusic(AudioManager.Instance.musicVolume);
		}
	}
	
	/// <summary>
	/// Disables all menus
	/// </summary>
	public void disableAll() {
		for(int i = 0; i < menus.Count; ++i)
		{
			if (menus[i].id != currentMenuIdx)
			{
				menus[i].gameObject.SetActive(true);
				menus[i].Hide();
			}
		}
	}
	
	/// <summary>
	/// Enable the back menu colliders.
	/// </summary>
	public void EnablePreviousMenuColliders()
	{
		if (disabledColliders == true && previousMenuIdx != MenuButton.MenuType.None)
		{
			Menu previousMenu = _menus[previousMenuIdx];
			foreach (Collider buttonCollider in previousMenu.GetComponentsInChildren<Collider>())
			{
				buttonCollider.enabled = true;
			}
		}
		disabledColliders = false;
	}
	
	/// <summary>
	/// Enable specific menu
	/// </summary>
	/// <param name='menuType'>
	/// Menu type
	/// </param>
	public void enableMenu(MenuButton.MenuType menuType, bool clearWindow = true) {
		int menuIdx;
		
		if (currentMenuIdx != MenuButton.MenuType.None && _menus[currentMenuIdx].modalMenu == true)
			EnablePreviousMenuColliders();
				
		previousMenuIdx = currentMenuIdx;
		currentMenuIdx = menuType;
		
		Menu currentMenu = _menus[menuType];
		currentMenu.Show();
		if (currentMenu.modalMenu)
		{
			Menu previousMenu = _menus[previousMenuIdx];
			foreach (Collider buttonCollider in previousMenu.GetComponentsInChildren<Collider>())
			{
				buttonCollider.enabled = false;
			}
			
			disabledColliders = true;
		}
		else
		{
			disableAll();
		}
		
		#region Upate Backgrounds
		// Hide previus overlay background
		if (currentActiveBackground != null)
			currentActiveBackground.SetActive(false);
		
		// If no full page background has been specified
		if (currentMenu.background == null)
		{
			// Update back to default texture (pixel) and color it
			background.mainTexture = defaultTexture;
			background.color = currentMenu.backgroundColorBack;
			
			// If there is an overlay background activate it and color it
			if (currentMenu.backgroundForeground != null)
			{
				currentMenu.backgroundForeground.gameObject.SetActive (true);
				currentMenu.backgroundForeground.color = currentMenu.backgroundColorFront;
				currentActiveBackground = currentMenu.backgroundForeground.gameObject;
			}
		}
		else // if full page background has been specified use just that
		{
			background.mainTexture = currentMenu.background;
			background.color = Color.white;
		}
		#endregion
		
		#region Upate Buttons
		// If a back button exists
		if (backButton != null)
		{
			// if the menu is not using it disable it
			if (currentMenu.backButton == MenuButton.MenuType.None)
				backButton.gameObject.SetActive(false);
			else // otherwise point it to the right menu
			{
				backButton.gameObject.SetActive(true);
				backButton.menuType = currentMenu.backButton;
			}
		}
		else
		{
			Debug.LogWarning("No back button");
		}
		
		// If a back button exists
		if (homeButton != null)
		{
			// if the menu is not using it disable it
			if (currentMenu.homeButton == MenuButton.MenuType.None)
				homeButton.gameObject.SetActive(false);
			else // otherwise point it to the right menu
			{
				homeButton.gameObject.SetActive(true);
				homeButton.menuType = currentMenu.homeButton;
			}
		}
		else
		{
			Debug.LogWarning("No back button");
		}
		#endregion
	}
	
	/// <summary>
	/// Check if we are in user menu screens
	/// </summary>
	public bool InUserMenu ()
	{
		return ((int) currentMenuIdx >= (int) MenuButton.MenuType.UserMainMenu);
	}

	public MenuButton.MenuType GetPreviousIndex ()
	{
		return previousMenuIdx;
	}

	public Menu GetCurrentMenu ()
	{
		return _menus[currentMenuIdx];
	}
}
