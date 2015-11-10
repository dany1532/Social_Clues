using UnityEngine;
using System.Collections;

public class Tab : MonoBehaviour {
	public GameObject controller;
	public GameObject input;
	public GameObject result;
	public UILabel password;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void OnClick() {
		//Debug.Log("Tab");
		if(AnalyticsController.Instance.currentTab == AnalyticsController.Tab.Child) {
			input.SetActive(true);
		} else if(AnalyticsController.Instance.currentTab == AnalyticsController.Tab.Parent) {
			input.SetActive (false);
			controller.GetComponent<AnalyticsController>().switchTabs (AnalyticsController.Tab.Child);
		}
	}
	
	void OnSubmit(string password2) {
		//Debug.Log ("Tab Submit");
		string storedPass = MainDatabase.Instance.getName("SELECT Password FROM PARENT");
		if(password2 == storedPass) {
			controller.GetComponent<AnalyticsController>().switchTabs (AnalyticsController.Tab.Parent);
		} else {
			result.SetActive(true);
			password.text = "";
			Invoke ("hideResult", 3.0f);
		}
	}
	
	void hideResult() {
		result.SetActive (false);
	}
	
	public void SwitchTab(){
		OnSubmit(password.text);	
	}
	
	public void HideInput(){
		password.text = "Password here";
		input.SetActive(false);	
	}
}
