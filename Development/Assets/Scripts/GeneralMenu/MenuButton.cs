using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	
	private MenuController menuController;
	public GameObject passwordInput;
	public enum MenuType {
		None			= -1,
		Main    	    = 0,
		Login	        = 1,
		Users   	    = 2,
		UserMenu    	= 3,
		UserSetup   	= 4,
		UserCharacterSelection = 5,
		ParentAccount	= 6,
		About 	 		= 7,
		ParentScreen	= 8,
		InputPassword	= 10,
		Analytics   	= 16,
		UserOptions 	= 17,
		Therapy     	= 18,
		UserMainMenu	= 64,
		Adventure		= 65,
		Play			= 66,
		Achievement		= 67,
		StoryMode		= 80,
		BuildLevel		= 81,
		Minigames		= 96,
		MinigamesDifficulty	= 97,
		ToyBox			= 98,
		UnderConstruction = 404		
	}	
	
	public MenuType menuType;
	
	public void Awake()
	{
		menuController = GameObject.FindObjectOfType(typeof(MenuController)) as MenuController;
	}
	
	void OnClick() {
		InputManager.Instance.ReceivedUIInput();
		
		switch(menuType)
		{
		case MenuType.Analytics: 
		case MenuType.UserOptions:
			menuController.enableMenu(menuType);
			break;
		case MenuType.Therapy:
			break;
		case MenuType.UserMenu:
			ParentUserSelect user = GetComponent<ParentUserSelect>();
			
			if (ApplicationState.Instance.previousLevel == ApplicationState.LevelNames.MAIN_MENU)
			{
				if (user != null)
				{
					if (user.SetSelectedChild())
					{
						menuController.enableMenu(menuType);
					}
					else
					{
						menuController.enableMenu(MenuType.UserSetup);
						menuController.backButton.menuType = MenuType.Users;
					}
				}
				else
				{
					menuController.enableMenu(menuType);
				}
			}
			else
			{
				LoginMenuOption menuOption = GetComponent<LoginMenuOption>();
				menuOption.menu.LoadUsers();
				menuController.enableMenu(MenuType.Users);
			}
			break;
		case MenuType.UserSetup:
			LoginMenuOption loginUser = GetComponent<LoginMenuOption>();
			
			if (loginUser != null && loginUser.id > 0)
			{
				ApplicationState.Instance.userID = loginUser.id;
				menuController.enableMenu(MenuType.UserCharacterSelection);
			}
			else
			{
				menuController.enableMenu(MenuType.UserSetup);
			}
			
			menuController.backButton.menuType = MenuType.Login;
			
			break;
		case MenuType.UserMainMenu:
			if (menuController.InUserMenu())
			{
				menuController.enableMenu(MenuType.UserMainMenu);
			}
			else
			{
				if (menuController.backButton.menuType == MenuType.Login)
				{
					ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.CHILDS_MENU);
				}
				else
				{
					LoginMenuOption menuOption = GetComponent<LoginMenuOption>();
					menuOption.menu.LoadUsers();
					menuController.enableMenu(MenuType.Users);
				}
			}
			break;
		case MenuType.None:
			break;
		case MenuType.UnderConstruction:
			menuController.enableMenu(menuType);
			menuController.backButton.menuType = menuController.GetPreviousIndex();
			break;
		case MenuType.MinigamesDifficulty:
			MinigameSelect minigameSelect = GetComponent<MinigameSelect>();
			string minigameLevel = minigameSelect.levelName;
			
			menuController.enableMenu(menuType);
			
			minigameSelect = menuController.GetCurrentMenu().GetComponent<MinigameSelect>();
			minigameSelect.levelName = minigameLevel;
			break;
		case MenuType.Main:			
			// If player in is in the users (child) menu
			if (menuController.InUserMenu())
			{
				GameObject loadGameManager = GameObject.Find("LoadGame_Manager");
				Destroy(loadGameManager);
				
				// Load Menu screen
				ApplicationState.Instance.LoadLevel(ApplicationState.LevelNames.MAIN_MENU);
			}
			else // Otherwise just enable Menu screen
				menuController.enableMenu(menuType);
			break;
		case MenuType.About:
			menuController.enableMenu(menuType);
			menuController.backButton.gameObject.SetActive(false);
			menuController.homeButton.gameObject.SetActive(false);
			break;
		case MenuType.InputPassword:
			menuController.enableMenu(menuType);
			break;
		case MenuType.ParentAccount:
			int count = MainDatabase.Instance.getIDs("SELECT count(*) FROM PARENT");
			if(count == -1)
			{
				menuController.enableMenu(menuType);
			}
			else
			{
				menuController.enableMenu(MenuType.InputPassword);
			}

			break;
		default:
			menuController.enableMenu(menuType);
			break;
		}	
		ApplicationState.Instance.ClearPreviousLevel();	
	}
}
