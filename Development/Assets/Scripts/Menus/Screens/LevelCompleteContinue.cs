using UnityEngine;
using System.Collections;

public class LevelCompleteContinue : MonoBehaviour {
	
	public LevelComplete screen;
		
	void OnClick () {
		screen.EndLevelCompletionSequence();
	}
}
