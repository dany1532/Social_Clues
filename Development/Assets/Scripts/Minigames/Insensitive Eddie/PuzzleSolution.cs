using UnityEngine;
using System.Collections;

public class PuzzleSolution : MonoBehaviour {
	
	public Dialogue audio;
		
	public float ShowDialogue()
	{
		if (audio != null)
		{
			Sherlock.Instance.PlaySequenceInstructions(audio, null);
			
			if (audio.voiceOver != null)
				return audio.voiceOver.length;
		}
		return 0;
	}
	
	public void SetActive(bool value)
	{
		gameObject.SetActive(value);
	}
}
