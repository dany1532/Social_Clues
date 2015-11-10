using UnityEngine;
using System.Collections;

public class DebugInfo : Singleton<DebugInfo> {
	public bool debug = false;
	public UILabel debugText;
	public ulong debugCounter = 0;
	
	// Use this for initialization
	void Start () {
		instance = this;
		
		if (!debug)
			gameObject.SetActive(false);
	}
	
	//// <summary>
	/// Update debug text on screen
	/// </summary>
	public void UpdateDebugText (string text) {
		if (debug)
		{
			debugText.text = string.Format("{0}: {1}", debugCounter, text);
			
			debugCounter++;
			if (debugCounter == ulong.MaxValue)
				debugCounter = ulong.MinValue;
		}
	}
}
