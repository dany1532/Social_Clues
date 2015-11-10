using UnityEngine;
using System.Collections;

public class RepeatAudio : MonoBehaviour {
	
	public InteractiveReplyElement reply;
	
	void OnPress (bool pressed)
	{
		if (enabled && NGUITools.GetActive(gameObject) && gameObject != null && reply.GetComponent<BoxCollider>().enabled)
		{
			if (pressed)
			{
				if (reply != null && reply.voiceClip != null)
				{
					InteractiveReplyElement[] replyElements = this.transform.parent.transform.parent.GetComponentsInChildren<InteractiveReplyElement>();
					foreach(InteractiveReplyElement element in replyElements) {
						if(element.sprite.color.a != 0) {
							element.sprite.color = Color.white;
						}
					}
					reply.sprite.color = Color.grey;
					AudioManager.Instance.PlayVoiceOver(reply.voiceClip, 1.0f);	
					Invoke ("ReturnBackgroundColor", reply.voiceClip.length);
				}
			}
		}
	}
	
	void ReturnBackgroundColor()
	{
		reply.sprite.color = Color.white;
	}
}
