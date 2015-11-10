using UnityEngine;
using System.Collections;

public class WorldMapButton : MonoBehaviour 
{
	public enum WorldButton { NONE, DIFFICULTY, LEVEL }
	public WorldButton button = WorldButton.NONE;
	public WorldMapManager.Difficulty diff;
	public string myLevel;
	public bool insideBubble;
	WorldMapManager manager;
	
	// Use this for initialization
	void Start () {
		manager = GameObject.Find("WorldMap").GetComponent<WorldMapManager>();
	}
	
	void OnPress(bool pressed)
	{
		if(pressed)
		{
			switch(button)
			{
			case WorldButton.NONE: manager.HideOptions();
								   break;
				
			case WorldButton.DIFFICULTY: manager.SetDifficulty(diff);
								   		 break;
				
			case WorldButton.LEVEL: manager.SelectLevel(myLevel);
									if(insideBubble) manager.DisplayDifficulty();
								   	break;
			}
		}
	}
}
