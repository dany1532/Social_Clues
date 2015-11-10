using UnityEngine;
using System.Collections;

public class BlakeGrayPanels : MonoBehaviour {
	
	public bool isCorrectPanel; 
	public GameObject manager;
	private bool turnOffPanelsRef; 
	private bool newPanelsRef; 
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
		turnOffPanelsRef = manager.GetComponent<BlamingBlakeManager>().turnOffPanels; 
		newPanelsRef = manager.GetComponent<BlamingBlakeManager>().newPanels; 
		
		if (turnOffPanelsRef == true && isCorrectPanel == false)
		{
			//this.GetComponent<UISprite>().color = Color.grey; 
			//Debug.Log ("black");
		}
		if (newPanelsRef)
		{
			//this is only changing the reference to this value in this script, it is not changing the value in the manager
			turnOffPanelsRef = false; 
			this.GetComponent<UISprite>().color = Color.blue; 
		}
	}
}
