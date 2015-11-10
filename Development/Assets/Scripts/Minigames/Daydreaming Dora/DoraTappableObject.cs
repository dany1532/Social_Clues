using UnityEngine;
using System.Collections;

public class DoraTappableObject : MonoBehaviour {
	
	public GameObject managerObj; 
	public bool clicked; 
	
	DoraManager manager;
	
	public bool isSolution;
	
	// Use this for initialization
	void Start () {
		manager = managerObj.GetComponent<DoraManager>();
		clicked = false;
	}
	
	void OnClick()
	{
		clicked = true; 
		Debug.Log (this.name + " clicked");
		if (isSolution)
		{
			Debug.Log (this.name + " isSolution");
			//manager.doraNextSquare(); 
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
