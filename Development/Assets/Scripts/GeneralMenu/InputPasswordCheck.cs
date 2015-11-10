using UnityEngine;
using System.Collections;

public class InputPasswordCheck : MonoBehaviour {
	public MenuController menuController;
	public UILabel password;
	public GameObject invalide;

	public GameObject closeUserSettings;
	public GameObject passwordMenu;
	public enum resultType { REGULAR, OPTIONS_MENU }
	public resultType type = resultType.REGULAR;

	void OnClick ()
	{
		string storedPass = MainDatabase.Instance.getName("SELECT Password FROM PARENT");
		if(storedPass == password.text)
        {
            UIInput uiInput = password.gameObject.GetComponent<UIInput>();
            uiInput.text="";
            
			switch(type) {
			case resultType.OPTIONS_MENU:
			    closeUserSettings.SendMessage("enableParentMenu", SendMessageOptions.DontRequireReceiver);
			    passwordMenu.SetActive(false);
			    break;
			case resultType.REGULAR:
    			menuController.enableMenu(MenuButton.MenuType.Users);
    			break;
			}
		}	
		else
		{
			invalide.SetActive(true);
			UIInput uiInput = password.gameObject.GetComponent<UIInput>();
			uiInput.text="";
			Invoke ("hideResult", 2.0f);
		}
	}

	void hideResult() {
		
		password.text = "";
		invalide.SetActive (false);
	}


}
