using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SherlockMultipleDialogues : MonoBehaviour {
	
	// Multiple options for the dialogue
	public List<Dialogue> mutlipleOptions;
	
	/// <summary>
	/// Gets a random dialogue from a list of options
	/// </summary>
	/// <returns>
	/// The random dialogue selected
	/// </returns>
	public Dialogue GetRandomDialogue()
	{
		if (mutlipleOptions.Count > 0)
			return mutlipleOptions[Random.Range (0, mutlipleOptions.Count)];
		
		return null;		
	}
}
