using UnityEngine;
using System.Collections;

public class LevelDot : MonoBehaviour {
	public int mylevelID;
	public bool hasLevels;
	WorldMapManager manager;
	
	// Use this for initialization
	void Start () {
		manager = GameObject.Find("WorldMap").GetComponent<WorldMapManager>();
	}
	
	void OnPress(bool pressed)
	{
		if(pressed){
			if(!manager.canMove) manager.GoToLevelPosition(mylevelID, hasLevels);
		}
	}
}
