using UnityEngine;
using System.Collections;

public class InteractiveReplyElement : MonoBehaviour
{
	
    public UISprite audioUI;
    public UISprite sprite;
    public UILabel text;	
    GameObject parent;
    Color finalAnswerColor;
    Reply reply;
    bool selected = false;
    public AudioClip voiceClip { get { return ((reply != null) ? reply.voiceOver : null); } }
	
    void Awake()
    {
        sprite.enabled = false;
        if (audioUI != null)
            audioUI.enabled = false;
        parent = sprite.transform.parent.gameObject;
    }
	
    public void Show()
    {
        parent.SetActive(true);
        sprite.enabled = true;
        if (audioUI != null)
            audioUI.enabled = true;
    }
	
    public void Hide()
    {
        parent.SetActive(false);
        sprite.alpha = 0;
        sprite.enabled = false;
        if (audioUI != null)
        {
            audioUI.alpha = 0;
            audioUI.enabled = false;
        }
    }
	
    public void Reset()
    {
        if (audioUI != null)
            audioUI.alpha = 0;
        sprite.color = Color.white;
        sprite.alpha = 0;
		
        if (text != null)
            text.text = "";
		
        reply = null;
    }
	
    public void Init(Reply newReply)
    {
        reply = newReply;
		
        if (text != null)
            text.text = reply.text;
        finalAnswerColor = reply.finalAnswerColor;
        if (reply.image != null)
        {
            sprite.atlas = reply.atlas;
            sprite.spriteName = Utilities.SetTextureName(reply.image.name);
            sprite.enabled = true;
            if (audioUI != null)
                audioUI.enabled = true;
        }
        sprite.alpha = 1;
        if (audioUI != null)
            audioUI.alpha = 1;
    }
	
    public void OnPress(bool pressed)
    {
        if (pressed && reply != null && reply.available == true && gameObject.GetComponent<BoxCollider>().enabled)
        {
            DialogueWindow.instance.SkipShowingOptions();

            //selected = true;
            DialogueWindow.instance.DisableReplyColliders();
            //DialogueWindow.instance.SkipShowingOptions();

            InputManager.Instance.ReceivedUIInput();
            //InteractiveReplyElement activeReply = DialogueWindow.instance.activeReply;

            DialogueWindow.instance.activeReply = this;
            // Update element to final color
            sprite.color = finalAnswerColor;
            // Reply to dialogue based on selected element
            reply.Response();

            /*
			// If element requires two clicks, and this is first time clicking on an option
			if (activeReply != this && false)//!reply.dialogue.singleClick)
			{
				// If there was a previously selected element
				if (activeReply != null)
					// Reset it's background color
					DialogueWindow.instance.activeReply.sprite.color = Color.white;
			
				// Select new element
				DialogueWindow.instance.activeReply = this;
				sprite.color = Color.yellow;
			}
			else // Final time clicking on an option (element is selected)
			{

			}
			*/
        }
    }
}
