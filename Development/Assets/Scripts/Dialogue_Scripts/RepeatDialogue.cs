using UnityEngine;
using System.Collections;

public class RepeatDialogue : MonoBehaviour {

	// Use this for initialization
	void Start () {
		enabled = false;
	}
	
	public void SetActive(bool value)
	{
		enabled = value;
	}

	void OnClick()
	{
		if (enabled)
		{
			SetActive(false);
			DialogueWindow.instance.RepeatDialogue();
		}
	}
}
