using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BedroomNPC : MonoBehaviour 
{
	public BedroomLevelManager manager;
	public UITexture myTexture;
	public NPC sherlockNPC;
	bool wantTalk = false;
	public NPCDialogueAnimation anim;
	public Transform sherlockTarget;
	public AudioClip voiceOver;

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
	
	void Start()
	{
		manager = GameObject.Find("BedroomManager").GetComponent<BedroomLevelManager>();
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE);
	}
	
	void HackNavigation()
	{
		Player.instance.MoveToDestination(transform.position);
	}
	
	void HackStopNavigation()
	{
		Player.instance.StopNavigation();
		Player.instance.Stand();
	}
	
	void OnPress(bool pressed)
	{
		if(pressed && Vector3.Distance(transform.position, Player.instance.transform.position) >= 10)
		{
			wantTalk = true;
			
			if(sherlockNPC == null){
				Player.instance.MoveToDestination(transform.position);
			}
			else{
				Player.instance.MoveToDestination(transform.position);
				Invoke("HackNavigation", 0.1f);
			}
		}
		else if(pressed){
			wantTalk = true;
			Invoke("HackStopNavigation", 0.2f);

		}
		
	}
	
	void ChangeBack()
	{
		setFriendAnimation(NPCAnimations.AnimationIndex.IDLE);
		if(name.Contains("temp")){
			manager.EnableSherlock();
		}
	}
	
	void Update()
	{
		if(sherlockNPC == null)
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
		
		else
		{
			if(wantTalk && Vector3.Distance(transform.position, Player.instance.transform.position) < 15)
			{
				wantTalk = false;
				Player.instance.interactingNPC = sherlockNPC;
				sherlockNPC.StartConversation();
				manager.EnableSherlockCage(true);
				GetComponent<BoxCollider>().enabled = false;
			}
		}
	}
}
