using UnityEngine;
using System.Collections;

public class backButtonClicked : MonoBehaviour {

	public GameObject managerObj; 
	AchievementsManager manager; 

	// Use this for initialization
	void Start () {
		manager = managerObj.GetComponent<AchievementsManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		manager.backToFullScreen(); 
	}
}
