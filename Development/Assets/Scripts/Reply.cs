using UnityEngine;
using System.Collections;

/// <summary>
/// Reply / Answer to a question during a conversation
/// </summary>
public class Reply : MonoBehaviour {
	
	/// <summary>
	/// Answer type. Can include text, image, or both
	/// </summary>
	public enum AnswerType
	{
		TEXT,
		IMAGE,
		TEXT_IMAGE
	}
	#region Reply Data
	public int id;
	// The type of the reply
	public AnswerType type = AnswerType.TEXT;
	// Text part of the reply
	public string text;
	// Image part of the reply
	public UIAtlas atlas = null;
	// Image part of the reply
	public Texture image = null;
	// Voice over for reply
	public AudioClip voiceOver;
	public DialogueVoiceOver.VoiceOverID voiceOverID = DialogueVoiceOver.VoiceOverID.NATIVE;
	#endregion
	
	// The index, identifier, in the dialogue of the reply
	public int index;
	
	// The dialogue / question the reply is part of
	public Dialogue dialogue;
	// The dialogue / sentence that follows if the reply is selected
	public Dialogue dialogueReply;
	
	// The color of the background sprite if the reply is selected (usually red for wrong / green for correct)
	public Color finalAnswerColor = Color.red;
	
	// If this reply is still available, or it has already been used
	public bool available = true;
	
	public void SetIndex(int newIndex)
	{
		index = newIndex;
	}
	
	public void Response ()
	{
		dialogue.Reply(index);
	}
}
