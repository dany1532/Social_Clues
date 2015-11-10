using UnityEngine;
using System.Collections;

public class RepeatNPCDialogue : MonoBehaviour {
	
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
		if (enabled && gameObject.activeInHierarchy)
		{
			SetActive(false);
			DialogueWindow.instance.RepeatNPCDialogue();
		}
	}
}
