using UnityEngine;
using System.Collections;

public class SherlockTutorial : MonoBehaviour {
    
    public Dialogue kateIntro;
    public Dialogue peteIntro;
    
    public Dialogue characterResponse;
    
    public AudioClip peteResponseAudio;
    public AudioClip kateResponseAudio;
    
	public ConversationTree conversationRoot;

	// Use this for initialization
	void Start () {
	    if (ApplicationState.Instance.selectedCharacter == "Pete")
        {
			conversationRoot.SetDialogue(peteIntro);
			if(peteResponseAudio != null)
            	characterResponse.voiceOver = peteResponseAudio;
        }
        else
        {
			conversationRoot.SetDialogue(kateIntro);
			if(kateResponseAudio != null)
            	characterResponse.voiceOver = kateResponseAudio;
		}
		LoadVoiceOver();
	}

	void LoadVoiceOver()
	{
		GameObject voiceOversPrefab = ResourceManager.LoadNPCVoiceOver("Sherlock", ApplicationState.Instance.selectedCharacter);
		DialogueVoiceOver voiceOver =  (DialogueVoiceOver) ((voiceOversPrefab != null) ? voiceOversPrefab.GetComponent<DialogueVoiceOver>() : null);
		// Upate owner of each dialogue to NPC
		foreach (Dialogue dialogue in conversationRoot.GetComponentsInChildren<Dialogue>(true))
		{			
			if (voiceOver != null && dialogue.voiceOverID != DialogueVoiceOver.VoiceOverID.NATIVE && dialogue.voiceOverID != DialogueVoiceOver.VoiceOverID.NONE)
			{
				dialogue.voiceOver = voiceOver.RetrieveVoiceOverAudio(dialogue.voiceOverID);
			}
		}
		
		foreach (Reply reply in conversationRoot.GetComponentsInChildren<Reply>(true))
		{							
			if (voiceOver != null && reply.voiceOverID != DialogueVoiceOver.VoiceOverID.NATIVE && reply.voiceOverID != DialogueVoiceOver.VoiceOverID.NONE)
			{
				reply.voiceOver = voiceOver.RetrieveVoiceOverAudio(reply.voiceOverID);
			}
		}
	}
}
