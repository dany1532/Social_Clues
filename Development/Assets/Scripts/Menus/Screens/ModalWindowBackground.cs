using UnityEngine;
using System.Collections;

public class ModalWindowBackground : MonoBehaviour {
	
	public MenuController menuController;
	
	// Update is called once per frame
	void OnClick () {
		menuController.enableMenu(menuController.GetPreviousIndex());
	}
}
