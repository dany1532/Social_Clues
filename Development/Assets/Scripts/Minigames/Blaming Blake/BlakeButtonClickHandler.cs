using UnityEngine;
using System.Collections;

public class BlakeButtonClickHandler : MonoBehaviour {
	
	public GameObject manager;
	private BlamingBlakeManager BlamingBlakeManagerScript;
	
	// Use this for initialization
	void Start () {
		BlamingBlakeManagerScript = manager.GetComponent<BlamingBlakeManager>();
	}
	
		BlamingBlakeManager.eType GetButtonClicked()
	{
		if (this.gameObject.name == "CorrectButton")
		{
			return BlamingBlakeManager.eType.Correct;
		}
		else if (this.gameObject.name == "FriendButton")
		{
			return BlamingBlakeManager.eType.Friend;
		}
		else if (this.gameObject.name == "HeadButton")
		{
			return BlamingBlakeManager.eType.Head;
		}
		return BlamingBlakeManager.eType.Invalid_Type; 
	}
	
		void OnClick()
	{
		
		BlamingBlakeManagerScript.Instance.SendMessage("OnButtonClickDown", GetButtonClicked(), SendMessageOptions.DontRequireReceiver);
		//bool isClicked
		/*if(isClicked)
		{
			//Send result to OnButtonClickDown
			BlamingBlakeManagerScript.Instance.SendMessage("OnButtonClickDown", GetButtonClicked(), SendMessageOptions.DontRequireReceiver);
			//Debug.Log ("press down");
		}
		
		else
		{
			//Send result to OnButtonClickUp
			//BlamingBlakeManagerScript.Instance.SendMessage("OnButtonClickUp", GetButtonClicked(), SendMessageOptions.DontRequireReceiver);
			//Debug.Log ("press up");
		}*/
	}
	
}
