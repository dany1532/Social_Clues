using UnityEngine;
using System.Collections;

public class PuzzleHint : MonoBehaviour {
	
	public EddiePuzzleManager manager;
	
	
	void OnClick()
	{
		manager.hintPressed();
	}
}
