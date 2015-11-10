using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TopicContainer: MonoBehaviour {
	GabyTopicsWheel wheel;
	public GabyMinigameManager manager;
	TopicSlotButton containedTopic;
	
	public UITexture myIcon;
	public UITexture myBackground;
	public UITexture myBorder;
	public UITexture myCheck;
	public UITexture myCross;
	
	public enum AnimationSelect
	{ 
		CORRECTANIM, INCORRECTANIM, NONE
	};
	
	AnimationSelect animSelect = AnimationSelect.NONE;
	
	public void SetWheel(GabyTopicsWheel theWheel)
	{
		wheel = theWheel;	
	}
	
//	void DropObject ()
//	{
//		// Is there a droppable container?
//		Collider col = UICamera.lastHit.collider;
//		TopicContainer container = (col != null) ? col.gameObject.GetComponent<TopicContainer>() : null;
//
//		if (container != null && container.IsEmpty())
//		{
//			container.CheckIfCorrectTopic(this);
//			Vector3 pos = container.transform.position;
//			pos.z = transform.position.z;
//			transform.position = pos;
//			droppedInContainer = true;
//			myContainer = container;
//		}
//		else
//		{
//			ReturnToBubble();
//		}
//	}
	
//	void OnDrop(GameObject topic){
//		containedTopic = topic.GetComponent<TopicSlotButton>();
//		
//		if(IsEmpty())
//		{
//			CheckIfCorrectTopic(containedTopic);
//			Vector3 pos = transform.position;
//			pos.z = containedTopic.transform.position.z;
//			containedTopic.transform.position = pos;
//			containedTopic.droppedInContainer = true;
//			containedTopic.myContainer = this;
//		}
//		
//		else
//		{
//			containedTopic.ReturnToBubble();
//			containedTopic = null;
//		}
//	}
	
	public bool CheckIfCorrectTopic(TopicSlotButton topic)
	{
//		if(FoundCorrectTopic(topicName))
//			animSelect = AnimationSelect.CORRECTANIM;
//		else
//			animSelect = AnimationSelect.INCORRECTANIM;
//		
		containedTopic = topic;
		
		if(FoundCorrectTopic(topic.name))
		{
			InvokeRepeating("CorrectAnimation", 0f, 0.5f);
			animSelect = AnimationSelect.CORRECTANIM;
			Invoke("CancelAnim", 2f);
			manager.GotCorrectAnswer();
			return true;
		}
		else
		{
			InvokeRepeating("IncorrectAnimation", 0f, 0.5f);
			animSelect = AnimationSelect.INCORRECTANIM;
			Invoke("CancelAnim", 2f);
			return false;
		}
			
	}
	
	public Texture GetTopic(){
		return containedTopic.myIcon.mainTexture;	
	}
	
	public bool IsEmpty(){
		if(containedTopic == null)
			return true;
		else
			return false;
	}
	
	
	bool FoundCorrectTopic(string topic)
	{
		List<string> answerList = manager.GetAnswerList();
		bool found = false;
		
		for(int i = 0; i < answerList.Count; i++){
			if(topic == answerList[i]){
				found = true;
				break;
			}
		}
		
		return found;
	}
	
	void CancelAnim()
	{
		CancelInvoke();
		
		if(animSelect == AnimationSelect.CORRECTANIM)
		{
			myCheck.enabled = true;
			myCross.enabled = false;
		}
		
		else
		{
			myCheck.enabled = false;
			myCross.enabled = false;
			myIcon.enabled = false;
			//wheel.ReturnSelectedTopic();
			containedTopic.ReturnToBubble();
			containedTopic.myContainer = null;
			containedTopic.droppedInContainer = false;
			containedTopic = null;
			manager.GotWrongAnswer();
		}
		
	}
	
	
	public void Reset(){
		containedTopic = null;
		myCheck.enabled = false;
		myCross.enabled = false;
	}
	
	void CorrectAnimation(){
		myCheck.enabled = !myCheck.enabled;
	}
	
	void IncorrectAnimation(){
		myCross.enabled = !myCross.enabled;
	}
	
//	void OnPress(bool pressed)
//	{
//		if(pressed){
//			wheel.RotateToNearestTopic(this);
//		}
//	}
	
	// Use this for initialization
	void Start () 
	{
		manager = GameObject.Find("GabbyMinigame").GetComponent<GabyMinigameManager>();
	}
	
}
