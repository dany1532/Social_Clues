using UnityEngine;
using System.Collections;

public class SkipDialogue : MonoBehaviour {

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
		if (enabled) {
			SetActive(false);

			if (UserSettings.Instance.instantAnswer)
			{
				DialogueWindow.instance.SkipShowingOptions ();
			}
		}
	}
}
