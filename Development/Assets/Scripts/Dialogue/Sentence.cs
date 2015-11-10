using UnityEngine;
using System.Collections;

public class Sentence : MonoBehaviour {

	// The text of the dialgoue
	public string text;
	// The voice over audio clip of the dialogue
	public AudioClip voiceOver;

	// Whether to show a speech bubble
	bool showSpeechBubble = true;
	
	// If the dialogue voice over can be repeated or not
	public bool forceRepetition = false;
	// If this is the first play of the dialogue
	[System.NonSerialized]
	public bool firstRepetition = true;

	#region Dialogue Type
	// Who owns the dialogue
	public enum Speaker
	{
		PLAYER,
		SHERLOK,
		NPC
	}
	public Speaker speaker = Speaker.SHERLOK;
	#endregion
	
	// The next dialogue after this
	public Sentence nextDialogue;

	public bool autoAdvance = false;

	public Question GetQuestion ()
	{
		return gameObject.GetComponent<Question>();
	}
}
