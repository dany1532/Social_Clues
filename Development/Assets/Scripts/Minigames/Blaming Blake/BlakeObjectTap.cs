using UnityEngine;
using System.Collections;

public class BlakeObjectTap : MonoBehaviour {
	
	public bool clicked; 
	
	// Use this for initialization
	void Start () {
		clicked = false; 
	}
	
	
	void OnClick() {
		clicked = true; 
	}
}
