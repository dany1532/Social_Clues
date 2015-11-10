using UnityEngine;
using System.Collections;

public class GabyHintButton : MonoBehaviour {
	public GabyMinigameManager manager;
	
	void OnPress(bool pressed)
	{
		if(pressed)
		{
			manager.HintButtonPressed();
		}
	}
}
