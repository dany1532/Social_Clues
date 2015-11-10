using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelInteractiveNPC : MonoBehaviour {

	public NPCDialogueAnimation anim;
	public AudioClip voiceOver;
	bool wantTalk;

	void Start()
	{
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE);
	}

	void HackStopNavigation()
	{
		Player.instance.StopNavigation();
		Player.instance.Stand();
	}
	
	void setFriendAnimation(NPCAnimations.AnimationIndex animationType) 
	{
		if(anim != null)
		{
			NPCAnimations.AnimationSequence currAnimSeq = anim.GetComponent<NPCAnimations>().RetrieveAnimationSequence(animationType);
			List<Texture> currAnimSeqTextures = currAnimSeq.textures;
			if (currAnimSeqTextures.Count > 0)
			{
				anim.StopAnimation();
				anim.SetAnimationList(currAnimSeqTextures);
				anim.PlayAnimation();
				anim.SetSpeed(currAnimSeq.speed);
			}
		}
	}

	void OnPress(bool pressed)
	{
		if(pressed && Vector3.Distance(transform.position, Player.instance.transform.position) >= 10)
		{
			wantTalk = true;
			Player.instance.MoveToDestination(transform.position);
		}
		else if(pressed){
			wantTalk = true;
			Invoke("HackStopNavigation", 0.2f);
		}
		
	}
	
	void ChangeBack()
	{
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE);
	}
	
	void Update()
	{
		if(wantTalk && Vector3.Distance(transform.position, Player.instance.transform.position) < 10)
		{
			
			wantTalk = false;
			Player.instance.StopNavigation();
			setFriendAnimation(NPCAnimations.AnimationIndex.TALKING);
			
			if (voiceOver != null) {
				AudioManager.Instance.PlayVoiceOver(voiceOver, 1);
				Invoke("ChangeBack", voiceOver.length);
			}
			else
			{
				Invoke("ChangeBack", 1);
			}
		}
	}
}
