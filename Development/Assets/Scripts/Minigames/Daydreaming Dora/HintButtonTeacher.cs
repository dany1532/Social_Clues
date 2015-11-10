using UnityEngine;
using System.Collections;

public class HintButtonTeacher : MonoBehaviour {
	
	public DoraManager manager;
	[System.NonSerialized]
	public bool isShowing = false;
	
	void OnClick ()
	{
		if(!isShowing) {
			isShowing = true;
			manager.showHint();
		}
	}
}
