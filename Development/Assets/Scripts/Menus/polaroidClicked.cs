using UnityEngine;
using System.Collections;

public class polaroidClicked : MonoBehaviour {

	public GameObject managerObj; 
	AchievementsManager manager; 

	//bool clicked; 

	// Use this for initialization
	void Start () {
		manager = managerObj.GetComponent<AchievementsManager>();
		//clicked = false; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick ()
	{
		Debug.Log ("polaroid clicked"); 
		Debug.Log (this.name); 
		/*if (!clicked) 
		{

			clicked = true; 
			 
		}
		else
		{
			clicked = false;
		}*/
		manager.pictureClicked(this.name);//, clicked);
	}
}
