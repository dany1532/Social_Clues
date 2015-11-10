using UnityEngine;
using System.Collections;

public class ShowTutorialButton : MonoBehaviour {
	
	public MinigameManager myManager;
	public bool willShowTutorial;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnPress(bool isDown)
	{
		if(isDown)
		{
			//Debug.Log("Item clicked");
			if(willShowTutorial)
				myManager.yesTutorial();
			else
				myManager.noTutorial();
		}
	}
}
